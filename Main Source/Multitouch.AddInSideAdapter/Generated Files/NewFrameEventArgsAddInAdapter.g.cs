//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3069
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Multitouch.Contracts.AddInSideAdapters
{
    
    public class NewFrameEventArgsAddInAdapter
    {
        internal static Multitouch.Contracts.NewFrameEventArgs ContractToViewAdapter(Multitouch.Contracts.Contracts.INewFrameEventArgsContract contract)
        {
            if (((System.Runtime.Remoting.RemotingServices.IsObjectOutOfAppDomain(contract) != true) 
                        && contract.GetType().Equals(typeof(NewFrameEventArgsViewToContractAddInAdapter))))
            {
                return ((NewFrameEventArgsViewToContractAddInAdapter)(contract)).GetSourceView();
            }
            else
            {
                return new NewFrameEventArgsContractToViewAddInAdapter(contract);
            }
        }
        internal static Multitouch.Contracts.Contracts.INewFrameEventArgsContract ViewToContractAdapter(Multitouch.Contracts.NewFrameEventArgs view)
        {
            if (view.GetType().Equals(typeof(NewFrameEventArgsContractToViewAddInAdapter)))
            {
                return ((NewFrameEventArgsContractToViewAddInAdapter)(view)).GetSourceContract();
            }
            else
            {
                return new NewFrameEventArgsViewToContractAddInAdapter(view);
            }
        }
    }
}

