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

        private static void Setup()
        {
            string logDirectory = ConfigurationManager.AppSettings.Get("LogDirectory");
            string filename = ConfigurationManager.AppSettings["LogFileName"];

            if(string.IsNullOrEmpty(logDirectory))  //If logPath is  empty
            {
                //If directory is not given, then this line selects the current directory of the application i.e inside the Debug Folder
                logDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName); 
            }

            if(string.IsNullOrEmpty(filename))
            {
                //If filename is not given then by default give it a name
                filename = "TMSLog.log"; 
            }

            string logPath = Path.Combine(logDirectory, filename); //Combine both to make a complete path
            logger = new LoggerConfiguration()
                .WriteTo.File(logPath, rollingInterval:RollingInterval.Day).CreateLogger();
            // RolllingInterval.Day indicates that the new logfile should be created daily.
        }

    }
}
