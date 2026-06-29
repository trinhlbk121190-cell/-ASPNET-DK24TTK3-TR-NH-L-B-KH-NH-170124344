# MedForum – Hu?ng d?n cài d?t

## Yêu c?u h? th?ng
- .NET 10.0 SDK tr? lên
- SQL Server 2019+ (ho?c SQL Server Express)
- Visual Studio 2022 Community Edition

## Các bu?c cài d?t

### Bu?c 1: Clone Repository
```bash
git clone https://github.com/antonio86doan/ASPNET-D24TTK3-trinhlekhanh-MedForum
cd ASPNET-D24TTK3-trinhlekhanh-MedForum
```

### Bu?c 2: C?u hình Connection String
Ch?nh s?a file src/MedicalForum.Mvc/appsettings.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MedForum;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

### Bu?c 3: T?o Database
```bash
cd src/MedicalForum.Mvc
dotnet ef database update
```

### Bu?c 4: Ch?y ?ng d?ng
```bash
dotnet run --urls "http://localhost:5050"
```

### Bu?c 5: Truy c?p h? th?ng
M? trình duy?t: http://localhost:5050

D? li?u m?u du?c t?o t? d?ng (xem test-accounts.md).
