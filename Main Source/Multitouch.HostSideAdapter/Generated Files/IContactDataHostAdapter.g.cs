//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3069
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Multitouch.Contracts.HostSideAdapters
{
    
    public class IContactDataHostAdapter
    {
        internal static Multitouch.Contracts.IContactData ContractToViewAdapter(Multitouch.Contracts.Contracts.IContactDataContract contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true) 
                        && contract.GetType().Equals(typeof(IContactDataViewToContractHostAdapter))))
            {
                return ((IContactDataViewToContractHostAdapter)(contract)).GetSourceView();
            }
            else
            {
                return new IContactDataContractToViewHostAdapter(contract);
            }
        }
        internal static Multitouch.Contracts.Contracts.IContactDataContract ViewToContractAdapter(Multitouch.Contracts.IContactData view)
        {
            if (view.GetType().Equals(typeof(IContactDataContractToViewHostAdapter)))
            {
                return ((IContactDataContractToViewHostAdapter)(view)).GetSourceContract();
            }
            else
            {
                return new IContactDataViewToContractHostAdapter(view);
            }
        }
    }
}

