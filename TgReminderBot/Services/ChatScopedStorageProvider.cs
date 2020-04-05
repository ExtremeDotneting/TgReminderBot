using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IRO.Storage;
using IRO.Storage.DefaultStorages;

namespace TgReminderBot.Services
{
    public class ChatScopedStorageProvider
    {
        public ChatScopedStorageProvider()
        {
            var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"user_storages");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        public IKeyValueStorage GetChatStorage(long chatId)
        {
            var dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"user_storages");
            //var path = Path.Combine(dirPath, $"storageForUser_{username}.json");
            return new FileStorage($"storageForUser_{chatId}.json", dirPath);
        }
    }
}
