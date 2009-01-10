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
    
    [System.AddIn.Pipeline.HostAdapterAttribute()]
    public class IProviderContractToViewHostAdapter : Multitouch.Contracts.IProvider
    {
        private Multitouch.Contracts.Contracts.IProviderContract _contract;
        private System.AddIn.Pipeline.ContractHandle _handle;
        private Multitouch.Contracts.HostSideAdapters.INewFrameEventHandlerViewToContractHostAdapter NewFrame_Handler;
        private static System.Reflection.MethodInfo s_NewFrameAddFire;
		public event System.EventHandler<Multitouch.Contracts.NewFrameEventArgs>NewFrame{
			add{
				if (_NewFrame == null)
				{
					_contract.NewFrameAdd(NewFrame_Handler);
				}
				_NewFrame += value;
				}
			remove{
					_NewFrame -= value;
				if (_NewFrame == null)
				{
					_contract.NewFrameRemove(NewFrame_Handler);
				}
				}
		}
        static IProviderContractToViewHostAdapter()
        {
            s_NewFrameAddFire = typeof(IProviderContractToViewHostAdapter).GetMethod("Fire_NewFrame", ((System.Reflection.BindingFlags)(36)));
        }
        public IProviderContractToViewHostAdapter(Multitouch.Contracts.Contracts.IProviderContract contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
            NewFrame_Handler = new Multitouch.Contracts.HostSideAdapters.INewFrameEventHandlerViewToContractHostAdapter(this, s_NewFrameAddFire);
        }
        public bool IsRunning
        {
            get
            {
                return _contract.IsRunning;
            }
        }
        public bool HasConfiguration
        {
            get
            {
                return _contract.HasConfiguration;
            }
        }
        public bool SendEmptyFrames
        {
            get
            {
                return _contract.SendEmptyFrames;
            }
            set
            {
                _contract.SendEmptyFrames = value;
            }
        }
        private event System.EventHandler<Multitouch.Contracts.NewFrameEventArgs> _NewFrame;
        public void Start()
        {
            _contract.Start();
        }
        public void Stop()
        {
            _contract.Stop();
        }
        public System.Windows.UIElement GetConfiguration()
        {
            return _contract.GetConfiguration();
        }
        public bool SendImageType(ImageType imageType, bool value)
        {
            return _contract.SendImageType(Multitouch.Contracts.HostSideAdapters.ImageTypeHostAdapter.ViewToContractAdapter(imageType), value);
        }
        internal virtual void Fire_NewFrame(Multitouch.Contracts.NewFrameEventArgs args)
        {
            if ((_NewFrame == null))
            {
            }
            else
            {
                _NewFrame.Invoke(this, args);
            }
        }
        internal Multitouch.Contracts.Contracts.IProviderContract GetSourceContract()
        {
            return _contract;
        }
    }
}

