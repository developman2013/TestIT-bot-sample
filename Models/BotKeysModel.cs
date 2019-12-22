using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProactiveBot.Models
{
    public class BotKeysModel
    {
        public Guid Id { get; set; }
        public string SecretKeyBot { get; set; }
        public string ConversationId { get; set; }
        public bool IsConversationMute { get; set; }
    }
}
