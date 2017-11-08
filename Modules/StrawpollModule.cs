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
        [Group("get")]
        public class GetModule : ModuleBase<CommandContext>
        {
            [Command]
            public async Task PollGet(int num)
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

            [Command("help")]
            public async Task PollGetHelp()
            {
                try
                {
                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "!poll get <\"number\">",
                            Value = "Number is the id of the strawpoll that you want to get."
                        },

                        new EmbedFieldBuilder
                        {
                            Name = "Example: !poll get \"123456\"",
                            Value = "--------------------------------------------------"
                        }
                    };

                    Embed embeddedjson = new EmbedBuilder()
                    {
                        Title = "<> = required - {} = optional",
                        Description = "--------------------------------------------------",
                        Color = Color.Green,
                        Fields = fields,
                    };

                    await Context.Message.DeleteAsync();
                    await Context.User.SendMessageAsync("", embed: embeddedjson);
                }

                catch(Exception ex)
                {
                    Global.LogMessage(ex.Message, Severity.ERROR);
                }
            }
        }

        [Group("create")]
        public class CreateModule : ModuleBase<CommandContext>
        {
            [Command]
            public async Task PollCreate(string title, string options, bool multi = false, bool captcha = false)
            {
                try
                {
                    List<string> optionList = options.Split(',').ToList();

                    if (optionList.Count < 2)
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

                catch (Exception ex)
                {
                    Global.LogMessage(ex.Message, Severity.ERROR);
                }
            }

            [Command("help")]
            public async Task PollCreateHelp()
            {
                try
                {
                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "!poll create <\"Title\"> <\"Option 1, Option 2, etc...\"> {\"true or false\"}",
                            Value = "Title is the title of your strawpoll." + Environment.NewLine +
                            "Options are the choices that users can vote for." + Environment.NewLine +
                            "The optional true or false parameter is whether or not you want users to be able to vote for multiple options. If not specified this will default to false."
                        },

                        new EmbedFieldBuilder
                        {
                            Name = "Example: !poll create \"What's you favorite color?\" \"Blue, Green, Yellow, Red\" \"false\"",
                            Value = "--------------------------------------------------"
                        }
                    };

                    Embed embeddedjson = new EmbedBuilder()
                    {
                        Title = "<> = required - {} = optional",
                        Description = "--------------------------------------------------",
                        Color = Color.Green,
                        Fields = fields,
                    };

                    await Context.Message.DeleteAsync();
                    await Context.User.SendMessageAsync("", embed: embeddedjson);
                }

                catch(Exception ex)
                {
                    Global.LogMessage(ex.Message, Severity.ERROR);
                }
            }
        }

        [Command("help")]
        public async Task PollHelp()
        {
            try
            {
                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        Name = "!poll get <\"number\">",
                        Value = "Returns a Strawpoll with the id given.",
                    },

                    new EmbedFieldBuilder
                    {
                        Name = "Type \"!poll get help\" to show more in-depth information about the command.",
                        Value = "--------------------------------------------------",
                    },

                    new EmbedFieldBuilder
                    {
                        Name = "!poll create <\"Title\"> <\"Option 1, Option 2, etc...\"> {\"true or false\"}",
                        Value = "Creates a Strawpoll with the given parameters.",
                    },

                    new EmbedFieldBuilder
                    {
                        Name = "Type \"!poll create help\" to show more in-depth information about the command.",
                        Value = "--------------------------------------------------",
                    }

                };

                Embed embeddedjson = new EmbedBuilder()
                {
                    Title = "<> = required - {} = optional",
                    Description = "----------------------------------------",
                    Color = Color.Green,
                    Fields = fields,
                };

                await Context.Message.DeleteAsync();
                await Context.User.SendMessageAsync("", embed: embeddedjson);
            }

            catch(Exception ex)
            {
                Global.LogMessage(ex.Message, Severity.ERROR);
            }
        }
    }
}