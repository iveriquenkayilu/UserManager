using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RainyCorp.UserManagerService.Interfaces.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RainyCorp.UserManagerService.Services
{
    /// <summary>
    /// Implements the background hosted service
    /// </summary>
    public class BackgroundHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackgroundHostedService> _logger;
        private Timer _timer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="logger"></param>
        public BackgroundHostedService(IServiceProvider serviceProvider, ILogger<BackgroundHostedService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Starts the service asynchromously.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            TimeSpan interval = TimeSpan.FromDays(1); // 30 seconds for testing
            var nextRunTime = DateTime.Now.AddDays(1);
            var firstInterval = nextRunTime.Subtract(DateTime.Now);

            Action action = async () =>
            {
                await DoWork();
                var t1 = Task.Delay(firstInterval);
                t1.Wait();

                //now schedule it to be called every 24 hours for future
                // timer repeates call to RemoveScheduledAccounts every 24 hours.

                _timer = new Timer(async state =>
                   await DoWork(),
                   null,
                   TimeSpan.Zero,
                   interval
               );
            };

            // no need to await this call here because this task is scheduled to run much much later.
            Task.Run(action);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stops the service asynchronously.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async Task DoWork()
        {
            _logger.LogInformation($"Background Service is running  {DateTime.Now.ToString("hh:mm:ss")}");
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}