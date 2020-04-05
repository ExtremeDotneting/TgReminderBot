using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TgReminderBot.Core.Services
{
    public static class ConversationScripts
    {
        public static IList<Func<ISimpleUserInteractionWrapper, Task>> Scripts { get; } = new List<Func<ISimpleUserInteractionWrapper, Task>>
        {
            async (userInteraction) =>
            {
                await userInteraction.SendMessage("Напиши фразу 'мат еч а' наоборот.");
                var msg = await userInteraction.ReadMessage(); 
                if(msg.Trim() == "а че там")
                {
                    await userInteraction.SendMessage("Так а че там? Расскажи мне, я твой друг. Честно...");
                }
                else
                {
                    await userInteraction.SendMessage("Неправильно! Я самоуничтошаюсь.");
                }
            }

        };
    }
}
