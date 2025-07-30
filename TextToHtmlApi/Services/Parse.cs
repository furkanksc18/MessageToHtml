using TextToHtmlApi.Models;

namespace TextToHtmlApi.Services
{
    static class Parse
    {
        public static ChatMessages? ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return null;

            string[] firstSplit = line.Split('-', 2);
            if (firstSplit.Length < 2)
                return null;

            string[] secondSplit = firstSplit[1].Split(':', 2);
            if (secondSplit.Length < 2)
                return null;

            if (secondSplit[1] == " null")
                return null;

            if (!DateTime.TryParse(firstSplit[0], out var dateTime))
                return null;

            return new ChatMessages
            {
                dateTime = dateTime,
                Name = secondSplit[0].Trim(),
                Message = secondSplit[1].Trim(),
                Document = secondSplit[1].Contains("(dosya ekli)") || secondSplit[1].Contains("<Medya dahil edilmedi>")
            };
        }
    }
}