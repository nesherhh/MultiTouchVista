using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Windows;
using Danilins.Multitouch.Common;

namespace Danilins.Multitouch.Common
{
	[DataContract(Namespace = "http://Danilins.Multitouch")]
	public class ContactInfo
	{
		private List<int> closest;
		private List<double> error;

		[DataMember]
		private Rect rect;
		[DataMember]
		private Point center;
		[DataMember]
		private int id;
		[DataMember]
		private Vector delta;
		[DataMember]
		private double area;
		[DataMember]
		private ContactState state;
		[DataMember]
		private double deltaArea;
		[DataMember]
		private Point predictedPos;
		[DataMember]
		private Vector displacement;

		public ContactInfo(Rect rect)
		{
			this.rect = rect;
			closest = new List<int>();
			error = new List<double>();

			center = rect.Location;
			center.Offset(rect.Width / 2, rect.Height / 2);
			id = -1;
			area = rect.Width * rect.Height;
		}

		public Point Center
		{
			get { return center; }
		}

		public Point PreviousCenter { get; set; }

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public Rect Rectangle
		{
			get { return rect; }
		}

		internal ContactState State
		{
			get { return state; }
			set { state = value; }
		}

		public Vector Delta
		{
			get { return delta; }
			set { delta = value; }
		}

		public double Area
		{
			get { return area; }
		}

		public double DeltaArea
		{
			get { return deltaArea; }
			set { deltaArea = value; }
		}

		public Point PredictedPos
		{
			get { return predictedPos; }
			set { predictedPos = value; }
		}

		public Vector Displacement
		{
			get { return displacement; }
			set { displacement = value; }
		}

		internal List<double> Error
		{
			get { return error; }
		}

		internal List<int> Closest
		{
			get { return closest; }
		}

		public override string ToString()
		{
			return string.Format("Center: {0}, State: {1}, Displacement: {2}", Center, State, Displacement);
		}
	}
}