//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1434
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Multitouch.Contracts.AddInSideAdapters
{
    
    public class IContactViewToContractAddInAdapter : System.AddIn.Pipeline.ContractBase, Multitouch.Contracts.Contracts.IContactContract
    {
        private Multitouch.Contracts.IContact _view;
        public IContactViewToContractAddInAdapter(Multitouch.Contracts.IContact view)
        {
            _view = view;
        }
        public int Id
        {
            get
            {
                return _view.Id;
            }
        }
        public double X
        {
            get
            {
                return _view.X;
            }
        }
        public double Y
        {
            get
            {
                return _view.Y;
            }
        }
        public double Width
        {
            get
            {
                return _view.Width;
            }
        }
        public double Height
        {
            get
            {
                return _view.Height;
            }
        }
        public Multitouch.Contracts.Contracts.ContactState State
        {
            get
            {
                return Multitouch.Contracts.AddInSideAdapters.ContactStateAddInAdapter.ViewToContractAdapter(_view.State);
            }
        }
        internal Multitouch.Contracts.IContact GetSourceView()
        {
            return _view;
        }
    }
}

