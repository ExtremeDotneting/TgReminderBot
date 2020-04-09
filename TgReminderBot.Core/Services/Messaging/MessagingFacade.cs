using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using TgReminderBot.Core.Data;

namespace TgReminderBot.Core.Services.Messaging
{
    public class MessagingFacade
    {
        readonly Random _rd = new Random();
        readonly ChatScopedStorageProvider _scopedStorageProvider;
        readonly ChatService _chatService;
        readonly ScriptedMessagesService _scriptedMessagesService;

        readonly IDictionary<string, PlainMessagesService> _plainMessagesServices =
            new ConcurrentDictionary<string, PlainMessagesService>();

        public MessagingFacade(ChatScopedStorageProvider scopedStorageProvider, ChatService userService)
        {
            _scopedStorageProvider = scopedStorageProvider;
            _chatService = userService;
            //Crunch here.
            _scriptedMessagesService = new ScriptedMessagesService(ConversationScripts.Scripts);
        }

        public async Task SendRandomMessage(ITelegramBotClient bot, ChatInfo chatInfo)
        {
            var pmServ = await GetPlainMessagesService(chatInfo);
            var rdNum = _rd.Next(pmServ.Count + _scriptedMessagesService.Count + 1);
            if (pmServ.Count > rdNum)
            {
                var text = pmServ.GetRandomPhrase();
                await pmServ.SendRandomPhrase(bot, chatInfo);
            }
            else
            {
                var user = new SimpleUserInteractionWrapper(bot, chatInfo, _chatService, _scopedStorageProvider);
                await _scriptedMessagesService.MakeRandomСonversation(user);
            }
            await _chatService.SetLastConversationTime(chatInfo.ChatIdentifier, DateTime.UtcNow);
        }

        public async Task AddPhrase(string str, ChatInfo chatInfo)
        {
            var pmServ = await GetPlainMessagesService(chatInfo);
            await pmServ.AddPhrase(str);
        }

        async Task<PlainMessagesService> GetPlainMessagesService(ChatInfo chatInfo)
        {
            var phrasesScope = await _chatService.GetChatScope(chatInfo.ChatIdentifier);
            if (_plainMessagesServices.TryGetValue(phrasesScope, out var pmServ))
            {
                return pmServ;
            }
            else
            {
                pmServ = new PlainMessagesService(phrasesScope);
                _plainMessagesServices[phrasesScope] = pmServ;
                return pmServ;
            }
        }
    }
}
