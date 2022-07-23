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


#### Windows Servis Olarak Kurulum

Örnek olması adına windows servis kurulumunu `C:\\` dizini altında `WorkerServiceDemo` klasörüne yükleyeceğiz.

- `appsettings.json` dosyasını açın. Windows servis olarak kurulum yapacağınız dizin neresiyse loglama için de o dizini belirtin.

```json
...
  "path": "C:\\WorkerServiceDemo\\Logs\\worker-service-logfile.txt",
...
```

- Projeyi derledikten sonra `Publish` diyerek ilgili klasöre çıkartın. (Örn; `C:\Users\demo\Desktop\WorkerServiceDemo\WorkerServiceNet6\bin\Release\net6.0\win-x64\publish`)

- Kurulumu yapacağınız klasöre `publish` klasöründeki dosyaları taşıyın. (Örn; C:\WorkerServiceDemo\)

![image](https://user-images.githubusercontent.com/6229029/180612076-d72b812d-7890-4d57-bd3f-abd71b1e3d9c.png)

- Komut satırını yönetici olarak çalıştırıp aşağıdaki gibi kurulum yapın.

```pwsh
sc.exe create WorkerServiceDemo binpath=C:\WorkerServiceDemo\WorkerServiceNet6.exe start=auto
```
![image](https://user-images.githubusercontent.com/6229029/180612106-02000dac-0216-40ce-baa8-71d9cba305a9.png)

- Servisi başlatmak için;

```pwsh
sc.exe start WorkerServiceDemo
```

Servisi kaldırmak için;

```pwsh
sc.exe stop WorkerServiceDemo
sc.exe delete WorkerServiceDemo
```

