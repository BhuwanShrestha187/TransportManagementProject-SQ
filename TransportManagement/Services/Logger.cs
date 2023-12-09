using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Serilog;
using Serilog.Events;
using System.IO.Abstractions;

namespace TransportManagement
{
    // Enum to convert into log titles
    public enum LogLevel
    {
        Trace, Debug, Information, Warning, Error, Critical
    }
    public class Logger
    {
        private static bool isSetup = false;
        private static ILogger logger;
        private static readonly IFileSystem fileSystem = new FileSystem();

        //To set up the log class if it is not set up already
        private static void Setup()
        {
            string logDirectory = ConfigurationManager.AppSettings.Get("LogDirectory");
            string logFileName = ConfigurationManager.AppSettings.Get("LogFileName");

            if ((string.IsNullOrEmpty(logDirectory)))
            {
                logDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }

            if ((string.IsNullOrEmpty(logFileName)))
            {
                logFileName = "TMSLogfile.log";
            }

            string logFilePath = Path.Combine(logDirectory, logFileName);
            logger = new LoggerConfiguration()
           .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
            .CreateLogger();
        }

        //Check if the logger is set up, if not, set it up. Call the writing function
        public static void Log(string message, LogLevel level)
        {
            if (logger == null)
            {
                Setup();
            }
            switch (level)
            {
                case LogLevel.Trace:
                    logger.Verbose(message);
                    break;
                case LogLevel.Debug:
                    logger.Debug(message);
                    break;
                case LogLevel.Information:
                    logger.Information(message);
                    break;
                case LogLevel.Warning:
                    logger.Warning(message);
                    break;
                case LogLevel.Error:
                    logger.Error(message);
                    break;
                case LogLevel.Critical:
                    logger.Fatal(message);
                    break;
            }
        }

        //Change the directory of the log file
        public static int ChangeLogDirectory(string newDirectory)
        {
            string oldDirectory = ConfigurationManager.AppSettings.Get("LogDirectory");

            if (oldDirectory == newDirectory)
            {
                return 0;
            }

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["LogDirectory"].Value = newDirectory;
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");

            try
            {
                if (isSetup)
                {
                    Logger.Log($"Log directory changed from \"{oldDirectory}\" to \"{newDirectory}\"", LogLevel.Information);

                    if (Trace.Listeners.Count == 3)
                    {
                        Trace.Listeners.Remove(Trace.Listeners[2]);
                    }

                    isSetup = false;
                }

                UpdateLogFileInNewDirectory(oldDirectory, newDirectory);

                if (!isSetup)
                {
                    Setup();
                }

                return 0;
            }
            catch (Exception e)
            {
                Logger.Log($"We couldn't change the log directory. {e.Message}", LogLevel.Error);

                configuration.AppSettings.Settings["LogDirectory"].Value = oldDirectory;
                configuration.Save(ConfigurationSaveMode.Full, true);
                ConfigurationManager.RefreshSection("appSettings");
                return 1;
            }
        }

        //Move the log file to the new directory
        public static void UpdateLogFileInNewDirectory(string oldDirectory, string newDirectory)
        {
            string logFileName = ConfigurationManager.AppSettings.Get("LogFileName");
            string oldPath = fileSystem.Path.Combine(oldDirectory, logFileName);
            string newPath = fileSystem.Path.Combine(newDirectory, logFileName);

            try
            {
                if (!fileSystem.Directory.Exists(newDirectory))
                {
                    fileSystem.Directory.CreateDirectory(newDirectory);
                }

                if (fileSystem.File.Exists(oldPath))
                {
                    fileSystem.File.Copy(oldPath, newPath, true);
                }
            }
            catch (Exception)
            {
                throw;
            }

            if (isSetup)
            {
                Logger.Log($"Log file {logFileName} was moved from \"{oldDirectory}\" to \"{newDirectory}\"", LogLevel.Information);
            }
        }

        //Get the current directory of the log
        public static string GetCurrentLogDirectory()
        {
            string logDirectory = ConfigurationManager.AppSettings.Get("LogDirectory");
            string logFileName = ConfigurationManager.AppSettings.Get("LogFileName");

            if (string.IsNullOrEmpty(logDirectory))
            {
                logDirectory = fileSystem.Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }

            if (string.IsNullOrEmpty(logFileName))
            {
                logFileName = "TMSLogfile.log";
            }

            return fileSystem.Path.Combine(logDirectory, logFileName);
        }


    }


}
