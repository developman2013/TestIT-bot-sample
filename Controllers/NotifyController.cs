// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ProactiveBot.Data;
using ProactiveBot.Models;
using ProactiveBot.Services;

namespace ProactiveBot.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;
        private BotContext DataBase;
        private string Message = string.Empty;
        private string SecretKeyBot = string.Empty;
        private DataBaseService Service;

        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration, ConcurrentDictionary<string, ConversationReference> conversationReferences, BotContext db)
        {
            _adapter = adapter;
            _conversationReferences = conversationReferences;
            _appId = configuration["MicrosoftAppId"];
            DataBase = db;
            Service = new DataBaseService(db);

            // If the channel is the Emulator, and authentication is not in use,
            // the AppId will be null.  We generate a random AppId for this case only.
            // This is not required for production, since the AppId will have a value.
            if (string.IsNullOrEmpty(_appId))
            {
                _appId = "<YOUR_APP_ID>";

                //if you have not MicrosoftAppId, use a random Guid
                //_appId = Guid.NewGuid().ToString(); 
            }
        }

        /// <summary>
        /// API POST method. Call him using POST-request and send nedded parametres in body and head.
        /// </summary>
        /// <param name="notify">Model with the Body content</param>
        /// <param name="secretKeyBot">Key from TestIT portal</param>
        /// <param name="USER_NAME">User name on portal TestIT</param>
        /// <param name="TEST_PLAN_STATUS">Test plan status on portal TestIT</param>
        /// <returns>Http status code</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NotifyModel notify, [FromHeader] string secretKeyBot, [FromHeader]string USER_NAME, [FromHeader]string TEST_PLAN_STATUS)
        {
            //Clone key to field
            SecretKeyBot = secretKeyBot;

            //Collecting messages from incoming data
            Message =
                $"Доброго времени суток!\n\n" +
                $"`{USER_NAME}` перевёл тест-план `{notify.Name}` в статус `{TEST_PLAN_STATUS}`\n \n ";
            switch (TEST_PLAN_STATUS)
            {
                case "New":
                    Message += "Тест-план в разработке";
                    break;
                case "InProgress":
                    Message +=
                        $"Начинается regression-тестирование проекта `{notify.ProductName}`.\n \n " +
                        $"Сборка: `{notify.Build}` \n \n " +
                        $"Тест-план: `{notify.Link}`\n \n " +
                        $"Все найденные дефекты будут продублированы в Slack и Telegram";
                    break;
                case "Paused":
                    Message += "Тест-план остановлен";
                    break;
                case "Completed":
                    Message += "Тест-план завершён.\n Всем `большое спасибо` за участие в тестировании!";
                    break;
            }

            try
            {
                //A bot can never write first
                //The user must start the dialogue first
                //The field stores current users
                foreach (var conversationReference in _conversationReferences.Values)
                {
                    if (await Service.IsConversationAvailable(secretKeyBot, conversationReference.Conversation.Id))
                    {
                        await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReference, BotCallback, default(CancellationToken));
                    }
                }
            }
            catch (Exception ex)
            {
                //You can post ex.Message to any log service
            }

            // Return 200 HttpCode
            return Ok();


        }

        /// <summary>
        /// Call back method is used to collect chats that need to be sent out and send them.
        /// </summary>
        /// <param name="turnContext">Context of example of posted message</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task BotCallback(ITurnContext turnContext, CancellationToken cancellationToken) => 
            await turnContext.SendActivityAsync(Message);
    }
}
