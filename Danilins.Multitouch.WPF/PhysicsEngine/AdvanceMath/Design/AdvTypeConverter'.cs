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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
using System.ComponentModel.Design.Serialization;
#endif
using System.Globalization;

namespace AdvanceMath.Design
{
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    public class AdvTypeConverter<TType> : ExpandableObjectConverter
    {
        ConstructorInfo instanceNoArgsCtor;
        ConstructorInfo instanceCtor;
        MethodInfo parse;
        PropertyDescriptorCollection descriptions;
        string[] instanceCtorParamNames;
        public AdvTypeConverter()
        {
            Type t = typeof(TType);
            this.parse = ParseMethodAttribute.GetParseMethod(t);
            this.descriptions = AdvBrowsableAttribute.GetDispMembers(t);
            if (descriptions != null)
            {
                this.instanceNoArgsCtor = t.GetConstructor(Type.EmptyTypes);
                this.instanceCtor = InstanceConstructorAttribute.GetConstructor(t, out instanceCtorParamNames);
                if (this.instanceCtor != null)
                {
                    ParameterInfo[] paraminfos = instanceCtor.GetParameters();
                    if (paraminfos.Length == instanceCtorParamNames.Length)
                    {
                        for (int index = 0; index < instanceCtorParamNames.Length; ++index)
                        {
                            string name = instanceCtorParamNames[index];
                            PropertyDescriptor descriptor = descriptions.Find(name, false);
                            if (descriptor == null || descriptor.PropertyType != paraminfos[index].ParameterType)
                            {
                                instanceCtor = null;
                                break;
                            }
                        }
                    }
                    else
                    {
                        instanceCtor = null;
                    }
                }
            }
        }
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return 
                (parse != null && sourceType == typeof(string) )  ||
                base.CanConvertFrom(context, sourceType);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return
                ((instanceCtor != null || instanceNoArgsCtor != null) &&
                destinationType == typeof(InstanceDescriptor)) ||
                base.CanConvertTo(context, destinationType);
        }
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return
                instanceCtor != null ||
                instanceNoArgsCtor != null ||
                base.GetCreateInstanceSupported(context);
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return
                descriptions != null ||
                base.GetPropertiesSupported(context);
        }




        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (parse != null && value is string)
            {
                try
                {
                    return parse.Invoke(null, new object[] { value });
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is TType &&
                destinationType == typeof(InstanceDescriptor))
            {
                if (instanceCtor != null)
                {
                    return new InstanceDescriptor(instanceCtor, GetInstanceDescriptorObjects(value));
                }
                else if (instanceNoArgsCtor != null)
                {
                    return new InstanceDescriptor(instanceNoArgsCtor, null);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            try
            {
                PropertyDescriptor propertyDescriptor = context.Instance as PropertyDescriptor;
                object result = null;
                if (instanceNoArgsCtor != null)
                {
                    result = instanceNoArgsCtor.Invoke(null);
                    foreach (string name in propertyValues.Keys)
                    {
                        descriptions.Find(name, false).SetValue(result, propertyValues[name]);
                    }
                }
                else if (instanceCtor != null)
                {
                    result = instanceCtor.Invoke(GetInstanceDescriptorObjects(propertyValues));
                }
                if (result != null)
                {
                    if (propertyDescriptor != null)
                    {
                        propertyDescriptor.SetValue(null, result);
                    }
                    return result;
                }
            }
            catch (TargetInvocationException ex) { throw ex.InnerException; }
            return base.CreateInstance(context, propertyValues);
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (this.descriptions != null)
            {
                return this.descriptions;
            }
            return base.GetProperties(context, value, attributes);
        }

        private object[] GetInstanceDescriptorObjects(IDictionary propertyValues)
        {
            object[] rv = new object[instanceCtorParamNames.Length];
            for (int index = 0; index < instanceCtorParamNames.Length; ++index)
            {
                rv[index] = propertyValues[descriptions.Find(instanceCtorParamNames[index], false).Name];
            }
            return rv;
        }
        private object[] GetInstanceDescriptorObjects(object value)
        {
            if (value is IDictionary)
            {
                return GetInstanceDescriptorObjects((IDictionary)value);
            }
            object[] rv = new object[instanceCtorParamNames.Length];
            for (int index = 0; index < instanceCtorParamNames.Length; ++index)
            {
                PropertyDescriptor descriptor = descriptions.Find(instanceCtorParamNames[index], false);
                rv[index] = descriptor.GetValue(value);
            }
            return rv;
        }

    }
#endif
}