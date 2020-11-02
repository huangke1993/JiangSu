using System;

namespace LoggerDeclare
{
    public interface ILogger
    {
        void WriteInfoLog(string message);
        void WriteInfoLog(string message, Exception ex);
    }
}
