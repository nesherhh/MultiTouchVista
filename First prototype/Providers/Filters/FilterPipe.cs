using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Common.Filters;
using Danilins.Multitouch.Framework.Filters;
using Danilins.Multitouch.Providers.Configuration;

namespace Danilins.Multitouch.Providers.Filters
{
	public class FilterPipe : IEnumerable<IFilter>
	{
		private List<IFilter> list;

		#region IEnumerable<IFilter> Members

		public IEnumerator<IFilter> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<IFilter>) this).GetEnumerator();
		}

		#endregion

		public byte[] Process(int width, int height, ref int bitPerPixel, byte[] pixels)
		{
			byte[] result = pixels;
			foreach (IFilter filter in list)
				result = filter.Process(width, height, ref bitPerPixel, result);
			return result;
		}

		internal void LoadFrom(FiltersSection section)
		{
			try
			{
				FilterCollection filterCollection = section.Filters;

				list = new List<IFilter>();
				foreach (FilterElement filterElement in filterCollection)
				{
					Type filterType = filterElement.Type;
					ConstructorInfo constructor = filterType.GetConstructor(Type.EmptyTypes);
					if (constructor == null)
						throw new MultitouchException(string.Format("no constructor found for filter '{0}'", filterType.FullName));
					IFilter filter = (IFilter)constructor.Invoke(new object[0]);
					list.Add(filter);

					foreach (FilterParameterElement parameterElement in filterElement.Parameters)
					{
						PropertyInfo propertyInfo = filterType.GetProperty(parameterElement.Name);

						TypeConverter converter = TypeDescriptor.GetConverter(parameterElement.Type);

						object value = converter.ConvertFrom(parameterElement.Value);
						propertyInfo.SetValue(filter, value, null);
					}
				}
			}
			catch (Exception e)
			{
				Trace.TraceError(e.ToString());
				list = new List<IFilter>();
				list.Add(new Grayscale());
				list.Add(new RemoveBackground());
				list.Add(new SimpleHighpass());
				list.Add(new Scaler());
				//list.Add(new Rectify());
				list.Add(new Threshold());
			}
		}

		internal void SaveTo(FiltersSection section)
		{
			FilterCollection filterCollection = section.Filters;
			filterCollection.Clear();
			foreach (IFilter filter in list)
			{
				Type filterType = filter.GetType();
				FilterElement filterElement = new FilterElement();
				filterElement.Name = filterType.Name;
				filterElement.Type = filterType;
				filterCollection.Add(filterElement);

				foreach (PropertyInfo propertyInfo in filterType.GetProperties())
				{
					if (propertyInfo.Name != "LastResult")
					{
						FilterParameterElement parameter = new FilterParameterElement();
						parameter.Name = propertyInfo.Name;
						
						object propertyValue = propertyInfo.GetValue(filter, null);
						parameter.Type = propertyValue.GetType();

						TypeConverter converter = TypeDescriptor.GetConverter(propertyValue);
						parameter.Value = converter.ConvertToString(propertyValue);
						
						filterElement.Parameters.Add(parameter);
					}
				}
			}
		}
	}
}