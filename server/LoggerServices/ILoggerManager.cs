using System;
using System.Collections.Generic;
using System.Text;

namespace LoggerServices
{
    /* interface for logging info, warn, debug and error messages */
    public interface ILoggerManager
    {
        void LogInfo(string message);
        void LogWarn(string message);
        void LogDebug(string message);
        void LogError(string message);
    }
}
