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

                if(poll == null)
                {
                    await ReplyAsync($"No poll found by the id of **{num}**, {Context.User.Mention}");
                }

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
                    Color = Color.LightOrange,
                    Url = poll.PollUrl,
                    ThumbnailUrl = "https://pbs.twimg.com/profile_images/737742455643070465/yNKcnrSA.jpg",
                    Title = poll.Title,
                    Fields = fields,
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = poll.PollUrl
                    }
                };

                Global.LogMessage($"{Context.User.Username} called a poll with the id of {num.ToString()}", Severity.SUCCESS);
                await ReplyAsync(Context.User.Mention, embed: embeddedjson);
            }

            catch (Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
            }
        }

        [Command("create")]
        public async Task CreatePoll(string title, string options, bool multi = false, bool captcha = false)
        {
            try
            {
                List<string> optionList = options.Split(',').ToList();

                if(optionList.Count < 2)
                {
                    await ReplyAsync($"Poll needs a minimum of 2 options, you set {optionList.Count} option.");
                }

                Poll poll = await CreatePollAsync(title, optionList, multi, captcha);
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
                    Color = Color.LightOrange,
                    Url = poll.PollUrl,
                    ThumbnailUrl = "https://pbs.twimg.com/profile_images/737742455643070465/yNKcnrSA.jpg",
                    Title = poll.Title,
                    Fields = fields,
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = poll.PollUrl
                    }
                };

                Global.LogMessage($"{Context.User.Username} successfully created new poll with id of {poll.Id}!", Severity.SUCCESS);
                await ReplyAsync(Context.User.Mention, embed: embeddedjson);
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
                    Name = "Displays a Strawpoll with the id given.",
                    Value = "!poll <\"number\">",
                },

                new EmbedFieldBuilder
                {
                    Name = "Example: !poll 123456",
                    Value = "----------------------------------------",
                    //Value = "poll (ID of the Strawpoll)",
                },

                new EmbedFieldBuilder
                {
                    Name = "Creates a Strawpoll with the given parameters.",
                    Value = "!poll create <\"Title\"> <\"Option 1, Option 2, etc...\"> {Can users vote for multiple options? \"true or false\"}",
                },
                
                new EmbedFieldBuilder
                {
                    Name = "Example: !poll create \"What's your favorite color?\" \"Blue, Red, Yellow, Green\" \"true\"",
                    Value = "----------------------------------------",
                    //Value = "poll create (Title of the Strawpoll) (Options others can vote for.) (**Optional**: If *true*, others can only vote for 1 option, if *false* they can vote for multiple options.)"
                }
                
            };

            Embed embeddedjson = new EmbedBuilder()
            {
                Title = "<> = required - {} = optional",
                Description = "----------------------------------------",
                Color = Color.Blue,
                Fields = fields,
            };

            await ReplyAsync("", embed: embeddedjson);
        }
    }
}