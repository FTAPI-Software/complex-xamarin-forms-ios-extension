using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace ComplexXamarinFormsIosExtApp.Pages
{
    public class MainPageModal: MainPage
    {
        #region Delegates

        public delegate void CloseHandler();
        public delegate void LogoutHandler();

        #endregion

        #region Events

        public event CloseHandler Closed;
        public event LogoutHandler LoggedOut;

        #endregion

        #region Contsructors

        public MainPageModal(List<Stream> attachments) : base()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                this.BtnCancel.IsEnabled = true;
                this.BtnCancel.IsVisible = true;
                this.RowCancelButtonModal.Height = new GridLength(60, GridUnitType.Absolute);
            }

            if (attachments != null && attachments.Count > 0)
            {
                base.LoadImage(attachments[0]);
            }
        }

        #endregion

        #region Methods

        protected override void Close()
        {
            this.Closed?.Invoke();
        }

        protected override void Logout()
        {
            this.LoggedOut?.Invoke();
        }

        #endregion
    }
}
