using Foundation;
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using Xamarin.Forms;
using ComplexXamarinFormsIosExtApp.Interfaces;

[assembly: Dependency(typeof(ComplexXamarinFormsIosExtApp.iOS.IosLogManager))]
namespace ComplexXamarinFormsIosExtApp.iOS
{
    /// <summary>
    /// Wraps the NLog Manager, configuring it for IOS environment.
    /// </summary>
    public class IosLogManager : ILogManager
    {
        private const string GROUP_ID = "group.com.[your companyname].ComplexXamarinformsIosExtApp.iOS";

        private bool isInitialized;

        public void Initialize()
        {
            if (!this.isInitialized)
            {
                Assembly assembly = this.GetType().Assembly;
                string assemblyName = assembly.GetName().Name;

                string location = $"{assemblyName}.NLog.config";

                try
                {
                    using (Stream stream = assembly.GetManifestResourceStream(location))
                    {
                        using (XmlReader xmlReader = XmlReader.Create(stream))
                        {
                            LogManager.Configuration = new XmlLoggingConfiguration(xmlReader);
                        }
                    }
                    SetupSharedDirectoryFileTarget((FileTarget)LogManager.Configuration.FindTargetByName("file"), assemblyName);

                    Logger logger = LogManager.GetCurrentClassLogger();
                    logger.Info("Logging for iOS Initialized");
                    isInitialized = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Loading NLog Configuration: " + ex.Message);
                }
            }
        }

        public Logger GetCurrentClassLogger()
        {
            if (!this.isInitialized)
            {
                Initialize();
            }
            return LogManager.GetCurrentClassLogger();
        }

        public void ChangeMinimumLogLevel(LogLevel logLevel)
        {
            if (this.isInitialized)
            {
                foreach (LoggingRule rule in LogManager.Configuration.LoggingRules)
                {
                    rule.SetLoggingLevels(logLevel, LogLevel.Fatal);
                }
                Logger logger = LogManager.GetCurrentClassLogger();
                LogManager.ReconfigExistingLoggers();

                logger.Info("Minimum Logging for iOS Changed to: " + logLevel.ToString());
            }
        }

        public string GetCurrentLogFilePath()
        {
            string filePath = string.Empty;

            if (this.isInitialized)
            {
                FileTarget target = (FileTarget)LogManager.Configuration.FindTargetByName("file");
                
                if (target != null)
                {
                    var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
                    filePath = target.FileName.Render(logEventInfo);
                }
            }
            return filePath;
        }

        private void SetupSharedDirectoryFileTarget(FileTarget fileTarget, string assemblyName)
        {
            string fileName = "ComplexXamarinFormsIosExtApp.log";
            string archiveFileName = "ComplexXamarinFormsIosExtApp.{#}.zip";

            if (assemblyName.Contains("ActionExtension"))
            {
                fileName = "ComplexXamarinFormsIosExtAppActionExtension.log";
                archiveFileName = "ComplexXamarinFormsIosExtAppActionExtension.{#}.zip";
            }

            try
            {
                string sharedDirectory = NSFileManager.DefaultManager.GetContainerUrl(GROUP_ID).Path;
                fileTarget.FileName = new NLog.Layouts.SimpleLayout(sharedDirectory + @"/Library/Logs/" + fileName);
                fileTarget.ArchiveFileName = new NLog.Layouts.SimpleLayout(sharedDirectory + @"/Library/Logs/" + archiveFileName);
                LogManager.ReconfigExistingLoggers();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting sharedDirectory! " + ex.Message);
            }
        }
    }
}