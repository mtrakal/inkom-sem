using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Logger
    {
        public enum Type
        {
            WARNING,
            ERROR,
            DEBUG,
            INFO
        }
        private static Logger instance = null;
        private static object zamek = new object();

        private Logger()
        {
        }
        public static Logger GetInstance()
        {
            if (instance == null)
            {
                // je to tu dvakrát, protože lock má velkou režii oproti pouhé podmínce instance==null, proto se obalí ještě předtím
                lock (zamek)
                {
                    if (instance == null)
                    {
                        instance = new Logger();
                    }
                }
            }
            return instance;
        }

        public void Log(String message)
        {
            Log(message, Type.DEBUG);
        }
        public void Log(String message, Type messageType)
        {
            String description = "";
            switch (messageType)
            {
                case Type.WARNING:
                    description = "Warning";
                    break;
                case Type.ERROR:
                    description = "Error";
                    break;
                case Type.DEBUG:
                    description = "Debug";
                    break;
                case Type.INFO:
                    description = "Info";
                    break;
            }
            Log(message, messageType, description);
        }

        public void Log(string message, Type messageType, string description)
        {
            ConsoleColor originalColor = Console.ForegroundColor;

            switch (messageType)
            {
                case Type.WARNING:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case Type.ERROR:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case Type.DEBUG:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case Type.INFO:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }

            Console.WriteLine(description + ": " + message);
            Console.ForegroundColor = originalColor;
        }

    }
}
