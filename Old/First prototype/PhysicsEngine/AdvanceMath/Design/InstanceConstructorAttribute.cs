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




using System;
using System.Reflection;

namespace AdvanceMath.Design
{
    [global::System.AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    public sealed class InstanceConstructorAttribute : Attribute
    {
        string[] parameterNames;



        /// <summary>
        /// 
        /// </summary>
        /// <param name="order">"CSV list"</param>
        public InstanceConstructorAttribute( string parameterNames)
        {
            this.parameterNames = parameterNames.Split(',');
        }
        public string[] ParameterNames
        {
            get { return parameterNames; }
        }
        public static ConstructorInfo GetConstructor(Type t, out string[] paramNames)
        {
            foreach (ConstructorInfo method in t.GetConstructors())
            {
                object[] atts = method.GetCustomAttributes(typeof(InstanceConstructorAttribute), true);

                if (atts.Length > 0)
                {
                    InstanceConstructorAttribute att = (InstanceConstructorAttribute)atts[0];
                    if (method.GetParameters().Length == att.ParameterNames.Length)
                    {
                        paramNames = att.ParameterNames;
                        return method;
                    }
                }
            }
            paramNames = null;
            return null;
        }
    }
}