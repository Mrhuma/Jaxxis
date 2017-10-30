using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jaxxis.Database;
using MySql.Data;

namespace Jaxxis
{
    //Global static variables
    public class Global
    {
        public static bool isFirstLaunch;
        public static string botToken = "";
        public static JSONHelper JsonHelper = new JSONHelper();
        public static HiddenData hiddenData;

        public static void Initialize()
        {
            hiddenData = JsonHelper.JsonDeserialize();

            isFirstLaunch = hiddenData.IsFirstLaunch;
            botToken = hiddenData.BotToken;
        }
    }
}