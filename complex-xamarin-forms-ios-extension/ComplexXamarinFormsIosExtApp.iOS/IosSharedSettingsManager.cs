using Foundation;
using Security;
using System;
using ComplexXamarinFormsIosExtApp.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(ComplexXamarinFormsIosExtApp.iOS.IosSharedSettingsManager))]
namespace ComplexXamarinFormsIosExtApp.iOS
{
    public class IosSharedSettingsManager : ISettingsManager
    {
        #region Fields

        private const string SERVICE_NAME = "ComplexXamarinFormsIosExtApp";
        private const string ACCESS_GROUP = "[Team ID].com.[your companyname].ComplexXamarinformsIosExtApp.iOS";
        private const string GROUP_ID = "group.com.[your companyname].ComplexXamarinformsIosExtApp.iOS";

        private static NSUserDefaults nsUserDefaults;

        #endregion

        #region Constructors

        public IosSharedSettingsManager()
        {
            Initializer();
        }

        #endregion

        #region Methods

        public bool SecuStorageSet(string key, string value)
        {
            SecRecord record = CreateGenericSecRecord(key);
            SecStatusCode resultCode;

            SecKeyChain.QueryAsRecord(record, out resultCode);
            if (resultCode == SecStatusCode.Success)
            {
                SecKeyChain.Remove(record);
            }

            record.ValueData = NSData.FromString(value, NSStringEncoding.UTF8);
            resultCode = SecKeyChain.Add(record);

            if (resultCode != SecStatusCode.Success)
            {
                throw new AccessViolationException("Failed to set key: " + key);
            }

            return true;
        }

        public string SecuStorageGet(string key)
        {
            string value = string.Empty;

            SecStatusCode resultCode;

            SecRecord existingRecord = CreateGenericSecRecord(key);
            existingRecord = SecKeyChain.QueryAsRecord(existingRecord, out resultCode);

            if (resultCode == SecStatusCode.Success)
            {
                value = NSString.FromData(existingRecord.ValueData, NSStringEncoding.UTF8);
            }

            return value;
        }

        public bool SecuStorageRemove(string key)
        {
            SecRecord existingRecord = CreateGenericSecRecord(key);
            SecStatusCode resultCode = SecKeyChain.Remove(existingRecord);

            if (resultCode != SecStatusCode.Success)
            {
                throw new AccessViolationException("Failed to remove of key: " + key);
            }

            return true;
        }

        public string Get(string key, string defaultValue)
        {
            string value = nsUserDefaults.StringForKey(new NSString(key));

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
            else
            {
                return defaultValue;
            }
        }

        public bool Get(string key)
        {
            return nsUserDefaults.BoolForKey(new NSString(key));
        }

        public int Get(string key, int defaultValue)
        {
            nint value = nsUserDefaults.IntForKey(new NSString(key));

            if (value != 0)
            {
                return (int)value;
            }
            else
            {
                return defaultValue;
            }
        }

        public void Set(string key, string value)
        {
            nsUserDefaults.SetString(value, key);
        }

        public void Set(string key, bool value)
        {
            nsUserDefaults.SetBool(value, key);
        }

        public void Set(string key, int value)
        {
            nsUserDefaults.SetInt(value, key);
        }

        public void Remove(string key)
        {
            nsUserDefaults.RemoveObject(key);
        }

        #endregion

        #region Static Methods
        private static void Initializer()
        {
            if (nsUserDefaults == null)
            {
                nsUserDefaults = CreateUserDefaults();
            }
        }

        private static SecRecord CreateGenericSecRecord(string key)
        {
            return new SecRecord(SecKind.GenericPassword)
            {
                Service = SERVICE_NAME,
                Account = key,
                AccessGroup = ACCESS_GROUP,
                Accessible = SecAccessible.AlwaysThisDeviceOnly,
                Synchronizable = false
            };
        }

        private static NSUserDefaults CreateUserDefaults()
        {
            NSUserDefaults userDefaults = new NSUserDefaults(GROUP_ID, NSUserDefaultsType.SuiteName);

            try
            {
                userDefaults.Synchronize();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to synchronize on NSUserDefaults creation: {0}", ex.Message);
            }
            return userDefaults;
        }

        #endregion
    }
}