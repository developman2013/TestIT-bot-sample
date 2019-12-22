using Microsoft.EntityFrameworkCore;
using ProactiveBot.Data;
using ProactiveBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProactiveBot.Services
{
    public class DataBaseService
    {
        private BotContext db;
        public DataBaseService(BotContext db)
        {
            this.db = db;
        }

        /// <summary>
        /// Add record to database
        /// </summary>
        /// <param name="secretKeyBot">Key of the bot (from TestIT portal)</param>
        /// <param name="conversationId">Id of the chat where this bot added</param>
        /// <returns></returns>
        public async Task Add(string secretKeyBot, string conversationId)
        {
            if (!db.BotKeys.Any(x => x.ConversationId == conversationId))
                await db.BotKeys.AddAsync(
                    new BotKeysModel()
                    {
                        ConversationId = conversationId,
                        SecretKeyBot = secretKeyBot,
                        IsConversationMute = false
                    });
            await Save();
        }

        /// <summary>
        /// Change chat status to Mute
        /// </summary>
        /// <param name="conversationId">Id of the chat</param>
        /// <returns></returns>
        public async Task MuteConversation(string conversationId)
        {
            var item = await db.BotKeys.Where(x => x.ConversationId == conversationId).FirstOrDefaultAsync();
            item.IsConversationMute = true;
            await Save();
        }

        /// <summary>
        /// Change chat status to Mute
        /// </summary>
        /// <param name="conversationId">Id of the chat</param>
        /// <returns></returns>
        public async Task UnmuteConversation(string conversationId)
        {
            var item = await db.BotKeys.Where(x => x.ConversationId == conversationId).FirstOrDefaultAsync();
            item.IsConversationMute = false;
            await Save();
        }

        /// <summary>
        /// Check Mute status of chat
        /// </summary>
        /// <param name="conversationId">Id of the chat</param>
        /// <returns></returns>
        public async Task<bool> CheckMuteStatus(string conversationId)
        {
            return (await db.BotKeys.FirstOrDefaultAsync(x => x.ConversationId == conversationId)).IsConversationMute;
        }

        /// <summary>
        /// Change bot key in conversation
        /// </summary>
        /// <param name="conversationid">Id of the chat</param>
        /// <param name="newBotKey">New bot key</param>
        /// <returns></returns>
        public async Task ChangeBotKey(string conversationid, string newBotKey)
        {
            var item = db.BotKeys.FirstOrDefault(x => x.ConversationId == conversationid);
            item.SecretKeyBot = newBotKey;
            await Save();
        }

        public async Task Save()
        {
            await db.SaveChangesAsync();
        }

        /// <summary>
        /// Get all chat ID using correct bot key
        /// </summary>
        /// <param name="secretKeyBot">Bot key</param>
        /// <returns>List with founded chat id's</returns>
        public async Task<List<string>> GetUnmuteConversationsIdBySecretKeyBot(string secretKeyBot)
        {
            List<string> conversations = await db.BotKeys.Where(x => (x.SecretKeyBot == secretKeyBot) && (!x.IsConversationMute))
                .Select(x => x.ConversationId)
                .ToListAsync();

            return conversations;
        }

        public async Task<bool> IsConversationAvailable(string secretKeyBot, string conversationId)
        {
            return await db.BotKeys.AnyAsync(x => (x.ConversationId == conversationId) && (x.SecretKeyBot == secretKeyBot) && (x.IsConversationMute == false));
        }

        public async Task<bool> IsConversationIdInDB(string secretKeyBot, string conversationId)
        {
            return await db.BotKeys.AnyAsync(x => (x.SecretKeyBot == secretKeyBot) && (x.ConversationId == conversationId));
        }

        public async Task<bool> IsConversationMuted(string secretKeyBot, string conversationId)
        {
            var item = await db.BotKeys.FirstOrDefaultAsync(x => (x.SecretKeyBot == secretKeyBot) && (x.ConversationId == conversationId));
            return item.IsConversationMute;
        }
    }
}
