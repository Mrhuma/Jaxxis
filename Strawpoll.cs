using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using Newtonsoft.Json;

namespace Jaxxis
{
    class Strawpoll
    {
        public class Poll
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public List<string> Options { get; set; }
            public List<int> Votes { get; set; }
            public bool Multi { get; set; }
            public string Dupcheck { get; set; }
            public bool Captcha { get; set; }

            public string PollUrl => $"https://strawpoll.me/{Id}";
        }

        public class PollRequest
        {
            public string Title { get; set; }
            public List<string> Options { get; set; }
            public DupCheck? Dupcheck { get; set; }
            public bool? Multi { get; set; }
            public bool? Capcha { get; set; }
        }

        public enum DupCheck
        {
            NORMAL,
            PERMISSIVE,
            DISABLED
        }

        public static async Task<Poll> GetPollAsync(int id)
        {
            HttpResponseMessage resultJson;

            using (var client = new HttpClient())
            {
                    resultJson = await client.GetAsync(Global.url + id);
            }

            return JsonConvert.DeserializeObject<Poll>(await resultJson.Content.ReadAsStringAsync());
        }
    }
}
