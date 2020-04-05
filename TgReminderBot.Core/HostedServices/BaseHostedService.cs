using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TgReminderBot.Core.HostedServices
{
    public class BaseHostedService<TScopedWorker> 
        where TScopedWorker : ScopedWorker
    {
        readonly IServiceProvider _serviceProvider;

        TScopedWorker _worker;

        public BaseHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            try
            {
                _worker = scope.ServiceProvider
                    .GetRequiredService<TScopedWorker>();
                await _worker.StartAsync(cancellationToken);
            }
            finally
            {
                scope.Dispose();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_worker == null)
                return;
            await _worker.StopAsync(cancellationToken);
        }
    }
}