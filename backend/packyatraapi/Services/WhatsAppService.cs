using System.Text;
using System.Text.Json;

namespace MoversAndPackerApi.Services
{
    public class WhatsAppService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public WhatsAppService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task SendWhatsAppWithPdfAsync(string mobile, string pdfUrl)
        {
            var payload = new
            {
                user = _config["WhatsApp:UserName"],
                pass = _config["WhatsApp:Password"],
                to = mobile,
                type = "template",
                template_name = _config["WhatsApp:TemplateName"],
                language = "en",
                document = new
                {
                    url = pdfUrl,
                    filename = "PackYatra_Document.pdf"
                }
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(
                _config["WhatsApp:BaseUrl"],
                content
            );
        }
    }
}
