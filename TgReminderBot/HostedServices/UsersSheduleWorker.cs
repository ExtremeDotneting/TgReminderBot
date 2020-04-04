using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TgReminderBot.HostedServices
{
    public class UsersSheduleWorker : ScopedWorker
    {
        protected override async Task Execute(CancellationToken cancellationToken)
        {
            while (true)
            {
                await Task.Delay(30000, cancellationToken);
            }
        }
    }
}
