using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoversAndPackerApi.Data;
using MoversAndPackerApi.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MoversAndPackerApi.Controllers
{
    [ApiController]
    [Route("api/phonepe")]
    public class PhonePeWebhookController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly ILogger<PhonePeWebhookController> _logger;

        public PhonePeWebhookController(
            ApplicationDbContext db,
            IConfiguration config,
            ILogger<PhonePeWebhookController> logger)
        {
            _db = db;
            _config = config;
            _logger = logger;
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            // 1️⃣ Read raw body
            string body;
            using (var reader = new StreamReader(Request.Body))
            {
                body = await reader.ReadToEndAsync();
            }

            if (string.IsNullOrWhiteSpace(body))
                return BadRequest("Empty payload");

            // 2️⃣ Log raw payload
            _logger.LogInformation("PhonePe Webhook RAW Body: {body}", body);

            // 3️⃣ Log headers
            foreach (var header in Request.Headers)
                _logger.LogInformation("Header: {Key} = {Value}", header.Key, header.Value.ToString());

            // 4️⃣ Verify X-VERIFY checksum
            if (!Request.Headers.TryGetValue("X-VERIFY", out var receivedXVerify))
                return Unauthorized("X-VERIFY missing");

            var saltKey = _config["PhonePe:SaltKey"];
            var saltIndex = _config["PhonePe:SaltIndex"];
            var calculatedXVerify = GenerateChecksum(body, saltKey, saltIndex);

            if (!string.Equals(receivedXVerify, calculatedXVerify, StringComparison.Ordinal))
                return Unauthorized("Checksum mismatch");

            // 5️⃣ Parse JSON and get event type
            using var json = JsonDocument.Parse(body);
            var root = json.RootElement;
            var eventType = root.GetProperty("event").GetString();
            _logger.LogInformation("PhonePe Webhook Event: {eventType}", eventType);

            // 6️⃣ Handle events
            switch (eventType)
            {
                case "checkout.order.completed":
                case "checkout.order.failed":
                    {
                        var paymentData = root.GetProperty("payload").GetProperty("payment");
                        await ProcessPaymentAsync(paymentData);
                        break;
                    }
                case "pg.refund.completed":
                case "pg.refund.failed":
                    {
                        var refundData = root.GetProperty("payload").GetProperty("refund");
                        await ProcessRefundAsync(refundData);
                        break;
                    }
                default:
                    _logger.LogWarning("Unhandled PhonePe webhook event: {eventType}", eventType);
                    break;
            }

            // 7️⃣ Return 200 OK
            return Ok(new { message = "Webhook processed" });
        }

        // -----------------------------
        // PROCESS PAYMENT
        // -----------------------------
        private async Task ProcessPaymentAsync(JsonElement paymentData)
        {
            var merchantTransactionId = paymentData.GetProperty("merchantTransactionId").GetString();
            var transactionId = paymentData.GetProperty("transactionId").GetString();
            var amount = paymentData.GetProperty("amount").GetInt64();
            var state = paymentData.GetProperty("state").GetString();

            // Normalize state
            var normalizedStatus = state switch
            {
                "COMPLETED" => "SUCCESS",
                "FAILED" => "FAILED",
                _ => "PENDING"
            };

            _logger.LogInformation(
                "Payment | Order:{orderId} | Status:{status} | Amount:{amount} | TxId:{txId}",
                merchantTransactionId, normalizedStatus, amount, transactionId
            );

            // Idempotent save
            var payment = await _db.PhonePePayments
                .FirstOrDefaultAsync(x => x.MerchantTransactionId == merchantTransactionId);

            if (payment == null)
            {
                payment = new PhonePePayment
                {
                    MerchantTransactionId = merchantTransactionId!,
                    PhonePeTransactionId = transactionId,
                    Amount = amount,
                    Status = normalizedStatus,
                    RawWebhookPayload = paymentData.GetRawText(),
                    CreatedAt = DateTime.UtcNow
                };
                _db.PhonePePayments.Add(payment);
            }
            else
            {
                if (payment.Status != "SUCCESS")
                {
                    payment.Status = normalizedStatus;
                    payment.PhonePeTransactionId = transactionId;
                    payment.RawWebhookPayload = paymentData.GetRawText();
                    payment.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync();
        }

        // -----------------------------
        // PROCESS REFUND
        // -----------------------------
        private async Task ProcessRefundAsync(JsonElement refundData)
        {
            var merchantTransactionId = refundData.GetProperty("merchantTransactionId").GetString();
            var refundId = refundData.GetProperty("refundId").GetString();
            var amount = refundData.GetProperty("amount").GetInt64();
            var state = refundData.GetProperty("state").GetString();

            // Normalize state
            var normalizedStatus = state switch
            {
                "REFUND_COMPLETED" => "SUCCESS",
                "REFUND_FAILED" => "FAILED",
                _ => "PENDING"
            };

            _logger.LogInformation(
                "Refund | Order:{orderId} | RefundId:{refundId} | Status:{status} | Amount:{amount}",
                merchantTransactionId, refundId, normalizedStatus, amount
            );

            // Idempotent save
            var refund = await _db.PhonePeRefunds
                .FirstOrDefaultAsync(x => x.RefundId == refundId);

            if (refund == null)
            {
                refund = new PhonePeRefund
                {
                    MerchantTransactionId = merchantTransactionId!,
                    RefundId = refundId!,
                    Amount = amount,
                    Status = normalizedStatus,
                    RawWebhookPayload = refundData.GetRawText(),
                    CreatedAt = DateTime.UtcNow
                };
                _db.PhonePeRefunds.Add(refund);
            }
            else
            {
                if (refund.Status != "SUCCESS")
                {
                    refund.Status = normalizedStatus;
                    refund.RawWebhookPayload = refundData.GetRawText();
                    refund.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _db.SaveChangesAsync();
        }

        // -----------------------------
        // CHECKSUM GENERATION
        // -----------------------------
        private static string GenerateChecksum(string body, string saltKey, string saltIndex)
        {
            var input = body + saltKey;

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            var hash = BitConverter.ToString(hashBytes)
                .Replace("-", "")
                .ToLower();

            return $"{hash}###{saltIndex}";
        }
    }
}
