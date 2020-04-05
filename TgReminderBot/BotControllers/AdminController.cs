using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using TgReminderBot.Services;
using TgReminderBot.Services.Messaging;

namespace TgReminderBot.BotControllers
{
    public class AdminController : BotController
    {
        readonly PlainMessagesService _plainMessagesService;
        readonly ChatService _chatService;

        public AdminController(ChatService userService, PlainMessagesService plainMessagesService)
        {
            _chatService = userService;
            _plainMessagesService = plainMessagesService;
        }

        [BotRoute("/admin_help")]
        public async Task Admin()
        {
            if (!UpdateContext.IsUserAdmin())
                return;
            await SendTextMessageAsync($"/add_phrase - Добавить сообщение в список сообщений. " +
                                       $"Это глобальный список и сообщения будут видны всем пользователям.\n");
        }

        [BotRoute("/add_phrase")]
        public async Task AddPhrase()
        {
            if (!UpdateContext.IsUserAdmin())
                return;
            await SendTextMessageAsync($"Введите фразу. Фраза должна начинаться с слеша '\\', иначе она не будет добавлена:\n");
            var msg = await BotExt.ReadMessageAsync();
            var text = msg.Text.Trim();
            if (text.StartsWith("\\"))
            {
                text = text.Substring(1);
                await _plainMessagesService.AddGlobalPhrase(text);
                await SendTextMessageAsync($"Добавлено.");
            }
            else
            {
                await SendTextMessageAsync($"Отменено.");
            }
        }
    }
}
