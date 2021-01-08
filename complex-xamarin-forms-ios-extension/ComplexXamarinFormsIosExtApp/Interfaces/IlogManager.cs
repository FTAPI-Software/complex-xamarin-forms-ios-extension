using NLog;

namespace ComplexXamarinFormsIosExtApp.Interfaces
{
    public interface ILogManager
    {
        void Initialize();
        Logger GetCurrentClassLogger();
        void ChangeMinimumLogLevel(LogLevel logLevel);
        string GetCurrentLogFilePath();
    }
}
