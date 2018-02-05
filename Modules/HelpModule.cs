using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.Rest;
using Jaxxis;

namespace Jaxxis.Modules
{
    [Group("help")]
    public class HelpModule : ModuleBase<CommandContext>
    {
        [Command]
        [Remarks("none")]
        public async Task Help()
        {
            Program program = new Program();
            await Task.Run(() => program.ReactionMenuStart(Context).GetAwaiter());
        }
    }
}
