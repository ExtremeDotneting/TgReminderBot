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
        readonly ChatScopedStorageProvider _scopedStorageProvider;
        readonly PlainMessagesService _plainMessagesService;
        readonly ChatService _chatService;
        readonly ScriptedMessagesService _scriptedMessagesService;

        public MessagingFacade(ChatScopedStorageProvider scopedStorageProvider, PlainMessagesService plainMessagesService, ChatService userService)
        {
            _scopedStorageProvider = scopedStorageProvider;
            _plainMessagesService = plainMessagesService;
            _chatService = userService;
            //Crunch here.
            _scriptedMessagesService = new ScriptedMessagesService(ConversationScripts.Scripts);
        }

        public async Task SendRandomMessage(ITelegramBotClient bot, ChatInfo chatInfo)
        {
            var rdNum = _rd.Next(_plainMessagesService.Count + _scriptedMessagesService.Count+1);
            if (_plainMessagesService.Count > rdNum)
            {
                var text = _plainMessagesService.GetRandomPhrase();
                var lastMsgInfo=await _chatService.GetLastMessageInfo(chatInfo.ChatIdentifier);
                await bot.SendTextMessageAsync(chatInfo.ChatIdentifier, text, replyToMessageId: lastMsgInfo.MessageId);
            }
            else
            {
                var user = new SimpleUserInteractionWrapper(bot, chatInfo, _chatService, _scopedStorageProvider);
                await _scriptedMessagesService.MakeRandomСonversation(user);
            }
            await _chatService.SetLastConversationTime(chatInfo.ChatIdentifier, DateTime.UtcNow);
        }
    }
}
