# TestIT-bot-sample

## TestIT-bot-sample - bot sending notifications from the [TestIT](https://testit.software) system.
This bot is a freeware example of how you can use webhooks from any platform to notify users in instant messengers.
You can try out the work of this bot in [Telegram](https://t.me/testIT_tg_bot) and Slack.
Before using the bot, familiarize yourself with the [Bot Framework](https://dev.botframework.com) technology.

## Instruction for use
1.  Register your new bot on the [Azure portal](http://portal.azure.com). Manual: [Microsoft Docs](https://docs.microsoft.com/en-en/azure/bot-service/abs-quickstart?view=azure-bot-service-4.0&viewFallbackFrom=azure-bot-service-3.0)
2.  Change `_appId` in [NotifyController](Controllers/NotifyController.cs) .
3.  Change [ConnectionString](appsettings.json) to your database. You can use local or server database.
4.  Create a new webhook on the TestIT portal. Manual: [TestIT docs](). For the webhook link, specify the following line: `https://<NAME_OF_YOUR_BOT>.azurewebsites.net/api/notify`. The webhook must contain the request body. Use the `Post` method when setting up your webhook.
The request headers should contain the following fields:
    - `secretKeyBot`: The unique key of dialogs. We recommend using a set of letters and numbers with a length of at least 15;
    - `USER_NAME`: `$USER_NAME` on the TestIT portal;
    - `TEST_PLAN_STATUS`: `$TEST_PLAN_STATUS` on the TestIT portal.
5.  Publish a bot on Azure. Manual: [Microsoft Docs](https://docs.microsoft.com/ru-ru/azure/bot-service/bot-builder-deploy-az-cli?view=azure-bot-service-4.0&tabs=csharp)
6.  Register new bot communication channels in instant messengers. Manual: [Microsoft Docs](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-manage-channels?view=azure-bot-service-3.0)
7.  Write any message to your bot. After that send the command to register the dialogue key: `/changechatkey <YOUR_KEY>`. If the bot says `Secret bot key changed to <YOUR_KEY>` to you, then the registration was successful.
8.  Try sending a webhook on the TestIT portal. If you did everything correctly and the bot key in the request matches the one specified in the personal dialogue with the bot, you will receive a message in the messenger used.

## Some details
This bot uses local storage of dialogue data. In the event of a server restart or repeated publication of the bot on Azure, it forgets the previous dialogs. To permanently store information about dialogs, it is recommended to use a database.