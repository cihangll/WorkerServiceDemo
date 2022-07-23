var host = Host.CreateDefaultBuilder(args)
	.ConfigureServices((hostContext, services) =>
	{
		/* https://github.com/HangfireIO/Cronos#cron-format
												Allowed values    Allowed special characters   Comment
		┌───────────── second (optional)       0-59              * , - /                      
		│ ┌───────────── minute                0-59              * , - /                      
		│ │ ┌───────────── hour                0-23              * , - /                      
		│ │ │ ┌───────────── day of month      1-31              * , - / L W ?                
		│ │ │ │ ┌───────────── month           1-12 or JAN-DEC   * , - /                      
		│ │ │ │ │ ┌───────────── day of week   0-6  or SUN-SAT   * , - / # L ?                Both 0 and 7 means SUN
		│ │ │ │ │ │
		* * * * * *
		
		*/

		// Every hour, for more information: https://crontab.guru/
		const string defaultCron = "0 * * * *";

		var jobSettings = hostContext.Configuration.GetSection("JobSettings");
		var cronExpression = jobSettings?.GetValue<string>("Cron") ?? defaultCron;
		var includeSeconds = jobSettings?.GetValue<bool>("IncludeSeconds") ?? false;

		services.AddCronJob<HelloWorldWorker>(options =>
		{
			options.IncludeSeconds = includeSeconds;
			options.CronExpression = cronExpression;
			options.TimeZoneInfo = TimeZoneInfo.Local;
		});

		//Every second worker
		services.AddCronJob<EverySecondWorker>(options =>
		{
			options.IncludeSeconds = true;
			options.CronExpression = "* * * * * *";
			options.TimeZoneInfo = TimeZoneInfo.Local;
		});

		services.AddCronJob<StoredProcedureWorker>(options =>
		{
			options.CronExpression = "* * * * *";
			options.TimeZoneInfo = TimeZoneInfo.Local;
		});
	})
	.UseSerilog()
	.UseWindowsService()
	.Build();

var configuration = new ConfigurationBuilder()
		.AddJsonFile("appsettings.json")
		.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
		.Build();

Log.Logger = new LoggerConfiguration()
	.ReadFrom.Configuration(configuration)
	.CreateLogger();

try
{
	Log.Information("Application Starting Up...");
	await host.RunAsync();
}
catch (Exception e)
{
	Log.Fatal(e, "The application failed to start correctly.");
}
finally
{
	Log.CloseAndFlush();
}