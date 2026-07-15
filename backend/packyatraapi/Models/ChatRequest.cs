namespace MoversAndPackerApi.Models
{
    public class ChatRequest
    {
        public string Message { get; set; }
        public string Language { get; set; } = "en"; // en, hi, kn, te, ta, ml
        public string Mode { get; set; } = "auto"; // auto, price, faq
    }
}
