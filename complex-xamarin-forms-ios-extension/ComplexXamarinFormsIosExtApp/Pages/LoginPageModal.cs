using Xamarin.Forms;

namespace ComplexXamarinFormsIosExtApp.Pages
{
    public class LoginPageModal: LoginPage
    {
        #region Delegates

        public delegate void AuthorizationCompletedHandler(bool success, string errorMessage);
        public delegate void LoginCancelledHandler();

        #endregion

        #region Events

        public event AuthorizationCompletedHandler AuthorizationCompleted;
        public event LoginCancelledHandler LoginCancelled;

        #endregion

        #region Constructors

        public LoginPageModal() : base()
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

        protected override void DisplayError(string errorMessage)
        {
            this.AuthorizationCompleted?.Invoke(false, errorMessage);
        }

        protected override void DisplayMainPage()
        {
            this.AuthorizationCompleted?.Invoke(true, "");
        }

        protected override void CancelLogin()
        {
            base.CancelLogin();
            this.LoginCancelled?.Invoke();
        }

        #endregion
    }
}
