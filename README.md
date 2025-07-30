# MessageToHtml

## Proje Açıklaması
Bu proje, whatsapp sohbetinin dışarı aktarıldığında txt dosyasını html dosyasına çevirerek görüntü sunmayı amaçlamaktadır. Kullanıcıdan alınan .txt veya .zip formatındaki Whatsapp sohbet dökümanlarını işleyerek HTML formatına dönüştüren bir .NET Core Web API uygulamasıdır. Basit bir frontend arayüzü ile dosya yükleme imkanı sağlar.

## Nasıl Çalıştırılır
Projeyi kullanmak için .net9.0 sdk indirilmeli:
- Winget kullanarak konsol indirme komutu -> winget install Microsoft.DotNet.SDK.9

Paketleri yükle:
- Program.cs nin bulunduğu dizinde "dotnet restore" kodunu çalıştır.

Ardından Program.cs nin bulunduğu dizinde "dotnet run" kodunu yazarak yada tercihen kullanılan debug aracı ile projeyi çalıştırabilirsiniz

## Kullanım

- Program çalıştırıldığında frontend otomatik olarak çalışmaktadır. Eğer bir sorun yaşanıyorsa wwwroot/index.html dosyasının çalışan api ile aynı portta çalıştırılması gerekir. 
- Whatsap sohbetinden dışarı aktarılan .zip dosyasını yada .zip dosyası içerisindeki .txt dosyasını dosya seç alanına ekledikten sonra Dosyayı Yükle Ve Analiz Et butonuna basılmalıdır. .txt dosyası yüklerseniz medyasız görüntüyle karşılaşırsınız. Detaylı rehbere ihtiyacınız varsa proje çalıştırıldığında açılan pencere üzerindeki Nasıl Çalışır tuşuna basın yada [Paragraf metniniz (2).pdf](https://github.com/user-attachments/files/21514693/Paragraf.metniniz.2.pdf) bu pdf dosyasını okuyun.
- Dosyayı analiz ettikten sonra api tarafından analiz sonucu gelecektir.
1. Kullanıcılar arasından mesajı sağda gözükecek kullanıcıyı seçin yada 3. bir şahıs tarafından bakmak istiyorsanız Tüm Kullanıcılar değerini seçin.
2. Tema olarak sadece varsayılan tema yani Klasik (Varsayılan) kullanılmaktadır.
3. Hangi tarih aralığındaki mesajları göstermek istiyorsanız onu seçin. Son tarih için 3 saat ileri alın, zaman sistemi doğru çalışmamaktadır.
- Seçeneklerinizi yaptıktan sonra Html önizlemesini oluşturun, önizlemeyi beğenirseniz "HTML Dosyasını İndir" tuşuna basın.
  
<br>**Dosyan indirildi artık dilediğiniz gibi taşıyabilir, saklayabilir, yada göstermek istediğiniz arkadaşınıza gönderebilirsin**
<br>

## Özellikler
📄 .txt veya .zip uzantılı WhatsApp sohbet dökümanını alır

🛠️ Satır satır ayrıştırarak sohbet mesajlarını işler

💬 JSON formatına dönüştürür, ardından HTML çıktısı üretir

🌐 API tabanlıdır, Swagger üzerinden test edilebilir

🧪 Veriler sadece bellek üzerinde işlenir, dosyalar kalıcı olarak kaydedilmez

🔐 Temel güvenlik önlemleri alınmıştır; kullanıcılar birbirinin verisine erişemez


- wwwroot/index.html üzerinden kullanıcı arayüzü sunulur

- Frontend, API ile aynı port üzerinden erişilir

- Dosya yükleme butonu ve API çağrısı içerir

- Program çalıştırıldığında frontend otomatik olarak açılır

## İletişim
Yardım, hata bildirimi veya öneriler içinw iletişim adreslerim:
- [linkedin:](https://www.linkedin.com/in/furkanksc)
- E-mail: furkankuscutr18@gmail.com

