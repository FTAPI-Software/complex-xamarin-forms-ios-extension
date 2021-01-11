using ComplexXamarinFormsIosExtApp.Pages;
using NLog;
using Xamarin.Forms;

namespace ComplexXamarinFormsIosExtApp
{
    public partial class App : Application
    {
        private Logger log;

        public App()
        {
            InitializeComponent();

            Utilities.LoggerFactory.Initialize();
            log = Utilities.LoggerFactory.GetCurrentClassLogger();

            Utilities.LoggerFactory.ChangeMinimumLogLevel(NLog.LogLevel.Debug);

            MainPage = new NavigationPage(new LoginPage());
        }

        private Page CurrentPage()
        {
            Page currentPage = null;

            if (Application.Current.MainPage.Navigation.NavigationStack.Count == 1)
            {
                currentPage = Application.Current.MainPage.Navigation.NavigationStack[0];
            }

            return currentPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            log.Debug("OnStart called");
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            log.Debug("OnSleep called");
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            log.Debug("OnResume called");

            Page currentPage = CurrentPage();

            if (currentPage != null)
            {
                //If current page presented is LoginPage, refresh it just in case credentials have been entered in the Application Extension
                LoginPage loginPage = currentPage as LoginPage;

                if (loginPage != null)
                {
                    loginPage.RefreshCredentials();
                }
            }
        }
    }
}
