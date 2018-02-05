using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Jaxxis.Database;
using Discord;
using Discord.Commands;
using static Jaxxis.Strawpoll;

namespace Jaxxis.Modules
{
    [Group("poll")]
    [Name("Poll")]
    [Remarks("info")]
    public class StrawpollModule : ModuleBase<CommandContext>
    {
        [Command("get")]
        [Name("Get")]
        [Summary("!poll get <\"number\">" +
            "-Number is the id of the strawpoll you want to view." +
            "-Example: !poll get \"123\"")]
        [Remarks("poll")]
        public async Task PollGet(int num)
        {
            await Task.Run(() => PollGetAsync(num));
        }

        public async Task PollGetAsync(int num)
        {
            try
            {
                Poll poll = await GetPollAsync(num);

                if (poll == null)
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
                    ThumbnailUrl = $"{Global.imageURL}/Strawpoll/strawpollicon",
                    Title = poll.Title,
                    Fields = fields,
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = poll.PollUrl
                    }
                }.Build();

                await ReplyAsync(Context.User.Mention, embed: embeddedjson);

                await Dataset.StatsInsertOrUpdate("Poll Get");
            }

            catch (Exception ex)
            {
                await Global.LogError(ex, Context);
            }
        }

        [Command("create")]
        [Name("Create")]
        [Summary("!poll create <\"Title\"> <\"Option 1, Option 2, etc...\"> {true or __false__}" +
            "-Title is the title of your strawpoll." +
            "~Options are the choices that users can vote for." +
            "~The optional true or false parameter is whether or not you want users to be able to vote for multiple options." +
            "-Example: !poll create \"What's your favorite color?\" \"Blue, Green, Yellow, Red\"")]
        [Remarks("poll")]
        public async Task PollCreate(string title, string options, bool multi = false)
        {
            await Task.Run(() => PollCreateAsync(title, options, multi));
        }

        public async Task PollCreateAsync(string title, string options, bool multi = false)
        {
            try
            {
                List<string> optionList = options.Split(',').ToList();

                if (optionList.Count < 2)
                {
                    await ReplyAsync($"Poll needs a minimum of 2 options, you set {optionList.Count} option.");
                }
                Poll poll = await CreatePollAsync(title, optionList, multi, false);
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
                    ThumbnailUrl = $"{Global.imageURL}/Strawpoll/strawpollicon",
                    Title = poll.Title,
                    Fields = fields,
                    Footer = new EmbedFooterBuilder()
                    {
                        Text = poll.PollUrl
                    }
                }.Build();

                await ReplyAsync(Context.User.Mention, embed: embeddedjson);

                await Dataset.StatsInsertOrUpdate("Poll Create");
            }

            catch (Exception ex)
            {
                await Global.LogError(ex, Context);
            }
        }
    }
}