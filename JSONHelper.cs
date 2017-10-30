using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Jaxxis
{
    //Hidden data class, used for Global vars
    public class HiddenData
    {
        public bool IsFirstLaunch { get; set; }
        public string BotToken { get; set; }
    }

    public class JSONHelper
    {
        static string filePath = @"..\..\Data\LocalData.json";

        //Write vars to json file
        public void JsonSerialize(HiddenData hiddenData)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(hiddenData, Formatting.Indented, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Include}));

        }

        //Read vars from Json file
        public HiddenData JsonDeserialize()
        {
             return JsonConvert.DeserializeObject<HiddenData>(File.ReadAllText(filePath));
        }
    }
}
