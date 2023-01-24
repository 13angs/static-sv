using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static_sv.Interfaces;

namespace static_sv.Services
{
    public class RequestValidator : IRequestValidator
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor contextAccessor;

        public RequestValidator(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            this.configuration = configuration;
            this.contextAccessor = contextAccessor;
        }
        public Tuple<bool, string> Validate(object content, string signature)
        {
            // get the x-static-signature header for validation
            

            // get static secret id from appsetting.json
            string staticSecret = configuration["Static:Secret"];

            // create a new instance of HMACSHA256
            var key = Encoding.UTF8.GetBytes(staticSecret);
            var hmac = new HMACSHA256(key);

            // Compute the HMAC of the request body
            // the request body need to be string
            var requestBody = JsonSerializer.Serialize(content);
            var bodyBytes = Encoding.UTF8.GetBytes(requestBody);
            var hmacBytes = hmac.ComputeHash(bodyBytes);

            // Compare the computed HMAC to the one sent in the request headers
            var receivedHmac = Convert.FromBase64String(signature);
            if (hmacBytes.SequenceEqual(receivedHmac))
            {
                // Request is valid
                Console.WriteLine("Success!");
                string calSignature = Convert.ToBase64String(hmacBytes);
                return new (true, calSignature);
            }
            
            // Request is not valid
            Console.WriteLine("Failed!");
            return new (false, "");
        }
    }
}