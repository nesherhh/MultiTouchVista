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

namespace Physics2DDotNet.Ignorers
{
    /// <summary>
    /// A collision ignorer that uses group numbers to do collision ignoring.
    /// If a object is member of a group that the other body is ignoring then they will not collide. 
    /// </summary>
    [Serializable]
    public class AdvGroupIgnorer : Ignorer, ICloneable
    {
        GroupCollection groups;
        GroupCollection ignoredGroups;
        public AdvGroupIgnorer()
        {
            this.groups = new GroupCollection();
            this.ignoredGroups = new GroupCollection();
        }
        protected AdvGroupIgnorer(AdvGroupIgnorer copy)
            : base(copy)
        {
            this.groups = new GroupCollection(copy.groups);
            this.groups = new GroupCollection(copy.ignoredGroups);
        }
        public override bool BothNeeded
        {
            get { return true; }
        }
        public GroupCollection Groups { get { return groups; } }
        public GroupCollection IgnoredGroups { get { return ignoredGroups; } }
        public bool CanCollide(AdvGroupIgnorer other)
        {
            if (other == null) { throw new ArgumentNullException("other"); }
            return CanCollideInternal(other);
        }
        private bool CanCollideInternal(AdvGroupIgnorer other)
        {
            return !GroupCollection.Intersect(ignoredGroups, other.groups);
        }
        protected override bool CanCollide(Ignorer other)
        {
            AdvGroupIgnorer value = other as AdvGroupIgnorer;
            return
                value == null ||
                CanCollideInternal(value);
        }
        public virtual object Clone()
        {
            return new AdvGroupIgnorer(this);
        }
    }
}