using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace Jaxxis.Modules
{
    [Group("siege")]
    public class SiegeModule : ModuleBase<CommandContext>
    {
        [Group("op")]
        public class OpModule : ModuleBase<CommandContext>
        {
            [Command]
            [Alias("Picks a random operator")]
            public async Task SiegeOp(string side, int opCount = 1)
            {
                Random random = new Random();
                string ops = "";
                List<string> AttOps = Global.AttOps.ToList();
                List<string> DefOps = Global.DefOps.ToList();
                if (opCount > 5) { opCount = 5; };

                if(side.ToLower() == "help")
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
                            AttOps.RemoveAt(rnd);
                        }

                        Embed embeddedjsonatk = new EmbedBuilder()
                        {
                            Title = "Attack Operators",
                            Description = ops,
                            Color = Color.Blue,
                        };

                        await ReplyAsync(Context.User.Mention, embed: embeddedjsonatk);
                        break;

                    case 'd':

                        for (int i = 0; i < opCount; i++)
                        {
                            int rnd = random.Next(0, DefOps.Count);
                            ops += DefOps[rnd] + Environment.NewLine;
                            DefOps.RemoveAt(rnd);
                        }

                        Embed embeddedjsondef = new EmbedBuilder()
                        {
                            Title = "Defense Operators",
                            Description = ops,
                            Color = Color.Orange,
                        };

                        await ReplyAsync(Context.User.Mention, embed: embeddedjsondef);
                        break;

                    default:
                        await ReplyAsync("Choose a side, Atk or Def.");
                        break;
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

                    await Context.Message.DeleteAsync();
                    await Context.User.SendMessageAsync("", embed: embededjson);
                }

                catch(Exception ex)
                {
                    Global.LogMessage(ex.Message, Severity.ERROR);
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
                Global.LogMessage(ex.Message, Severity.ERROR);
            }
        }
    }
}