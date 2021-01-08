using Xamarin.Forms;
using ComplexXamarinFormsIosExtApp.Models;

namespace ComplexXamarinFormsIosExtApp.Pages
{
    public class MainPageModal: MainPage
    {
        #region Delegates

        public delegate void CloseHandler();

        #endregion

        #region Events

        public event CloseHandler Closed;

        #endregion

        #region Contsructors

        public MainPageModal()
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                this.BtnCancel.IsEnabled = true;
                this.BtnCancel.IsVisible = true;
                this.BtnCancel.Text = "Cancel";
                this.RowCancelButtonModal.Height = new GridLength(60, GridUnitType.Absolute);
            }
        }

        #endregion

        #region Methods

        protected override void Close()
        {
            this.Closed?.Invoke();
        }

        #endregion
    }
}
