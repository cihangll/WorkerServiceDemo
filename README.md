## WorkerServiceDemo

Belirlenen zamanlarda çalışan windows servis projesidir.

#### Kullanılanlar
- [Worker Services](https://docs.microsoft.com/en-us/dotnet/core/extensions/workers) - `for worker service`
- [CronJob](https://github.com/HangfireIO/Cronos) - `for Cron Expression`
- [Serilog](https://serilog.net/) - `for Logging`
- [Dapper](https://github.com/DapperLib/Dapper) - `for Execute Stored Procedures`

İçerisinde 3 adet örnek worker servis yer almaktadır.

- Console 'a "Hello World!" yazan servis.
- Her saniye çalışan servis.
- Örnek olması açısından stored procedure çalıştıran servis.

#### Projeyi Çalıştırmak

Projeyi bu haliyle çalıştırmak için aşağıda belirtilen adımları uygulayın.

- `appsettings.json` dosyası içerisindeki _connection string_ kısmını uygun hale getirin.
```json
"ConnectionStrings": {
  "Default": "Server=SERVER;Database=DATABASE;User Id=USERNAME;Password=PASSWORD;MultipleActiveResultSets=true"
},
```

- Çalıştıracağınız cronu ve sp name alanını düzenleyin. Cron ile ilgili detaylı bilgi için [tıklayınız.](https://crontab.guru)

```json
...
"JobSettings": {
    "StoredProcedureName": "uspExample",
    "Cron": "* * * * *"
},
```

#### Worker Servis Oluşturma ve Kullanma

Örneğin `DemoWorkerService` adında yeni bir worker servis oluşturmak istediğimizi varsayalım. 

Servisin `CronJobService<T>` sınıfından türetilmesi gerekiyor. Dependency injection ile `CronJobService` sınıfına `IScheduleConfig` ve `ILogger` nesnelerinin gönderilmesi gerekli.

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

`DoWorkAsync` metodunu ihtiyacınıza göre doldurun.

```csharp
public override async Task DoWorkAsync(CancellationToken cancellationToken)
{
  //API call or something
  await Task.Delay(3000, cancellationToken);

  // Do stuff...
}
```

Son hali şu şekilde gözükecektir. 

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

Worker servis olarak çalışabilmesi için `Program.cs` içerisinde kayıt işlemi yapmamız gerekli. Bunun için `AddCronJob<T>` adındaki extension'ı kullanabiliriz.

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

Bu kadar kolay. Artık belirtmiş olduğunuz cron expression'a göre belirlediğiniz zaman aralıklarında `DoWorkAsync` metodu içerisine yazdıklarınız sorunsuz çalışacaktır. 

#### Windows Servis Olarak Kurulum

Örnek olması adına windows servis kurulumunu `C:\\` dizini altında `WorkerServiceDemo` klasörüne yükleyelim.

- `appsettings.json` dosyasını açın. Windows servis olarak kurulum yapacağınız dizin neresiyse loglama için de o dizini belirtin.

```json
...
  "path": "C:\\WorkerServiceDemo\\Logs\\worker-service-logfile.txt",
...
```

- Publish için aşağıdaki gibi düzenleme yapabilirsiniz.

![image](https://user-images.githubusercontent.com/6229029/180613576-a285904b-1140-456e-8cce-d86a4627d532.png)

- Projeyi derledikten sonra `Publish` diyerek ilgili klasöre çıkartın. (Örn; `C:\Users\demo\Desktop\WorkerServiceDemo\WorkerServiceNet6\bin\Release\net6.0\win-x64\publish`)

![image](https://user-images.githubusercontent.com/6229029/180613602-99fe56a2-62e3-43cd-bf20-ffae01815305.png)

- Kurulumu yapacağınız klasöre `publish` klasöründeki dosyaları taşıyın. (Örn; C:\WorkerServiceDemo\)

![image](https://user-images.githubusercontent.com/6229029/180613661-cb6248b5-21c6-4410-b294-cdfbcfe6907c.png)

- Powershell'i yönetici olarak çalıştırıp aşağıdaki gibi kurulum yapın.

```pwsh
sc.exe create WorkerServiceDemo binpath=C:\WorkerServiceDemo\WorkerServiceDemo.exe start=auto
```

![image](https://user-images.githubusercontent.com/6229029/180613692-48aa3e1b-15ae-4e89-ba5d-828a8ce17de1.png)

- Servisi başlatmak için;

```pwsh
sc.exe start WorkerServiceDemo
```

![image](https://user-images.githubusercontent.com/6229029/180613718-6a1ba34a-d2ce-4f47-babc-6d87dad6d56d.png)

![image](https://user-images.githubusercontent.com/6229029/180613786-b461050c-b17c-43d3-9d50-5150981553ec.png)

Servisi durdurmak için;

```pwsh
sc.exe stop WorkerServiceDemo
```

Servisi kaldırmak için;

```pwsh
sc.exe stop WorkerServiceDemo
sc.exe delete WorkerServiceDemo
```

![image](https://user-images.githubusercontent.com/6229029/180613809-b3229471-9ae5-4a7f-9ccf-c80398a1f7f4.png)

Daha fazla bilgi için [adresini](https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service) ziyaret edebilirsiniz.
