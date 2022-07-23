[![en](https://img.shields.io/badge/lang-en-green.svg)](https://github.com/cihangll/WorkerServiceDemo/blob/master/README.en.md)
[![tr](https://img.shields.io/badge/lang-tr-red.svg)](https://github.com/cihangll/WorkerServiceDemo/blob/master/README.md)

## WorkerServiceDemo

It is a windows service project that runs at specified times.

#### Used
- [Worker Services](https://docs.microsoft.com/en-us/dotnet/core/extensions/workers) - `for worker service`
- [CronJob](https://github.com/HangfireIO/Cronos) - `for Cron Expression`
- [Serilog](https://serilog.net/) - `for Logging`
- [Dapper](https://github.com/DapperLib/Dapper) - `for Execute Stored Procedures`

It includes 3 sample worker services.

- "Hello World!" service.
- Service running every second.
- Service running the stored procedure as an example.

#### Running the Project

Follow the steps given below to run the project.

- `appsettings.json` change the _ConnectionStrings_ part in the file.
```json
"ConnectionStrings": {
  "Default": "Server=SERVER;Database=DATABASE;User Id=USERNAME;Password=PASSWORD;MultipleActiveResultSets=true"
},
```

- Edit the cron and the sp name field. For detailed information about cron [click here.](https://crontab.guru)

```json
...
"JobSettings": {
    "StoredProcedureName": "uspExample",
    "Cron": "* * * * *"
},
```

#### Creating and Using Worker Service

For example, let's assume we want to create a new worker service named `DemoWorkerService`.

The service needs to be extend from the `CronJobService<T>` class. We need to send `IScheduleConfig` and `ILogger` objects to `CronJobService` class with dependency injection.

```csharp
public class DemoWorkerService : CronJobService<DemoWorkerService>
{
	private readonly ILogger<DemoWorkerService> _logger;

	public DemoWorkerService(
		IScheduleConfig<DemoWorkerService> config,
		ILogger<DemoWorkerService> logger
	) : base(config, logger)
	{
		_logger = logger;
	}

	public override Task DoWorkAsync(CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}
```

Fill the `DoWorkAsync` method according to your need.

```csharp
public override async Task DoWorkAsync(CancellationToken cancellationToken)
{
  //API call or something
  await Task.Delay(3000, cancellationToken);

  // Do stuff...
}
```

The final version will look like this.

```csharp
namespace WorkerServiceDemo.Workers;

public class DemoWorkerService : CronJobService<DemoWorkerService>
{
	private readonly ILogger<DemoWorkerService> _logger;

	public DemoWorkerService(
		IScheduleConfig<DemoWorkerService> config,
		ILogger<DemoWorkerService> logger
	) : base(config, logger)
	{
		_logger = logger;
	}

	public override async Task DoWorkAsync(CancellationToken cancellationToken)
	{
		//API call or something
		await Task.Delay(5000, cancellationToken);

		// Do stuff...
	}
}
```

In order for the `DemoWorkerService` to work as a service, we need to register in `Program.cs`. We can use the extension named `AddCronJob<T>` for this.

```csharp
var host = Host.CreateDefaultBuilder(args)
.ConfigureServices((hostContext, services) =>
{
//...

  services.AddCronJob<DemoWorkerService>(options =>
  {
    options.CronExpression = "0 * * * *"; //run every hour
    options.TimeZoneInfo = TimeZoneInfo.Local;
  });

})
...
```

It's that easy. Now, what you write in the `DoWorkAsync` method will run smoothly at the time intervals you have determined according to the cron expression you have specified.

#### Installing as a Windows Service

As an example, let's install the windows service installation in the `WorkerServiceDemo` folder under the `C:\\` directory.

- Open the `appsettings.json` file. Specify `C:\\WorkerServiceDemo` directory for logging.

```json
...
  "path": "C:\\WorkerServiceDemo\\Logs\\worker-service-logfile.txt",
...
```

- You can set Publish config as follows.

![image](https://user-images.githubusercontent.com/6229029/180613576-a285904b-1140-456e-8cce-d86a4627d532.png)

- After compiling the project, click `Publish` button and extract it to the relevant folder. (Ex: `C:\Users\demo\Desktop\WorkerServiceDemo\WorkerServiceNet6\bin\Release\net6.0\win-x64\publish`)

![image](https://user-images.githubusercontent.com/6229029/180613602-99fe56a2-62e3-43cd-bf20-ffae01815305.png)

- Move the files in the `publish` folder to the folder where you will install.(Ex; C:\WorkerServiceDemo\)

![image](https://user-images.githubusercontent.com/6229029/180613661-cb6248b5-21c6-4410-b294-cdfbcfe6907c.png)

- Run Powershell as administrator and execute command below for install a windows service.

```pwsh
sc.exe create WorkerServiceDemo binpath=C:\WorkerServiceDemo\WorkerServiceDemo.exe start=auto
```

![image](https://user-images.githubusercontent.com/6229029/180613692-48aa3e1b-15ae-4e89-ba5d-828a8ce17de1.png)

- To start the service;

```pwsh
sc.exe start WorkerServiceDemo
```

![image](https://user-images.githubusercontent.com/6229029/180613718-6a1ba34a-d2ce-4f47-babc-6d87dad6d56d.png)

![image](https://user-images.githubusercontent.com/6229029/180613786-b461050c-b17c-43d3-9d50-5150981553ec.png)

- To stop the service;

```pwsh
sc.exe stop WorkerServiceDemo
```

- To delete the service;

```pwsh
sc.exe stop WorkerServiceDemo
sc.exe delete WorkerServiceDemo
```

![image](https://user-images.githubusercontent.com/6229029/180613809-b3229471-9ae5-4a7f-9ccf-c80398a1f7f4.png)

For more information, you can visit [windows-service page](https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service).
