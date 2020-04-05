using System;
using System.Threading.Tasks;
using IRO.Storage;
using TgReminderBot.Core.Data;

namespace TgReminderBot.Core.Services
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