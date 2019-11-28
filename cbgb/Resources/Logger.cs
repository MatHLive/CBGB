using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;
using System.Reflection;

namespace cbgb.Resources
{
    class Logger
    {
        private static string basedir = Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location);
        public static NLog.Logger Log { get; private set; }
        static Logger()
        {
            var config = new LoggingConfiguration();
            var fileTarget = new FileTarget("fileTarget")
            {
                FileName = $"{basedir}/Log/log.log",
                Layout = "${longdate} ${level} ${message} ${exception}"
            };

            config.AddTarget(fileTarget);
            config.AddRuleForOneLevel(LogLevel.Info, fileTarget);

            LogManager.Configuration = config;

            Log = LogManager.GetLogger("Logger");
        }

        public static void EnableDebug(bool enable)
        {
            if (enable)
                UpdateLoggingLevelForAllRules(LogLevel.Trace);
            else
                UpdateLoggingLevelForAllRules(LogLevel.Info);
        }

        public static void Error(Exception e, string message)
        {
            Log.Error(e, message);
        }

        private static void UpdateLoggingLevelForAllRules(LogLevel level)
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
                rule.EnableLoggingForLevel(level);

            LogManager.ReconfigExistingLoggers();
        }
    }
}
