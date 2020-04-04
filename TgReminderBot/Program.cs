using System;
using System.Threading;
using System.Threading.Tasks;
using IRO.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Telegram.Bot;
using Telegram.Bot.AspNetPipeline.Core;
using Telegram.Bot.AspNetPipeline.Extensions;
using Telegram.Bot.AspNetPipeline.Mvc.Builder;
using Telegram.Bot.AspNetPipeline.Mvc.Extensions;
using TgReminderBot.HostedServices;
using TgReminderBot.Services;
using TgReminderBot.Services.Messaging;

namespace TgReminderBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new TelegramBotClient(AppSettings.BotToken);
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
                serv.AddSingleton<PlainMessagesService>();
                serv.AddSingleton<ScriptedMessagesService>();
                serv.AddSingleton<UsersSheduleWorker>();
                serv.AddSingleton<BaseHostedService<UsersSheduleWorker>>();
                serv.AddSingleton<IKeyValueStorage>(new IRO.Storage.DefaultStorages.FileStorage("mainStorage.json"));
                serv.AddSingleton<UserScopedStorageProvider>();
                serv.AddSingleton<PlainMessagesService>();
                serv.AddSingleton<MessagingFacade>();
                serv.AddSingleton<UserService>();

                servicesWrap.AddMvc(new MvcOptions()
                {
                    CheckEqualsRouteInfo = true
                });
            });
            botManager.ConfigureBuilder((builder) =>
            {
                builder.Use(async (ctx, next) =>
                {
                    var userService=ctx.Services.GetRequiredService<UserService>();
                    await userService.SetUserChatId(ctx.Message.From.Username, ctx.Message.From.Id);
                    await next();
                });

                if (AppSettings.IsDevEnvironment)
                    builder.UseDevEceptionMessage();
                builder.UseOldUpdatesIgnoring();
                builder.UseMvc(mvcBuilder =>
                {
                    //Write /debug to see info about routing.
                    mvcBuilder.UseDebugInfo();
                });
            });

            //Start
            botManager.Start();
            var usersSheduleWorker = botManager.Services.GetRequiredService<UsersSheduleWorker>();
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
