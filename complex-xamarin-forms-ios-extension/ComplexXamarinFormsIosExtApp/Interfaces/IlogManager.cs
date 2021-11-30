using ComplexXamarinFormsIosExtApp.Models;

namespace ComplexXamarinFormsIosExtApp.Interfaces
{
    public interface ILogManager
    {
        void Initialize();
        ILogger GetCurrentClassLogger();
        void ChangeMinimumLogLevel(LogLevel logLevel);
        string GetCurrentLogFilePath();
    }
}
