using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MoversAndPackerApi.Models;
using MoversAndPackerApi.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MoversAndPackerApi.Services
{
    public class SendGridEmailService : ISendGridEmailService
    {
        private readonly IConfiguration _config;

        public SendGridEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendQuotationAsync(SendQuotationEmailDto dto)
        {
            try
            {
                // Enable TLS 1.2
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var apiKey = _config["ZeptoMail:ApiKey"];
                var fromEmail = _config["ZeptoMail:FromEmail"];
                var fromName = _config["ZeptoMail:FromName"];

                // Convert PDF base64 to bytes and then to base64 for JSON
                var pdfBytes = Convert.FromBase64String(dto.PdfBase64);
                var pdfBase64 = Convert.ToBase64String(pdfBytes);

                // Build the request body
                var requestBody = new JObject
                {
                    ["from"] = new JObject
                    {
                        ["address"] = fromEmail
                    },
                    ["to"] = new JArray
                    {
                        new JObject
                        {
                            ["email_address"] = new JObject
                            {
                                ["address"] = dto.RecipientEmail,
                                ["name"] = dto.CustomerName
                            }
                        }
                    },
                    ["subject"] = $"Quotation #{dto.QuotationNumber} - PackYatra",
                    ["htmlbody"] = $@"
                    <div>
                        <h2>Hello {dto.CustomerName}</h2>
                        <p>Your quotation <b>{dto.QuotationNumber}</b> is attached.</p>
                        <p><b>Total:</b> ₹{dto.TotalAmount}</p>
                        <p><b>Pickup Date:</b> {dto.PickupDate}</p>
                        <br/>
                        <p>Thank you for choosing PackYatra!</p>
                    </div>",
                    ["textbody"] = $@"
                    Hello {dto.CustomerName},
                    
                    Your quotation #{dto.QuotationNumber} is attached.
                    Total: ₹{dto.TotalAmount}
                    Pickup Date: {dto.PickupDate}
                    
                    Thank you for choosing PackYatra!",
                    ["attachments"] = new JArray
                    {
                        new JObject
                        {
                            ["name"] = $"Quotation_{dto.QuotationNumber}.pdf",
                            ["content"] = pdfBase64,
                            ["mime_type"] = "application/pdf"
                        }
                    }
                };

                // Create and send the request
                var baseAddress = "https://api.zeptomail.com/v1.1/email";
                var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
                http.Accept = "application/json";
                http.ContentType = "application/json";
                http.Method = "POST";
                http.PreAuthenticate = true;
                http.Headers.Add("Authorization", $"{apiKey}");

                var bytes = Encoding.UTF8.GetBytes(requestBody.ToString());

                using (var newStream = await http.GetRequestStreamAsync())
                {
                    await newStream.WriteAsync(bytes, 0, bytes.Length);
                }

                using (var response = (HttpWebResponse)await http.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    using (var sr = new StreamReader(stream))
                    {
                        var content = await sr.ReadToEndAsync();
                        Console.WriteLine(content); // For logging
                    }

                    return response.StatusCode == HttpStatusCode.Created ||
                           response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (WebException webEx)
            {
                // Handle WebException separately to get response details
                using (var response = webEx.Response as HttpWebResponse)
                {
                    if (response != null)
                    {
                        using (var stream = response.GetResponseStream())
                        using (var reader = new StreamReader(stream))
                        {
                            var errorContent = await reader.ReadToEndAsync();
                            Console.WriteLine($"Error Response: {errorContent}");
                        }
                    }
                }
                Console.WriteLine($"WebException: {webEx.Message}");
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending email: {e.Message}");
                return false;
            }
        }
    }
}