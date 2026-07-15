//using Microsoft.Extensions.Configuration;
//using Microsoft.SqlServer.Server;
//using System.Net;
//using System.Numerics;
//using static System.Net.Mime.MediaTypeNames;
//using static System.Net.WebRequestMethods;

//public class BhashSmsService
//{
//    private readonly IConfiguration _config;
//    private readonly HttpClient _httpClient;

//    public BhashSmsService(IConfiguration config, HttpClient httpClient)
//    {
//        _config = config;
//        _httpClient = httpClient;
//    }

//    public async Task<bool> SendOtpSmsAsync(string mobile, string OTP)
//    {

//        var message =
//            $"Packyatra Technologies Private Limited:\r\n" +
//            $"Your login OTP is" +
//            $"{OTP}. Do not share this code." +
//            $"https://packyatra.com/";

//        var data = new Dictionary<string, string>
//         {
//            { "user", _config["BhashSms:User"] },
//            { "pass", _config["BhashSms:Password"] },
//            { "sender", _config["BhashSms:Sender"] },
//            { "phone", mobile },
//            { "text", message },
//            { "priority", _config["BhashSms:Priority"] },
//            { "stype", _config["BhashSms:SmsType"] }
//        };

//        try
//        {
//            _httpClient.Timeout = TimeSpan.FromSeconds(60);

//            var response = await _httpClient.PostAsync(
//                "https://bhashsms.com/api/sendmsg.php",
//                new FormUrlEncodedContent(data)
//            );

//            return response.IsSuccessStatusCode;
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex.Message);
//            // log exception
//            return false;
//        }
//    }

//}
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Server;
using System.Net;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

public class BhashSmsService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public BhashSmsService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public async Task<bool> SendOtpSmsAsync(string mobile, string OTP)
    {

        var message =
          $"OTP for Packyatra Relocation Private Limited is {OTP}. Do not share.\r\nhttps://packyatra.com/";

        var data = new Dictionary<string, string>
         {
            { "user", _config["BhashSms:User"] },
            { "pass", _config["BhashSms:Password"] },
            { "sender", _config["BhashSms:Sender"] },
            { "phone", mobile },
            { "text", message },
            { "priority", _config["BhashSms:Priority"] },
            { "stype", _config["BhashSms:SmsType"] }
        };

        try
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(60);

            var response = await _httpClient.PostAsync(
                "https://bhashsms.com/api/sendmsg.php",
                new FormUrlEncodedContent(data)
            );

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            // log exception
            return false;
        }
    }

}