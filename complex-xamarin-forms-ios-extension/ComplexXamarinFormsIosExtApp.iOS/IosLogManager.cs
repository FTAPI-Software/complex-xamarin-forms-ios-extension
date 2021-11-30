using Xamarin.Forms;
using ComplexXamarinFormsIosExtApp.Interfaces;
using ComplexXamarinFormsIosExtApp.Models;

[assembly: Dependency(typeof(ComplexXamarinFormsIosExtApp.iOS.IosLogManager))]
namespace ComplexXamarinFormsIosExtApp.iOS
{
    /// <summary>
    /// Wraps the console Manager, configuring it for IOS environment.
    /// </summary>
    public class IosLogManager : ILogManager
    {
        public void Initialize()
        {
            //Do nothing
        }

        public ILogger GetCurrentClassLogger()
        {
            return new IosLogger("iOSApplication");
        }

        public void ChangeMinimumLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Info:
                case LogLevel.Warn:
                    IosLogger.LogInfo = true;
                    IosLogger.LogWarn = true;
                    IosLogger.LogDebug = false;
                    IosLogger.LogTrace = false;
                    break;
                case LogLevel.Debug:
                    IosLogger.LogInfo = true;
                    IosLogger.LogWarn = true;
                    IosLogger.LogDebug = true;
                    IosLogger.LogTrace = false;
                    break;
                case LogLevel.Trace:
                    IosLogger.LogInfo = true;
                    IosLogger.LogWarn = true;
                    IosLogger.LogDebug = true;
                    IosLogger.LogTrace = true;
                    break;
            }
        }

        public string GetCurrentLogFilePath()
        {
            return string.Empty;
        }
    }
}