using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TgReminderBot.Services
{
    public static class ConversationScripts
    {
        public static IList<Func<ISimpleUserInteractionWrapper, Task>> Scripts { get; } = new List<Func<ISimpleUserInteractionWrapper, Task>>
        {
            async (userInteraction) =>
            {
                await userInteraction.SendMessage("Напиши фразу 'мат еч а' наоборот.");
                var msg = await userInteraction.ReadMessage();
                while (msg.Trim() != "а че там")
                {
                    await userInteraction.SendMessage("Неправильно! Еще раз.");
                    msg = await userInteraction.ReadMessage();
                }
                await userInteraction.SendMessage("Так а че там?))");
            }

        };
    }
}
