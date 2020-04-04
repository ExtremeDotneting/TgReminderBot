using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Routing.Metadata;
using TgReminderBot.Data;
using TgReminderBot.Services;
using TgReminderBot.Services.Messaging;

namespace TgReminderBot.BotControllers
{
    public class MainBotController : BotController
    {
        readonly MessagingFacade _messagingFacade;
        readonly UserService _userService;

        public MainBotController(MessagingFacade messagingFacade, UserService userService)
        {
            _messagingFacade = messagingFacade;
            _userService = userService;
        }

        [BotRoute("/start")]
        public async Task Start()
        {
            await SendTextMessageAsync($"Привет, меня зовут Игорян. Я психоделический бот-напоминалка. " +
                                                     $"Моя цель, заложенная моим больным создателем, - напоминать что ты все еще жив.\n\n" +
                                                     $"Если ты тоже ощущаешь что время идет слишком быстро, что ты стал слишком ленивым и тебе не хватает мотивации " +
                                                     $"или просто хочешь попрактиковаться в осознанных сноведениях, то добро пожаловать.\n\n" +
                                                     $"Составь расписание и получай сообщения. Жмакни /help");
            await _userService.SetIsMailing(Message.From.Username, true);
        }
   

        [BotRoute("/help")]
        public async Task Help()
        {
            await SendTextMessageAsync($"/start_mailing - Начать присылать сообщения.\n" +
                                                     $"/stop_mailing - Отписаться от сообщений.\n" +
                                                     $"/schedule - Посмотреть/изменить расписание. Жми это если впервые здесь.\n" +
                                                     $"/random - Получить напутствие от Игоряна сейчас.\n");
        }

        [BotRoute("/schedule")]
        public async Task Schedule()
        {
            var schedule = await _userService.GetSchedule(Message.From.Username);
            await SendTextMessageAsync(
                $"Текущее расписание '{schedule.ToHoursString()}'." +
                "\n\nЧтоб изменить расписание - отправьте сообщение формата" +
                "\n/schedule StartHour-{EndHour, +TimeZone, DelayMinutes");

            if (Message.Text.Trim() != "/schedule")
            {
                var scheduleStr = Message.Text.Replace("/schedule", "").Trim();
                try
                {
                    schedule = MailingSchedule.FromHoursString(scheduleStr);
                    await _userService.SetSchedule(Message.From.Username, schedule);
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
            await _userService.SetIsMailing(Message.From.Username, true);
            await SendTextMessageAsync($"Буду радовать тебя сообщениями))");
        }

        [BotRoute("/stop_mailing")]
        public async Task StopMailing()
        {
            await _userService.SetIsMailing(Message.From.Username, false);
            await SendTextMessageAsync($"Сообщения преостановленны.");
        }

        [BotRoute("/random")]
        public async Task Random()
        {
            await _messagingFacade.SendRandomMessage(Bot, Message.From.Username);
        }
    }
}
