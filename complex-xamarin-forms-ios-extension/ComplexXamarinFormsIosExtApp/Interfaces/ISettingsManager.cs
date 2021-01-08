namespace ComplexXamarinFormsIosExtApp.Interfaces
{
    public interface ISettingsManager
    {
        string Get(string key, string defaultValue);
        bool Get(string key);
        int Get(string key, int defaultValue);
        void Set(string key, string value);
        void Set(string key, bool value);
        void Set(string key, int value);
        void Remove(string key);
        string SecuStorageGet(string key);
        bool SecuStorageSet(string key, string value);
        bool SecuStorageRemove(string key);
    }
}
