using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WorkerServiceDemo.Abstract;

namespace WorkerServiceDemo.Concrete
{
	public class HelloWorldWorker : CronJobService
	{
		private readonly ILogger<HelloWorldWorker> _logger;

		public HelloWorldWorker(
			IScheduleConfig<HelloWorldWorker> config,
			ILogger<HelloWorldWorker> logger
		) : base(config.CronExpression, config.TimeZoneInfo)
		{
			_logger = logger;
		}

		public override Task DoWorkAsync(CancellationToken cancellationToken)
		{
			Console.WriteLine("Hello World! {time}", DateTimeOffset.Now);
			return Task.CompletedTask;
		}

		public override Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(HelloWorldWorker)} start at: {DateTimeOffset.Now}");
			return base.StartAsync(cancellationToken);
		}
		public override Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"{nameof(HelloWorldWorker)} stop at: {DateTimeOffset.Now}");
			return base.StopAsync(cancellationToken);
		}

		public override void Dispose()
		{
			_logger.LogInformation($"{nameof(HelloWorldWorker)} dispose at: {DateTimeOffset.Now}");
			base.Dispose();
		}
	}
}
