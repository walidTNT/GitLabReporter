using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FwameworkChangesReporter
{
    public class Logger
    {
        private const string LogFileName = "log.txt";
        public static Logger Instance = new Logger();

        public static void Log(Exception ex)
        {
            Log(ex.ToString());
        }

        public static void Log(string message)
        {
            var logMessage = new StringBuilder();

            logMessage.Append("\r\nLog Entry : ");
            logMessage.AppendLine(string.Format("{0} {1}", DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortDateString()));
            logMessage.AppendLine(string.Empty);
            logMessage.AppendLine(message);
            logMessage.AppendLine("-------------------------------");

            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFileName);
            File.AppendAllText(logFilePath, logMessage.ToString());
        }
    }
}
