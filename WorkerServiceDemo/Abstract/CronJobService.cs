using Cronos;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerServiceDemo.Abstract
{
	public abstract class CronJobService : IHostedService, IDisposable
	{
		private System.Timers.Timer _timer;
		private readonly CronExpression _expression;
		private readonly TimeZoneInfo _timeZoneInfo;

		protected CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo)
		{
			_expression = CronExpression.Parse(cronExpression);
			_timeZoneInfo = timeZoneInfo;
		}

		public virtual async Task StartAsync(CancellationToken cancellationToken)
		{
			await ScheduleJobAsync(cancellationToken);
		}

		public virtual async Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Stop();
			await Task.CompletedTask;
		}

		public virtual void Dispose()
		{
			_timer?.Dispose();
		}

		protected async Task ScheduleJobAsync(CancellationToken cancellationToken)
		{
			var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
			if (next.HasValue)
			{
				var delay = next.Value - DateTimeOffset.Now;
				_timer = new System.Timers.Timer(delay.TotalMilliseconds);
				_timer.Elapsed += async (sender, args) =>
				{
					_timer.Dispose();  // reset and dispose timer
					_timer = null;

					if (!cancellationToken.IsCancellationRequested)
					{
						await DoWorkAsync(cancellationToken);
					}

					if (!cancellationToken.IsCancellationRequested)
					{
						await ScheduleJobAsync(cancellationToken);    // reschedule next
					}
				};
				_timer.Start();
			}
			await Task.CompletedTask;
		}

		public abstract Task DoWorkAsync(CancellationToken cancellationToken);
	}
}
