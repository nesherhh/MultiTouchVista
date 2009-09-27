using System;
using System.Runtime.Serialization;

namespace Danilins.Multitouch.Common
{
	[DataContract(Namespace = "http://Danilins.Multitouch")]
	public enum ContactState
	{
		[EnumMember]
		None = 0,
		[EnumMember]
		Down = 1,
		[EnumMember]
		Up = 2,
		[EnumMember]
		Move = 3
	}
}