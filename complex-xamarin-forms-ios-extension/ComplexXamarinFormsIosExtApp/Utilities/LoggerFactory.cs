using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using ComplexXamarinFormsIosExtApp.Interfaces;

namespace ComplexXamarinFormsIosExtApp.Utilities
{
    public static class LoggerFactory
    {
        private static bool isInitialized;
        private static ILogManager logManager;

        public static void Initialize()
        {
            if (!isInitialized)
            {
                logManager = DependencyService.Get<ILogManager>();
                logManager.Initialize();
                isInitialized = true;
            }
        }

        public static Logger GetCurrentClassLogger()
        {
            if (!isInitialized)
            {
                Initialize();
            }
            return logManager.GetCurrentClassLogger();
        }

        public static void ChangeMinimumLogLevel(LogLevel logLevel)
        {
            if (isInitialized)
            {
                logManager.ChangeMinimumLogLevel(logLevel);
            }
        }

        public static string[] GetLogFilePaths(bool includeArchived = false)
        {
            List<string> logsFilesList = new List<string>();
            DirectoryInfo logDirectory = GetCurrentLogDirectory();

            if (logDirectory == null || !logDirectory.Exists)
            {
                logsFilesList.Add(GetCurrentLogFilePath());
            }
            else
            {
                logsFilesList.AddRange(GetFilePathsFromDirectory(logDirectory, "*.log"));

                if (includeArchived)
                {
                    logsFilesList.AddRange(GetFilePathsFromDirectory(logDirectory, "*.zip"));
                }
            }
            return logsFilesList.ToArray();
        }

        private static List<string> GetFilePathsFromDirectory(DirectoryInfo logDirectory, string searchPattern)
        {
            List<string> paths = new List<string>();

            FileInfo[] logFiles = logDirectory.GetFiles(searchPattern);

            if (logFiles != null && logFiles.Length > 0)
            {
                paths.AddRange(logFiles.Select(x => x.FullName).ToArray());
            }
            return paths;
        }

        private static DirectoryInfo GetCurrentLogDirectory()
        {
            DirectoryInfo logDirectory = null;
            string path = GetCurrentLogFilePath();

            if (!string.IsNullOrWhiteSpace(path))
            {
                FileInfo fileInfo = new FileInfo(path);
                logDirectory = fileInfo.Directory;
            }

            return logDirectory;
        }

        private static string GetCurrentLogFilePath()
        {
            if (isInitialized)
            {
                return logManager.GetCurrentLogFilePath();
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
