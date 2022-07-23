## WorkerServiceDemo

Belirlenen zamanlarda çalışan windows servis projesi. İçerisinde 3 adet örnek worker servis yer almaktadır.

- Console 'a "Hello World!" yazan servis.
- Her saniye çalışan servis.
- Örnek olması açısından stored procedure çalıştıran servis.

#### Kullanılanlar
- [Worker Services](https://docs.microsoft.com/en-us/dotnet/core/extensions/workers) - `for worker service`
- [CronJob](https://github.com/HangfireIO/Cronos) - `for Cron Expression`
- [Serilog](https://serilog.net/) - `for Logging`
- [Dapper](https://github.com/DapperLib/Dapper) - `for Execute Stored Procedures`

#### Kurulum

- `appsettings.json` dosyası içerisindeki _connection string_ kısmını uygun hale getirin.
```json
"ConnectionStrings": {
  "Default": "Server=SERVER;Database=DATABASE;User Id=USERNAME;Password=PASSWORD;MultipleActiveResultSets=true"
},
```

- Çalıştıracağınız cronu ve sp name alanını düzenleyin. Cron ile ilgili detaylı bilgi için [https://crontab.guru](tıklayınız.)

```json
...
"JobSettings": {
    "StoredProcedureName": "uspExample",
    "Cron": "* * * * *"
},
```

- Windows servis olarak kurulum yapacağınız dizin neresiyse loglama için de o dizini belirtin.

```json
...
  "path": "C:\\WorkerServiceDemo\\Logs\\worker-service-logfile.txt",
...
```

- Projeyi derledikten sonra `Publish` diyerek ilgili klasöre çıkartın. (Örn; `C:\Users\cihangll\source\repos\WorkerServiceDemo\WorkerServiceDemo\bin\Release\netcoreapp3.1\publish\`)
- Windows servis dosyalarının bulunacağı dizini belirleyin ve `win-x64` klasöründeki dosyaları bu dizine kopyalayın. (Örn; `C:\Users\cihangll\source\repos\WorkerServiceDemo\WorkerServiceDemo\bin\Release\netcoreapp3.1\win-x64` => `C:\WorkerServiceDemo\`)
- Komut satırını yönetici olarak çalıştırıp aşağıdaki gibi kurulum yapın.

```pwsh
sc.exe create WorkerServiceDemo binpath=C:\WorkerServiceDemo\WorkerServiceDemo.exe start=auto
```

- Servisi başlatın.

```pwsh
sc.exe start WorkerServiceDemo
```

Servisi kaldırmak için;

```pwsh
sc.exe stop WorkerServiceDemo
sc.exe delete WorkerServiceDemo
```

