using System;
using System.ComponentModel;
using System.Threading;
using Xamarin.Forms;
using ComplexXamarinFormsIosExtApp.Controllers;
using ComplexXamarinFormsIosExtApp.Models;

namespace ComplexXamarinFormsIosExtApp.Pages
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class LoginPage : ContentPage
    {
        #region Fields

        private readonly NLog.Logger log = Utilities.LoggerFactory.GetCurrentClassLogger();
        private bool loginInProgress;
        protected CancellationTokenSource loginCancellationTokenSource;

        #endregion

        #region Properties

        protected Button BtnCancel { get { return this.btnCancel; } }
        protected RowDefinition RowCancelButtonModal { get { return this.rowCancelButtonModal; } }
        protected Grid GrdContent { get { return this.grdContent; } }

        #endregion

        #region Constructors

        public LoginPage()
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(SessionController.Instance.UserAccount.Server))
            {
                LoadCredentials(SessionController.Instance.UserAccount);
            }
        }

        #endregion

        #region Methods

        public void RefreshCredentials()
        {
            if (!this.loginInProgress)
            {
                SessionController.Instance.ClearAccountCache();
                LoadCredentials(SessionController.Instance.UserAccount);
            }
        }

        private void LoadCredentials(UserAccount UserAccount)
        {
            if (UserAccount != null)
            {
                if (string.IsNullOrWhiteSpace(UserAccount.Server))
                {
                    this.txtServer.Text = @"https://";
                }
                else
                {
                    this.txtServer.Text = UserAccount.Server;
                }

                this.txtUserName.Text = UserAccount.UserName;
                this.chkSavePassword.IsToggled = UserAccount.SavePassword;

                if (this.chkSavePassword.IsToggled)
                {
                    this.txtPassword.Text = UserAccount.Password;

                    if (!string.IsNullOrWhiteSpace(this.txtPassword.Text))
                    {
                        this.Login();
                    }
                }
            }
        }

        public void SaveCredentials(UserAccount UserAccount)
        {
            if (UserAccount != null)
            {
                UserAccount.Server = this.txtServer.Text.Trim();
                UserAccount.UserName = this.txtUserName.Text.Trim();
                UserAccount.SavePassword = this.chkSavePassword.IsToggled;
                UserAccount.Password = this.txtPassword.Text;
                UserAccount.Save();
            }
        }

        private void Login()
        {
            if (this.loginInProgress)
            {
                this.loginCancellationTokenSource.Cancel();
            }
            else
            {
                bool passed = SessionController.Instance.ValidateLoginFields(this.txtServer.Text.Trim(), this.txtUserName.Text.Trim());
                if (passed)
                {
                    this.loginInProgress = true;
                    this.EnableInput(false);
                    this.loginCancellationTokenSource = new CancellationTokenSource();

                    SessionController.Instance.AuthorizationCompleted += Instance_AuthorizationCompleted;
                    SessionController.Instance.Login(this.txtServer.Text.Trim(), this.txtUserName.Text.Trim(), this.txtPassword.Text.Trim(), this.chkSavePassword.IsToggled, this.loginCancellationTokenSource.Token);
                }
                else
                {
                    this.DisplayError("Invalid loging Details");
                }
            }
        }

        protected virtual void CancelLogin()
        {
            if (this.loginCancellationTokenSource != null)
            {
                this.loginCancellationTokenSource.Cancel();
            }
        }

        protected virtual void DisplayLoginError(string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                log.Error("Error during Authorization: " + errorMessage);
                this.DisplayError(errorMessage);
            }
            this.EnableInput(true);
        }

        public void EnableInput(bool enabled)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (enabled)
                {
                    this.btnLogin.Text = "Login";
                }
                else
                {
                    this.btnLogin.Text = "Cancel";
                }

                this.txtServer.IsEnabled = enabled;
                this.txtServer.IsVisible = enabled;
                this.txtUserName.IsEnabled = enabled;
                this.txtUserName.IsVisible = enabled;
                this.txtPassword.IsEnabled = enabled;
                this.txtPassword.IsVisible = enabled;
                this.lblLoginStatus.IsVisible = !enabled;
                this.chkSavePassword.IsEnabled = enabled;
                this.chkSavePassword.IsVisible = enabled;
                this.lblSavePassword.IsVisible = enabled;

                this.spinLoggingIn.IsRunning = !enabled;
                this.spinLoggingIn.IsVisible = !enabled;
            });
        }

        private void DisplayError(string erroMessage)
        {
            if (!string.IsNullOrWhiteSpace(erroMessage))
            {
                DisplayAlert("Error", erroMessage, "OK");
            }
        }

        protected async virtual void DisplayMainPage()
        {
            await Navigation.PushAsync(new MainPage());
            Navigation.RemovePage(this);
        }

        #endregion

        #region Event Handlers

        private void Login_Clicked(object sender, EventArgs e)
        {
            log.Debug("Login Clicked");

            this.Login();
        }

        private void Instance_AuthorizationCompleted(object sender, bool success, string errorMessage)
        {
            SessionController.Instance.AuthorizationCompleted -= Instance_AuthorizationCompleted;

            Device.BeginInvokeOnMainThread(() =>
            {
                if (success)
                {
                    this.DisplayMainPage();
                }
                else
                {
                    this.loginInProgress = false;
                    this.DisplayLoginError(errorMessage);
                    this.EnableInput(true);
                }
            });
        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            this.CancelLogin();
        }

        #endregion
    }
}
