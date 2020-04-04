using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.Types;
using TgReminderBot.Data;

namespace TgReminderBot.Services.Messaging
{
    public class MessagingFacade
    {
        readonly Random _rd = new Random();
        readonly UserScopedStorageProvider _scopedStorageProvider;
        readonly PlainMessagesService _plainMessagesService;
        readonly UserService _userService;
        readonly ScriptedMessagesService _scriptedMessagesService;

        public MessagingFacade(UserScopedStorageProvider scopedStorageProvider, PlainMessagesService plainMessagesService, UserService userService)
        {
            _scopedStorageProvider = scopedStorageProvider;
            _plainMessagesService = plainMessagesService;
            _userService = userService;
            //Crunch here.
            _scriptedMessagesService = new ScriptedMessagesService(ConversationScripts.Scripts);
        }

        public async Task SendRandomMessage(ITelegramBotClient bot, ChatInfo chatInfo)
        {
            var rdNum = _rd.Next(_plainMessagesService.Count + _scriptedMessagesService.Count+1);
            if (_plainMessagesService.Count > rdNum)
            {
                var text = _plainMessagesService.GetRandomPhrase();
                await bot.SendTextMessageAsync(chatInfo.ChatIdentifier, text);
            }
            else
            {
                var user = new SimpleUserInteractionWrapper(bot, username, _userService, _scopedStorageProvider);
                await _scriptedMessagesService.MakeRandomСonversation(user);
            }
            await _userService.SetLastConversationTime(username, DateTime.UtcNow);
        }
    }
}
