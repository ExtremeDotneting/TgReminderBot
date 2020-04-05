using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
using TgReminderBot.Core;
using TgReminderBot.Core.Data;
using TgReminderBot.Core.HostedServices;
using TgReminderBot.Core.Services;
using TgReminderBot.Core.Services.Messaging;
using TgReminderBot.Wpf.Services;

namespace TgReminderBot.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        void Application_Startup(object sender, StartupEventArgs e)
        {
            TrayIconHelper.ShowNotification("", "1111.");
            TrayIconHelper.InitTrayIcon(TrayIconHelper.MenuFromDict(new Dictionary<string, Action>()
            {
                {
                    "Add to Windows startup",
                    ()=>
                    {
                        WindowsStartupService.SetAutolaunch(true);
                    }
                },
                {
                    "Remove from Windows startup",
                    ()=>
                    {
                        WindowsStartupService.SetAutolaunch(false);
                    }
                },
                {
                    "Exit",
                    ()=>
                    {
                        Application.Current.Shutdown();
                    }
                }
            }));

            StartBot();
            TrayIconHelper.ShowNotification("", "IgorReminderBot launched.");
            //Crunch to keep it launched.
            var w = new Window() {Height = 100, Width = 100};
            w.Show();
            w.Visibility = Visibility.Hidden;
        }

        void StartBot()
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
                serv.AddSingleton<ScheduleWorker>();
                serv.AddSingleton<BaseHostedService<ScheduleWorker>>();
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
            Task.Run(async () =>
            {
                botManager.Start();
                var usersScheduleWorker = botManager.Services.GetRequiredService<ScheduleWorker>();
                await usersScheduleWorker.StartAsync(default(CancellationToken));
            });
        }
    }
}
