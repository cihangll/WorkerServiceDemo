using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerServiceDemo
{
	public class StoredProcedureWorker : BackgroundService
	{
		private readonly ILogger<StoredProcedureWorker> _logger;
		private readonly IConfiguration _configuration;

		public StoredProcedureWorker(ILogger<StoredProcedureWorker> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
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

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				_logger.LogInformation("Worker checking at: {time}", DateTimeOffset.Now);
				DateTime runTime = DateTime.ParseExact(_configuration.GetSection("JobSettings:RunTime")?.Value ?? "12:00:00", "HH:mm:ss", CultureInfo.InvariantCulture);
				var now = DateTime.Now;

				if (now.Hour == runTime.Hour && now.Minute == runTime.Minute)
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

				await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
			}
		}
	}
}
