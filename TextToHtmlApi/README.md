Bu proje, kullanıcıdan alınan `.txt` veya `.zip` formatındaki WhatsApp sohbet dökümanlarını işleyerek HTML formatına dönüştüren bir .NET Core Web API uygulamasıdır. Basit bir frontend arayüzü ile dosya yükleme imkanı sağlar.

- 📄 `.txt` veya `.zip` uzantılı WhatsApp sohbet dökümanını alır
- 🛠️ Satır satır ayrıştırarak sohbet mesajlarını işler
- 💬 JSON formatına dönüştürür, ardından HTML çıktısı üretir
- 🌐 API tabanlıdır, Swagger üzerinden test edilebilir
- 🧪 Veriler sadece bellek üzerinde işlenir, dosyalar kalıcı olarak kaydedilmez
- 🔐 Temel güvenlik önlemleri alınmıştır; kullanıcılar birbirinin verisine erişemez

- `wwwroot/index.html` üzerinden kullanıcı arayüzü sunulur
- Frontend, API ile aynı port üzerinden erişilir
- Dosya yükleme butonu ve API çağrısı içerir
- Program çalıştırıldığında frontend otomatik olarak açılır