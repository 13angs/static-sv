using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using static_sv.DTOs;
using static_sv.Exceptions;
using static_sv.Interfaces;

namespace static_sv.Controllers
{
    [ApiController]
    [Route("api/v1/statics")]
    public class StaticController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IRequestValidator requestValidator;

        public StaticController(IConfiguration configuration, IHttpContextAccessor contextAccessor, IRequestValidator requestValidator)
        {
            this.configuration = configuration;
            this.contextAccessor = contextAccessor;
            this.requestValidator = requestValidator;
        }

        [HttpPost]
        public ActionResult<StaticResModel> CreateImage([FromBody] StaticModel model)
        {
            string xStaticSig = contextAccessor.HttpContext!
                .Request.Headers[configuration["Static:Header"]].ToString();

            object content = new
            {
                type = model.Type,
                name = model.Name
            };
            var serverSig = requestValidator.Validate(content, xStaticSig);

            // Decode the Base64 encoded image data
            var imageBytes = Convert.FromBase64String(model.Base64EncodedFile!);

            // Create a MemoryStream from the image bytes
            string[] allTypes = model.Type!.Split('/');
            string staticType = allTypes.ElementAt(0);


            string imgType = allTypes.ElementAt(1);

            if (staticType == StaticTypes.Image)
            {
                using (var memoryStream = new MemoryStream(imageBytes))
                {
                    // Create a new unique file name

                    DateTime now = DateTime.Now;
                    string outputDate = now.ToString("yyyy-MM-dd_HH-mm-ss");

                    string fileName = model.Name!.Replace(" ", "-")
                                    .Replace("--", "-");

                    var fullName = $"{fileName}_{outputDate}.{imgType}";

                    // Save the image to the server's file system
                    var imagePath = Path.Combine(configuration["Static:Name"], configuration["Static:Types:Image"], fullName);
                    System.IO.File.WriteAllBytes(imagePath, memoryStream.ToArray());

                    string url = configuration["ASPNETCORE_DOMAIN_URL"];
                    string imageUrl = Path.Combine(url, imagePath);

                    // return Ok(new { imagePath = $"images/{fileName}" });
                    StaticResModel resModel = new StaticResModel
                    {
                        ImageUrl = imageUrl,
                        Signature = serverSig,
                        ErrorCode = "SUCCESS"
                    };
                    return resModel;
                }
            }

            throw new ErrorResponseException(
                    StatusCodes.Status400BadRequest,
                    $"Available types: {StaticTypes.Image}",
                    new List<Error>()
                );
        }
    }
}