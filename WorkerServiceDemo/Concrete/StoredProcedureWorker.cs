using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using WorkerServiceDemo.Abstract;

namespace WorkerServiceDemo
{
	public class StoredProcedureWorker : CronJobService
	{
		private readonly ILogger<StoredProcedureWorker> _logger;
		private readonly IConfiguration _configuration;

		public StoredProcedureWorker(
			IScheduleConfig<StoredProcedureWorker> config,
			ILogger<StoredProcedureWorker> logger,
			IConfiguration configuration
		) : base(config.CronExpression, config.TimeZoneInfo)
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

				var dynamicParameters = new DynamicParameters();
				dynamicParameters.Add("@LogLevel", "Error", DbType.String);

				var spName = _configuration.GetSection("JobSettings:StoredProcedureName")?.Value;
				if (string.IsNullOrEmpty(spName))
				{
					throw new ArgumentNullException("Stored procedure cannot found! Please check configuration file.");
				}

				//Just run
				var executeResult = await connection.ExecuteAsync(spName, dynamicParameters, commandType: CommandType.StoredProcedure);
				_logger.LogInformation("{spName} successfully run: {time}", spName, DateTimeOffset.Now);

				//If you want to result from sp
				//var result = await connection.QueryAsync(spName, dynamicParameters, commandType: CommandType.StoredProcedure);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occured while execute sp!");
			}

			#endregion

			_logger.LogInformation("Worker ended for SP: {time}", DateTimeOffset.Now);
		}

		public override Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Worker start at: {time}", DateTimeOffset.Now);
			return base.StartAsync(cancellationToken);
		}
		public override Task StopAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Worker stop at: {time}", DateTimeOffset.Now);
			return base.StopAsync(cancellationToken);
		}

		public override void Dispose()
		{
			_logger.LogInformation("Worker dispose at: {time}", DateTimeOffset.Now);
			base.Dispose();
		}
	}
}
