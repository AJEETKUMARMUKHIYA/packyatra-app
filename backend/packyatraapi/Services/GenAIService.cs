using MoversAndPackerApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MoversAndPackerApi.Services
{
    public class GenAIService
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public GenAIService(IConfiguration config)
        {
            _config = config;
            _http = new HttpClient();
        }

        public async Task<GenAIIntent> ExtractIntent(string userMessage, string language = "en")
        {
            var apiKey = _config["GenAI:ApiKey"];
            var model = _config["GenAI:Model"];

            var prompt = $$"""
You are a movers and packers assistant.
Analyze the user message and detect intent.
User speaks {{GetLanguageName(language)}}.

Return ONLY valid JSON. No markdown. No explanation.

User message:
"{{userMessage}}"

JSON:
{
  "intent": "PRICE | FAQ | GREETING | UNKNOWN",
  "distance": number or null,
  "cft": number or null,
  "language": "{{language}}"
}
""";

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

            var rawText = JObject.Parse(json)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
            var cleanJson = CleanJson(rawText);

            return JsonConvert.DeserializeObject<GenAIIntent>(cleanJson);
        }

        public async Task<string> DetectLanguage(string text)
        {
            var apiKey = _config["GenAI:ApiKey"];
            var model = _config["GenAI:Model"];

            var prompt = $$"""
Detect the language of this text. Return ONLY the language code.

Options: en (English), hi (Hindi), kn (Kannada), te (Telugu), ta (Tamil), ml (Malayalam)

Text: "{{text}}"

Language code:
""";

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

            var language = JObject.Parse(json)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString()?.Trim();

            return language ?? "en";
        }

        public async Task<string> DetectMode(string message, string language)
        {
            var apiKey = _config["GenAI:ApiKey"];
            var model = _config["GenAI:Model"];

            var prompt = $$"""
Is the user asking for price calculation or general FAQ?
User speaks {{GetLanguageName(language)}}.

Message: "{{message}}"

Return ONLY: "price" or "faq"
""";

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

            var mode = JObject.Parse(json)["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString()?.Trim()?.ToLower();

            return mode ?? "faq";
        }

        public async Task<string> TranslateResponse(string text, string targetLanguage)
        {
            if (targetLanguage == "en") return text;

            var apiKey = _config["GenAI:ApiKey"];
            var model = _config["GenAI:Model"];

            var prompt = $"Translate to {GetLanguageName(targetLanguage)}: {text}";

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
        //    public async Task<string> GetLanguageResponse(string key, string language)
        //    {
        //        var responses = new Dictionary<string, Dictionary<string, string>>
        //{
        //    {
        //        "greeting", new Dictionary<string, string>
        //        {
        //            { "en", "Hi 👋 I can help you calculate moving prices or answer questions about PackYatra services. Choose: 1️⃣ Price Calculator 2️⃣ FAQ Questions" },
        //            { "hi", "नमस्ते 👋 मैं आपको मूविंग कीमतों की गणना करने या PackYatra सेवाओं के बारे में प्रश्नों का उत्तर देने में मदद कर सकता हूं। चुनें: 1️⃣ मूल्य कैलकुलेटर 2️⃣ सामान्य प्रश्न" },
        //            { "kn", "ನಮಸ್ಕಾರ 👋 ನಾನು ನಿಮಗೆ ಮೂವಿಂಗ್ ಬೆಲೆಗಳನ್ನು ಲೆಕ್ಕಾಚಾರ ಮಾಡಲು ಅಥವಾ PackYatra ಸೇವೆಗಳ ಕುರಿತು ಪ್ರಶ್ನೆಗಳಿಗೆ ಉತ್ತರಿಸಲು ಸಹಾಯ ಮಾಡಬಲ್ಲೆ. ಆಯ್ಕೆಮಾಡಿ: 1️⃣ ಬೆಲೆ ಕ್ಯಾಲ್ಕುಲೇಟರ್ 2️⃣ ಸಾಮಾನ್ಯ ಪ್ರಶ್ನೆಗಳು" },
        //            { "te", "నమస్తే 👋 నేను మీకు మూవింగ్ ధరలను లెక్కించడంలో లేదా PackYatra సేవల గురించి ప్రశ్నలకు సమాధానం ఇవ్వడంలో సహాయపడగలను. ఎంచుకోండి: 1️⃣ ధర కాలిక్యులేటర్ 2️⃣ సామాన್ಯ ప్రశ్నలు" },
        //            { "ta", "வணக்கம் 👋 நான் உங்களுக்கு நகர்வு விலைகளை கணக்கிட அல்லது PackYatra சேவைகள் பற்றிய கேள்விகளுக்கு பதிலளிக்க உதவலாம். தேர்ந்தெடுக்கவும்: 1️⃣ விலை கால்குலேட்டர் 2️⃣ பொது கேள்விகள்" },
        //            { "ml", "ഹലോ 👋 നിങ്ങൾക്ക് മൂവിംഗ് വിലകൾ കണക്കാക്കുന്നതിനോ PackYatra സേവനങ്ങളെക്കുറിച്ചുള്ള ചോദ്യങ്ങൾക്ക് ഉത്തരം നൽകുന്നതിനോ എനിക്ക് സഹായിക്കാനാകും. തിരഞ്ഞെടുക്കുക: 1️⃣ വില കാൽക്കുലേറ്റർ 2️⃣ പൊതു ചോദ്യങ്ങൾ" }
        //        }
        //    },
        //    {
        //        "price_prompt", new Dictionary<string, string>
        //        {
        //            { "en", "Please provide distance in KM and volume in CFT. Example: 'Price for 350 km and 450 cft'" },
        //            { "hi", "कृपया दूरी KM में और आयतन CFT में दें। उदाहरण: '350 किमी और 450 सीएफटी के लिए कीमत'" },
        //            { "kn", "ದಯವಿಟ್ಟು ದೂರವನ್ನು KM ಮತ್ತು ಪರಿಮಾಣವನ್ನು CFT ನಲ್ಲಿ ನೀಡಿ. ಉದಾಹರಣೆ: '350 ಕಿಮೀ ಮತ್ತು 450 ಸಿಎಫ್ಟಿಗೆ ಬೆಲೆ'" },
        //            { "te", "దయచేసి దూరాన్ని KM మరియు వాల్యూమ్ CFT లో ఇవ్వండి. ఉదాహరణ: '350 కి.మీ మరియు 450 సిఎఫ్టి ధర'" },
        //            { "ta", "தொலைவை KM இல் மற்றும் தொகுதியை CFT இல் கொடுங்கள். உதாரணம்: '350 கிமீ மற்றும் 450 சிஎஃப்டிக்கான விலை'" },
        //            { "ml", "ദൂരം KM-ൽ വോളിയം CFT-ൽ നൽകുക. ഉദാഹരണം: '350 കിലോമീറ്ററിനും 450 സിഎഫ്ടിക്കുമുള്ള വില'" }
        //        }
        //    },
        //    {
        //        "price_mode_prompt", new Dictionary<string, string>  // ADD THIS
        //        {
        //            { "en", "Switched to Price Calculator mode! Please provide distance in KM and volume in CFT." },
        //            { "hi", "मूल्य कैलकुलेटर मोड में बदला! कृपया दूरी KM में और आयतन CFT में दें।" },
        //            { "kn", "ಬೆಲೆ ಕ್ಯಾಲ್ಕುಲೇಟರ್ ಮೋಡ್ಗೆ ಬದಲಾಯಿತು! ದಯವಿಟ್ಟು ದೂರವನ್ನು KM ಮತ್ತು ಪರಿಮಾಣವನ್ನು CFT ನಲ್ಲಿ ನೀಡಿ." },
        //            { "te", "ధర కాలిక్యులేటర్ మోడ్కి మార్చబడింది! దయచేసి దూరాన్ని KM మరియు వాల్యూమ్ CFT లో ఇవ్వండి." },
        //            { "ta", "விலை கால்குலேட்டர் பயன்முறைக்கு மாற்றப்பட்டது! தொலைவை KM இல் மற்றும் தொகுதியை CFT இல் கொடுங்கள்." },
        //            { "ml", "വില കാൽക്കുലേറ്റർ മോഡിലേക്ക് മാറ്റി! ദൂരം KM-ൽ വോളിയം CFT-ൽ നൽകുക." }
        //        }
        //    },
        //    {
        //        "faq_mode_prompt", new Dictionary<string, string>  // ADD THIS
        //        {
        //            { "en", "Switched to FAQ Assistant mode! Ask me anything about PackYatra services." },
        //            { "hi", "एफएक्यू असिस्टेंट मोड में बदला! PackYatra सेवाओं के बारे में कुछ भी पूछें।" },
        //            { "kn", "FAQ ಸಹಾಯಕ ಮೋಡ್ಗೆ ಬದಲಾಯಿತು! PackYatra ಸೇವೆಗಳ ಕುರಿತು ಏನಾದರೂ ಕೇಳಿ." },
        //            { "te", "FAQ అసిస్టెంట్ మోడ్కి మార్చబడింది! PackYatra సేవల గురించి ఏదైనా అడగండి." },
        //            { "ta", "FAQ உதவியாளர் பயன்முறைக்கு மாற்றப்பட்டது! PackYatra சேவைகள் பற்றி எதையும் கேளுங்கள்." },
        //            { "ml", "FAQ അസിസ്റ്റന്റ് മോഡിലേക്ക് മാറ്റി! PackYatra സേവനങ്ങളെക്കുറിച്ച് എന്തും ചോദിക്കുക." }
        //        }
        //    },
        //    {
        //        "unknown", new Dictionary<string, string>
        //        {
        //            { "en", "I didn't understand that. Please ask about moving prices or PackYatra services." },
        //            { "hi", "मैं समझ नहीं पाया। कृपया मूविंग कीमतों या PackYatra सेवाओं के बारे में पूछें।" },
        //            { "kn", "ನಾನು ಅದನ್ನು ಅರ್ಥಮಾಡಿಕೊಂಡಿಲ್ಲ. ದಯವಿಟ್ಟು ಮೂವಿಂಗ್ ಬೆಲೆಗಳು ಅಥವಾ PackYatra ಸೇವೆಗಳ ಬಗ್ಗೆ ಕೇಳಿ." },
        //            { "te", "నేను దానిని అర్థం చేసుకోలేదు. దయచేసి మూవింగ్ ధరలు లేదా PackYatra సేవల గురించి అడగండి." },
        //            { "ta", "நான் அதைப் புரிந்து கொள்ளவில்லை. நகரும் விலைகள் அல்லது PackYatra சேவைகள் பற்றி கேளுங்கள்." },
        //            { "ml", "ഞാൻ അത് മനസ്സിലാക്കിയില്ല. മൂവിംഗ് വിലകളോ PackYatra സേവനങ്ങളോ ചോദിക്കുക." }
        //        }
        //    }
        //};

        //        if (responses.ContainsKey(key) && responses[key].ContainsKey(language))
        //        {
        //            return responses[key][language];
        //        }

        //        return responses[key]["en"]; // Fallback to English
        //    }
        //public async Task<string> GetLanguageResponse(string key, string language)
        //{
        //    var responses = new Dictionary<string, Dictionary<string, string>>
        //    {
        //        {
        //            "greeting", new Dictionary<string, string>
        //            {
        //                { "en", "Hi 👋 I can help you calculate moving prices or answer questions about PackYatra services. Choose: 1️⃣ Price Calculator 2️⃣ FAQ Questions" },
        //                { "hi", "नमस्ते 👋 मैं आपको मूविंग कीमतों की गणना करने या PackYatra सेवाओं के बारे में प्रश्नों का उत्तर देने में मदद कर सकता हूं। चुनें: 1️⃣ मूल्य कैलकुलेटर 2️⃣ सामान्य प्रश्न" },
        //                { "kn", "ನಮಸ್ಕಾರ 👋 ನಾನು ನಿಮಗೆ ಮೂವಿಂಗ್ ಬೆಲೆಗಳನ್ನು ಲೆಕ್ಕಾಚಾರ ಮಾಡಲು ಅಥವಾ PackYatra ಸೇವೆಗಳ ಕುರಿತು ಪ್ರಶ್ನೆಗಳಿಗೆ ಉತ್ತರಿಸಲು ಸಹಾಯ ಮಾಡಬಲ್ಲೆ. ಆಯ್ಕೆಮಾಡಿ: 1️⃣ ಬೆಲೆ ಕ್ಯಾಲ್ಕುಲೇಟರ್ 2️⃣ ಸಾಮಾನ್ಯ ಪ್ರಶ್ನೆಗಳು" },
        //                { "te", "నమస్తే 👋 నేను మీకు మూవింగ్ ధరలను లెక్కించడంలో లేదా PackYatra సేవల గురించి ప్రశ్నలకు సమాధానం ఇవ్వడంలో సహాయపడగలను. ఎంచుకోండి: 1️⃣ ధర కాలిక్యులేటర్ 2️⃣ సాధారణ ప్రశ్నలు" },
        //                { "ta", "வணக்கம் 👋 நான் உங்களுக்கு நகர்வு விலைகளை கணக்கிட அல்லது PackYatra சேவைகள் பற்றிய கேள்விகளுக்கு பதிலளிக்க உதவலாம். தேர்ந்தெடுக்கவும்: 1️⃣ விலை கால்குலேட்டர் 2️⃣ பொது கேள்விகள்" },
        //                { "ml", "ഹലോ 👋 നിങ്ങൾക്ക് മൂവിംഗ് വിലകൾ കണക്കാക്കുന്നതിനോ PackYatra സേവനങ്ങളെക്കുറിച്ചുള്ള ചോദ്യങ്ങൾക്ക് ഉത്തരം നൽകുന്നതിനോ എനിക്ക് സഹായിക്കാനാകും. തിരഞ്ഞെടുക്കുക: 1️⃣ വില കാൽക്കുലേറ്റർ 2️⃣ പൊതു ചോദ്യങ്ങൾ" }
        //            }
        //        },
        //        {
        //            "price_prompt", new Dictionary<string, string>
        //            {
        //                { "en", "Please provide distance in KM and volume in CFT. Example: 'Price for 350 km and 450 cft'" },
        //                { "hi", "कृपया दूरी KM में और आयतन CFT में दें। उदाहरण: '350 किमी और 450 सीएफटी के लिए कीमत'" },
        //                // Add translations for all languages...
        //            }
        //        }
        //    };

        //    if (responses.ContainsKey(key) && responses[key].ContainsKey(language))
        //    {
        //        return responses[key][language];
        //    }

        //    return responses[key]["en"]; // Fallback to English
        //}
        public async Task<string> GetLanguageResponse(string key, string language)
        {
            var responses = new Dictionary<string, Dictionary<string, string>>
    {
        {
            "greeting", new Dictionary<string, string>
            {
                { "en", "Hi 👋 I can help you calculate moving prices or answer questions about PackYatra services. Choose: 1️⃣ Price Calculator 2️⃣ FAQ Questions" },
                { "hi", "नमस्ते 👋 मैं आपको मूविंग कीमतों की गणना करने या PackYatra सेवाओं के बारे में प्रश्नों का उत्तर देने में मदद कर सकता हूं। चुनें: 1️⃣ मूल्य कैलकुलेटर 2️⃣ सामान्य प्रश्न" },
                { "kn", "ನಮಸ್ಕಾರ 👋 ನಾನು ನಿಮಗೆ ಮೂವಿಂಗ್ ಬೆಲೆಗಳನ್ನು ಲೆಕ್ಕಾಚಾರ ಮಾಡಲು ಅಥವಾ PackYatra ಸೇವೆಗಳ ಕುರಿತು ಪ್ರಶ್ನೆಗಳಿಗೆ ಉತ್ತರಿಸಲು ಸಹಾಯ ಮಾಡಬಲ್ಲೆ. ಆಯ್ಕೆಮಾಡಿ: 1️⃣ ಬೆಲೆ ಕ್ಯಾಲ್ಕುಲೇಟರ್ 2️⃣ ಸಾಮಾನ್ಯ ಪ್ರಶ್ನೆಗಳು" },
                { "te", "నమస్తే 👋 నేను మీకు మూవింగ్ ధరలను లెక్కించడంలో లేదా PackYatra సేవల గురించి ప్రశ్నలకు సమాధానం ఇవ్వడంలో సహాయపడగలను. ఎంచుకోండి: 1️⃣ ధర కాలిక్యులేటర్ 2️⃣ సామాన్య ప్రశ్నలు" },
                { "ta", "வணக்கம் 👋 நான் உங்களுக்கு நகர்வு விலைகளை கணக்கிட அல்லது PackYatra சேவைகள் பற்றிய கேள்விகளுக்கு பதிலளிக்க உதவலாம். தேர்ந்தெடுக்கவும்: 1️⃣ விலை கால்குலேட்டர் 2️⃣ பொது கேள்விகள்" },
                { "ml", "ഹലോ 👋 നിങ്ങൾക്ക് മൂവിംഗ് വിലകൾ കണക്കാക്കുന്നതിനോ PackYatra സേവനങ്ങളെക്കുറിച്ചുള്ള ചോദ്യങ്ങൾക്ക് ഉത്തരം നൽകുന്നതിനോ എനിക്ക് സഹായിക്കാനാകും. തിരഞ്ഞെടുക്കുക: 1️⃣ വില കാൽക്കുലേറ്റർ 2️⃣ പൊതു ചോദ്യങ്ങൾ" },
                { "bn", "নমস্কার 👋 আমি আপনাকে মুভিং খরচ হিসাব করতে বা PackYatra পরিষেবা সম্পর্কে প্রশ্নের উত্তর দিতে সাহায্য করতে পারি। বেছে নিন: 1️⃣ প্রাইস ক্যালকুলেটর 2️⃣ FAQ প্রশ্ন" },
                { "gu", "નમસ્તે 👋 હું તમને મૂવિંગ ખર્ચ ગણતરી કરવા અથવા PackYatra સેવાઓ વિશે પ્રશ્નોના જવાબ આપવા મદદ કરી શકું છું. પસંદ કરો: 1️⃣ પ્રાઈસ કેલ્ક્યુલેટર 2️⃣ FAQ પ્રશ્નો" },
                { "mr", "नमस्कार 👋 मी तुम्हाला मूव्हिंग खर्चाची गणना करण्यात किंवा PackYatra सेवांबद्दल प्रश्नांची उत्तरे देण्यात मदत करू शकतो. निवडा: 1️⃣ किंमत कॅल्क्युलेटर 2️⃣ FAQ प्रश्न" },
                { "bho", "नमस्कार 👋 हम राउर मूविंग के दाम निकाले में या PackYatra सेवा के सवालन के जवाब दे सकतानी। चुनीं: 1️⃣ प्राइस कैलकुलेटर 2️⃣ FAQ सवाल" },
                { "mai", "नमस्कार 👋 हम अहाँ के मूविंग लागत गणना या PackYatra सेवा सँ जुड़ल प्रश्न के जवाब दे सकैत छी। चुनू: 1️⃣ प्राइस कैलकुलेटर 2️⃣ FAQ प्रश्न" },
                { "or",  "ନମସ୍କାର 👋 ମୁଁ ଆପଣଙ୍କୁ ମୁଭିଂ ଖର୍ଚ୍ଚ ଗଣନା କିମ୍ବା PackYatra ସେବା ବିଷୟରେ ପ୍ରଶ୍ନର ଉତ୍ତର ଦେବାରେ ସାହାଯ୍ୟ କରିପାରିବି। ବାଛନ୍ତୁ: 1️⃣ ପ୍ରାଇସ୍ କ୍ୟାଲକୁଲେଟର 2️⃣ FAQ ପ୍ରଶ୍ନ" },
                { "ur",  "سلام 👋 میں آپ کو موونگ کی قیمت کا حساب لگانے یا PackYatra سروسز سے متعلق سوالات کے جواب دینے میں مدد کر سکتا ہوں۔ منتخب کریں: 1️⃣ پرائس کیلکولیٹر 2️⃣ FAQ سوالات" }
            }
        },
        {
            "price_prompt", new Dictionary<string, string>
            {
                { "en", "Please provide distance in KM and volume in CFT." },
                { "hi", "कृपया दूरी KM में और आयतन CFT में दें।" },
                { "kn", "ದಯವಿಟ್ಟು ದೂರವನ್ನು KM ಮತ್ತು ಪರಿಮಾಣವನ್ನು CFT ನಲ್ಲಿ ನೀಡಿ." },
                { "te", "దయచేసి దూరాన్ని KM మరియు వాల్యూమ్ CFT లో ఇవ్వండి." },
                { "ta", "தொலைவை KM இல் மற்றும் தொகுதியை CFT இல் கொடுங்கள்." },
                { "ml", "ദൂരം KM-ൽ വോളിയം CFT-ൽ നൽകുക." },
                { "bn", "অনুগ্রহ করে দূরত্ব KM এ এবং ভলিউম CFT এ দিন।" },
                { "gu", "કૃપા કરીને અંતર KM માં અને વોલ્યુમ CFT માં આપો." },
                { "mr", "कृपया अंतर KM मध्ये आणि व्हॉल्युम CFT मध्ये द्या." },
                { "bho", "कृपया दूरी KM में आ सामान CFT में दीं।" },
                { "mai", "कृपया दूरी KM मे आ वॉल्यूम CFT मे दिअ।" },
                { "or",  "ଦୟାକରି ଦୂରତା KM ଓ ଭଲ୍ୟୁମ CFT ରେ ଦିଅନ୍ତୁ।" },
                { "ur",  "براہ کرم فاصلہ KM میں اور حجم CFT میں فراہم کریں۔" }
            }
        },
        {
            "price_mode_prompt", new Dictionary<string, string>
            {
                { "en", "Switched to Price Calculator mode!" },
                { "hi", "मूल्य कैलकुलेटर मोड में बदला!" },
                { "kn", "ಬೆಲೆ ಕ್ಯಾಲ್ಕುಲೇಟರ್ ಮೋಡ್ಗೆ ಬದಲಾಯಿತು!" },
                { "te", "ధర కాలిక్యులేటర్ మోడ్కి మార్చబడింది!" },
                { "ta", "விலை கால்குலேட்டர் பயன்முறைக்கு மாற்றப்பட்டது!" },
                { "ml", "വില കാൽക്കുലേറ്റർ മോഡിലേക്ക് മാറ്റി!" },
                { "bn", "প্রাইস ক্যালকুলেটর মোডে পরিবর্তন হয়েছে!" },
                { "gu", "પ્રાઈસ કેલ્ક્યુલેટર મોડમાં બદલાયું!" },
                { "mr", "किंमत कॅल्क्युलेटर मोडमध्ये बदलले!" },
                { "bho", "प्राइस कैलकुलेटर मोड में बदल गइल!" },
                { "mai", "प्राइस कैलकुलेटर मोड मे बदल गेल!" },
                { "or",  "ପ୍ରାଇସ୍ କ୍ୟାଲକୁଲେଟର ମୋଡକୁ ବଦଳାଗଲା!" },
                { "ur",  "پرائس کیلکولیٹر موڈ میں تبدیل ہو گیا!" }
            }
        },
        {
            "faq_mode_prompt", new Dictionary<string, string>
            {
                { "en", "Switched to FAQ Assistant mode!" },
                { "hi", "FAQ असिस्टेंट मोड में बदला!" },
                { "kn", "FAQ ಸಹಾಯಕ ಮೋಡ್ಗೆ ಬದಲಾಯಿತು!" },
                { "te", "FAQ అసిస్టెంట్ మోడ్కి మార్చబడింది!" },
                { "ta", "FAQ உதவியாளர் பயன்முறைக்கு மாற்றப்பட்டது!" },
                { "ml", "FAQ അസിസ്റ്റന്റ് മോഡിലേക്ക് മാറ്റി!" },
                { "bn", "FAQ সহকারী মোডে পরিবর্তন হয়েছে!" },
                { "gu", "FAQ સહાયક મોડમાં બદલાયું!" },
                { "mr", "FAQ सहाय्यक मोडमध्ये बदलले!" },
                { "bho", "FAQ सहायक मोड में बदल गइल!" },
                { "mai", "FAQ सहायक मोड मे बदल गेल!" },
                { "or",  "FAQ ସହାୟକ ମୋଡକୁ ବଦଳାଗଲା!" },
                { "ur",  "FAQ اسسٹنٹ موڈ میں تبدیل ہو گیا!" }
            }
        },
        {
            "unknown", new Dictionary<string, string>
            {
                { "en", "I didn't understand that." },
                { "hi", "मैं समझ नहीं पाया।" },
                { "kn", "ನಾನು ಅದನ್ನು ಅರ್ಥಮಾಡಿಕೊಂಡಿಲ್ಲ." },
                { "te", "నేను దానిని అర్థం చేసుకోలేదు." },
                { "ta", "நான் அதைப் புரிந்து கொள்ளவில்லை." },
                { "ml", "ഞാൻ അത് മനസ്സിലാക്കിയില്ല." },
                { "bn", "আমি বুঝতে পারিনি।" },
                { "gu", "હું સમજી શક્યો નહીં." },
                { "mr", "मला समजले नाही." },
                { "bho", "हम समझ ना पवनी।" },
                { "mai", "हम बुझि नहि सकलहुँ।" },
                { "or",  "ମୁଁ ବୁଝିପାରିଲି ନାହିଁ।" },
                { "ur",  "میں سمجھ نہیں سکا۔" }
            }
        }
    };

            if (!responses.TryGetValue(key, out var langMap))
                langMap = responses["unknown"];

            if (!langMap.TryGetValue(language, out var message))
                message = langMap["en"];

            return await Task.FromResult(message);
        }

        private static string CleanJson(string text)
        {
            if (string.IsNullOrEmpty(text)) return "{}";

            text = text.Trim();
            if (text.StartsWith("```"))
            {
                text = text
                    .Replace("```json", "")
                    .Replace("```", "")
                    .Trim();
            }
            return text;
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
                _ => "English"
            };
        }
    }
}