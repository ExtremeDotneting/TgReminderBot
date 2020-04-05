using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace TgReminderBot.Core.HostedServices
{
    public abstract class ScopedWorker
    {
        bool _isRunning;
        Task _runningTask;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _isRunning = true;
            var t = Execute(cancellationToken);
            _runningTask = t;
            await t;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _isRunning = false;
            var t = _runningTask;
            _runningTask = null;
            if (t != null)
            {
                //Wait while finishing.
                await t;
            }
        }

        /// <summary>
        /// Override this to write only loop code.
        /// Set delays in your code if needed.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task ExecuteLoop(CancellationToken cancellationToken)
        {

        }

        protected virtual async Task Execute(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _isRunning)
            {
                await ExecuteLoop(cancellationToken);
            }
        }

        protected void Try(Action act)
        {
            try
            {
                act();
            }
            catch (Exception ex)
            {
                Log.Error (ex.ToString());
            }
        }

        protected async Task Try(Func<Task> func)
        {
            try
            {
                await func();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}