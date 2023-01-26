using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using static_sv.DTOs;
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
        public Tuple<bool, string> Validate(StaticModel content, string signature)
        {
            // get the x-static-signature header for validation


            // get static secret id from appsetting.json
            string staticSecret = configuration["Static:Secret"];
            
            // convert secret key to byte array
            var signKeyBytes = Convert.FromBase64String(staticSecret);

            using (var hmacsha256 = new HMACSHA256(signKeyBytes))
            {
                var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new {
                    type=content.Type,
                    name=content.Name
                }));

                var hashResult = hmacsha256.ComputeHash(bytes);
                var contentSignature = Convert.ToBase64String(hashResult);

                if(signature == contentSignature)
                {
                    return new(true, contentSignature);
                }
            }

            return new(false, "");
        }

    }
}