using System.Text.Encodings.Web;

namespace TextToHtmlApi.Services
{
    static class VcfCard
    {
        // VCF dosyasını güzel HTML kartlarına çevir
    public static string ParseVcfToHtml(string vcfContent)
    {
      try
      {
        Console.WriteLine($"VCF içeriği: {vcfContent}");
        var lines = vcfContent.Split('\n');
        Console.WriteLine($"VCF satır sayısı: {lines.Length}");

        // VCF dosyasında satır devamı olabilir, bunları birleştir
        var processedLines = new List<string>();
        for (int i = 0; i < lines.Length; i++)
        {
          var line = lines[i];
          if (line.StartsWith(" ") || line.StartsWith("\t"))
          {
            // Bu satır önceki satırın devamı
            if (processedLines.Count > 0)
            {
              processedLines[processedLines.Count - 1] += line.Trim();
            }
          }
          else
          {
            processedLines.Add(line);
          }
        }

        var contacts = new List<Dictionary<string, string>>();
        var currentContact = new Dictionary<string, string>();

        foreach (var line in processedLines)
        {
          var trimmedLine = line.Trim();
          if (trimmedLine.StartsWith("BEGIN:VCARD"))
          {
            currentContact = new Dictionary<string, string>();
          }
          else if (trimmedLine.StartsWith("END:VCARD"))
          {
            if (currentContact.Count > 0)
              contacts.Add(currentContact);
          }
          else if (trimmedLine.StartsWith("FN:"))
          {
            currentContact["name"] = trimmedLine.Substring(3).Trim();
          }
          else if (trimmedLine.StartsWith("TEL"))
          {
            // TEL: veya TEL;TYPE=CELL: formatlarını destekle
            var phoneNumber = "";
            if (trimmedLine.Contains(":"))
            {
              phoneNumber = trimmedLine.Split(':').Last().Trim();
            }
            else
            {
              phoneNumber = trimmedLine.Substring(4).Trim();
            }

            Console.WriteLine($"TEL satırı bulundu: '{trimmedLine}' -> '{phoneNumber}'");

            if (!string.IsNullOrEmpty(phoneNumber))
            {
              if (!currentContact.ContainsKey("phone"))
                currentContact["phone"] = phoneNumber;
              else
                currentContact["phone"] += ", " + phoneNumber;
            }
          }
          else if (trimmedLine.StartsWith("TEL;"))
          {
            // TEL;TYPE=CELL: formatı
            var phoneNumber = "";
            if (trimmedLine.Contains(":"))
            {
              phoneNumber = trimmedLine.Split(':').Last().Trim();
            }

            Console.WriteLine($"TEL; satırı bulundu: '{trimmedLine}' -> '{phoneNumber}'");

            if (!string.IsNullOrEmpty(phoneNumber))
            {
              if (!currentContact.ContainsKey("phone"))
                currentContact["phone"] = phoneNumber;
              else
                currentContact["phone"] += ", " + phoneNumber;
            }
          }
          else if (trimmedLine.StartsWith("TEL;TYPE="))
          {
            // TEL;TYPE=CELL: formatı
            var phoneNumber = "";
            if (trimmedLine.Contains(":"))
            {
              phoneNumber = trimmedLine.Split(':').Last().Trim();
            }

            Console.WriteLine($"TEL;TYPE= satırı bulundu: '{trimmedLine}' -> '{phoneNumber}'");

            if (!string.IsNullOrEmpty(phoneNumber))
            {
              if (!currentContact.ContainsKey("phone"))
                currentContact["phone"] = phoneNumber;
              else
                currentContact["phone"] += ", " + phoneNumber;
            }
          }
          else if (trimmedLine.Contains("TEL") && trimmedLine.Contains(":"))
          {
            // item1.TEL;...: formatı dahil tüm varyasyonlar
            var telIndex = trimmedLine.IndexOf("TEL");
            var phonePart = trimmedLine.Substring(telIndex);
            var phoneNumber = phonePart.Split(':').Last().Trim();
            Console.WriteLine($"GENERIC TEL satırı bulundu: '{trimmedLine}' -> '{phoneNumber}'");
            if (!string.IsNullOrEmpty(phoneNumber))
            {
              if (!currentContact.ContainsKey("phone"))
                currentContact["phone"] = phoneNumber;
              else
                currentContact["phone"] += ", " + phoneNumber;
            }
          }
          else if (trimmedLine.StartsWith("N:"))
          {
            // N: formatı (Name formatı)
            var nameValue = trimmedLine.Substring(2).Trim();
            if (!currentContact.ContainsKey("name") && !string.IsNullOrEmpty(nameValue))
            {
              currentContact["name"] = nameValue;
            }
          }
          else if (trimmedLine.StartsWith("EMAIL:"))
          {
            if (!currentContact.ContainsKey("email"))
              currentContact["email"] = trimmedLine.Substring(6).Trim();
          }
          else if (trimmedLine.StartsWith("ORG:"))
          {
            if (!currentContact.ContainsKey("organization"))
              currentContact["organization"] = trimmedLine.Substring(4).Trim();
          }
        }

        if (contacts.Count == 0)
        {
          // Tek kişi varsa
          var name = "";
          var phone = "";
          var email = "";
          var org = "";

          Console.WriteLine("VCARD formatı bulunamadı, tek kişi olarak işleniyor...");

          foreach (var line in processedLines)
          {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("FN:"))
              name = trimmedLine.Substring(3).Trim();
            else if (trimmedLine.StartsWith("TEL"))
            {
              // TEL: veya TEL;TYPE=CELL: formatlarını destekle
              var phoneNumber = "";
              if (trimmedLine.Contains(":"))
              {
                phoneNumber = trimmedLine.Split(':').Last().Trim();
              }
              else
              {
                phoneNumber = trimmedLine.Substring(4).Trim();
              }

              Console.WriteLine($"Tek kişi TEL satırı: '{trimmedLine}' -> '{phoneNumber}'");

              if (!string.IsNullOrEmpty(phoneNumber))
              {
                if (string.IsNullOrEmpty(phone))
                  phone = phoneNumber;
                else
                  phone += ", " + phoneNumber;
              }
            }
            else if (trimmedLine.StartsWith("TEL;"))
            {
              // TEL;TYPE=CELL: formatı
              var phoneNumber = "";
              if (trimmedLine.Contains(":"))
              {
                phoneNumber = trimmedLine.Split(':').Last().Trim();
              }

              Console.WriteLine($"Tek kişi TEL; satırı: '{trimmedLine}' -> '{phoneNumber}'");

              if (!string.IsNullOrEmpty(phoneNumber))
              {
                if (string.IsNullOrEmpty(phone))
                  phone = phoneNumber;
                else
                  phone += ", " + phoneNumber;
              }
            }
            else if (trimmedLine.StartsWith("TEL;TYPE="))
            {
              // TEL;TYPE=CELL: formatı
              var phoneNumber = "";
              if (trimmedLine.Contains(":"))
              {
                phoneNumber = trimmedLine.Split(':').Last().Trim();
              }

              Console.WriteLine($"Tek kişi TEL;TYPE= satırı: '{trimmedLine}' -> '{phoneNumber}'");

              if (!string.IsNullOrEmpty(phoneNumber))
              {
                if (string.IsNullOrEmpty(phone))
                  phone = phoneNumber;
                else
                  phone += ", " + phoneNumber;
              }
            }
            else if (trimmedLine.Contains("TEL") && trimmedLine.Contains(":"))
            {
              var telIndex = trimmedLine.IndexOf("TEL");
              var phonePart = trimmedLine.Substring(telIndex);
              var phoneNumber = phonePart.Split(':').Last().Trim();
              Console.WriteLine($"Tek kişi GENERIC TEL satırı: '{trimmedLine}' -> '{phoneNumber}'");
              if (!string.IsNullOrEmpty(phoneNumber))
              {
                if (string.IsNullOrEmpty(phone))
                  phone = phoneNumber;
                else
                  phone += ", " + phoneNumber;
              }
            }
            else if (trimmedLine.StartsWith("N:"))
            {
              // N: formatı (Name formatı)
              var nameValue = trimmedLine.Substring(2).Trim();
              if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(nameValue))
              {
                name = nameValue;
              }
            }
            else if (trimmedLine.StartsWith("EMAIL:"))
              email = trimmedLine.Substring(6).Trim();
            else if (trimmedLine.StartsWith("ORG:"))
              org = trimmedLine.Substring(4).Trim();
          }

          Console.WriteLine($"Tek kişi sonuç: Name='{name}', Phone='{phone}', Email='{email}', Org='{org}'");
          return $@"
                    <div class='contact-card'>
                        <div class='contact-avatar'>👤</div>
                        <div class='contact-info'>
                            <div class='contact-name'>{HtmlEncoder.Default.Encode(name)}</div>
                            {(!string.IsNullOrEmpty(phone) ? $"<div class='contact-phone'>📞 {HtmlEncoder.Default.Encode(phone)}</div>" : "")}
                            {(!string.IsNullOrEmpty(email) ? $"<div class='contact-email'>📧 {HtmlEncoder.Default.Encode(email)}</div>" : "")}
                            {(!string.IsNullOrEmpty(org) ? $"<div class='contact-org'>🏢 {HtmlEncoder.Default.Encode(org)}</div>" : "")}
                        </div>
                    </div>";
        }

        var html = "";
        foreach (var contact in contacts)
        {
          var name = contact.GetValueOrDefault("name", "");
          var phone = contact.GetValueOrDefault("phone", "");
          var email = contact.GetValueOrDefault("email", "");
          var org = contact.GetValueOrDefault("organization", "");

          html += $@"
                    <div class='contact-card'>
                        <div class='contact-avatar'>👤</div>
                        <div class='contact-info'>
                            <div class='contact-name'>{HtmlEncoder.Default.Encode(name)}</div>
                            {(!string.IsNullOrEmpty(phone) ? $"<div class='contact-phone'>📞 {HtmlEncoder.Default.Encode(phone)}</div>" : "")}
                            {(!string.IsNullOrEmpty(email) ? $"<div class='contact-email'>📧 {HtmlEncoder.Default.Encode(email)}</div>" : "")}
                            {(!string.IsNullOrEmpty(org) ? $"<div class='contact-org'>🏢 {HtmlEncoder.Default.Encode(org)}</div>" : "")}
                        </div>
                    </div>";
        }

        return html;
      }
      catch (Exception ex)
      {
        return $@"<div class='vcf-error'>VCF dosyası okunamadı: {HtmlEncoder.Default.Encode(ex.Message)}</div>";
      }
    }
    }
}