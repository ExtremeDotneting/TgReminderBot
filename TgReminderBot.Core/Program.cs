using System;
using System.Threading;
using System.Threading.Tasks;
using IRO.Storage;
using IRO.Storage.WithLiteDB;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions;
using Telegram.Bot.Types.Enums;
using TgReminderBot.Core.Data;
using TgReminderBot.Core.HostedServices;
using TgReminderBot.Core.Services;
using TgReminderBot.Core.Services.Messaging;

namespace TgReminderBot.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }
}
