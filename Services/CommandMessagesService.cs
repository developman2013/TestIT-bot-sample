using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using ProactiveBot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProactiveBot.Services
{
    public class CommandMessagesService
    {
        private BotContext db;
        public CommandMessagesService(BotContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Parse incoming command, do smth and return the answer
        /// </summary>
        /// <param name="context">Message context</param>
        /// <returns>String with answer to incoming command</returns>
        public async Task<string> GetAnswer(ITurnContext<IMessageActivity> context)
        {
            string userMessage = context.Activity.Text;
            DataBaseService dataBaseService = new DataBaseService(db);
            if (userMessage.Contains("/help"))
                return $"Help\n Help message will be here later";

            else if (userMessage.Contains("/mute"))
            {
                await dataBaseService.MuteConversation(context.Activity.Conversation.Id);
                return $"This conversation muted.\n Use /unmute to unmute this conversation.";
            }
            else if (userMessage.Contains("/unmute"))
            {
                await dataBaseService.UnmuteConversation(context.Activity.Conversation.Id);
                return $"This conversation unmuted.\n Use /mute to mute this conversation.";
            }
            else if (userMessage.Contains("/changechatkey"))
            {
                string secretKeyBot = userMessage.Split(" ").Last();
                if (secretKeyBot == string.Empty)
                    return $"Incorrect command. Call this command like '/changechatkey ABCEFG123'";
                await dataBaseService.ChangeBotKey(context.Activity.Conversation.Id, secretKeyBot);
                return $"Secret bot key changed to {secretKeyBot}";
            }

            return
                $"Hmmm...\n I don't know this command.";
        }
    }
}
