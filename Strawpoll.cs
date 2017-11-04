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
            try
            {
                HttpResponseMessage resultJson;

                using (var client = new HttpClient())
                {
                    resultJson = await client.GetAsync(strawpollUrl + @"/" + id);
                }

                if (resultJson.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new Exception("No poll found by the id of " + id);
                }

                return JsonConvert.DeserializeObject<Poll>(await resultJson.Content.ReadAsStringAsync());
            }
            
            catch(Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
                return null;
            }
            
        }

        public static async Task<Poll> CreatePollAsync(PollRequest pollreq)
        {
            try
            {
                HttpResponseMessage resultJson;
                var jsondata = CreateRequest(pollreq);

                if(pollreq.Options.Count < 2)
                {
                    throw new Exception($"User attempted to create poll with {pollreq.Options.Count} option(s).");
                }

                using (var client = new HttpClient())
                {
                    var content = new StringContent(jsondata, Encoding.UTF8, "application/json");
                    resultJson = await client.PostAsync(strawpollUrl, content);
                }

                return JsonConvert.DeserializeObject<Poll>(await resultJson.Content.ReadAsStringAsync());
            }

            catch(Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
                return null;
            }
        }

        public static async Task<Poll> CreatePollAsync(string title, List<string> options, bool multi = true, bool captcha = false)
        {
            try
            {
                HttpResponseMessage resultJson;
                var jsondata = CreateRequest(title, options, multi, captcha);

                if (options.Count < 2)
                {
                    throw new Exception($"User attempted to create poll with {options.Count} option(s).");
                }

                using (var client = new HttpClient())
                {
                    var content = new StringContent(jsondata, Encoding.UTF8, "application/json");
                    resultJson = await client.PostAsync(strawpollUrl, content);
                }

                return JsonConvert.DeserializeObject<Poll>(await resultJson.Content.ReadAsStringAsync());
            }

            catch (Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
                return null;
            }
        }

        internal static string CreateRequest(PollRequest req)
        {
            try
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

            catch(Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
                return null;
            }
        }

        internal static string CreateRequest(string title, List<string> options, bool multi = true, bool captcha = false)
        {
            try
            {
                JObject obj = new JObject
            {
                { "title", title },
                { "options", new JArray(options) },
                { "multi", multi }
            };

                obj.Add("dupcheck", "normal");
                obj.Add("captcha", captcha);

                return obj.ToString();
            }

            catch(Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
                return null;
            }
        }
    }
}
