namespace RStyleTranslator.Models
{
    /// <summary>
    /// Направление перевода
    /// </summary>
    public class Route
    {
        public int Id { get; set; }
        public string FromLangId { get; set; }
        public string ToLangId { get; set; }
    }
}
