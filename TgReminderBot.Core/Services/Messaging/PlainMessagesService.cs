using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace TgReminderBot.Core.Services.Messaging
{
    public class PlainMessagesService
    {
        readonly string BotPhrasesFilePath;

        readonly Random _rd = new Random();
        List<string> _phrases;

        public int Count => _phrases.Count;

        public PlainMessagesService(string scopeName = null)
        {
            scopeName ??= "default";
            BotPhrasesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"..\\..\\..\\BotPhrases\\{scopeName}.txt");
            if (!File.Exists(BotPhrasesFilePath))
            {
                File.WriteAllText(BotPhrasesFilePath, "");
            }
            ReloadFile().Wait();
        }

        public string GetRandomPhrase()
        {
            var index = _rd.Next(_phrases.Count);
            return _phrases[index];
        }

        public async Task AddPhrase(string str)
        {
            var unescapedStr = str
                .Replace("\n", "\\n")
                .Replace("\r", "\\r");
            unescapedStr = "\n" + unescapedStr;
            await File.AppendAllTextAsync(BotPhrasesFilePath, unescapedStr);
            ReloadFile().Wait();
        }

        async Task ReloadFile()
        {
            _phrases = new List<string>();
            if (!File.Exists(BotPhrasesFilePath))
            {
                throw new Exception("Can't find bot phrases file.");
            }

            var text = await File.ReadAllTextAsync(BotPhrasesFilePath);
            var arr = text.Split('\n');
            foreach (var str in arr)
            {
                var escapedStr = str
                    .Replace("\\n", "\n")
                    .Replace("\\r", "\r")
                    .Replace("\\t", "    ");
                if (string.IsNullOrWhiteSpace(escapedStr))
                    continue;
                _phrases.Add(escapedStr);
            }
        }
    }
}
