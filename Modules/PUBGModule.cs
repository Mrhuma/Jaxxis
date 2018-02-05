using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Jaxxis.Database;
using PUBGSharp.Data;
using PUBGSharp.Helpers;
using PUBGSharp.Exceptions;
using PUBGSharp;

namespace Jaxxis.Modules
{
    [Group("pubg")]
    [Name("PUBG")]
    [Remarks("game")]
    public class PUBGModule : ModuleBase<CommandContext>
    {
        [Command("stats")]
        [Name("Stats")]
        [Summary("!pubg stats <username> <gamemode>" +
            "-Username is your PUBG username." +
            "~Gamemode is the gamemode that you want to get stats for." +
            "~Gamemodes consist of \"solo\", \"solofpp\", \"duo\", \"duofpp\", \"squad\", \"squadfpp\"." +
            "-Example: !pubg stats mrhuma solofpp")]
        [Remarks("pubg")]
        public async Task PUBG(string playerName, string gamemode = "solo")
            {
                await Task.Run(() => PUBGAsync(playerName, gamemode).ConfigureAwait(false));
            }

        public async Task PUBGAsync(string playerName, string gamemode = "solo")
        {
            // Create client and send a stats request You can either use the "using" keyword or
            // dispose the PUBGStatsClient manually with the Dispose method.
            using (var statsClient = new PUBGStatsClient(Global.PUBGApiKey))
            {
                try
                {
                    string modeText = "";
                    Mode mode;
                    switch (gamemode)
                    {
                        case "solo":
                            mode = Mode.Solo;
                            modeText = "solo";
                            break;
                        case "solofpp":
                            mode = Mode.SoloFpp;
                            modeText = "solo-fpp";
                            break;
                        case "duo":
                            mode = Mode.Duo;
                            modeText = "duo";
                            break;
                        case "duofpp":
                            mode = Mode.DuoFpp;
                            modeText = "duo-fpp";
                            break;
                        case "squad":
                            mode = Mode.Squad;
                            modeText = "squad";
                            break;
                        case "squadfpp":
                            mode = Mode.SquadFpp;
                            modeText = "squad-fpp";
                            break;
                        default:
                            mode = Mode.Solo;
                            modeText = "solo";
                            break;
                    }

                    var stats = await statsClient.GetPlayerStatsAsync("shroud", Region.AGG, mode).ConfigureAwait(false);

                    Console.WriteLine($"{playerName}, last updated at: {stats.LastUpdated}");

                    var rating = stats.Stats.Find(x => x.Region == Region.AGG && x.Mode == mode).Stats.Find(x => x.Stat == Stats.Rating).Value;
                    var winPercentage = stats.Stats.Find(x => x.Region == Region.AGG && x.Mode == mode).Stats.Find(x => x.Stat == Stats.WinPercentage).Value;
                    var topTenPercentage = stats.Stats.Find(x => x.Region == Region.AGG && x.Mode == mode).Stats.Find(x => x.Stat == Stats.Top10).Value;
                    var kills = stats.Stats.Find(x => x.Region == Region.AGG && x.Mode == mode).Stats.Find(x => x.Stat == Stats.Kills).Value;
                    var wins = stats.Stats.Find(x => x.Region == Region.AGG && x.Mode == mode).Stats.Find(x => x.Stat == Stats.Wins).Value;
                    var kdr = stats.Stats.Find(x => x.Region == Region.AGG && x.Mode == mode).Stats.Find(x => x.Stat == Stats.KDR).Value;

                    List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>()
                    {
                        new EmbedFieldBuilder
                        {
                            Name  = "Rating",
                            Value = rating,
                            IsInline = true
                        },

                        new EmbedFieldBuilder
                        {
                            Name  = "Kills",
                            Value = kills,
                            IsInline = true
                        },

                        new EmbedFieldBuilder
                        {
                            Name  = "KDR",
                            Value = kdr,
                            IsInline = true
                        },

                        new EmbedFieldBuilder
                        {
                            Name = "Wins",
                            Value = wins,
                            IsInline = true
                        },

                        new EmbedFieldBuilder
                        {
                            Name  = "Win %",
                            Value = winPercentage,
                            IsInline = true
                        },

                        new EmbedFieldBuilder
                        {
                            Name  = "Top 10 %",
                            Value = topTenPercentage,
                            IsInline = true
                        },
                    };

                    Embed embeddedjson = new EmbedBuilder()
                    {
                        Title = $"{playerName} - {mode.ToString()}",
                        Color = Color.LightOrange,
                        Fields = fields,
                        Url = $"https://pubgtracker.com/profile/pc/{playerName}/{modeText}",
                        Footer = new EmbedFooterBuilder
                        {
                            Text = $"https://pubgtracker.com/"
                        },
                    }.Build();

                    await ReplyAsync("", embed: embeddedjson);

                    await Dataset.StatsInsertOrUpdate("PUBG Stats");
                }

                catch (PUBGSharpException ex)
                {
                    await Global.LogError(ex, Context);
                }

                /* IMPORTANT STUFF ABOUT EXCEPTIONS:
                 The LINQ and other selector methods (e.g. .Find) will throw NullReferenceException in case the stats don't exist.
                 So if player has no stats in specified region or game mode, it will throw NullReferenceException.
                 For example, if you only have played in Europe and try to look up your stats in the Asia server, instead of showing 0's everywhere it throws this. */
                /*
               catch (PUBGSharpException ex)
               {
                   Console.WriteLine($"Could not retrieve stats for {stats.PlayerName}, error: {ex.Message}");
               }
               catch (NullReferenceException)
               {
                   await ReplyAsync($"Could not retrieve {modeText.ToString()} stats for {stats.PlayerName}.");
                   Console.WriteLine("The player might not exist or have stats in the specified mode or region.");
               }
               */

            }
        }
    }
}
