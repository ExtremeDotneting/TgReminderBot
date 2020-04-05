using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using Telegram.Bot.Types;
using TgReminderBot.Core.Data;
using TgReminderBot.Core.Services;
using TgReminderBot.Core.Services.Messaging;

namespace TgReminderBot.Core.BotControllers
{
    public class MainBotController : BotController
    {
        readonly MessagingFacade _messagingFacade;
        readonly ChatService _chatService;

        public MainBotController(MessagingFacade messagingFacade, ChatService userService)
        {
            _messagingFacade = messagingFacade;
            _chatService = userService;
        }

        [BotRoute("/start")]
        public async Task Start()
        {
            await SendTextMessageAsync($"Привет, меня зовут Игорян. Я психоделический бот-напоминалка." +
                                                     $"Моя цель, заложенная моим больным создателем, - напоминать что ты все еще жив.\n\n" +
                                                     $"Если ты тоже ощущаешь что время идет слишком быстро, что ты стал слишком ленивым и тебе не хватает мотивации " +
                                                     $"или просто хочешь попрактиковаться в осознанных сноведениях, то добро пожаловать.\n\n" +
                                                     $"Составь расписание и получай сообщения." +
                                                     $"\n\nЖмакни /help . Или напиши 'Игорь помоги'.");
            await _chatService.SetIsMailing(this.GetChatInfo().ChatIdentifier, true);
        }


        [BotRoute("/help")]
        public async Task Help()
        {
            await SendTextMessageAsync($"/start_mailing - Начать присылать сообщения.\n" +
                                                     $"/stop_mailing - Отписаться от сообщений.\n" +
                                                     $"/schedule - Посмотреть/изменить расписание. Жми это если впервые здесь.\n" +
                                                     $"/random (Игорь) - Получить напутствие от Игоряна сейчас.\n" +
                                                     $"/propose_phrase - Предложить фразу.");
        }

        [BotRoute("/schedule")]
        public async Task Schedule()
        {
            var schedule = await _chatService.GetSchedule(this.GetChatInfo().ChatIdentifier);
            await SendTextMessageAsync(
                $"Текущее расписание '{schedule.ToHoursString()}'." +
                "\n\nЧтоб изменить расписание - отправьте сообщение формата" +
                "\n/schedule StartHour-{EndHour, +TimeZone, DelayMinutes");

            var scheduleStr = Message.Text.Trim();
            if (scheduleStr != "/schedule")
            {
                var index = scheduleStr.IndexOf(" ") + 1;
                scheduleStr = scheduleStr.Substring(index);

                try
                {
                    schedule = MailingSchedule.FromHoursString(scheduleStr);
                    await _chatService.SetSchedule(this.GetChatInfo().ChatIdentifier, schedule);
                    await SendTextMessageAsync($"Новое расписание '{schedule.ToHoursString()}'.");
                }
                catch
                {
                    await SendTextMessageAsync("Ошибка парсинга, расписание не изменено.");
                }
            }
        }

        [BotRoute("/start_mailing")]
        public async Task StartMailing()
        {
            await _chatService.SetIsMailing(this.GetChatInfo().ChatIdentifier, true);
            await SendTextMessageAsync($"Буду радовать тебя сообщениями))");
        }

        [BotRoute("/stop_mailing")]
        public async Task StopMailing()
        {
            await _chatService.SetIsMailing(this.GetChatInfo().ChatIdentifier, false);
            await SendTextMessageAsync($"Сообщения преостановленны.");
        }

        [BotRoute("/random")]
        public async Task Random()
        {
            await _messagingFacade.SendRandomMessage(Bot, this.GetChatInfo());
        }

        [BotRoute(Order = 1)]
        public async Task LanguageFriendly()
        {
            var text = Message.Text.Trim().Replace(" ", "").ToLower();
            if (text == "игорь" || text == "игорян")
            {
                await Random();
            }
            if (text == "игорьпомоги" || text == "игорянпомоги")
            {
                await Help();
            }
        }

        [BotRoute("/propose_phrase")]
        public async Task ProposePhrase()
        {
            if (!UpdateContext.IsUserAdmin())
                return;
            await SendTextMessageAsync($"Введите фразу. Фраза должна начинаться с слеша '\\', иначе она не будет добавленна:\n");
            var msg = await BotExt.ReadMessageAsync();
            var text = msg.Text.Trim();
            if (text.StartsWith("\\"))
            {
                var chatId = new ChatId(AppSettings.ProposedMessagesChannelId);
                await Bot.ForwardMessageAsync(chatId, Chat, msg.MessageId);
                await SendTextMessageAsync($"Отправленно админу на проверку.");
            }
            else
            {
                await SendTextMessageAsync($"Отменено.");
            }
        }
    }
}
