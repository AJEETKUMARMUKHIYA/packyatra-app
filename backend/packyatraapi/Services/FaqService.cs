using MoversAndPackerApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MoversAndPackerApi.Services
{
    public class FAQService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;
        private readonly Dictionary<string, List<string>> _faqDatabase;

        public FAQService(IConfiguration config)
        {
            _config = config;
            _http = new HttpClient();

            // Initialize with PDF content (could load from database)
            _faqDatabase = new Dictionary<string, List<string>>
{
    // Q1 – Q10
    { "what is packyatra", new List<string> {
        "PackYatra is a packer and mover relocation and logistics platform that helps people move homes, offices, and vehicles safely."
    }},
    { "packyatra services", new List<string> {
        "Packing, moving, transportation, unpacking and relocation support."
    }},
    { "is packyatra trusted", new List<string> {
        "Yes, PackYatra is a registered private limited company."
    }},
    { "packyatra city availability", new List<string> {
        "PackYatra operates across multiple cities in India."
    }},
    { "single item move", new List<string> {
        "Yes, PackYatra moves single items."
    }},
    { "small moves", new List<string> {
        "Yes, both small and large moves are supported."
    }},
    { "pg hostel items", new List<string> {
        "Yes, PG or hostel items can be moved."
    }},
    { "students use packyatra", new List<string> {
        "Yes, students can use PackYatra."
    }},
    { "senior citizens", new List<string> {
        "Yes, PackYatra is suitable for senior citizens with assistance."
    }},

    // Q11 – Q20
    { "how to book move", new List<string> {
        "Fill the booking form on the PackYatra website packyatra.com."
    }},
    { "unknown item quantity", new List<string> {
        "Yes, approximate details are acceptable."
    }},
    { "change booking date", new List<string> {
        "Yes, subject to availability."
    }},
    { "sudden plan change", new List<string> {
        "You can reschedule as per company policy."
    }},
    { "preferred time slot", new List<string> {
        "Yes, based on availability."
    }},
    { "no slot available", new List<string> {
        "You can choose another date or contact support."
    }},
    { "advance payment", new List<string> {
        "Yes, advance payment is required."
    }},
    { "booking confirmation", new List<string> {
        "Yes, booking confirmation is provided."
    }},
    { "urgent same day shifting", new List<string> {
        "Subject to availability."
    }},
    { "track booking", new List<string> {
        "Yes, booking tracking is available."
    }},

    // Q21 – Q30
    { "packing materials", new List<string> {
        "Yes, cartons, bubble wrap, and protective materials are provided."
    }},
    { "fragile items packing", new List<string> {
        "Yes, fragile items are packed separately."
    }},
    { "electronics packing", new List<string> {
        "Yes, with proper safety packing."
    }},
    { "self packing allowed", new List<string> {
        "Yes, but items will be checked by ground staff as per company policy."
    }},
    { "trained packers", new List<string> {
        "Yes, packers are trained professionals."
    }},
    { "insurance available", new List<string> {
        "Yes, insurance is optional but recommended."
    }},
    { "gas cylinder transport", new List<string> {
        "Only empty gas cylinders are transported."
    }},
    { "filled gas cylinder", new List<string> {
        "No, only empty cylinders are allowed."
    }},
    { "flammable items", new List<string> {
        "No, petrol, diesel, or flammable items are not transported."
    }},
    { "pets transport", new List<string> {
        "No, pets are not transported."
    }},

    // Q31 – Q40
    { "cash jewellery documents", new List<string> {
        "No, customers should carry them personally."
    }},
    { "furniture dismantling", new List<string> {
        "Basic dismantling may be included. Complex dismantling is chargeable."
    }},
    { "bed dismantling", new List<string> {
        "Yes, if complex dismantling is required."
    }},
    { "wardrobe dismantling", new List<string> {
        "No, it is chargeable."
    }},
    { "modular furniture dismantling", new List<string> {
        "No, extra charges apply."
    }},
    { "ac dismantling", new List<string> {
        "No, it is chargeable."
    }},
    { "ac installation", new List<string> {
        "No, installation is chargeable."
    }},
    { "extra charges reason", new List<string> {
        "Because third-party electricians or carpenters are used."
    }},
    { "electrician carpenter service", new List<string> {
        "Yes, third-party technicians can be arranged at extra cost."
    }},
    { "approval for extra charges", new List<string> {
        "Yes, customer approval is taken before proceeding."
    }},

    // Q41 – Q50
    { "own electrician", new List<string> {
        "Yes, you can arrange your own electrician or carpenter."
    }},
    { "third party refund", new List<string> {
        "No, once the service is used."
    }},
    { "charges on invoice", new List<string> {
        "Yes, all charges are clearly mentioned."
    }},
    { "price calculation", new List<string> {
        "Based on distance, volume, services, and requirements."
    }},
    { "hidden charges", new List<string> {
        "No, there are no hidden charges."
    }},
    { "gst applicable", new List<string> {
        "Yes, GST is applicable."
    }},
    { "payment methods", new List<string> {
        "UPI, debit cards, credit cards, and online payments are accepted."
    }},
    { "invoice provided", new List<string> {
        "Yes, an invoice is provided."
    }},
    { "item damaged", new List<string> {
        "Report immediately; support will assist."
    }},
    { "feedback after delivery", new List<string> {
        "Yes, feedback can be given after delivery."
    }},

    // Q51 – Q75 (booking, slots, packing, safety)
    { "change drop location", new List<string> {
        "Case-specific and chargeable."
    }},
    { "pickup floor change", new List<string> {
        "Yes, as per company policy."
    }},
    { "lift information", new List<string> {
        "Yes, lift information is important."
    }},
    { "lift not working", new List<string> {
        "Additional charges may apply."
    }},
    { "multiple services", new List<string> {
        "Yes, you can book multiple services together."
    }},
    { "weather impact", new List<string> {
        "Usually no, unless extreme conditions."
    }},
    { "mobile booking", new List<string> {
        "Yes, booking from mobile is supported."
    }},
    { "whatsapp booking", new List<string> {
        "Support can assist via WhatsApp."
    }},
    { "moving reminders", new List<string> {
        "Yes, reminders are provided."
    }},
    { "supervisor contact", new List<string> {
        "Yes, you can speak to the supervisor."
    }},
    { "cartons included", new List<string> {
        "Yes, cartons are included."
    }},
    { "tv packing", new List<string> {
        "With bubble wrap and protective packing."
    }},
    { "fridge packing", new List<string> {
        "With safety covers and padding."
    }},
    { "trained movers", new List<string> {
        "Yes, movers are trained."
    }},
    { "items safety", new List<string> {
        "Yes, item safety is prioritized."
    }},

    // Q76 – Q100 (insurance & restricted items)
    { "item breakage", new List<string> {
        "Insurance and support will assist."
    }},
    { "insurance cost", new List<string> {
        "Depends on declared value."
    }},
    { "declare expensive items", new List<string> {
        "Yes, declaration is required."
    }},
    { "gold cash transport", new List<string> {
        "No, gold or cash cannot be transported."
    }},
    { "documents transport", new List<string> {
        "Carry them personally."
    }},
    { "perishable food", new List<string> {
        "Not recommended."
    }},
    { "plants move", new List<string> {
        "Yes, plants can be moved."
    }},
    { "liquids move", new List<string> {
        "Limited quantities only."
    }},
    { "paint move", new List<string> {
        "No."
    }},
    { "batteries move", new List<string> {
        "Small household batteries only."
    }},
    { "overloading", new List<string> {
        "No, overloading is not allowed."
    }},
    { "safe routes", new List<string> {
        "Yes, routes are planned safely."
    }},

    // Q101 – Q135 (electrician, dismantling, policy)
    { "electrician charges", new List<string> {
        "They are added separately."
    }},
    { "geyser dismantling", new List<string> {
        "No, it is not included."
    }},
    { "fan removal", new List<string> {
        "No, it is chargeable."
    }},
    { "light fitting removal", new List<string> {
        "No, it is chargeable."
    }},
    { "reassembly charges", new List<string> {
        "Yes, if technician is required."
    }},
    { "gst on electrician", new List<string> {
        "As per partner policy."
    }},
    { "refuse dismantling", new List<string> {
        "Yes, contact support."
    }},
    { "dismantling delay", new List<string> {
        "It may add time."
    }},
    { "third party optional", new List<string> {
        "Yes, it is optional."
    }},
    { "already dismantled furniture", new List<string> {
        "Yes, you can keep it dismantled."
    }},
    { "dismantle without permission", new List<string> {
        "No, permission is always taken."
    }},
    { "tools provided", new List<string> {
        "Only basic tools are provided."
    }},
    { "wall drilling", new List<string> {
        "No."
    }},
    { "false ceiling", new List<string> {
        "No, it is not included."
    }},
    { "skip electrical work", new List<string> {
        "Yes."
    }},
    { "ac gas safety", new List<string> {
        "Yes, handled safely by technicians."
    }},
    { "electrician complaint", new List<string> {
        "Yes, complaints can be raised."
    }},
    { "electrician guarantee", new List<string> {
        "As per third-party terms."
    }}
};


        }

        public async Task<string> GetAnswer(string question, string language = "en")
        {
            // 1. Try to find exact match in FAQ database
            var answer = FindInFAQ(question);

            if (!string.IsNullOrEmpty(answer))
            {
                // Translate if needed
                if (language != "en")
                {
                    answer = await TranslateText(answer, language);
                }
                return answer;
            }

            // 2. Use AI to search and answer from PDF context
            return await GetAIAnswer(question, language);
        }

        private string FindInFAQ(string question)
        {
            var normalizedQuestion = question.ToLower().Trim();

            foreach (var key in _faqDatabase.Keys)
            {
                if (normalizedQuestion.Contains(key))
                {
                    return _faqDatabase[key].First();
                }
            }

            return null;
        }

        private async Task<string> GetAIAnswer(string question, string language)
        {
            var apiKey = _config["GenAI:ApiKey"];
            var model = _config["GenAI:Model"];

            // Create context from PDF content
            var context = CreateContextFromPDF();

            var prompt = $$"""
You are PackYatra customer support assistant.
Answer the question based on this context only.
If answer is not in context, say "I don't have information about that in my knowledge base."

Context from PackYatra FAQ:
{{context}}

Question in {{GetLanguageName(language)}}:
"{{question}}"

Provide answer in {{GetLanguageName(language)}} language.
""";

            var body = new
            {
                contents = new[]
                {
                    new {
                        role = "user",
                        parts = new[] { new { text = prompt } }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.2,
                    maxOutputTokens = 500
                }
            };

            var req = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}"
            );

            req.Content = new StringContent(
                JsonConvert.SerializeObject(body),
                Encoding.UTF8,
                "application/json"
            );

            var res = await _http.SendAsync(req);
            var json = await res.Content.ReadAsStringAsync();

            var answer = JObject.Parse(json)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

            return answer?.Trim() ?? "I couldn't find an answer. Please contact support.";
        }

        private string CreateContextFromPDF()
        {
            // This would be your PDF content - you can store it in appsettings or database
            return @"
            PackYatra is a relocation and logistics platform that helps people move homes, offices, and vehicles safely.
            Services: Packing, moving, transportation, and relocation support.
            Booking: Fill the booking form on the PackYatra website.
            Packing materials: Cartons, bubble wrap, and protective materials are provided.
            Insurance: Yes, insurance is optional but recommended.
            Dismantling: Basic dismantling may be included. Complex dismantling is chargeable.
            Payment methods: UPI, cards, and online payments.
            Gas cylinders: Only empty cylinders are transported.
            Pets: No, pets cannot be transported.
            // ... include all PDF content
            ";
        }

        private async Task<string> TranslateText(string text, string targetLanguage)
        {
            var apiKey = _config["GenAI:ApiKey"];
            var model = _config["GenAI:Model"];

            var prompt = $"Translate this to {GetLanguageName(targetLanguage)}: {text}";

            var body = new
            {
                contents = new[]
                {
                    new {
                        role = "user",
                        parts = new[] { new { text = prompt } }
                    }
                }
            };

            var req = new HttpRequestMessage(
                HttpMethod.Post,
                $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}"
            );

            req.Content = new StringContent(
                JsonConvert.SerializeObject(body),
                Encoding.UTF8,
                "application/json"
            );

            var res = await _http.SendAsync(req);
            var json = await res.Content.ReadAsStringAsync();

            return JObject.Parse(json)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString() ?? text;
        }

        private string GetLanguageName(string code)
        {
            return code switch
            {
                "en" => "English",
                "hi" => "Hindi",
                "kn" => "Kannada",
                "te" => "Telugu",
                "ta" => "Tamil",
                "ml" => "Malayalam",
                "bn" => "Bengali",
                "gu" => "Gujarati",
                "mr" => "Marathi",
                "bho" => "Bhojpuri",
                "mai" => "Maithili",
                "or" => "Odia",
                "ur" => "Urdu",
                _ => "English"
            };
        }
    }
}