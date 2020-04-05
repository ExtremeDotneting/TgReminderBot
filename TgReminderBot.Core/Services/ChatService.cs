using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IRO.Storage;
using TgReminderBot.Core.Data;

namespace TgReminderBot.Core.Services
{
    public class ChatService
    {
        readonly IKeyValueStorage _storage;

        public ChatService(IKeyValueStorage storage)
        {
            _storage = storage;
        }

        public async Task SetLastConversationTime(long chatId, DateTime lastConversationTime)
        {
            await _storage.Set($"user_last_conversation_time__{chatId}", lastConversationTime);
        }

        public async Task<DateTime> GetLastConversationTime(long chatId)
        {
            var res = (await _storage.GetOrDefault<DateTime?>($"user_last_conversation_time__{chatId}")) ?? DateTime.MinValue;
            return res;
        }

        public async Task SetSchedule(long chatId, MailingSchedule mailingSchedule)
        {
            await _storage.Set($"user_schedule__{chatId}", mailingSchedule);
        }

        public async Task<MessageShortInfo> GetLastMessageInfo(long chatId)
        {
            var schedule = (await _storage.GetOrDefault<MessageShortInfo>($"user_last_message_info__{chatId}")) ?? new MessageShortInfo()
            {
                ChatIdentifier = chatId
            };
            return schedule;
        }

        public async Task SetLastMessageInfo(long chatId, MessageShortInfo messageShortInfo)
        {
            await _storage.Set($"user_last_message_info__{chatId}", messageShortInfo);
        }

        public async Task<MailingSchedule> GetSchedule(long chatId)
        {
            var schedule = (await _storage.GetOrDefault<MailingSchedule>($"user_schedule__{chatId}")) ?? new MailingSchedule();
            return schedule;
        }

        public async Task SetIsMailing(long chatId, bool mailingEnabled)
        {
            var collection = await GetChatsHashSetWithEnabledMailing();
            if (collection.Contains(chatId))
            {
                if (!mailingEnabled)
                {
                    collection.Remove(chatId);
                }
            }
            else
            {
                if (mailingEnabled)
                {
                    collection.Add(chatId);
                }
            }
            await _storage.Set("users_mailing_enabled", collection);
        }

        public async Task<bool> IsMailing(long chatId)
        {
            var collection = await GetChatsHashSetWithEnabledMailing();
            return collection.Contains(chatId);
        }

        public async Task<ICollection<long>> GetChatsWithEnabledMailing() => await GetChatsHashSetWithEnabledMailing();

        //public async Task SetUserChatId(string username, long identifier)
        //{
        //    username = PrepareUsername(username);
        //    await _storage.Set("chat_id_for__" + username, identifier);
        //}

        //public async Task<ChatId> GetUserChatId(string username)
        //{
        //    username = PrepareUsername(username);
        //    var identifier = await _storage.Get<long>("chat_id_for__" + username);
        //    var chatId = new ChatId(identifier: identifier);
        //    return chatId;
        //}

        string PrepareUsername(string username)
        {
            if (username == null)
            {
                throw new Exception("Username is null.");
            }
            if (!username.StartsWith("@"))
            {
                username = "@" + username;
            }
            return username;
        }

        async Task<HashSet<long>> GetChatsHashSetWithEnabledMailing()
        {
            var collection = (await _storage.GetOrDefault<HashSet<long>>("users_mailing_enabled")) ?? new HashSet<long>();
            return collection;
        }
    }
}
