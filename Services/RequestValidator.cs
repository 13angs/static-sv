using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using static_sv.Exceptions;
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
        public string Validate(object content, string signature)
        {
            // get static secret id from appsetting.json
            string staticSecret = configuration["Static:Secret"];
            
            // convert secret key to byte array
            var signKeyBytes = Convert.FromBase64String(staticSecret);

            using (var hmacsha256 = new HMACSHA256(signKeyBytes))
            {
                string serverReqBody = JsonConvert.SerializeObject(content);
                var bytes = Encoding.UTF8.GetBytes(serverReqBody);

                var hashResult = hmacsha256.ComputeHash(bytes);
                var contentSignature = Convert.ToBase64String(hashResult);

                if(signature == contentSignature)
                {
                    return signature;
                }

                throw new ErrorResponseException(
                    StatusCodes.Status401Unauthorized,
                    "Invalid signature",
                    new List<Error>{
                        new Error{
                            Field="server_signature",
                            Message=contentSignature
                        },
                        new Error{
                            Field="client_signature",
                            Message=signature
                        },
                        new Error{
                            Field="server_req_body",
                            Message=serverReqBody
                        }
                    }
                );
            }
        }

    }
}