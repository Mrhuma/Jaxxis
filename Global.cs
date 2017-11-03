using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Jaxxis
{
    //Global static variables
    public class Global
    {
        public static bool isFirstLaunch;
        public static string botToken = "";
        public static string filePath = @"..\..\Data\";
        public static JSONHelper JsonHelper = new JSONHelper();
        public static HiddenData hiddenData;

        public static void Initialize()
        {
            hiddenData = JsonHelper.JsonDeserialize();

            isFirstLaunch = hiddenData.IsFirstLaunch;
            botToken = hiddenData.BotToken;
        }

        //Logs messages to console(with color coding) and writes to /Data/MessageLog.txt
        public static void LogMessage(string msg, Severity sev)
        {
            string fileName = "MessageLog.txt";
            ConsoleColor color = ConsoleColor.White;
            //Based on severity, change console FG color
            try
            {
                switch(sev.ToString())
                {
                    case " Success":
                        color = ConsoleColor.Green;
                        fileName = "MessageLog.txt";
                        break;
                    case "    Info":
                        color = ConsoleColor.White;
                        fileName = "MessageLog.txt";
                        break;
                    case "   Error":
                        color = ConsoleColor.Red;
                        fileName = "ErrorLog.txt";
                        break;
                    case "Critical":
                        color = ConsoleColor.DarkRed;
                        fileName = "ErrorLog.txt";
                        break;
                }
                //Write message to fileName text file.
                msg = $"{DateTime.Now.ToString()} [{sev}] {msg}";
                File.AppendAllText(filePath + fileName, msg + Environment.NewLine);
            }
            //If message didn't write correctly, add text and change console color
            catch
            {
                msg = "Failed to log - " + msg;
                color = ConsoleColor.Red;
            }
            //Print message to console with assigned color
            finally
            {
                var cc = Console.ForegroundColor;
                Console.ForegroundColor = color;
                Console.WriteLine(msg);
                Console.ForegroundColor = cc;
            }
        }
    }

    //Type-Safe-Enum - https://stackoverflow.com/questions/424366/c-sharp-string-enums?page=1&tab=votes#tab-top
    public sealed class Severity
    {
        private readonly String Message;
        private readonly int Value;

        public static readonly Severity SUCCESS = new Severity(0, " Success");
        public static readonly Severity INFO = new Severity(1, "    Info");
        public static readonly Severity ERROR = new Severity(2, "   Error");
        public static readonly Severity CRITICAL = new Severity(3, "Critical");

        private Severity(int Value, String Message)
        {
            this.Message = Message;
            this.Value = Value;
        }

        public override string ToString()
        {
            return Message;
        }
    }
}