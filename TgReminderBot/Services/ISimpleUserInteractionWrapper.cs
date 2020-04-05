using System;
using System.Threading.Tasks;
using IRO.Storage;
using Telegram.Bot.Types;
using TgReminderBot.Data;

namespace TgReminderBot.Services
{
    public interface ISimpleUserInteractionWrapper
    {
        DateTime LastConversation { get; set; }

        MailingSchedule Schedule { get; set; }

        IKeyValueStorage UserStorage { get; }
        ChatInfo ChatInfo { get; }

        Task SendMessage(string msg);

        Task<string> ReadMessage();
    }
}