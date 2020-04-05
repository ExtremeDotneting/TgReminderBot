using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Mvc.Controllers.Core;
using Telegram.Bot.Types;
using TgReminderBot.Data;

namespace TgReminderBot.Services
{
    public static class CommonExtension
    {
        public static bool IsUserAdmin(this UpdateContext uc)
        {
            var admins = AppSettings.AdminsUsernames;
            return admins.Contains(uc.Message.From.Username);
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
