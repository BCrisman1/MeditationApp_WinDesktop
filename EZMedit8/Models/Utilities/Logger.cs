using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace EZMedit8.Models.Utilities
{
    public class Logger
    {
        private static readonly Lazy<Logger> _lazySessionDataInstance = new Lazy<Logger>(() => new Logger());
        public static Logger Instance => _lazySessionDataInstance.Value;

        private Logger()
        {
            // Empty Constructor
        }

        public async Task LogException(Exception exception)
        {
            string message = exception.Message;
            message += "\n" + exception.StackTrace;

            File.WriteAllText(GetLogPath(), message);
            await Task.CompletedTask;
        }

        private string GetLogPath()
        {
            var logsFolder = Statics.GetFolder(FolderType.Logs);
            Directory.CreateDirectory(logsFolder);
            var logNamePrefix = "Exception";
            var timestamp = DateTime.Now.ToString("_MM.dd.yyyy_hh.mm.ss");
            var extension = ".txt";
            var filename = $"{logNamePrefix}{timestamp}{extension}";
            Trace.WriteLine(Path.Combine(logsFolder, filename));
            return Path.Combine(logsFolder, filename);
        }
    }
}
