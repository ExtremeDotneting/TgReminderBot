using System;
using System.Threading.Tasks;
using IRO.Storage;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.AspNetPipeline.Exceptions;
using Telegram.Bot.AspNetPipeline.Extensions.ReadWithoutContext;
using TgReminderBot.Data;

namespace TgReminderBot.Services
{
    public class SimpleUserInteractionWrapper : ISimpleUserInteractionWrapper
    {
        readonly ChatService _chatService;

        public DateTime LastConversation
        {
            get
            {
                var res = _chatService.GetLastConversationTime(ChatId).Result;
                return res;
            }
            set
            {
                _chatService.SetLastConversationTime(ChatId, value).Wait();
            }
        }

        public MailingSchedule Schedule
        {
            get
            {
                var res = _chatService.GetSchedule(ChatId).Result;
                return res;
            }
            set
            {
                _chatService.SetSchedule(ChatId, value).Wait();
            }
        }

        public ITelegramBotClient Bot { get; }
        public ChatInfo ChatInfo { get; }
        public IKeyValueStorage UserStorage { get; }

        long ChatId => ChatInfo.ChatIdentifier;

        public SimpleUserInteractionWrapper(ITelegramBotClient bot, ChatInfo chatInfo, ChatService userService, ChatScopedStorageProvider userScopedStorageProvider)
        {
            ChatInfo = chatInfo;
            UserStorage = userScopedStorageProvider.GetChatStorage(ChatId);
            _chatService = userService;
            Bot = bot;
        }
        public async Task SendMessage(string msg)
        {
            await Bot.SendTextMessageAsync(ChatId, msg);
        }

        public async Task<string> ReadMessage()
        {
            var msg = await Bot.ReadMessageAsync(ChatId);
            return msg.Text;
        }
    }


}