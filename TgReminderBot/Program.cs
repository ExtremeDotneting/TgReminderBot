using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IRO.Storage;
using IRO.Storage.WithLiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions;
using Telegram.Bot.Types.Enums;
using TgReminderBot.Data;
using TgReminderBot.HostedServices;
using TgReminderBot.Services;
using TgReminderBot.Services.Messaging;

namespace TgReminderBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new TelegramBotClient(AppSettings.BotToken, new QueuedHttpClient(TimeSpan.FromMilliseconds(100)));
            var botManager = new BotManager(bot);
            botManager.ConfigureServices((servicesWrap) =>
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console(LogEventLevel.Debug)
                    .WriteTo.Debug(LogEventLevel.Debug)
                    .CreateLogger();
                var serv = servicesWrap.Services;
                serv.AddLogging(conf =>
                {
                    conf.AddSerilog(Log.Logger);
                });
                serv.AddSingleton<ITelegramBotClient>(bot);
                serv.AddSingleton<PlainMessagesService>();
                serv.AddSingleton<ScriptedMessagesService>();
                serv.AddSingleton<SheduleWorker>();
                serv.AddSingleton<BaseHostedService<SheduleWorker>>();
                serv.AddSingleton<IKeyValueStorage>(
                    new LiteDatabaseStorage()
                    );
                serv.AddSingleton<ChatScopedStorageProvider>();
                serv.AddSingleton<PlainMessagesService>();
                serv.AddSingleton<MessagingFacade>();
                serv.AddSingleton<ChatService>();

                servicesWrap.AddMvc(new MvcOptions()
                {
                    CheckEqualsRouteInfo = true
                });
            });
            botManager.ConfigureBuilder((builder) =>
            {
                builder.Use(async (ctx, next) =>
                {
                    if (ctx.Update.Type == UpdateType.Message)
                    {
                        var userService = ctx.Services.GetRequiredService<ChatService>();
                        await userService.SetLastMessageInfo(ctx.Chat.Id, new MessageShortInfo()
                        {
                            ChatIdentifier = ctx.Chat.Id,
                            FromUsername = ctx.Message.From.Username,
                            MessageId = ctx.Message.MessageId
                        });
                    }
                    await next();
                });

                if (AppSettings.IsDevEnvironment)
                    builder.UseDevEceptionMessage();
                builder.UseOldUpdatesIgnoring();
                builder.UseMvc(mvcBuilder =>
                {
                    //Write /debug to see info about routing.
                    if (AppSettings.IsDevEnvironment)
                        mvcBuilder.UseDebugInfo();
                });
            });

            //Start
            botManager.Start();
            var usersSheduleWorker = botManager.Services.GetRequiredService<SheduleWorker>();
            Task.Run(async () =>
            {
                await usersSheduleWorker.StartAsync(default(CancellationToken));
            });

            


            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
