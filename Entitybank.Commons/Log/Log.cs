using log4net;
using log4net.Config;
using System;
using System.IO;

namespace XData.Data.Diagnostics
{
    public static class Log
    {
        private static ILog Logger = null;

        static Log()
        {
            string fileName = "log4net.config";
            if (!File.Exists(fileName))
            {
                fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            }
            FileInfo file = new FileInfo(fileName);
            XmlConfigurator.ConfigureAndWatch(file);
            Logger = LogManager.GetLogger("logger");
        }

        public static void Error(object message)
        {
            Logger.Error(message);
        }

        public static void Error(object message, Exception exception)
        {
            Logger.Error(message, exception);
        }


    }
}