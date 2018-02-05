using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Jaxxis.Database;

namespace Jaxxis.Modules
{
    [Group("siege")]
    [Name("Siege")]
    [Remarks("game")]
    public class SiegeModule : ModuleBase<CommandContext>
    {
        [Command("op")]
        [Name("OP")]
        [Summary("!siege op <atk or def> {number}" +
        "-Atk or Def is the side you want operators picked from." +
        "~Number is the amount of operators you want randomly picked. 5 is the limit." +
        "-Example: !siege op atk 3")]
        [Remarks("siege")]
        public async Task SiegeOp(string side, int opCount = 1)
        {
            try
            {
                Random random = new Random();
                string ops = "";
                string firstop = "";
                string msgstr = "";
                List<string> AttOps = Global.SiegeAttackOps.ToList();
                List<string> DefOps = Global.SiegeDefenseOps.ToList();
                if (opCount > 5) { opCount = 5; };
                if (opCount < 1) { opCount = 1; };

                switch (side.ToLower().First())
                {
                    case 'a':
                        for (int i = 0; i < opCount; i++)
                        {
                            int rnd = random.Next(0, AttOps.Count);
                            ops += AttOps[rnd] + Environment.NewLine;
                            if (i == 0)
                            {
                                firstop = AttOps[rnd].ToLower();
                                if (opCount <= 1)
                                {
                                    msgstr = "Attack Operator";
                                }
                                else
                                {
                                    msgstr = "Attack Operators";
                                }
                            };
                            AttOps.RemoveAt(rnd);
                        }

                        Embed embeddedjsonatk = new EmbedBuilder()
                        {
                            Title = msgstr,
                            Description = ops,
                            Color = Color.Blue,
                            ThumbnailUrl = $"{Global.imageURL}/Siege/Ops/{firstop}",
                        }.Build();

                        await ReplyAsync(Context.User.Mention, embed: embeddedjsonatk);
                        break;

                    case 'd':

                        for (int i = 0; i < opCount; i++)
                        {
                            int rnd = random.Next(0, DefOps.Count);
                            ops += DefOps[rnd] + Environment.NewLine;
                            if (i == 0)
                            {
                                firstop = DefOps[rnd].ToLower();
                                if (opCount <= 1)
                                {
                                    msgstr = "Defense Operator";
                                }
                                else
                                {
                                    msgstr = "Defense Operators";
                                }
                            };
                            DefOps.RemoveAt(rnd);
                        }

                        Embed embeddedjsondef = new EmbedBuilder()
                        {
                            Title = msgstr,
                            Description = ops,
                            Color = Color.Orange,
                            ThumbnailUrl = $"{Global.imageURL}/Siege/Ops/{firstop}",
                        }.Build();

                        await ReplyAsync(Context.User.Mention, embed: embeddedjsondef);
                        break;

                    default:
                        await ReplyAsync("Choose a side, Atk or Def.");
                        break;
                }

                await Dataset.StatsInsertOrUpdate("Siege OP");
            }
            catch (Exception ex)
            {
                await ReplyAsync("", embed: await Global.LogError(ex, Context));
            }
        }

        [Command("map")]
        [Name("Map")]
        [Summary("!siege map <cas or rank>" +
        "-Cas or Rank is the map pool you want the map picked from." +
        "-Example: !siege map rank")]
        [Remarks("siege")]
        public async Task SiegeMap(string mappool = "casual")
        {
            try
            {
                Random random = new Random();
                string map = "";

                if (mappool.ToLower().StartsWith("c"))
                {
                    map = Global.SiegeCasualMapPool[random.Next(0, Global.SiegeCasualMapPool.Count)];
                    string mapshort = map.Split(' ').First();

                    EmbedBuilder embeddedjsoncas = new EmbedBuilder
                    {
                        Title = map,
                        ImageUrl = $"{Global.imageURL}/Siege/maps/{mapshort.ToLower()}"
                    };

                    await ReplyAsync(Context.User.Mention, embed: embeddedjsoncas.Build());
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

                    await ReplyAsync(Context.User.Mention, embed: embeddedjsonrank.Build());
                }

                await Dataset.StatsInsertOrUpdate("Siege Map");
            }

            catch (Exception ex)
            {
                await ReplyAsync("", embed: await Global.LogError(ex, Context));
            }

        }
    }
}