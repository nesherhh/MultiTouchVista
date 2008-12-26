using System;
using System.ServiceModel;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	/// <summary>
	/// Interface for configuration of Multitouch service
	/// </summary>
	[ServiceContract]
	public interface IConfigurationInterface
	{
		/// <summary>
		/// Returns all available input providers.
		/// </summary>
		/// <returns>List of tokens representing input provider addins.</returns>
		[OperationContract]
		InputProviderToken[] GetAvailableInputProviders();

		/// <summary>
		/// Sets current input provider
		/// </summary>
		[OperationContract] 
		void SetCurrentInputProvider(InputProviderToken value);

		/// <summary>
		/// Returns current input provider
		/// </summary>
		[OperationContract]
		InputProviderToken GetCurrentInputProvider();

		/// <summary>
		/// Restarts multitouch service.
		/// </summary>
		[OperationContract]
		void RestartService();

		/// <summary>
		/// Shows configuration window
		/// </summary>
		/// <param name="parent">Parent window handle</param>
		[OperationContract]
		void ShowConfiguration(IntPtr parent);

		/// <summary>
		/// <c>True</c> if input provider is configurable.
		/// </summary>
		[OperationContract]
		bool HasConfiguration();
	}
}
