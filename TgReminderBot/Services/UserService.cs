using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IRO.Storage;
using Telegram.Bot.Types;
using TgReminderBot.Data;

namespace TgReminderBot.Services
{
    public class UserService
    {
        readonly IKeyValueStorage _storage;

        public UserService(IKeyValueStorage storage)
        {
            _storage = storage;
        }

        public async Task SetLastConversationTime(string username, DateTime lastConversationTime)
        {
            username = PrepareUsername(username);
            await _storage.Set($"user_last_conversation_time__{username}", lastConversationTime);
        }

        public async Task<DateTime> GetLastConversationTime(string username)
        {
            username = PrepareUsername(username);
            var res = (await _storage.GetOrDefault<DateTime?>($"user_last_conversation_time__{username}")) ?? DateTime.MinValue;
            return res;
        }

        public async Task SetSchedule(string username, MailingSchedule mailingSchedule)
        {
            username = PrepareUsername(username);
            await _storage.Set($"user_schedule__{username}", mailingSchedule);
        }

        public async Task<MailingSchedule> GetSchedule(string username)
        {
            username = PrepareUsername(username);
            var schedule = (await _storage.GetOrDefault<MailingSchedule>($"user_schedule__{username}")) ?? new MailingSchedule();
            return schedule;
        }

        public async Task SetIsMailing(string username, bool mailingEnabled)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username));
            var collection = await GetUsersHashSetWithEnabledMailing();
            if (collection.Contains(username))
            {
                if (!mailingEnabled)
                {
                    collection.Remove(username);
                }
            }
            else
            {
                if (mailingEnabled)
                {
                    collection.Add(username);
                }
            }
            await _storage.Set("users_mailing_enabled", collection);
        }

        public async Task<bool> IsMailing(string username)
        {
            if (username == null)
                throw new ArgumentNullException(nameof(username));
            var collection = await GetUsersHashSetWithEnabledMailing();
            return collection.Contains(username);
        }

        public async Task<ICollection<string>> GetUsersWithEnabledMailing() => await GetUsersHashSetWithEnabledMailing();

        public async Task SetUserChatId(string username, long identifier)
        {
            username = PrepareUsername(username);
            await _storage.Set("chat_id_for__" + username, identifier);
        }

        public async Task<ChatId> GetUserChatId(string username)
        {
            username = PrepareUsername(username);
            var identifier = await _storage.Get<long>("chat_id_for__" + username);
            var chatId = new ChatId(identifier: identifier);
            return chatId;
        }

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

        async Task<HashSet<string>> GetUsersHashSetWithEnabledMailing()
        {
            var collection = (await _storage.GetOrDefault<HashSet<string>>("users_mailing_enabled")) ?? new HashSet<string>();
            return collection;
        }
    }
}
