using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Jaxxis.Modules
{
    [Group("siege")]
    public class SiegeModule : ModuleBase<CommandContext>
    {
        [Group("op")]
        public class SiegeOpModule : ModuleBase<CommandContext>
        {
            [Command]
            [Alias("Picks a random Siege operator")]
            public async Task SiegeOp(string side, int opCount = 1)
            {
                try
                {
                    Random random = new Random();
                    string ops = "";
                    string firstop = "";
                    List<string> AttOps = Global.SiegeAttackOps.ToList();
                    List<string> DefOps = Global.SiegeDefenseOps.ToList();
                    if (opCount > 5) { opCount = 5; };

                    if (side.ToLower() == "help")
                    {
                        await SiegeOpHelp();
                        return;
                    }

                    switch (side.ToLower().First())
                    {
                        case 'a':
                            for (int i = 0; i < opCount; i++)
                            {
                                int rnd = random.Next(0, AttOps.Count);
                                ops += AttOps[rnd] + Environment.NewLine;
                                if(i == 0) { firstop = AttOps[rnd].ToLower(); };
                                AttOps.RemoveAt(rnd);
                            }

                            Embed embeddedjsonatk = new EmbedBuilder()
                            {
                                Title = "Attack Operators",
                                Description = ops,
                                Color = Color.Blue,
                                ThumbnailUrl = $"{Global.imageURL}/Siege/Ops/{firstop}"
                            };

                            await ReplyAsync(Context.User.Mention, embed: embeddedjsonatk);
                            break;

                        case 'd':

                            for (int i = 0; i < opCount; i++)
                            {
                                int rnd = random.Next(0, DefOps.Count);
                                ops += DefOps[rnd] + Environment.NewLine;
                                if (i == 0) { firstop = DefOps[rnd].ToLower(); };
                                DefOps.RemoveAt(rnd);
                            }

                            Embed embeddedjsondef = new EmbedBuilder()
                            {
                                Title = "Defense Operators",
                                Description = ops,
                                Color = Color.Orange,
                                ThumbnailUrl = $"{Global.imageURL}/Siege/Ops/{firstop}"
                            };

                            await ReplyAsync(Context.User.Mention, embed: embeddedjsondef);
                            break;

                        default:
                            await ReplyAsync("Choose a side, Atk or Def.");
                            break;
                    }
                }

                catch(Exception ex)
                {
                    Global.LogMessage(ex, Severity.ERROR);
                }
            }

            [Command("help")]
            [Alias("Help for the *siege op* command.")]
            public async Task SiegeOpHelp()
            {
                try
                {
                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "!siege op <\"atk or def\"> {\"number\"}",
                            Value = "Atk or Def is the side you want operators picked from." + Environment.NewLine +
                            "Number is the amount of operators you want randomly picked. 5 is the limit.",
                        },

                        new EmbedFieldBuilder
                        {
                            Name = "Example: !siege op \"atk\" \"3\"",
                            Value = "--------------------------------------------------",
                        }
                    };

                    Embed embededjson = new EmbedBuilder()
                    {
                        Title = "<> = required - {} = optional",
                        Description = "--------------------------------------------------",
                        Color = Color.Green,
                        Fields = fields,
                    };

                    if (!Context.IsPrivate)
                    {
                        await Context.Message.DeleteAsync();
                    };

                    await Context.User.SendMessageAsync("", embed: embededjson);
                }

                catch(Exception ex)
                {
                    Global.LogMessage(ex, Severity.ERROR);
                }
            }
        }

        [Group("map")]
        public class SiegeMapModule : ModuleBase<CommandContext>
        {
            [Command]
            [Alias("Picks a random Siege map")]
            public async Task SiegeMap(string mappool = "casual")
            {
                try
                {
                    Random random = new Random();
                    string map = "";

                    if (mappool.ToLower() == "help")
                    {
                        await SiegeMapHelp();
                        return;
                    }

                    if (mappool.ToLower().StartsWith("c"))
                    {
                        map = Global.SiegeCasualMapPool[random.Next(0, Global.SiegeCasualMapPool.Count)];
                        string mapshort = map.Split(' ').First();

                        EmbedBuilder embeddedjsoncas = new EmbedBuilder
                        {
                            Title = map,
                            ImageUrl = $"{Global.imageURL}/Siege/maps/{mapshort.ToLower()}"
                        };

                        await ReplyAsync(Context.User.Mention, embed: embeddedjsoncas);
                    }
                    else if (mappool.ToLower().StartsWith("r"))
                    {
                        map = Global.SiegeRankedMapPool[random.Next(0, Global.SiegeRankedMapPool.Count)];
                        string mapshort = map.Split(' ').First();

                        EmbedBuilder embeddedjsonrank = new EmbedBuilder
                        {
                            Title = map,
                            ImageUrl = $"{Global.imageURL}/Siege/maps/{mapshort.ToLower()}"
                        };

                        await ReplyAsync(Context.User.Mention, embed: embeddedjsonrank);
                    }
                }

                catch(Exception ex)
                {
                    Global.LogMessage(ex, Severity.ERROR);
                }

            }

            [Command("help")]
            [Alias("Help for the *siege map* command.")]
            public async Task SiegeMapHelp()
            {
                try
                {
                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>
                    {
                        new EmbedFieldBuilder
                        {
                            Name = "!siege map <\"cas or rank\">",
                            Value = "Cas or Rank is which map pool you want the map picked from, if you're not sure, just go with Cas."
                        },

                        new EmbedFieldBuilder
                        {
                            Name = "Example: !siege map \"rank\"",
                            Value = "--------------------------------------------------",
                        }
                    };

                    Embed embededjson = new EmbedBuilder()
                    {
                        Title = "<> = required - {} = optional",
                        Description = "--------------------------------------------------",
                        Color = Color.Green,
                        Fields = fields,
                    };

                    if (!Context.IsPrivate)
                    {
                        await Context.Message.DeleteAsync();
                    };

                    await Context.User.SendMessageAsync("", embed: embededjson);
                }

                catch (Exception ex)
                {
                    Global.LogMessage(ex, Severity.ERROR);
                }
            }
        }

        [Command("help")]
        [Alias("Help for the *siege* commands")]
        public async Task SiegeHelp()
        {
            try
            {
                List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        Name = "!siege op <\"atk or def\"> {\"number\"}",
                        Value = "Returns the number of random ops from the side you chose.",
                    },

                    new EmbedFieldBuilder
                    {
                        Name = "Type \"!siege op help\" to show more in-depth information about the command.",
                        Value = "--------------------------------------------------",
                    },
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
                Global.LogMessage(ex, Severity.ERROR);
            }
        }
    }
}