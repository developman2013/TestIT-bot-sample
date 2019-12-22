// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using ProactiveBot.Data;
using ProactiveBot.Services;

namespace Microsoft.BotBuilderSamples
{
    public class ProactiveBot : ActivityHandler
    {
        private BotContext db;

        // Dependency injected dictionary for storing ConversationReference objects used in NotifyController to proactively message users
        private ConcurrentDictionary<string, ConversationReference> _conversationReferences;

        public ProactiveBot(ConcurrentDictionary<string, ConversationReference> conversationReferences, BotContext db)
        {
            _conversationReferences = conversationReferences;
            this.db = db;
        }

        private void AddConversationReference(Activity activity)
        {
            var conversationReference = activity.GetConversationReference();
            _conversationReferences.AddOrUpdate(conversationReference.User.Id, conversationReference, (key, newValue) => conversationReference);
        }

        protected override Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            AddConversationReference(turnContext.Activity as Activity);

            return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello {member.Name}! \n Welcome to our chat!"), cancellationToken);
                }
            }
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            AddConversationReference(turnContext.Activity as Activity);
            DataBaseService dataBaseService = new DataBaseService(db);

            //Save this chat
            await dataBaseService.Add(null, turnContext.Activity.Conversation.Id);
            if (!string.IsNullOrEmpty(turnContext.Activity.Text))
            {
                string userMessage = turnContext.Activity.Text;

                //Create answer if user message contains some command
                if (userMessage.Contains("/"))
                {
                    CommandMessagesService commandMessageService = new CommandMessagesService(db);
                    await turnContext.SendActivityAsync(MessageFactory.Text(await commandMessageService.GetAnswer(turnContext)), cancellationToken);
                }
            }
        }
    }
}
