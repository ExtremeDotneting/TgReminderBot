using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace TgReminderBot
{
    public static class AppSettings
    {
        private static IConfigurationRoot Config { get; }

        static AppSettings()
        {
            var jsonPath = "appsettings.json";
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!string.IsNullOrWhiteSpace(env))
            {
                jsonPath = $"appsettings.{env}.json";
            }
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(jsonPath)
                .AddEnvironmentVariables();
            Config = builder.Build();
        }

        public static string GetAppSettingsJson()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var jsonPath = !string.IsNullOrWhiteSpace(env) ? $"appsettings.{env}.json" : $"appsettings.json";
            return File.ReadAllText(jsonPath);
        }

        public static string BotToken
        {
            get
            {
                var token = Config["botToken"];
                return token;
            }
        }

        public static long ProposedMessagesChannelId
        {
            get
            {
                var res = Config["proposedMessagesChannelId"];
                return Convert.ToInt64(res);
            }
        }

        public static bool IsDevEnvironment
        {
            get
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                return env == "Development";
            }
        }

        static HashSet<string> _adminsUsernames;
        public static HashSet<string> AdminsUsernames
        {
            get
            {
                if (_adminsUsernames == null)
                {
                    var jToken = JToken.Parse(GetAppSettingsJson());
                    _adminsUsernames = jToken["adminsUsernames"].ToObject<HashSet<string>>();
                }
                return _adminsUsernames;
            }
        }
    }
}