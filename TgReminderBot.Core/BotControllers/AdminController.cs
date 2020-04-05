using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Extensions.ImprovedBot;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using TgReminderBot.Core.Services;
using TgReminderBot.Core.Services.Messaging;

namespace TgReminderBot.Core.BotControllers
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

        [BotRoute("/add_phrase", Name = "AddPhrase")]
        public async Task AddPhrase()
        {
            if (!UpdateContext.IsUserAdmin())
                return;
            await SendTextMessageAsync($"Введите фразу или отправьте 'n' для отмены:\n");
            var msg = await BotExt.ReadMessageAsync(ReadCallbackFromType.CurrentUserReply);
            var text = msg.Text.Trim();
            if (text == "n")
            {
                await SendTextMessageAsync($"Отменено.");
            }
            else
            {
                await _plainMessagesService.AddGlobalPhrase(text);
                await SendTextMessageAsync($"Добавлено.");
            }
        }
    }
}
