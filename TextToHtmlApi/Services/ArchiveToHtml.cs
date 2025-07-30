using System.IO.Compression;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using TextToHtmlApi.Models;

namespace TextToHtmlApi.Services
{
    static class ArchiveToHtml
    {


        // ZIP veya RAR arşivden ilk .txt dosyasını bulup işleyen fonksiyon
        public static async Task<List<ChatMessages>?> CreateMessagesListFromArchiveAsync(IFormFile archiveFile)
        {
           
            var ext = Path.GetExtension(archiveFile.FileName).ToLower();
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await archiveFile.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    if (ext == ".zip")
                    {
                        using (var archive = ArchiveFactory.Open(memoryStream))
                        {
                            var txtEntry = archive.Entries.FirstOrDefault(e => Path.GetExtension(e.Key).ToLower() == ".txt" && !e.IsDirectory);
                            if (txtEntry == null)
                            {
                                Console.WriteLine($"ZİP arşivinde .txt dosyası bulunamadı. Mevcut dosyalar: {string.Join(", ", archive.Entries.Select(e => e.Key))}");
                                return null;
                            }
                                
                            using (var entryStream = txtEntry.OpenEntryStream())
                            using (var reader = new StreamReader(entryStream))
                            {
                                var messages = await TextToHtml.CreateMessagesListAsync(reader);
                                Console.WriteLine($"ZİP arşivinden {messages.Count} mesaj okundu.");
                                return messages;
                            }
                        }
                    }

                    else if (ext == ".rar")
                    {

                        using (var archive = ArchiveFactory.Open(memoryStream))
                        {
                            var txtEntry = archive.Entries.FirstOrDefault(e => Path.GetExtension(e.Key).ToLower() == ".txt" && !e.IsDirectory);
                            if (txtEntry == null)
                            {
                                Console.WriteLine($"RAR arşivinde .txt dosyası bulunamadı. Mevcut dosyalar: {string.Join(", ", archive.Entries.Select(e => e.Key))}");
                                return null;
                            }
                            using (var entryStream = txtEntry.OpenEntryStream())
                            using (var reader = new StreamReader(entryStream))
                            {
                                var messages = await TextToHtml.CreateMessagesListAsync(reader);

                                Console.WriteLine($"RAR arşivinden {messages.Count} mesaj okundu.");
                                return messages;
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Arşiv dosyası açılırken hata: {ex.Message}");
                return null;
            }
        }
    }
}