using LoggerDeclare;
using System;

namespace DefaultLogger
{
    public class DefaultLoggerImp: ILogger
    {
        private readonly NLog.Logger _logger;
        public DefaultLoggerImp()
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }
        public void WriteInfoLog(string message, Exception ex)
        {
            _logger.Info(ex,message);
        }
        public void WriteInfoLog(string message)
        {
            _logger.Info(message);
        }
    }
}
