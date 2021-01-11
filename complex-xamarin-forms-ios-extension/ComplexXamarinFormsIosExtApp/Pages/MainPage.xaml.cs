using ComplexXamarinFormsIosExtApp.Controllers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ComplexXamarinFormsIosExtApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        #region Fields

        private readonly NLog.Logger log = Utilities.LoggerFactory.GetCurrentClassLogger();

        #endregion

        #region Properties

        protected Button BtnCancel { get { return this.btnCancel; } }
        protected RowDefinition RowCancelButtonModal { get { return this.rowCancelButtonModal; } }
        protected Grid GrdContent { get { return this.grdContent; } }

        #endregion

        #region Constructors

        public MainPage()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        protected virtual void Close()
        {
            //Do Nothing
        }

        protected async virtual void Logout()
        {
            SessionController.Instance.Logout();
            await Navigation.PushAsync(new LoginPage());
            Navigation.RemovePage(this);
        }

        #endregion

        #region Event Handlers

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            log.Debug("Cancel Clicked");

            this.Close();

        }
        private void btnLogout_Clicked(object sender, EventArgs e)
        {
            log.Debug("Logout Clicked");

            this.Logout();
        }

        #endregion
    }
}