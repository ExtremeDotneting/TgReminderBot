using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.AspNetPipeline.Core;

namespace TgReminderBot.Services
{
    public static class IsAdminExtension
    {
        public static bool IsUserAdmin(this UpdateContext uc)
        {
            var admins = AppSettings.AdminsUsernames;
            return admins.Contains(uc.Message.From.Username);
        }
    }
}
