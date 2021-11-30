using System;
namespace ComplexXamarinFormsIosExtApp.Interfaces
{
    public interface ILogger
    {
        void Fatal(string message);
        void Fatal(Exception ex, string message);
        void Error(string message);
        void Error(Exception ex, string message);
        void Trace(string message);
        void Trace(Exception ex, string message);
        void Debug(string message);
        void Debug(Exception ex, string message);
        void Warn(string message);
        void Warn(Exception ex, string message);
        void Info(string message);
        void Info(Exception ex, string message);
    }
}
