using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using static Jaxxis.Strawpoll;

namespace Jaxxis.Modules
{
    public class StrawpollModule : ModuleBase
    {
        [Command("poll"), Summary("Displays a poll")]
        public async Task GetPoll([Summary("The id of the poll.")] int num)
        {
            try
            {
                Poll poll = await GetPollAsync(num);
                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
                int j = 0;
                foreach (string i in poll.Options)
                {
                    fields.Add(new EmbedFieldBuilder()
                    {
                        IsInline = true,
                        Name = i.ToString(),
                        Value = poll.Votes[j]
                    });

                    j++;
                }

                Embed embeddedjson = new EmbedBuilder()
                {
                    Url = poll.PollUrl,
                    ThumbnailUrl = "https://pbs.twimg.com/profile_images/737742455643070465/yNKcnrSA.jpg",
                    Title = poll.Title,
                    Fields = fields,
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = poll.PollUrl
                    }
                };

                await ReplyAsync("", embed: embeddedjson);
            }

            catch(Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
            }
        }
    }
}
