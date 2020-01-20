## WorkerServiceDemo

Her gün belirlenen saatte `stored procedure` çalıştıran windows servis

#### Kullanılanlar
- Serilog
- Dapper
- Worker Service

#### Kurulum

- `appsettings.json` dosyası içerisindeki _connection string_ kısmını uygun hale getirin.
```json
"ConnectionStrings": {
  "Default": "Server=SERVER;Database=DATABASE;User Id=USERNAME;Password=PASSWORD;MultipleActiveResultSets=true"
},
```

- Çalıştıracağınız saati ve sp name alanını düzenleyin.

```json
...
"JobSettings": {
    "StoredProcedureName": "uspExample",
    "RunTime": "12:30:00"
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

