using System;
using System.Collections.Generic;
using System.Windows;
using Danilins.Multitouch.Common;

namespace Danilins.Multitouch.Providers
{
	class ContactRecognizer
	{
		private List<ContactInfo> current;
		private List<List<ContactInfo>> history;
		private List<List<int>> matrix;

		private List<int> ids;
		private int extraIDs;
		private int numcheck;
		private int currentID;
		private int HISTORY_FRAMES = 10;
		private double reject_distance_threshold = 250;
		private double minimumDisplacementThreshold = 2.0;

		public ContactRecognizer()
		{
			currentID = 1;
			current = new List<ContactInfo>();
			history = new List<List<ContactInfo>>();
		}

		public ContactInfo[] Recognize(List<Rect> blobList)
		{
			// Fire off events, set id's..
			history.Add(current);

			if (history.Count > HISTORY_FRAMES)
				history.RemoveAt(0);

			current = new List<ContactInfo>();

			int numblobs = blobList.Count;

			for (int i = 0; i < numblobs; i++)
				current.Add(new ContactInfo(blobList[i]));


			List<ContactInfo> prev = history[history.Count - 1];

			int cursize = current.Count;
			int prevsize = prev.Count;

			// now figure out the 'error' for all the blobs in the current frame
			// error is defined as the distance between the current frame blobs and the blobs
			// in the last frame
			// potentially error could encompass things like deviation from the predicted
			// position, change in size, etc, but it's difficult to come up with a fair metric which 
			// includes more than one axis. Simply optimizing for the least change in distance
			// seems to work the best.

			for (int i = 0; i < cursize; i++)
			{
				current[i].Error.Clear();
				current[i].Closest.Clear();

				for (int j = 0; j < prevsize; j++)
				{
					double error = GetError(prev[j], current[i]);
					current[i].Error.Add(error);
					current[i].Closest.Add(j);
				}
			}

			// sort so we can make a list of the closest blobs in the previous frame..
			for (int i = 0; i < cursize; i++)
			{
				// Bubble sort closest.
				for (int j = 0; j < prevsize; j++)
				{
					for (int k = 0; k < prevsize - 1 - j; k++)
					{
						// ugly as hell, I know.

						if (current[i].Error[current[i].Closest[k + 1]] < current[i].Error[current[i].Closest[k]])
						{
							int tmp = current[i].Closest[k]; // swap
							current[i].Closest[k] = current[i].Closest[k + 1];
							current[i].Closest[k + 1] = tmp;
						}
					}
				}
			}

			// Generate a matrix of all the possible choices
			// If we know there were four points last time and 6 points this time then 2 ID's will be -1.
			// then we will calculate the error and pick the matrix that has the lowest error
			// the more points the slower this will be.. fortunately we will be dealing with low numbers of points..
			// unfortunately the cpu time grows explosively since this is an NP Complete problem.
			// to remedy, we will assume that the chosen ID is going to be from one of the top 4 closest points.
			// this will eliminate possiblities that shouldn't lead to an optimal solution.

			ids = new List<int>();

			//map<int, int> idmap;

			// collect id's.. 
			for (int i = 0; i < cursize; i++)
				ids.Add(-1);


			extraIDs = cursize - prevsize;
			if (extraIDs < 0)
				extraIDs = 0;
			matrix = new List<List<int>>();

			// FIXME: we could scale numcheck depending on how many blobs there are
			// if we are tracking a lot of blobs, we could check less.. 


			if (cursize <= 4)
				numcheck = 4;
			else if (cursize <= 6)
				numcheck = 3;
			else if (cursize <= 10)
				numcheck = 2;
			else
				numcheck = 1;

			if (prevsize < numcheck)
				numcheck = prevsize;

			if (current.Count > 0)
				Permute2(0);


			int num_results = matrix.Count;

			//if(cursize > 0)
			//printf("matrix size: %d\n", num_results);

			// loop through all the potential ID configurations and find one with lowest error
			double best_error = 99999;
			int best_error_ndx = -1;

			for (int j = 0; j < num_results; j++)
			{
				double error = 0;
				// get the error for each blob and sum
				for (int i = 0; i < cursize; i++)
				{
					if (matrix[j][i] != -1)
						error += current[i].Error[matrix[j][i]];
				}

				if (error < best_error)
				{
					best_error = error;
					best_error_ndx = j;
				}
			}

			// now that we know the optimal configuration, set the IDs and calculate some things..
			if (best_error_ndx != -1)
			{
				for (int i = 0; i < cursize; i++)
				{
					if (matrix[best_error_ndx][i] != -1)
						current[i].Id = prev[matrix[best_error_ndx][i]].Id;
					else
						current[i].Id = -1;

					if (current[i].Id != -1)
					{
						ContactInfo oldfinger = prev[matrix[best_error_ndx][i]];
						current[i].Delta = (current[i].Center - oldfinger.Center);
						current[i].DeltaArea = current[i].Area - oldfinger.Area;
						current[i].PredictedPos = current[i].Center + current[i].Delta;
						current[i].Displacement = oldfinger.Displacement + current[i].Delta;
					}
					else
					{
						current[i].Delta = new Vector(0, 0);
						current[i].DeltaArea = 0;
						current[i].PredictedPos = current[i].Center;
						current[i].Displacement = new Vector(0, 0);
					}
				}

				//printf("Best index = %d\n", best_error_ndx);
			}

			return GatherEvents().ToArray();
		}

		private List<ContactInfo> GatherEvents()
		{
			List<ContactInfo> result = new List<ContactInfo>();
			List<ContactInfo> prev = history[history.Count - 1];

			int cursize = current.Count;
			int prevsize = prev.Count;

			// assign ID's for any blobs that are new this frame (ones that didn't get 
			// matched up with a blob from the previous frame).
			for (int i = 0; i < cursize; i++)
			{
				if (current[i].Id == -1)
				{
					current[i].Id = currentID;

					currentID ++;
					if (currentID >= 65535)
						currentID = 0;

					current[i].State = ContactState.Down;
					result.Add(current[i]);
				}
				else
				{
					if (current[i].Displacement.Length >= minimumDisplacementThreshold)
					{
						current[i].State = ContactState.Move;
						result.Add(current[i]);
						current[i].Displacement = new Vector(0, 0);
					}
				}
			}

			// if a blob disappeared this frame, send a finger up event
			for (int i = 0; i < prevsize; i++) // for each one in the last frame, see if it still exists in the new frame.
			{
				bool found = false;
				for (int j = 0; j < cursize; j++)
				{
					if (current[j].Id == prev[i].Id)
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					prev[i].State = ContactState.Up;
					result.Add(prev[i]);
				}
			}

			return result;
		}


		private void Permute2(int start)
		{
			if (start == ids.Count)
				matrix.Add(new List<int>(ids));
			else
			{
				int numchecked = 0;

				for (int i = 0; i < current[start].Closest.Count; i++)
				{
					if (current[start].Error[current[start].Closest[i]] > reject_distance_threshold)
						break;

					ids[start] = current[start].Closest[i];
					if (CheckValid(start))
					{
						Permute2(start + 1);
						numchecked++;
					}

					if (numchecked >= numcheck)
						break;
				}

				if (extraIDs > 0)
				{
					ids[start] = -1; // new ID
					if (CheckValidNew(start))
						Permute2(start + 1);
				}
			}
		}

		private bool CheckValidNew(int start)
		{
			int newidcount = 0;

			newidcount++;
			for (int i = 0; i < start; i++)
			{
				if (ids[i] == -1)
					newidcount++;
			}

			// Check to see whether we have too many 'new' id's 
			if (newidcount > extraIDs)		//extraIDs > 0 
				return false;

			return true;
		}

		private bool CheckValid(int start)
		{
			for (int i = 0; i < start; i++)
			{
				// check to see whether this ID exists already
				if (ids[i] == ids[start])
					return false;
			}

			return true;
		}

		private double GetError(ContactInfo prev, ContactInfo curr)
		{
			Vector dev = curr.Center - prev.Center;
			return dev.Length;
		}
	}
}