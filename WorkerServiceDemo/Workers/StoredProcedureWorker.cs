namespace WorkerServiceNet6.Workers;

public class StoredProcedureWorker : CronJobService<StoredProcedureWorker>
{
	private readonly ILogger<StoredProcedureWorker> _logger;
	private readonly IConfiguration _configuration;

	public StoredProcedureWorker(
		IScheduleConfig<StoredProcedureWorker> config,
		ILogger<StoredProcedureWorker> logger,
		IConfiguration configuration
	) : base(config, logger)
	{
		_logger = logger;
		_configuration = configuration;
	}

	public override async Task DoWorkAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Worker started for SP: {time}", DateTimeOffset.Now);

		#region Do Stuff

		try
		{
			using var connection = new SqlConnection(_configuration.GetConnectionString("Default"));
			connection.Open();

			var spName = _configuration.GetSection("JobSettings:StoredProcedureName")?.Value;
			if (string.IsNullOrEmpty(spName))
			{
				throw new ArgumentNullException("Stored procedure cannot found! Please check configuration file.");
			}

			//Just run
			//var executeResult = await connection.ExecuteAsync(spName, commandType: CommandType.StoredProcedure);

			//If you want to result from sp
			var result = await connection.QueryAsync(spName, commandType: CommandType.StoredProcedure);

			//work here...

			_logger.LogInformation("{spName} successfully run: {time}", spName, DateTimeOffset.Now);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occured while execute sp!");
		}

		#endregion

		_logger.LogInformation("Worker ended for SP: {time}", DateTimeOffset.Now);
	}
}
