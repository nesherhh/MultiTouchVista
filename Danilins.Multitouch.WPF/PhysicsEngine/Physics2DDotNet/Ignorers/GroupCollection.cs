#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion



#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.Collections.Generic;

namespace Physics2DDotNet.Ignorers
{
    /// <summary>
    /// A collection that stores ints that represent groups
    /// </summary>
    [Serializable]
    public class GroupCollection : ICollection<int>
    {
        public static bool Intersect(GroupCollection groups1, GroupCollection groups2)
        {
            List<int> g1 = groups1.groups;
            List<int> g2 = groups2.groups;
            int index1 = 0;
            int index2 = 0;
            while (index1 < g1.Count && index2 < g2.Count)
            {
                if (g1[index1] == g2[index2])
                {
                    return true;
                }
                else if (g1[index1] < g2[index2])
                {
                    index1++;
                }
                else
                {
                    index2++;
                }
            }
            return false;
        }

        List<int> groups = new List<int>();

        public GroupCollection()
        {
            groups = new List<int>();
        }
        public GroupCollection(GroupCollection copy)
        {
            this.groups = new List<int>(copy.groups);
        }

        /// <summary>
        /// Gets the number of collison Groups the ignorer is part of.
        /// </summary>
        public int Count
        {
            get { return groups.Count; }
        }
        bool ICollection<int>.IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Trys to add a group.
        /// </summary>
        /// <param name="item">The group ID to add.</param>
        /// <returns>false if the ignorer was already part of the group; otherwise false.</returns>
        public bool Add(int item)
        {
            for (int index = 0; index < groups.Count; ++index)
            {
                if (groups[index] == item)
                {
                    return false;
                }
                if (groups[index] < item)
                {
                    groups.Insert(index, item);
                    return true;
                }
            }
            groups.Add(item);
            return true;
        }
        /// <summary>
        /// adds an array of group ids.
        /// </summary>
        /// <param name="array">The array of group IDs. (this will be sorted)</param>
        /// <returns>the number of IDs that were not already part of the group.</returns>
        public int AddRange(int[] array)
        {
            if (array == null) { throw new ArgumentNullException("collection"); }
            Array.Sort(array);
            List<int> newGroups = new List<int>(groups.Count + array.Length);

            int newIndex = 0;
            int oldIndex = 0;
            int index = 0;
            int count = 0;
            while ((newIndex < array.Length) && (oldIndex < groups.Count))
            {
                if (array[newIndex] == groups[oldIndex])
                {
                    oldIndex++;
                }
                else if (array[newIndex] > groups[oldIndex])
                {
                    newGroups.Add(groups[oldIndex]);
                    oldIndex++;
                    index++;
                }
                else
                {
                    newGroups.Add(array[newIndex]);
                    newIndex++;
                    index++;
                    count++;
                }
            }
            for (; newIndex < array.Length; ++newIndex)
            {
                if (index == 0 || newGroups[index - 1] != array[newIndex])
                {
                    newGroups.Add(array[newIndex]);
                    index++;
                    count++;
                }
            }
            for (; oldIndex < groups.Count; ++oldIndex)
            {
                if (index == 0 || newGroups[index - 1] != groups[oldIndex])
                {
                    newGroups.Add(groups[oldIndex]);
                    index++;
                }
            }
            this.groups = newGroups;
            return count;
        }
        /// <summary>
        /// returns true if the ignorer is part of the group.
        /// </summary>
        /// <param name="item">The group ID.</param>
        /// <returns>true if the ignorer is part of the group; otherwise false.</returns>
        public bool Contains(int item)
        {
            return groups.BinarySearch(item) >= 0;
        }
        /// <summary>
        /// returns the number of groups in the array it is part of.
        /// </summary>
        /// <param name="array">The array of group IDs. (this will be sorted)</param>
        /// <returns>The number of groups in the array it is part of.</returns>
        public int ContainsRange(int[] array)
        {
            if (array == null) { throw new ArgumentNullException("collection"); }
            Array.Sort(array);
            int index1 = 0;
            int index2 = 0;
            int count = 0;
            while (index1 < groups.Count && index2 < array.Length)
            {
                if (groups[index1] == array[index2])
                {
                    count++;
                    index1++;
                    index2++;
                }
                else if (groups[index1] < array[index2])
                {
                    index1++;
                }
                else
                {
                    index2++;
                }
            }
            return count;
        }
        /// <summary>
        /// Trys to remove the ignorer from a group.
        /// </summary>
        /// <param name="item">The group ID.</param>
        /// <returns>true if the ignore was part of the group; otherwise false.</returns>
        public bool Remove(int item)
        {
            int index = groups.BinarySearch(item);
            if (index < 0) { return false; }
            groups.RemoveAt(index);
            return true;
        }
        /// <summary>
        /// Trys to remove the ignorer from a range of groups.
        /// </summary>
        /// <param name="array">The array of group IDs. (this will be sorted)</param>
        /// <returns>the number of groups the ignore was removed from.</returns>
        public int RemoveRange(int[] array)
        {
            if (array == null) { throw new ArgumentNullException("collection"); }
            Array.Sort(array);
            int index = 0;
            return groups.RemoveAll(delegate(int value)
            {
                while (index < array.Length)
                {
                    if (value == array[index])
                    {
                        index++;
                        return true;
                    }
                    else if (value > array[index])
                    {
                        index++;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            });
        }
        /// <summary>
        /// returns if the 2 ignores are not part of the same group.
        /// </summary>
        /// <param name="other">the other CollisionGroupIgnorer</param>
        /// <returns>true if they are not part of the same group; otherwiase false.</returns>

        void ICollection<int>.Add(int item)
        {
            Add(item);
        }
        /// <summary>
        /// removes the ignorer from all groups.
        /// </summary>
        public void Clear()
        {
            groups.Clear();
        }
        public void CopyTo(int[] array, int arrayIndex)
        {
            groups.CopyTo(array, arrayIndex);
        }
        public IEnumerator<int> GetEnumerator()
        {
            return groups.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}