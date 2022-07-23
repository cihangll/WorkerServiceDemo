namespace WorkerServiceNet6.Workers.Abstract;

public abstract class CronJobService<T> : BackgroundService where T : BackgroundService
{
	private System.Timers.Timer _timer;
	private readonly CronExpression _expression;
	private readonly TimeZoneInfo _timeZoneInfo;

	private readonly ILogger<T> _logger;

	protected CronJobService(IScheduleConfig<T> config, ILogger<T> logger)
	{
		_expression = CronExpression.Parse(
			config.CronExpression,
			config.IncludeSeconds ? CronFormat.IncludeSeconds : CronFormat.Standard
		);
		_timeZoneInfo = config.TimeZoneInfo;

		_logger = logger;
	}

	public override Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("{worker} start at: {time}", typeof(T).Name, DateTimeOffset.Now);
		return base.StartAsync(cancellationToken);
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		await ScheduleJobAsync(stoppingToken);
	}

	public override Task StopAsync(CancellationToken cancellationToken)
	{
		_timer?.Stop();
		_logger.LogWarning("{worker} stop at: {time}", typeof(T).Name, DateTimeOffset.Now);
		return base.StopAsync(cancellationToken);
	}

	public override void Dispose()
	{
		_timer?.Dispose();
		_logger.LogWarning("{worker} dispose at: {time}", typeof(T).Name, DateTimeOffset.Now);
		base.Dispose();
	}

	private async Task ScheduleJobAsync(CancellationToken cancellationToken)
	{
		var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
		if (next.HasValue)
		{
			var delay = next.Value - DateTimeOffset.Now;
			_timer = new System.Timers.Timer(delay.TotalMilliseconds);
			_timer.Elapsed += async (sender, args) =>
			{
				// reset and dispose timer
				_timer.Dispose();
				_timer = null;

				if (!cancellationToken.IsCancellationRequested)
				{
					try
					{
						await DoWorkAsync(cancellationToken);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "An error occured while executing the {method} method", nameof(DoWorkAsync));
					}
				}

				if (!cancellationToken.IsCancellationRequested)
				{
					// reschedule next
					await ScheduleJobAsync(cancellationToken);
				}
			};
			_timer.Start();
		}
		await Task.CompletedTask;
	}

	public abstract Task DoWorkAsync(CancellationToken cancellationToken);
}
