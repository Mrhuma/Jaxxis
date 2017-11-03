using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jaxxis
{
    class Strawpoll
    {
        public const string strawpollUrl = "https://strawpoll.me/api/v2/polls";

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
            public bool? Captcha { get; set; }
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
                resultJson = await client.GetAsync(strawpollUrl + @"/" + id);
            }

            Console.WriteLine(resultJson.Content.ToString());

            return JsonConvert.DeserializeObject<Poll>(await resultJson.Content.ReadAsStringAsync());
        }

        public static async Task<Poll> CreatePollAsync(PollRequest pollreq)
        {
            HttpResponseMessage resultJson;
            var jsondata = CreateRequest(pollreq);

            using (var client = new HttpClient())
            {
                var content = new StringContent(jsondata, Encoding.UTF8, "application/json");
                resultJson = await client.PostAsync(strawpollUrl, content);
            }

            return JsonConvert.DeserializeObject<Poll>(await resultJson.Content.ReadAsStringAsync());
        }

        internal static string CreateRequest(PollRequest req)
        {
            JObject obj = new JObject
            {
                {"title", req.Title},
                {"options", new JArray(req.Options)},
                {"multi", req.Multi ?? true }
            };

            switch (req.Dupcheck ?? DupCheck.NORMAL)
            {
                case DupCheck.NORMAL:
                    obj.Add("dupcheck", "normal");
                    break;
                case DupCheck.PERMISSIVE:
                    obj.Add("dupcheck", "permissive");
                    break;
                case DupCheck.DISABLED:
                    obj.Add("dupcheck", "disabled");
                    break;
            }

            obj.Add("captcha", req.Captcha ?? false);

            return obj.ToString();
        }

        internal static string CreateRequest(string title, List<string> options, bool multi = true, DupCheck dupcheck = DupCheck.NORMAL, bool capcha = false)
        {
            JObject obj = new JObject
            {
                { "title", title },
                { "options", new JArray(options) },
                { "multi", multi }
            };

            switch (dupcheck)
            {
                case DupCheck.NORMAL:
                    obj.Add("dupcheck", "normal");
                    break;
                case DupCheck.PERMISSIVE:
                    obj.Add("dupcheck", "permissive");
                    break;
                case DupCheck.DISABLED:
                    obj.Add("dupcheck", "disabled");
                    break;
            }

            obj.Add("captcha", capcha);

            return obj.ToString();
        }
    }
}
