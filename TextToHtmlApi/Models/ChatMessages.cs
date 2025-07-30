
namespace TextToHtmlApi.Models
{
    public class ChatMessages
    {
        public DateTime dateTime { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public bool Document { get; set; }
    }

    public class ViewRequest
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string? user { get; set; }
        public int themeIndex { get; set; }
    }
}