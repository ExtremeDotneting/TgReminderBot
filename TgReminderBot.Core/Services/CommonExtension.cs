using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using TgReminderBot.Core.Data;

namespace TgReminderBot.Core.Services
{
    public static class CommonExtension
    {
        public static bool HasAdminRights(this UpdateContext uc)
        {
            var admins = AppSettings.AdminsUsernames;
            var isAdmin= admins.Contains(uc.Message.From.Username) ||
                         admins.Contains(uc.Message.Chat.Id.ToString()) || 
                         admins.Contains(uc.Message.Chat.Username ?? "---");
            return isAdmin;
        }

        public static ChatInfo GetChatInfo(this BotController @this)
        {
            return new ChatInfo()
            {
                FromUsername = @this.Message.From.Username,
                ChatIdentifier = @this.Chat.Id
            };
        }
    }
}
