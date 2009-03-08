using System;
using Multitouch.Configuration.Service;

namespace Multitouch.Configuration
{
	/// <summary>
	/// This class represents input provider.
	/// </summary>
	public class InputProvider : IEquatable<InputProvider>
	{
		InputProviderToken token;

		internal InputProvider(InputProviderToken token)
		{
			Check.NotNull(token, "token");
			this.token = token;
		}

		internal InputProviderToken Token
		{
			get { return token; }
		}
		
		/// <summary>
		/// Description of addin.
		/// </summary>
		public string Description
		{
			get { return token.Description; }
			set { token.Description = value; }
		}

		/// <summary>
		/// Name of addin.
		/// </summary>
		public string Name
		{
			get { return token.Name; }
			set { token.Name = value; }
		}

		/// <summary>
		/// Publisher of addin.
		/// </summary>
		public string Publisher
		{
			get { return token.Publisher; }
			set { token.Publisher = value; }
		}

		/// <summary>
		/// Version of addin.
		/// </summary>
		public string Version
		{
			get { return token.Version; }
			set { token.Version = value; }
		}

		public bool Equals(InputProvider other)
		{
			if(other == null)
				return false;
			return Name.Equals(other.Name);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as InputProvider);
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}
	}
}