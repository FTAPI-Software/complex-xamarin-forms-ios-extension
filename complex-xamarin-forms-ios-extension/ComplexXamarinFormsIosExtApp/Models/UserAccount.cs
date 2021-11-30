using System;
using Xamarin.Forms;
using ComplexXamarinFormsIosExtApp.Interfaces;

namespace ComplexXamarinFormsIosExtApp.Models
{
    public class UserAccount
    {
        #region Const

        private const string SERVER_ADDRESS = "server_address";
        private const string USER_NAME = "user_name";
        private const string SAVE_PASSWORD = "save_password";
        private const string PASSWORD = "password";

        #endregion

        #region Fields

        private readonly ILogger log = Utilities.LoggerFactory.GetCurrentClassLogger();
        private readonly ISettingsManager preferences = DependencyService.Get<ISettingsManager>();

        #endregion

        #region Properties

        public virtual string Server { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual bool SavePassword { get; set; }

        #endregion

        #region Constructors

        public UserAccount()
        {
            this.Load();
        }

        #endregion

        #region Methods

        public void Load()
        {
            this.Server = this.preferences.Get(SERVER_ADDRESS, string.Empty);
            this.UserName = this.preferences.Get(USER_NAME, string.Empty);
            this.SavePassword = this.preferences.Get(SAVE_PASSWORD);

            log.Debug("Got existing credentials Server: " + this.Server + " UserName: " + this.UserName + " SavePassword: " + this.SavePassword.ToString());

            if (this.SavePassword)
            {
                try
                {
                    string password = this.preferences.SecuStorageGet(PASSWORD);
                    this.Password = (password != null ? password: string.Empty);
                }
                catch (Exception ex)
                {
                    // Possible that device doesn't support secure storage on device.
                    log.Error(ex, "Error loading password! " + ex.Message);
                }
            }
        }

        public void Save()
        {
            log.Debug("Saving Server: " + this.Server + " UserName: " + this.UserName + " SavePassword: " + this.SavePassword.ToString());

            this.preferences.Set(SERVER_ADDRESS, this.Server);
            this.preferences.Set(USER_NAME, this.UserName);
            this.preferences.Set(SAVE_PASSWORD, this.SavePassword);

            try
            {
                if (this.SavePassword && !string.IsNullOrWhiteSpace(this.Password))
                {
                    this.preferences.SecuStorageSet(PASSWORD, this.Password);
                    log.Debug("Password saved successfully");
                }
                else
                {
                    this.preferences.SecuStorageRemove(PASSWORD);
                    log.Debug("Password removed successfully");
                }
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                log.Error(ex, "Error saving password! " + ex.Message);
            }
        }

        public void Clear()
        {
            log.Debug("Clearing UserName: " + this.UserName + " and Password");
            this.UserName = string.Empty;
            this.Password = string.Empty;
            this.ClearSavedPassword();
            log.Debug("Cleared username and password");
        }

        public void ClearSavedPassword()
        {
            this.preferences.Remove(USER_NAME);

            try
            {
                this.preferences.SecuStorageRemove(PASSWORD);
            }
            catch (Exception ex)
            {
                // Possible that device doesn't support secure storage on device.
                log.Error(ex, "Error removing password! " + ex.Message);
            }
        }

        #endregion
    }
}
