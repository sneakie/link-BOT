using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace sneakie_BOT.Modules
{
    public class DiceRoll : ModuleBase<SocketCommandContext>
    {

        [Command("penis")]
        public async Task PingAsync()
        {
           await ReplyAsync("hej");
        }
    }
}
