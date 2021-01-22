using System;
using System.Threading;
using System.Threading.Tasks;
using ComplexXamarinFormsIosExtApp.Models;
using ComplexXamarinFormsIosExtApp.Utilities;

namespace ComplexXamarinFormsIosExtApp.Controllers
{
    public class SessionController
    {
        #region Event Declarations

        public delegate void AuthorizationCompletedHandler(object sender, bool success, string errorMessage);

        #endregion

        #region Events

        public event AuthorizationCompletedHandler AuthorizationCompleted;

        #endregion

        #region Fields

        private static SessionController instance;

        protected UserAccount userAccount;
        protected bool isloggedIn;
        private readonly NLog.Logger log = LoggerFactory.GetCurrentClassLogger();

        #endregion

        #region Properties

        public static SessionController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SessionController();
                }
                return instance;
            }
        }

        public bool IsLoggedIn
        { 
            get
            {
                return this.isloggedIn;
            }
        }

        public UserAccount UserAccount
        {
            get
            {
                if (this.userAccount == null)
                {
                    this.userAccount = new UserAccount();
                }
                return this.userAccount;
            }
        }

        #endregion

        #region Methods

        public bool ValidateLoginFields(string server, string userName, string password)
        {
            if (string.IsNullOrWhiteSpace(server) || string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password) || server.Equals("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Login(string server, string userName, string password, bool savePassword, CancellationToken cancellationToken)
        {
            try
            {
                // Async Login code here
                Task.Run(() =>
                {
                    log.Debug("Logging in...");
                    int totalSecs = 5;
                    while (!cancellationToken.IsCancellationRequested && totalSecs > 0)
                    {
                        Thread.Sleep(1000);
                        totalSecs--;
                    }
                    this.isloggedIn = true;
                    OnAuthenticationCompleted(this, true, "");
                });

            }
            catch (Exception ex)
            {
                log.Error(ex, "Error during login " + ex.Message);
                OnAuthenticationCompleted(this, false, ex.Message);
            }
        }

        public void ClearAccountCache()
        {
            this.userAccount = null;
        }

        public void Logout()
        {
            this.userAccount.Clear();
            this.isloggedIn = false;
        }

        private void OnAuthenticationCompleted(object sender, bool success, string errorMessage)
        {
            this.AuthorizationCompleted?.Invoke(sender, success, errorMessage);
        }

        public static void DisposeSession()
        {
            if (instance != null)
            {
                instance.ClearAccountCache();
                instance = null;
            }
        }

        #endregion

        #region Event Handlers

        #endregion
    }
}
