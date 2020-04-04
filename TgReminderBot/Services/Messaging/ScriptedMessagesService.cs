using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TgReminderBot.Services.Messaging
{
    public class ScriptedMessagesService
    {
        readonly Random _rd = new Random();
        readonly IList<Func<ISimpleUserInteractionWrapper, Task>> _conversationScripts;

        public int Count => _conversationScripts.Count;

        public ScriptedMessagesService(IList<Func<ISimpleUserInteractionWrapper, Task>> conversationScripts)
        {
            _conversationScripts = conversationScripts;
        }

        public async Task MakeRandomСonversation(ISimpleUserInteractionWrapper userInteractionWrapper)
        {
            var index = _rd.Next(_conversationScripts.Count);
            var script = _conversationScripts[index];
            await script.Invoke(userInteractionWrapper);

        }
    }
}