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
        readonly ChatService _chatService;
        readonly MessagingFacade _messagingFacade;

        public AdminController(ChatService userService, MessagingFacade messagingFacade)
        {
            _chatService = userService;
            _messagingFacade = messagingFacade;
        }

        [BotRoute("/admin_help", Name = "AdminHelp")]
        public async Task Admin()
        {
            if (!UpdateContext.HasAdminRights())
                return;
            await SendTextMessageAsync($"/add_phrase - Добавить сообщение в список сообщений. " +
                                       $"Это глобальный список и сообщения будут видны всем пользователям.\n" +
                                       $"/scope - Посмотреть/изменить окружение чата. Окружение - это набор фраз используемых ботом в диалогах.\n" +
                                       $"/stop_all_mailing - Остановить все переписки.");
        }

        [BotRoute("/scope")]
        public async Task Scope()
        {
            if (!UpdateContext.HasAdminRights())
                return;

            var scope = await _chatService.GetChatScope(this.GetChatInfo().ChatIdentifier);
            await SendTextMessageAsync(
                $"Текущее окружение чата '{scope}'.");

            var str = Message.Text.Trim();
            if (str.Contains(" "))
            {
                var index = str.IndexOf(" ") + 1;
                var newScope = str
                    .Substring(index)
                    .ToLower()
                    .Trim();

                try
                {
                    await _chatService.SetChatScope(this.GetChatInfo().ChatIdentifier, newScope);
                    await SendTextMessageAsync($"Новое окружение '{newScope}'.");
                }
                catch
                {
                    await SendTextMessageAsync("Ошибка парсинга, окружение не изменено.");
                }
            }
        }

        [BotRoute("/add_phrase", Name = "AddPhrase")]
        public async Task AddPhrase()
        {
            if (!UpdateContext.HasAdminRights())
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
                await _messagingFacade.AddPhrase(text, this.GetChatInfo());
                await SendTextMessageAsync($"Добавлено.");
            }
        }

        [BotRoute("/stop_all_mailing")]
        public async Task StopAllMailing()
        {
            if (!UpdateContext.HasAdminRights())
                return;
            await _chatService.StopAllMailing();
            await SendTextMessageAsync($"Все сообщения приостановленны.");
        }
    }
}
