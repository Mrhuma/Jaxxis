using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using static Jaxxis.Strawpoll;

namespace Jaxxis.Modules
{
    [Group("poll")]
    public class StrawpollModule : ModuleBase<CommandContext>
    {
        [Command]
        public async Task GetPoll(int num)
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

            catch (Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
            }
        }

        [Command("create")]
        public async Task CreatePoll(string title, string options, bool multi = false)
        {
            try
            {
                List<string> optionList = options.Split(',').ToList();

                PollRequest pollRequest = new PollRequest()
                {
                    Title = title,
                    Options = optionList,
                    Multi = multi
                };

                Poll poll = await CreatePollAsync(pollRequest);
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

        [Command("help")]
        public async Task PollHelp()
        {
            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder
                {
                    Value = "poll <\"number\">",
                    Name = "Displays a Strawpoll with the id given. Example: !poll 123456",
                },

                new EmbedFieldBuilder
                {
                    Value = "poll create (Title of your Strawpoll)<\"Title\"> (The options that people can vote for)<\"Option1,Option2,...\"> (Can users vote for multiple options?)<\"true or false\">",
                    Name = "Creates a Strawpoll with the given parameters. Example: !poll create \"Poll Title\" \"Option 1,Option 2,Option 3\" \"true\""
                }
            };

            Embed embeddedjson = new EmbedBuilder()
            {
                Title = "Poll Commands",
                Fields = fields,
            };

            await ReplyAsync("", embed: embeddedjson);
        }
    }
}