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
using System.Xml.Serialization;
using System.IO.IsolatedStorage;

namespace TransportManagement
{
    public enum LogLevel //To determine the loglevel
    { 
        Verbose, Debug, Information, Warning, Error, Fatal
    };
    public class Logger
    {
        private static bool isSetup = false; //used to track whether the logger has been set up. This prevents unnecessary repeated setup attempts.
        private static ILogger logger;  //It holds the instance of Serilog logger. 
        private static readonly IFileSystem filesystem = new FileSystem(); // It is a interface in Serilog.Sinks.IO It is used for file-based logging. 

        /*
         * Configure and initialize the Serilog logger. It checks for Configuration Setting related to the log directory
           and filename using ConfigurationManager. If these are provided then it sets the directory and filename for the log. 
           
         */

        private static void LogSetup()
        {
            string logDirectory = ConfigurationManager.AppSettings.Get("LogDirectory");
            string filename = ConfigurationManager.AppSettings["LogFileName"];

            if (string.IsNullOrEmpty(logDirectory))  //If logPath is  empty
            {
                //If directory is not given, then this line selects the current directory of the application i.e inside the Debug Folder
                logDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }

            if (string.IsNullOrEmpty(filename))
            {
                //If filename is not given then by default give it a name
                filename = "TMSLogFile.log";
            }

            string logPath = Path.Combine(logDirectory, filename); //Combine both to make a complete path
            logger = new LoggerConfiguration()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day).CreateLogger();
            // RolllingInterval.Day indicates that the new logfile should be created daily.
        }

        /*
         * This function is a utility method for logging messages using Serilog.
         * message: The actual log message that I want to record
         * level: The log level indicating the severity of the message. 
         */

        public static void Log(string message, LogLevel level)
        {
            if (logger == null)
            {
                LogSetup();
            }

            switch (level)
            {
                case LogLevel.Verbose:
                    logger.Verbose(message);
                    break;

                case LogLevel.Debug:
                    logger.Debug(message);
                    break;

                case LogLevel.Warning:
                    logger.Warning(message);
                    break;
                case LogLevel.Error:
                    logger.Error(message);
                    break;
                case LogLevel.Fatal:
                    logger.Fatal(message);
                    break;

                default:
                    break;
            }
        }


        /*
         * Admin Functionality: Can change the log Directory. 
         */

        public static int ChangeLogDirectory(string newLogDirectory)
        {
            try
            {
                string oldDirectory = ConfigurationManager.AppSettings["LogDirectory"];
                if (oldDirectory == newLogDirectory)
                {
                    return 0;
                }

                UpdateConfiguration(newLogDirectory);
                if (isSetup)
                {
                    Logger.Log($"Log Directroy changed from \"{oldDirectory}\" to \"{newLogDirectory}\"", LogLevel.Information);
                    UpdateTraceListeners(oldDirectory);
                    isSetup = false;
                }

                MoveLogFileInNewDirectory(oldDirectory, newLogDirectory);

                if (!isSetup)
                {
                    LogSetup();
                }

                return 0; //Success
            }

            catch (Exception e)
            {
                HandleError(e);
                return 1; //Failure
            }
        }


        //To update the new directory in the config file
        private static void UpdateConfiguration(string newDirectory)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configuration.AppSettings.Settings["LogDirectory"].Value = newDirectory;
            configuration.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
        }


        private static void UpdateTraceListeners(string oldDirectory)
        {
            if (Trace.Listeners.Count == 3)
                Trace.Listeners.Remove(Trace.Listeners[2]);
        }

        private static void HandleError(Exception e)
        {
            Logger.Log($"Failed to change the log directory. {e.Message}", LogLevel.Error);

            //Revert the changes in the configuration
            string oldDirectory = ConfigurationManager.AppSettings["LogDirectory"];
            UpdateConfiguration(oldDirectory);
        }


        /*
         * Move the log file to the new directory
         */
        public static void MoveLogFileInNewDirectory(string oldDirectory, string newDirectory)
        {
            try
            {
                //Get the log filename in the old Directory
                string logFilename = ConfigurationManager.AppSettings["LogFileName"];

                //Combine newDirectory with filename
                string newFilePath = Path.Combine(newDirectory, logFilename);

                //Check if the log file exists
                if(File.Exists(logFilename))
                {
                    if(!Directory.Exists(newFilePath))
                    {
                        Directory.CreateDirectory(newFilePath);
                    }
                }

            
                File.Move(logFilename, newFilePath);
            }

            catch(Exception e)
            {
                Logger.Log($"Error MOving the log file: {e}", LogLevel.Error);
            }
            
        }


        /*
         * Get the current directory of the log
         */ 

        public static string GetCurrentLogDirectory()
        {
            string logDirectory = ConfigurationManager.AppSettings["LogDirectory"];
            string logFileName = ConfigurationManager.AppSettings["LogFileName"]; 

            if(string.IsNullOrEmpty(logDirectory))
            {
                logDirectory = AppDomain.CurrentDomain.BaseDirectory; //This is the way to get the durectory where the executable is located.
            }

            if(String.IsNullOrEmpty(logFileName))
            {
                logFileName = "TMSLogFile.log"; 
            }

            return filesystem.Path.Combine(logDirectory, logFileName);
        }

    }
}
