using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace WorkerServiceDemo
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var configurationBuilder = new ConfigurationBuilder();
			configurationBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			var aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
			if (!string.IsNullOrEmpty(aspNetCoreEnvironment))
			{
				configurationBuilder.AddJsonFile($"appsettings.{aspNetCoreEnvironment}.json", optional: true, reloadOnChange: true);
			}

			var configuration = configurationBuilder.Build();
			var loggerConfiguration = new LoggerConfiguration()
			  .ReadFrom.Configuration(configuration);

			Log.Logger = loggerConfiguration.CreateLogger();

			try
			{
				Log.Information("Application Starting Up...");
				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception e)
			{
				Log.Fatal(e, "The application failed to start correctly.");
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{
			return Host.CreateDefaultBuilder(args)
				.UseWindowsService()
				.ConfigureServices((hostContext, services) =>
				{
					services.AddHostedService<StoredProcedureWorker>();
				})
				.UseSerilog();
		}
	}
}
