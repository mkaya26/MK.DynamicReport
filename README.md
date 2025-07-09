
# MK.DynamicReport

Dinamik raporlama sistemlerine açık kaynak katkısı sağlamak amacıyla geliştirilmiş bir backend projesidir.

- 🛠  Katmanlı mimari (Onion Architecture)
- 🔢 Dinamik SQL sorguları
- 🗄 PDF / Excel export
- 🌆 Hangfire ile zamanlanmış raporlar
- 📧 E-posta ile otomatik gönderim
- 🌟 MIT Lisansı ile açık kaynak

---

## 🚀 Kurulum

1. Bu repoyu klonlayın:

```bash
git clone https://github.com/mkaya26/MK.DynamicReport.git
```

2. Projeyi Visual Studio veya VS Code ile açın.

3. `appsettings.json` dosyasındaki bağlantı ayarlarını yapın:

- Veritabanı bağlantısı (`DefaultConnection`)
- SMTP sunucu ayarları (`SmtpSettings`)
- JWT anahtarı (`Jwt`)

Örnek:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=...;Database=...;User Id=...;Password=...;"
},
"SmtpSettings": {
  "Host": "mail.ornek.com",
  "Port": 587,
  "User": "kullanici@ornek.com",
  "Password": "sifre",
  "EnableSsl": true,
  "From": "kullanici@ornek.com"
},
"Jwt": {
  "Key": "Your_Very_Secure_And_Long_Secret_Key"
}
```

4. SQL Server üzerinde aşağıdaki tabloları oluşturun:

```sql
CREATE TABLE ReportDefinitions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ReportName NVARCHAR(250),
    Description NVARCHAR(1000),
    ReportJson NVARCHAR(MAX),
    CreatedDate DATETIME
);

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    Role NVARCHAR(50) NOT NULL
);

CREATE TABLE ReportHistories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ReportDefinitionId INT NOT NULL,
    ReportName NVARCHAR(250) NOT NULL,
    ExportFormat NVARCHAR(50) NOT NULL,
    FilePath NVARCHAR(500) NOT NULL,
    CreatedDate DATETIME NOT NULL,
    CreatedBy NVARCHAR(100) NULL
);
```

> 🔗 Not: Hangfire tabloları otomatik olarak ilk çalıştırmada oluşur.

5. Projeyi çalıştırın ve Swagger UI üzerinden API uç noktalarını test edin.

---

## ⚙ Nasıl Kullanılır

- `/api/Report/executeReport` ➔ Dinamik SQL raporu çalıştırır.
- `/api/Report/export/pdf` ➔ PDF export alır.
- `/api/Report/export/excel` ➔ Excel export alır.
- `/api/ReportHistory/getall` ➔ Rapor geçmişlerini listeler.
- `/hangfire` ➔ Zamanlanmış raporları yönetmek için Hangfire Dashboard (güvenlik eklenmeli).

---

## 📅 Tamamlanan Özellikler

- ✅ Dinamik rapor çalıştırma
- ✅ PDF ve Excel export
- ✅ JWT kimlik doğrulama & rol bazlı yetkilendirme
- ✅ Hangfire ile zamanlanmış iş altyapısı
- ✅ Export edilen raporların e-posta ile otomatik gönderimi
- ✅ Rapor geçmişlerinin veritabanında saklanması

---

## 🔗 Yol Haritası (Bekleyen Tasklar)

- [ ] **Task 9.3:** Grafik ve görsellik desteği (raporları görsel olarak zenginleştirmek için grafik desteği)
- [ ] **Task 9.4:** Rapor bazlı yetkilendirme (her kullanıcının hangi raporları görebileceğini yönetme)
- [ ] **Task 9.5:** Çoklu dil desteği (uygulamanın birden fazla dili desteklemesi)
- [ ] **Task 10:** Rapor zamanlama yönetimi (kullanıcıların kendi zamanlanmış raporlarını oluşturabilmesi ve yönetebilmesi)
- [ ] **Task 11:** Hangfire dashboard güvenlik iyileştirmesi (dashboard erişimini sınırlamak ve güvenliğini artırmak)

---

## 📚 Lisans

Bu proje MIT lisansı ile sunulmaktadır.
