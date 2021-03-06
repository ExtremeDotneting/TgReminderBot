﻿using System;
using System.Threading;
using System.Threading.Tasks;
using IRO.Common.Services;
using Serilog;
using Telegram.Bot;
using TgReminderBot.Core.Services;
using TgReminderBot.Core.Services.Messaging;

namespace TgReminderBot.Core.HostedServices
{
    public class ScheduleWorker : ScopedWorker
    {
        readonly ChatService _chatService;
        readonly MessagingFacade _messagingFacade;
        readonly ITelegramBotClient _bot;

        public ScheduleWorker(ChatService chatService, ITelegramBotClient bot, MessagingFacade messagingFacade)
        {
            _chatService = chatService;
            _bot = bot;
            _messagingFacade = messagingFacade;
        }

        protected override async Task Execute(CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(30000, cancellationToken);
                var chats = await _chatService.GetChatsWithEnabledMailing();
                foreach (var chatId in chats)
                {
                    try
                    {
                        var t = ProcessChat(chatId);
                        await TaskExt.WhenAll(
                            new Task[] { t }, 
                            TimeSpan.FromSeconds(5)
                            );
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.ToString());
                    }
                }
            }
        }

        async Task ProcessChat(long chatId)
        {

            var schedule = await _chatService.GetSchedule(chatId);

            //Skip by schedule.
            var chatTime = TimeSpan.FromHours(schedule.TimeZone) + TimeSpan.FromHours(DateTime.UtcNow.Hour);
            if (chatTime.TotalHours > 24)
            {
                chatTime -= TimeSpan.FromHours(24);
            }

            if (chatTime.TotalHours < schedule.Range.StartTime.TotalHours ||
                chatTime.TotalHours > schedule.Range.EndTime.TotalHours)
            {
                return;
            }

            //Skip by delay.
            var lastConv = await _chatService.GetLastConversationTime(chatId);
            if (DateTime.UtcNow - lastConv < schedule.Delay)
                return;

            //Send message.
            await _messagingFacade.SendRandomMessage(_bot, new Data.ChatInfo()
            {
                ChatIdentifier = chatId
            });
        }
    }
}
