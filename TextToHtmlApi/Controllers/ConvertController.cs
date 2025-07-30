using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using TextToHtmlApi.Models;
using TextToHtmlApi.Services;

namespace TextToHtmlApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConvertController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            //dosyayı al -> temiz ise analiz et sonuçları kullanıcıya gönder

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var ext = Path.GetExtension(file.FileName)?.ToLower();
            List<ChatMessages>? messages = null;

            if (ext == ".txt")
            {
                messages = await TextToHtml.CreateMessagesListAsync(new StreamReader(file.OpenReadStream()));
            }
            else if (ext == ".zip" || ext == ".rar")
            {
                messages = await ArchiveToHtml.CreateMessagesListFromArchiveAsync(file);
            }
            else
            {
                return BadRequest("Only .txt, .zip or .rar files are allowed.");
            }

            if (messages == null || messages.Count == 0)
                return BadRequest("No messages found.");

            // Benzersiz kullanıcı listesi
            var users = messages.Select(m => m.Name).Distinct().ToList();

            // İlk ve son mesaj tarihi
            DateTime firstDate = new DateTime();
            DateTime lastDate = new DateTime();
            if (messages.Count > 0)
            {
                var orderedDates = messages.Select(m => m.dateTime).OrderBy(d => d).ToList();
                firstDate = orderedDates.First();
                lastDate = orderedDates.Last();
            }

            return Ok(new
            {
                users,
                firstDate,
                lastDate,
                messageCount = messages.Count
            });
        }

        [HttpPost("view")]
        public async Task<IActionResult> ViewHtml(
            [FromForm] IFormFile file, 
            [FromForm] string previewRequestJson)
        {

            var req = JsonSerializer.Deserialize<ViewRequest>(previewRequestJson);
            // seçenekler ve  dosyayı al, dosya temiz ise işle kullanıcıya gönder

            if (req.startDate == null || req.endDate == null || req.endDate < req.startDate)
                return BadRequest("Date between is wrong");

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");


            var ext = Path.GetExtension(file.FileName)?.ToLower();
            List<ChatMessages>? messages = null;
            string html = null;

            if (ext == ".txt")
            {
                messages = await TextToHtml.CreateMessagesListAsync(new StreamReader(file.OpenReadStream()));
                if (messages == null || messages.Count == 0)
                    return BadRequest("No messages found.");

                var filteredMessages = messages.Where(m => m.dateTime >= req.startDate && m.dateTime <= req.endDate).ToList();
                html = TextToHtml.CreateHtml(filteredMessages, req.user ?? "", req.themeIndex, null);
            }
            else if (ext == ".zip" || ext == ".rar")
            {
                messages = await ArchiveToHtml.CreateMessagesListFromArchiveAsync(file);
                if (messages == null || messages.Count == 0)
                    return BadRequest("No messages found.");

                var filteredMessages = messages.Where(m => m.dateTime >= req.startDate && m.dateTime <= req.endDate).ToList();
                using (var memoryStream = new MemoryStream())
                {

                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    // Dosya uzantısına bakmadan, içeriğe bakarak türünü tespit et
                    bool isZip = false;
                    bool isRar = false;

                    try
                    {
                        var buffer = new byte[4];
                        memoryStream.Read(buffer, 0, 4);

                        // ZIP dosyası başlangıcı: 0x50 0x4B 0x03 0x04
                        if (buffer[0] == 0x50 && buffer[1] == 0x4B && buffer[2] == 0x03 && buffer[3] == 0x04)
                        {
                            isZip = true;
                        }
                        // RAR dosyası başlangıcı: 0x52 0x61 0x72 0x21
                        else if (buffer[0] == 0x52 && buffer[1] == 0x61 && buffer[2] == 0x72 && buffer[3] == 0x21)
                        {
                            isRar = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Dosya formatı tespit edilirken hata: {ex.Message}");
                    }
                    html = TextToHtml.CreateHtml(filteredMessages, req.user ?? "", req.themeIndex, memoryStream);
                }
            }
            else
            {
                return BadRequest("Only .txt, .zip or .rar files are allowed.");
            }

            return Content(html, "text/html");
        }
    }
}
