namespace MoversAndPackerApi.Models
{
    public class GenAIIntent
    {
        public string Intent { get; set; } // PRICE, FAQ, GREETING, UNKNOWN
        public decimal? Distance { get; set; }
        public decimal? CFT { get; set; }
        public string Language { get; set; }
    }

}
