using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ComplexXamarinFormsIosExtApp.Utilities;
using ComplexXamarinFormsIosExtApp.Interfaces;

namespace ComplexXamarinFormsIosExtApp.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorPage : ContentPage
    {
        #region Fields

        private readonly ILogger log = LoggerFactory.GetCurrentClassLogger();

        #endregion

        #region Properties

        protected Grid GrdContent { get { return this.grdContent; } }

        #endregion

        #region Constructors

        public ErrorPage(string errorMessage)
        {
            InitializeComponent();

            this.lblMessage.Text = errorMessage;
            this.btnClose.Text = "Back";
        }

        #endregion

        #region Methods
        protected async virtual void Close()
        {
            await Navigation.PopAsync();
        }

        #endregion

        #region Event Handlers

        private void btnClose_Clicked(object sender, System.EventArgs e)
        {
            log.Info("Close Clicked");

            this.Close();
        }

        #endregion
    }
}