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
        [Command]
        public async Task GetOperator(string side = "", int opCount = 1)
        {
            Random random = new Random();

            if(opCount > 5) { opCount = 5; };

            if (side == "")
            {
                for(int i = 0; i < opCount; i++)
                {
                    await ReplyAsync(Global.AllOps[random.Next(0, Global.AllOps.Count)]);
                }
            }
            else if (side.StartsWith("a"))
            {
                for(int i = 0; i < opCount; i++)
                {
                    await ReplyAsync(Global.AttOps[random.Next(0, Global.AttOps.Count)]);
                }
            }
            else if (side.StartsWith("d"))
            {
                for (int i = 0; i < opCount; i++)
                {
                    await ReplyAsync(Global.DefOps[random.Next(0, Global.DefOps.Count)]);
                }
            }
        }
    }
}