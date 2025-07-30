using System.Text.Encodings.Web;
using TextToHtmlApi.Models;
using System.IO.Compression;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives;
using System.Text;

namespace TextToHtmlApi.Services
{
  static class TextToHtml
  {
    public static async Task<List<ChatMessages>> CreateMessagesListAsync(StreamReader reader)
    {
      var lines = new List<string>();

      while (!reader.EndOfStream)
        lines.Add(await reader.ReadLineAsync());

      var messages = lines
          .Select(Parse.ParseLine)
          .Where(m => m != null)
          .OrderBy(m => m.dateTime)
          .ToList();

      return messages;
    }





    public static string CreateHtml(List<ChatMessages> messages, string user, int temaIndex, MemoryStream? memoryStream)
    {
      if (messages.Count == 0)
        return "<html><body><p>No messages found.</p></body></html>";

      string html = @"<!DOCTYPE html>
 <html lang='tr'>
 <head>
   <meta charset='UTF-8'>
   <title>Mesajlar</title>
   " + Theme.Tema(temaIndex) + @"
 </head>
 <body>
 " + File.ReadAllText("HtmlText/modal.txt") + @"
 <div class='chat-container'>
     <div class='messages'>";

      DateTime? lastDate = null;

      foreach (var reader in messages)
      {
        string name = reader.Name ?? "";
        string message = HtmlEncoder.Default.Encode(reader.Message ?? "");
        DateTime? dateTime = reader.dateTime;
        string time = dateTime?.ToString("HH:mm");
        string rowClass = (name == user) ? "sent" : "received";
        string dateStr = dateTime?.ToString("dd MMMM yyyy");

        if (dateTime.HasValue && (!lastDate.HasValue || lastDate.Value.Date != dateTime.Value.Date))
        {
          html += $"<div class='date-separator'><span>{dateStr}</span></div>\n";
          lastDate = dateTime.Value;
        }

        if (reader.Document)
        {
          if (memoryStream != null)
          {
            var fileName = "";
            var messageContent = "";
            var docIndex = message.IndexOf("(dosya ekli)");
            if (docIndex > 0)
            {
              fileName = message.Substring(0, docIndex).Trim();
              messageContent = message.Substring(docIndex + "(dosya ekli)".Length).Trim();
            }

            // HTML decode ve LTR karakterini temizle
            fileName = System.Web.HttpUtility.HtmlDecode(fileName);
            fileName = fileName.Replace("\u200E", "").Trim(); // LTR karakterini kaldır

            string? fileBase64;
            using (var archiveFiles = ArchiveFactory.Open(memoryStream))
            {
              if (memoryStream.Length > 0 && !string.IsNullOrEmpty(fileName))
              {
                
                var entry = archiveFiles.Entries
                            .FirstOrDefault(e => !e.IsDirectory &&
                               Path.GetFileName(e.Key).Equals(fileName, StringComparison.OrdinalIgnoreCase));

                if (entry != null)
                {
                  // Dosya bulundu, türüne göre render et
                  using var entryStream = entry.OpenEntryStream();
                  using var ms = new MemoryStream();
                  entryStream.CopyTo(ms);
                  var bytes = ms.ToArray();
                  fileBase64 = Convert.ToBase64String(bytes);

                  var fileExtension = Path.GetExtension(fileName).ToLower();
                  var fileContent = GetFilePreviewHtml(fileName, fileBase64, fileExtension);

                  message = $"<div class='file-attachment'><strong>{HtmlEncoder.Default.Encode(fileName)}</strong></div>" +
                           $"<div class='file-preview'>{fileContent}</div>" +
                           $"{HtmlEncoder.Default.Encode(messageContent)}";
                }
                else if (entry != null)
                {
                  // Dosya arşivde var ama çıkarılamadı
                  message = $"<div class='file-attachment'><strong>{HtmlEncoder.Default.Encode(fileName)}</strong></div>{HtmlEncoder.Default.Encode(messageContent)}";
                }
                else
                {
                  message = $"<div class='file-attachment missing'><strong>{HtmlEncoder.Default.Encode(fileName)}</strong> <span class='missing-file'>Missing file</span></div>{HtmlEncoder.Default.Encode(messageContent)}";
                }
              }
            }
          }
          else
          {
            var fileName = "";
            var messageContent = "";
            var docIndex = message.IndexOf("(dosya ekli)");
            if (docIndex > 0)
            {
              fileName = message.Substring(0, docIndex).Trim();
              messageContent = message.Substring(docIndex + "(dosya ekli)".Length).Trim();
            }

            // HTML decode ve LTR karakterini temizle
            fileName = System.Web.HttpUtility.HtmlDecode(fileName);
            fileName = fileName.Replace("\u200E", "").Trim(); // LTR karakterini kaldır

            message = $"<div class='file-attachment missing'><strong>{HtmlEncoder.Default.Encode(fileName)}</strong> <span class='missing-file'>Missing file</span></div>{HtmlEncoder.Default.Encode(messageContent)}";
          }
        }
          

        html += $"<div class='message-row {rowClass}'>" +
                "<div class='message'>" +
                // Sadece gönderici değilse ismi göster
                $"{(rowClass == "sent" ? "" : $"<span class='name'>{name}</span>")}" +
                $"{message}" +
                $"<span class='time'>{time}</span></div></div>\n";
      }

      html += @"    </div>
  </div>
</body>
</html>";
      return html;
    }


    // Dosya türüne göre HTML preview oluştur
    private static string GetFilePreviewHtml(string fileName, string fileBase64, string fileExtension)
    {
      try
      {
        var bytes = Convert.FromBase64String(fileBase64);
        var fileSize = FormatFileSize(bytes.Length);

        switch (fileExtension)
        {
          case ".jpg":
          case ".jpeg":
          case ".png":
          case ".gif":
          case ".bmp":
          case ".webp":
            return $@"
                         <div class='file-preview-image'>
                             <img src='data:image/webp;base64,{fileBase64}' alt='{HtmlEncoder.Default.Encode(fileName)}' style='max-width: 300px; max-height: 300px; border-radius: 8px; cursor: pointer;' onclick='openImageModal(this.src, ""{HtmlEncoder.Default.Encode(fileName)}"")'>
                             <div class='file-info'>
                                 <span class='file-size'>filesize</span>
                                 <a href='indirme linki' download='{HtmlEncoder.Default.Encode(fileName)}' class='download-btn'>İndir</a>
                             </div>
                         </div>";

          case ".mp4":
          case ".avi":
          case ".mov":
          case ".wmv":
          case ".flv":
          case ".webm":
            return $@"
                         <div class='file-preview-video'>
                             <video controls style='max-width: 400px; max-height: 300px; border-radius: 8px; cursor: pointer;' onclick='openVideoModal(this.src, ""{HtmlEncoder.Default.Encode(fileName)}"")'>
                                 <source src='data:video/webm;base64,{fileBase64}' type='video/{fileExtension.Substring(1)}'>
                                 Tarayıcınız video oynatmayı desteklemiyor.
                             </video>
                             <div class='file-info'>
                                 <span class='file-size'>{fileSize}</span>
                                 <a href='indirme linki' download='{HtmlEncoder.Default.Encode(fileName)}' class='download-btn'>İndir</a>
                             </div>
                         </div>";

          case ".mp3":
          case ".wav":
          case ".ogg":
          case ".flac":
            return $@"
                        <div class='file-preview-audio'>
                            <audio controls style='width: 100%; max-width: 400px;'>
                                <source src='data:audio/flac;base64,{fileBase64}' type='audio/{fileExtension.Substring(1)}'>
                                Tarayıcınız ses oynatmayı desteklemiyor.
                            </audio>
                            <div class='file-info'>
                                <span class='file-size'>{fileSize}</span>
                                <a href='indirme linki' download='{HtmlEncoder.Default.Encode(fileName)}' class='download-btn'>İndir</a>
                            </div>
                        </div>";

          case ".pdf":
            try
            {
              var pdfDataUrl = $"data:application/pdf;base64,{fileBase64}";

              return $@"
                             <div class='file-preview-pdf'>
                                 <object data='{pdfDataUrl}#toolbar=0&navpanes=0&scrollbar=0&view=FitH' type='application/pdf' style='width: 100%; height: 400px; border: 1px solid #ddd; border-radius: 8px;'>
                                     <p>PDF görüntülenemiyor. <a href='data:image/webp;base64,{fileBase64}' target='_blank'>PDF'i yeni sekmede aç</a></p>
                                 </object>
                                 <div class='file-info'>
                                     <span class='file-size'>{fileSize}</span>
                                     <a href='indirme linki' download='{HtmlEncoder.Default.Encode(fileName)}' class='download-btn'>İndir</a>
                                 </div>
                             </div>";
            }
            catch (Exception ex)
            {
              return $@"
                             <div class='file-preview-pdf'>
                                 <div class='pdf-error'>
                                     <p>PDF yüklenirken hata: {HtmlEncoder.Default.Encode(ex.Message)}</p>
                                     <a href='data:image/webp;base64,{fileBase64}' target='_blank' class='download-btn'>PDF'i yeni sekmede aç</a>
                                 </div>
                                 <div class='file-info'>
                                     <span class='file-size'>{fileSize}</span>
                                     <a href='indirme linki' download='{HtmlEncoder.Default.Encode(fileName)}' class='download-btn'>İndir</a>
                                 </div>
                             </div>";
            }

          case ".vcf":
            return $@"
                         <div class='file-preview-vcf'>
                             <div class='vcf-content'>
                                 {VcfCard.ParseVcfToHtml(Encoding.UTF8.GetString(bytes))}
                             </div>
                             <div class='file-info'>
                                 <span class='file-size'>{fileSize}</span>
                                 <a href='indirme linki' download='{HtmlEncoder.Default.Encode(fileName)}' class='download-btn'>İndir</a>
                             </div>
                         </div>";

          case ".txt":
          case ".log":
            return $@"
                        <div class='file-preview-text'>
                            <div class='text-content'>
                                <pre>{HtmlEncoder.Default.Encode(Encoding.UTF8.GetString(bytes))}</pre>
                            </div>
                            <div class='file-info'>
                                <span class='file-size'>{fileSize}</span>
                                <a href='indirme linki' download='{HtmlEncoder.Default.Encode(fileName)}' class='download-btn'>İndir</a>
                            </div>
                        </div>";

          default:
            return $@"
                        <div class='file-preview-generic'>
                            <div class='generic-file'>
                                <span class='file-icon'>📄</span>
                                <span class='file-name'>{HtmlEncoder.Default.Encode(fileName)}</span>
                            </div>
                            <div class='file-info'>
                                <span class='file-size'>{fileSize}</span>
                                <a href='indirme linki' download='{HtmlEncoder.Default.Encode(fileName)}' class='download-btn'>İndir</a>
                            </div>
                        </div>";
        }
      }
      catch
      {
        return $@"
                <div class='file-preview-error'>
                    <span class='error-message'>Dosya Yüklenemedi</span>
                </div>";
      }
    }

    

    // Dosya boyutunu formatla
    private static string FormatFileSize(long bytes)
    {
      string[] sizes = { "B", "KB", "MB", "GB" };
      double len = bytes;
      int order = 0;
      while (len >= 1024 && order < sizes.Length - 1)
      {
        order++;
        len = len / 1024;
      }
      return $"{len:0.##} {sizes[order]}";
    }

  }
}