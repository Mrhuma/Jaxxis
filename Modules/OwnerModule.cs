using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Jaxxis.Strawpoll;

namespace Jaxxis.Modules
{
    [Group("owner")]
    [RequireOwner]
    public class OwnerModule : ModuleBase<CommandContext>
    {
        [Group("siege")]
        public class OwnerSiegeModule : ModuleBase<CommandContext>
        {
            [Group("map")]
            public class OwnerSiegeMapModule : ModuleBase<CommandContext>
            {
                [Command("add")]
                public async Task OwnerSiegeMapAdd(string mapname, string mappool = "c")
                {
                    await Task.Run(() => OwnerSiegeMapAddAsync(mapname, mappool).GetAwaiter());
                }

                public async Task OwnerSiegeMapAddAsync(string mapname, string mappool)
                {
                    try
                    {
                        if (mappool.ToLower().StartsWith("r"))
                        {
                            Global.hiddenData.SiegeRankedMapPool.Add(mapname);
                        }
                        Global.hiddenData.SiegeCasualMapPool.Add(mapname);
                        Global.JsonHelper.JsonSerialize(Global.hiddenData);
                    }

                    catch (Exception ex)
                    {
                        await Global.LogError(ex);
                    }
                }

                [Command("remove")]
                public async Task OwnerSiegeMapRemove(string mapname, string mappool = "c")
                {
                    await Task.Run(() => OwnerSiegeMapRemoveAsync(mapname, mappool).GetAwaiter());
                }

                public async Task OwnerSiegeMapRemoveAsync(string mapname, string mappool)
                {
                    try
                    {
                        if (mappool.ToLower().StartsWith("r"))
                        {
                            Global.hiddenData.SiegeRankedMapPool.Remove(mapname);
                        }
                        else if (mappool.ToLower().StartsWith("c"))
                        {
                            Global.hiddenData.SiegeCasualMapPool.Remove(mapname);
                        }

                        Global.JsonHelper.JsonSerialize(Global.hiddenData);
                    }

                    catch (Exception ex)
                    {
                        await Global.LogError(ex);
                    }
                }

                [Command("pool")]
                public async Task OwnerSiegeMapPool()
                {
                    try
                    {
                        List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder> { };
                        EmbedFieldBuilder cas = new EmbedFieldBuilder { Name = "Casual", IsInline = true };
                        EmbedFieldBuilder rank = new EmbedFieldBuilder { Name = "Ranked", IsInline = true };

                        foreach (string m in Global.SiegeCasualMapPool)
                        {
                            cas.Value += m + Environment.NewLine;
                        }

                        foreach (string m in Global.SiegeRankedMapPool)
                        {
                            rank.Value += m + Environment.NewLine;
                        }

                        fields.Add(cas);
                        fields.Add(rank);

                        EmbedBuilder embeddedjson = new EmbedBuilder
                        {
                            Fields = fields
                        };

                        await Context.User.SendMessageAsync("", embed: embeddedjson.Build());
                    }

                    catch (Exception ex)
                    {
                        await Global.LogError(ex);
                    }
                }
            }
        }
    }
}
