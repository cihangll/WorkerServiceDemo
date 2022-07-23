namespace WorkerServiceNet6.Workers;

public class EverySecondWorker : CronJobService<EverySecondWorker>
{
	private readonly ILogger<EverySecondWorker> _logger;

	public EverySecondWorker(
		IScheduleConfig<EverySecondWorker> config,
		ILogger<EverySecondWorker> logger
	) : base(config, logger)
	{
		_logger = logger;
	}

	public override Task DoWorkAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine($"{nameof(EverySecondWorker)} running at: {DateTimeOffset.Now}");
		return Task.CompletedTask;
	}
}
