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
using AdvanceMath;
using System.Runtime.Serialization;
namespace Physics2DDotNet
{
    /// <summary>
    /// This is a Proxy. That keeps 2 bodies velocities synchronized. 
    /// </summary>
    [Serializable]
    public sealed class BodyProxy : IDeserializationCallback
    {
        Body body1;
        Body body2;
        internal Matrix2x2 transformation;
        [NonSerialized]
        internal LinkedListNode<BodyProxy> node;
        internal BodyProxy invertedTwin;
        internal BodyProxy(Body body1, Body body2, Matrix2x2 transformaiton)
        {
            this.body1 = body1;
            this.body2 = body2;
            this.transformation = transformaiton;
            this.node = new LinkedListNode<BodyProxy>(this);
        }
        /// <summary>
        /// This is the other body to be Synchronized with.
        /// </summary>
        public Body Body1 { get { return body1; } }
        /// <summary>
        /// This is the other body to be Synchronized with.
        /// </summary>
        public Body Body2 { get { return body2; } }
        /// <summary>
        /// This is how the Velocity will be transformed when syncronized.
        /// </summary>
        public Matrix2x2 Transformation { get { return transformation; } }
        /// <summary>
        /// This is the inverted twin of this velocity. It's matrix will be invert of this one's.
        /// And its body will be the body that contains this.
        /// </summary>
        public BodyProxy InvertedTwin { get { return invertedTwin; } }


        void IDeserializationCallback.OnDeserialization(object sender)
        {
            if (this.invertedTwin == null) { return; }
            for (LinkedListNode<BodyProxy> node = this.invertedTwin.body2.proxies.First;
                node != null;
                node = node.Next)
            {
                if (node.Value == this)
                {
                    this.node = node;
                    return;
                }
            }
            throw new SerializationException();
        }
    }
}