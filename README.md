# Trafik Cezası Yönetim ve Onay Modülü

Araçlara ait trafik cezalarının kaydedildiği, çok aşamalı bir onay sürecinden geçirildiği ve tüm işlemlerin geçmişinin tutulduğu ASP.NET Core MVC uygulaması.

## Kullanılan Teknolojiler

- .NET 9, ASP.NET Core MVC
- Entity Framework Core (Code First, Fluent API)
- SQL Server (LocalDB)
- ASP.NET Core Identity (kimlik doğrulama ve rol yönetimi)
- Bootstrap 5

## Özellikler

- **Araç yönetimi:** Araç ekleme ve listeleme. Araç tipleri: Binek, Çekici, Dorse, Kiralık Araç.
- **Ceza yönetimi:** Ceza ekleme, listeleme, detay görüntüleme ve düzenleme.
- **Onay süreci:** Yeni → Yönetici Onayı → Finans Onayı → Tamamlandı. Her aşamada yalnızca yetkili rol işlem yapabilir.
- **Ret:** Ceza her onay aşamasında reddedilebilir; ret nedeni zorunludur.
- **Onay geçmişi:** Her işlem için işlemi yapan kullanıcı, tarih, işlem tipi, açıklama/ret nedeni, önceki durum ve yeni durum kaydedilir.
- **İşlem bekleyenler:** Her kullanıcı kendisinden işlem bekleyen cezaları ayrı bir sekmede görür.
- **Ana sayfa özeti:** Duruma göre ceza sayıları ve kullanıcıyı bekleyen işlem sayısı.

## Kurulum ve Çalıştırma

### Gereksinimler

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (Visual Studio ile birlikte gelir; ayrıca SQL Server Express kurulumundan da yüklenebilir)

### Adımlar

```bash
git clone <repo-adresi>
cd TrafikCezaYonetimi/TrafikCezaYonetimi.Web
dotnet run
```

Uygulama ilk açılışta:

- Veritabanını oluşturur ve migration'ları otomatik uygular,
- Rolleri ve test kullanıcılarını oluşturur.

Ek bir veritabanı kurulumu gerekmez. Bağlantı ayarı `appsettings.json` içindeki `DefaultConnection` alanındadır.

## Test Kullanıcıları

Tüm kullanıcıların şifresi: `Test123!`

| Rol | E-posta | Yetkileri |
|---|---|---|
| Personel | `personel@test.com` | Ceza oluşturur, düzenler ve onaya gönderir |
| Yönetici | `yonetici@test.com` | Yönetici onayı aşamasındaki cezaları onaylar veya reddeder |
| Finans | `finans@test.com` | Finans onayı aşamasındaki cezaları onaylar veya reddeder |

Araç ekleme tüm giriş yapmış kullanıcılara açıktır.

## Onay Süreci

```
                 Onaya Gönder            Onayla               Onayla
  [ Yeni ] ──────────────────► [ Yönetici ] ────────► [ Finans ] ────────► [ Tamamlandı ]
      ▲         (Personel)      [ Onayında ] (Yönetici)[Onayında] (Finans)
      │                              │                      │
      │                           Reddet                 Reddet
      │                              ▼                      ▼
      │                         ┌──────────────────────────────┐
      └── Düzelt ve yeniden ─── │         [ Reddedildi ]        │
          onaya gönder          └──────────────────────────────┘
```

## Tasarım Kararları

### Onay mekanizması

Onay kuralları `Services/OnayAkisi.cs` içinde iki tablo olarak tanımlanmıştır:

- Hangi durumda hangi rolün karar verdiği,
- Onay verildiğinde cezanın hangi duruma geçtiği.

Hem işlemleri yapan servis hem de arayüzdeki butonlar aynı kurallara başvurur. Bu sayede bir butonun görünmesi ile işlemin gerçekten yapılabilmesi her zaman tutarlıdır. Yeni bir onay aşaması (örneğin Hukuk onayı) eklemek için enum'a yeni bir durum ve bu iki tabloya birer satır eklemek yeterlidir.

### Reddedilen kaydın süreci

Reddedilen ceza kapanmaz; cezayı oluşturan personel tarafından düzeltilip yeniden onaya gönderilebilir. Yeniden gönderilen ceza süreci baştan (Yönetici onayından) başlatır. Önceki ret kayıtları geçmişte korunur, böylece bir cezanın kaç kez ve hangi nedenle reddedildiği izlenebilir.

### Onaylanmış kaydın değiştirilebilirliği

Ceza yalnızca **Yeni** veya **Reddedildi** durumundayken ve yalnızca oluşturan kişi tarafından düzenlenebilir. Onay sürecine girmiş veya tamamlanmış cezalar değiştirilemez. Bu kural sadece arayüzde değil, servis katmanında da uygulanır; adres çubuğundan doğrudan erişim denemeleri de engellenir.

### Onay geçmişinin tutulması

Durum değişikliği ve geçmiş kaydı aynı `SaveChanges` çağrısında, yani tek bir veritabanı işleminde kaydedilir. Geçmişte izi olmayan bir durum değişikliği oluşamaz. Cezanın oluşturulması da geçmişe ilk kayıt olarak yazılır.

### Eşzamanlı işlemler

`TrafikCezasi` tablosunda SQL Server `rowversion` alanı bulunur (optimistic concurrency). İki kullanıcı aynı cezayı aynı anda açıp işlem yaparsa ikinci işlem reddedilir ve kullanıcıdan sayfayı yenilemesi istenir. Böylece örneğin aşaması değişmiş bir ceza üzerinde eski sayfadan işlem yapılamaz.

### Veri bütünlüğü

- Plaka ve tutanak numarası benzersizdir (veritabanında unique index).
- Plakalar boşluksuz ve büyük harfe normalize edilerek saklanır (`34 abc 123` → `34ABC123`). Büyük harfe çevirmede kültürden bağımsız `ToUpperInvariant` kullanılır ("Turkish I" sorununu önlemek için).
- İlişkilerde silme davranışı `Restrict` olarak ayarlanmıştır; denetim kayıtları dolaylı olarak silinemez.
- Enum değerleri veritabanında okunabilirlik için metin olarak saklanır.

### Kültür ayarları

Tutar ve tarih değerleri kültürden bağımsız (invariant) okunur. Türkçe işletim sistemlerinde ondalık ayıracının yanlış yorumlanmasını önlemek için bu tercih edilmiştir. Kullanıcıya gösterilen tutarlar Türkçe biçimde (`1.250,50 ₺`) formatlanır.

### Hata yönetimi

- Form girdileri hem istemci hem sunucu tarafında doğrulanır.
- İş kuralı hataları servislerden ortak bir `Sonuc` nesnesiyle döner ve kullanıcıya anlaşılır mesaj olarak gösterilir.
- 404 ve 403 durumları için Türkçe bilgilendirme sayfaları, beklenmeyen hatalar için hata kodu içeren genel bir hata sayfası bulunur.
- Kullanıcılar yönetim tarafından tanımlanır; dışarıdan kayıt sayfası kapatılmıştır.

## Proje Yapısı

```
TrafikCezaYonetimi.Web/
├── Common/        Ortak yardımcılar (Sonuc, enum ve format uzantıları)
├── Constants/     Rol sabitleri
├── Controllers/   MVC controller'ları
├── Data/          DbContext, EF konfigürasyonları, migration'lar, seed verisi
├── Entities/      Veritabanı varlıkları ve enum'lar
├── Services/      İş kuralları ve onay akışı
├── ViewModels/    Form ve sayfa modelleri
└── Views/         Razor sayfaları
```

## Geliştirilebilecek Noktalar

- Servis katmanı ve onay akışı için birim testleri
- Listelerde sayfalama, arama ve filtreleme
- Araç düzenleme ve silme (cezası olan araçlar için kurallarla birlikte)
- Kullanıcı ve rol yönetimi ekranı
- Identity sayfalarının (giriş vb.) Türkçeleştirilmesi
- Onay aşamalarında e-posta bildirimleri