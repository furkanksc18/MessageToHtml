namespace TextToHtmlApi.Services
{
    class Theme
    {
        public static string Tema(int index)
        {
            return File.ReadAllText("HtmlText/theme.txt");
        }
        
    }
}