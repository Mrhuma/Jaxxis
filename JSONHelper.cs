using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Jaxxis
{
    //Hidden data class, used for Global vars
    public class HiddenData
    {
        public bool IsFirstLaunch { get; set; }
        public string BotToken { get; set; }
        public string ImageURL { get; set; }
        public string PUBGApiKey { get; set; }
        public string ShortcutPath { get; set; }
        public List<string> SiegeAttackOps { get; set; }
        public List<string> SiegeDefenseOps { get; set; }
        public List<string> SiegeRankedMapPool { get; set; }
        public List<string> SiegeCasualMapPool { get; set; }
    }

    public class JSONHelper
    {
        //Write vars to json file
        public void JsonSerialize(HiddenData hiddenData)
        {
            File.WriteAllText(Global.filePath + "LocalData.json", JsonConvert.SerializeObject(hiddenData, Formatting.Indented, new JsonSerializerSettings {NullValueHandling = NullValueHandling.Include}));
        }

        //Read vars from Json file
        public HiddenData JsonDeserialize()
        {
             return JsonConvert.DeserializeObject<HiddenData>(File.ReadAllText(Global.filePath + "LocalData.json"));
        }
    }
}
