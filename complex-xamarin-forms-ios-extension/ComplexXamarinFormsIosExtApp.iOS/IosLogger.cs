using ComplexXamarinFormsIosExtApp.Interfaces;
using System;

namespace ComplexXamarinFormsIosExtApp.iOS
{
    public class IosLogger : ILogger
    {
        #region Fields

        private const string CONSOLE_STR_FORMAT = "[{0}] {1} {2} {3}";

        private string className;

        #endregion

        #region Properties

        private string DateNow
        {
            get
            {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
        }

        public static bool LogTrace { get; set; }
        public static bool LogDebug { get; set; }
        public static bool LogWarn { get; set; }
        public static bool LogInfo { get; set; }

        #endregion

        #region Constructors

        public IosLogger()
        {
        }

        public IosLogger(string className)
        {
            this.className = className;
        }

        #endregion

        #region Methods

        public void Debug(string message)
        {
            if (LogDebug)
                this.WriteMessage("DEBUG", this.className, message);
        }

        public void Debug(Exception ex, string message)
        {
            if (LogDebug)
                this.WriteMessage("DEBUG", this.className, message, ex);
        }

        public void Error(string message)
        {
            this.WriteMessage("ERROR", this.className, message);
        }

        public void Error(Exception ex, string message)
        {
            this.WriteMessage("ERROR", this.className, message, ex);
        }

        public void Fatal(string message)
        {
            this.WriteMessage("FATAL", this.className, message);
        }

        public void Fatal(Exception ex, string message)
        {
            this.WriteMessage("FATAL", this.className, message);
        }

        public void Info(string message)
        {
            if (LogInfo)
                this.WriteMessage("INFO ", this.className, message);
        }

        public void Info(Exception ex, string message)
        {
            if (LogInfo)
                this.WriteMessage("INFO ", this.className, message, ex);
        }

        public void Trace(string message)
        {
            if (LogTrace)
                this.WriteMessage("TRACE", this.className, message);
        }

        public void Trace(Exception ex, string message)
        {
            if (LogTrace)
                this.WriteMessage("TRACE", this.className, message, ex);
        }

        public void Warn(string message)
        {
            if (LogWarn)
                this.WriteMessage("WARN ", this.className, message);
        }

        public void Warn(Exception ex, string message)
        {
            if (LogWarn)
                this.WriteMessage("WARN ", this.className, message, ex);
        }

        private void WriteMessage(string level, string className, string message)
        {
            Console.WriteLine(string.Format(CONSOLE_STR_FORMAT, level, this.DateNow, className, message));
        }

        private void WriteMessage(string level, string className, string message, Exception ex)
        {
            WriteMessage(level, className, message);
            WriteMessage(level, className, ex.Message);
        }

        #endregion
    }
    
}