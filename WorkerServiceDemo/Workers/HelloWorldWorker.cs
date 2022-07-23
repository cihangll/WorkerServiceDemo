namespace WorkerServiceNet6.Workers;

public class HelloWorldWorker : CronJobService<HelloWorldWorker>
{
	private readonly ILogger<HelloWorldWorker> _logger;

	public HelloWorldWorker(
		IScheduleConfig<HelloWorldWorker> config,
		ILogger<HelloWorldWorker> logger
	) : base(config, logger)
	{
		_logger = logger;
	}

	public override Task DoWorkAsync(CancellationToken cancellationToken)
	{
		Console.WriteLine($"{nameof(HelloWorldWorker)} running at: {DateTimeOffset.Now}");
		return Task.CompletedTask;
	}
}
