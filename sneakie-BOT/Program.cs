using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace sneakie_BOT
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        public SocketUserMessage currentUserMessage;

        //add the id to your default channel in discord(need developer on in discord to be able to do this)
        public ulong DefaultChannel = 90647294138912768;
        //add the id to the channel you want the bot to copy&paste the allowedurls
        public ulong LinkChannel = 435210055705690128;
        //The server admin's id
        string adminId = "############";

        public string BotName = "sneakie-BOT";
        static Random Dice = new Random();

        public List<string> AllowedUrls = new List<string>();

        //public bool IsDefaultChannel(ulong id) => id == DefaultChannel;
        public bool IsDefaultChannel(ulong id)
        {
            if (id == DefaultChannel)
                return true;
            else
                return false;
        }


        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            //add the token.txt to the root of the .exe/dll's it should ONLY contain the token
            string botToken = File.ReadAllText("token.txt");

            //add more url's if you want more included
            AllowedUrls.Add("https://www.youtube.com");
            AllowedUrls.Add("https://www.plays.tv");
            AllowedUrls.Add("https://clips.twitch.tv");
            AllowedUrls.Add("https://www.twitch.tv");
            AllowedUrls.Add("https://youtu.be");
            AllowedUrls.Add("https://streamable.com");
            AllowedUrls.Add("https://neatclip.com/clip");



            // event Subscriptions
            _client.Log += log;
            _client.UserJoined += AnnounceUserJoined;
            _client.MessageReceived += MessageReceived;

            //await RegisterCommandsAsync();

            await _client.LoginAsync(Discord.TokenType.Bot, botToken);

            await _client.StartAsync();

            await Task.Delay(-1);

        }

        public void botReply(string msg)
        {
            currentUserMessage.Channel.SendMessageAsync(msg, false);
        }

        private Task MessageReceived(SocketMessage arg)
        {

            // declare message
            var socketMessage = arg as SocketUserMessage;
            currentUserMessage = socketMessage;

            //  if message is empty/null or its from - ignore it
            if (socketMessage is null || socketMessage.Author.IsBot) return Task.CompletedTask;

            //  Message, user sent
            string userMessage = socketMessage.Content;
            
            //  The name of the user who sent the message
            string userName = socketMessage.Author.Username;
            string userNameWithTag = socketMessage.Author.Mention;


            //strings for roll command and also string/ulongs for admin check.
            string rolled = " rolled ";
            ulong ulongMsgAuthorCheck= socketMessage.Author.Id;
            string msgAuthorCheck = ulongMsgAuthorCheck.ToString();
            var embed = new EmbedBuilder();

            // bool for confirmation if author is an admin or not
            bool isAdmin = false;
            
            //confirmation
            if(msgAuthorCheck == adminId)
            {
                isAdmin = true;
            }

            // Commands
            switch (userMessage.ToLower())
            {
                case "!quit":
                    if (isAdmin)
                    {
                        botReply("sneakie-BOT turning off");
                        System.Environment.Exit(1);
                    }
                    break;

                case "!twotime":
                    botReply("Stupid fucking mistakes man, stupid fucking mistakes..");
                    break;

                case "!mladris":
                    botReply("If they cant beat me, they deserve to get raped - Mladris");
                    break;

                case "!addeponken":

                    embed.ImageUrl = "https://i.imgur.com/1KOA8QQ.png";
                    botReply(@"https://i.imgur.com/1KOA8QQ.png");
                    break;

                case "!renuilz":
                    botReply("5sek arrow, HORUNGE!");
                    break;

                case "!jhoey":
                    botReply("12 btw xd");
                    break;

                case "!roll":
                    int diceRoll = Dice.Next(0, 101);
                    botReply(userNameWithTag + rolled + diceRoll.ToString());
                    socketMessage.DeleteAsync();
                    break;
                case "!pepsidood":
                    embed.ImageUrl = "https://i.imgur.com/maopPX9.jpg";
                    botReply(@"https://i.imgur.com/maopPX9.jpg");
                    break;

                case "!commands":
                    socketMessage.Author.SendMessageAsync("Hello! \n" +
                        "These are my commands: \n \n" + 
                        "!quit(admin only) \n" +
                        "!twotime \n" +
                        "!mladris \n" +
                        "!addeponken \n" +
                        "!renuilz \n" +
                        "!jhoey \n" +
                        "!roll \n" +
                        "!pepsidood \n");
                    socketMessage.DeleteAsync();
                    break;
            }


            //  if the message contains "http", do stuff
            if (userMessage.Contains("http"))
            {
                // cool
                //var link = socketMessage.Content.Split(' ').ToList().FirstOrDefault(x => x.Contains("http"));

                //  split the msg for every space and put it into a list
                var msgSplit = userMessage.Split(new char[] { ' ', '\n' }).ToList();

                //  declare the link string list
                List<string> links = new List<string>();

                //  search the split list and take the first string that contains 'http' and break the loop
                foreach (string msg in msgSplit)
                {

                    //  check if the msg cotains any of the Allowed prefixes from AllowedUrls, if yes,  link = url.
                    foreach (string url in AllowedUrls)
                    {

                        //  check if msg containts an url
                        if (msg.Contains(url))
                        {
                            //  add the found link to the links-List
                            links.Add(msg);
                            break;
                        }
                    }

                }


                //  if we found links, send links
                if (links.Count > 0)
                {
                    var context = new SocketCommandContext(_client, socketMessage);

                    //  get the LinkChannel by linkId
                    var theLinkChannel = context.Guild.GetTextChannel(LinkChannel);


                    //  send the link to the linkChannel for each link in links
                    string allLinksCombined = "";
                    foreach (string link in links)
                    {
                        allLinksCombined += link + "\n";
                    }

                    theLinkChannel.SendMessageAsync(allLinksCombined);
                }

            }

            return Task.CompletedTask;
        }

        //private void SendMsgToChannel(ISocketMessageChannel channel, string msg)
        //{
        //    channel.SendMessageAsync(msg);
        //}

        private async Task AnnounceUserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            var channel = guild.GetTextChannel(DefaultChannel);
            await channel.SendMessageAsync($"Welcome, {user.Mention}");
        }
        private Task log(LogMessage arg)
        {
            Console.WriteLine(arg);

            return Task.CompletedTask;
        }
    }
}
