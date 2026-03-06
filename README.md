# MessageToHtml

WhatsApp sohbet dışa aktarımlarını (txt/zip/rar) analiz ederek, tarih aralığı ve kullanıcı filtreleriyle **filtrelenebilir bir HTML çıktısına** dönüştüren **ASP.NET Core Web API**.

- **Amaç**: WhatsApp sohbet export dosyalarını okunabilir/taşınabilir HTML formatına çevirmek
- **Hedef kullanıcı**: Sohbetini görünür HTML formatında saklamak isteyen herkes
- **Platform**: Cross-platform (Windows / Linux / macOS), Web API

## Özellikler

- **Dosya yükleme**: `.txt`, `.zip`, `.rar` sohbet dışa aktarımlarını kabul eder
- **Ön analiz**: Sohbetteki kullanıcıları ve tarih aralığını çıkarır
- **Filtrelenmiş HTML üretimi**: Belirlenen tarih aralığı ve kullanıcı seçimine göre HTML döner
- **Swagger/OpenAPI**: Geliştirme ortamında Swagger UI ile API keşfi
- **Statik içerik**: `wwwroot` üzerinden statik dosya servis edebilir

## Ekran Görüntüsü

> Placeholder

```text
/docs/screenshot.png
```

## Kurulum

### Gereksinimler

- `.NET 9 SDK`

### Çalıştırma

Proje dizininde:

```bash
dotnet restore
dotnet run
```

Geliştirme ortamında Swagger UI varsayılan olarak etkinleşir.

## Kullanım

API, `ConvertController` üzerinden aşağıdaki endpoint’leri sağlar.

### 1) Dosya Yükle (Ön Analiz)

- **Endpoint**: `POST /api/convert/upload`
- **Form field**: `file` (IFormFile)
- **Amaç**: Dosyayı okuyup kullanıcı listesini ve tarih aralığını döndürmek

Örnek `curl`:

```bash
curl -X POST "https://localhost:5001/api/convert/upload" \
  -F "file=@./chat.txt"
```

Örnek yanıt:

```json
{
  "users": ["Kullanıcı 1", "Kullanıcı 2"],
  "firstDate": "2024-01-01T00:00:00",
  "lastDate": "2024-12-31T23:59:59",
  "messageCount": 12345
}
```

### 2) HTML Görüntüle (Filtreli)

- **Endpoint**: `POST /api/convert/view`
- **Form fields**:
  - `file`: sohbet dosyası
  - `previewRequestJson`: JSON string
- **Dönüş**: `text/html`

`previewRequestJson` örneği:

```json
{
  "startDate": "2024-01-01T00:00:00",
  "endDate": "2024-02-01T00:00:00",
  "user": "Kullanıcı 1",
  "themeIndex": 0
}
```

Örnek `curl`:

```bash
curl -X POST "https://localhost:5001/api/convert/view" \
  -F "file=@./chat.txt" \
  -F 'previewRequestJson={"startDate":"2024-01-01T00:00:00","endDate":"2024-02-01T00:00:00","user":"Kullanıcı 1","themeIndex":0}' \
  -H "Accept: text/html"
```

Notlar:

- `startDate`/`endDate` zorunludur; `endDate < startDate` olamaz.
- `.zip` / `.rar` için dosya türü içeriğe bakılarak tespit edilir.

## Proje Yapısı

```text
TextToHtmlApi/
  Controllers/
    ConvertController.cs
  Models/
  Services/
  HtmlText/
  wwwroot/
  Program.cs
  TextToHtmlApi.csproj
  appsettings.json
  appsettings.Development.json
```

## Katkıda Bulunma

Bu depo **açık kaynak değildir**. Fork/modify/redistribute gibi işlemler yalnızca yazılı izinle yapılabilir.

- Hata bildirimi ve öneriler için Issue açabilirsiniz.
- Katkı yapmak isterseniz önce bir Issue üzerinden değişikliği tartışın.

## Lisans

Bu proje **Proprietary** lisanslıdır (**All Rights Reserved**).

- Kişisel kullanım için görüntüleme/çalıştırma izni verilebilir.
- **Fork**, **değiştirme**, **yeniden dağıtım**, **ticari kullanım** ve türev çalışmalar **yazılı izin olmadan yasaktır**.

Lisans/izin talepleri için: `furkankuscutr18@gmail.com`

Detaylar için `LICENSE` dosyasına bakın.
