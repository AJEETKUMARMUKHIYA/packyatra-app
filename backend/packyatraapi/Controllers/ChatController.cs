using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services;

namespace MoversAndPackerApi.Controllers
{
    [ApiController]
    [Route("api/chat")]
    public class ChatController : ControllerBase
    {
        private readonly GenAIService _genAI;
        private readonly PricingService _pricing;
        private readonly FAQService _faqService;

        public ChatController(GenAIService genAI, PricingService pricing, FAQService faqService)
        {
            _genAI = genAI;
            _pricing = pricing;
            _faqService = faqService;
        }

        [HttpPost]
        public async Task<IActionResult> Chat(ChatRequest req)
        {
            // Auto-detect language if not specified
            if (string.IsNullOrEmpty(req.Language) || req.Language == "auto")
            {
                req.Language = await _genAI.DetectLanguage(req.Message);
            }

            // Auto-detect mode if not specified
            var mode = req.Mode.ToLower();
            if (mode == "auto")
            {
                mode = await _genAI.DetectMode(req.Message, req.Language);
            }

            // Handle based on mode
            if (mode == "price")
            {
                return await HandlePriceQuery(req.Message, req.Language);
            }
            else if (mode == "faq")
            {
                return await HandleFAQQuery(req.Message, req.Language);
            }
            else
            {
                // Auto mode - let AI decide
                return await HandleAutoQuery(req);
            }
        }

        private async Task<IActionResult> HandlePriceQuery(string message, string language)
        {
            var intent = await _genAI.ExtractIntent(message, language);

            if (intent.Intent == "PRICE" && intent.Distance.HasValue && intent.CFT.HasValue)
            {
                var price = await _pricing.CalculatePrice(Convert.ToInt32( intent.Distance.Value),Convert.ToInt32( intent.CFT.Value));

                var responseText = await _genAI.TranslateResponse(
                    $"For {intent.Distance} KM and {intent.CFT} CFT, estimated moving cost is ₹{price:N0}",
                    language
                );

                return Ok(new ChatResponse
                {
                    Message = responseText,
                    Language = language,
                    Mode = "price"
                });
            }
            else
            {
                var prompt = await _genAI.GetLanguageResponse("price_prompt", language);
                return Ok(new ChatResponse
                {
                    Message = prompt,
                    Language = language,
                    Mode = "price"
                });
            }
        }

        private async Task<IActionResult> HandleFAQQuery(string message, string language)
        {
            var answer = await _faqService.GetAnswer(message, language);

            return Ok(new ChatResponse
            {
                Message = answer,
                Language = language,
                Mode = "faq"
            });
        }

        private async Task<IActionResult> HandleAutoQuery(ChatRequest req)
        {
            var intent = await _genAI.ExtractIntent(req.Message, req.Language);

            if (intent.Intent == "GREETING")
            {
                var greeting = await _genAI.GetLanguageResponse("greeting", req.Language);
                return Ok(new ChatResponse
                {
                    Message = greeting,
                    Language = req.Language,
                    Mode = "auto"
                });
            }
            else if (intent.Intent == "PRICE")
            {
                return await HandlePriceQuery(req.Message, req.Language);
            }
            else if (intent.Intent == "FAQ")
            {
                return await HandleFAQQuery(req.Message, req.Language);
            }
            else
            {
                var unknown = await _genAI.GetLanguageResponse("unknown", req.Language);
                return Ok(new ChatResponse
                {
                    Message = unknown,
                    Language = req.Language,
                    Mode = "auto"
                });
            }
        }

        // New endpoint to explicitly choose mode
        [HttpPost("mode")]
        public async Task<IActionResult> SetChatMode([FromBody] ModeRequest request)
        {
            if (request.Mode == "price")
            {
                var prompt = await _genAI.GetLanguageResponse("price_mode_prompt", request.Language);
                return Ok(new ChatResponse
                {
                    Message = prompt,
                    Language = request.Language,
                    Mode = "price"
                });
            }
            else if (request.Mode == "faq")
            {
                var prompt = await _genAI.GetLanguageResponse("faq_mode_prompt", request.Language);
                return Ok(new ChatResponse
                {
                    Message = prompt,
                    Language = request.Language,
                    Mode = "faq"
                });
            }

            return BadRequest("Invalid mode. Use 'price' or 'faq'.");
        }
    }

    public class ModeRequest
    {
        public string Mode { get; set; }
        public string Language { get; set; } = "en";
    }
}