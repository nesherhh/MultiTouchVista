//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1434
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Multitouch.Contracts.HostSideAdapters
{
    
    public class ContactStateArrayHostAdapter
    {
        public static Multitouch.Contracts.ContactState[] ContractToViewAdapter(Multitouch.Contracts.Contracts.ContactState[] contract)
        {
            Multitouch.Contracts.ContactState[] result = new Multitouch.Contracts.ContactState[contract.Length];
            for (int i = 0; (i < contract.Length); i = (i + 1))
            {
                result[i] = Multitouch.Contracts.HostSideAdapters.ContactStateHostAdapter.ContractToViewAdapter(contract[i]);
            }
            return result;
        }
        public static Multitouch.Contracts.Contracts.ContactState[] ViewToContractAdapter(Multitouch.Contracts.ContactState[] view)
        {
            Multitouch.Contracts.Contracts.ContactState[] result = new Multitouch.Contracts.Contracts.ContactState[view.Length];
            for (int i = 0; (i < view.Length); i = (i + 1))
            {
                result[i] = Multitouch.Contracts.HostSideAdapters.ContactStateHostAdapter.ViewToContractAdapter(view[i]);
            }
            return result;
        }
    }
}

