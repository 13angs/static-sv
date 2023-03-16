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
        private readonly IStaticfile _staticSv;
        private readonly string _xStaticSig;

        public StaticController(IConfiguration configuration, IHttpContextAccessor contextAccessor, IRequestValidator requestValidator, IStaticfile staticSv)
        {
            this.configuration = configuration;
            this.contextAccessor = contextAccessor;
            this.requestValidator = requestValidator;
            _staticSv = staticSv;

            _xStaticSig = contextAccessor.HttpContext!
                .Request.Headers[configuration["Static:Header"]].ToString();
        }

        [HttpPost]
        public ActionResult<StaticResModel> CreateImage([FromBody] StaticModel model)
        {


            object content = new
            {
                type = model.Type,
                name = model.Name
            };
            var serverSig = requestValidator.Validate(content, _xStaticSig);

            // Decode the Base64 encoded image data
            var imageBytes = Convert.FromBase64String(model.Base64EncodedFile!);

            // Create a MemoryStream from the image bytes
            string[] allTypes = model.Type!.Split('/');
            string staticType = allTypes.ElementAt(0);

            string imgType = allTypes.ElementAt(1);

            bool isStaticType = !string.IsNullOrEmpty(StaticTypes.AllTypes.FirstOrDefault(x => x == staticType));

            if (!isStaticType)
                throw new ErrorResponseException(
                    StatusCodes.Status400BadRequest,
                    $"{staticType} not Available",
                    new List<Error>()
                );

            // Create a new unique file name

            DateTime now = DateTime.Now;
            string outputDate = now.ToString("yyyy-MM-dd_HH-mm-ss");

            string fileName = model.Name!.Replace(" ", "-")
                            .Replace("--", "-");

            var fullName = $"{fileName}_{outputDate}.{imgType}";
            var imagePath = "";

            if (staticType == StaticTypes.Image)
            {
                imagePath = Path.Combine(configuration["Static:Name"], configuration["Static:Types:Image"], fullName);
            }
            else if (staticType == StaticTypes.Video)
            {
                imagePath = Path.Combine(configuration["Static:Name"], configuration["Static:Types:Video"], fullName);
            }
            else
            {
                imagePath = Path.Combine(configuration["Static:Name"], configuration["Static:Types:File"], fullName);
            }


            using (var memoryStream = new MemoryStream(imageBytes))
            {
                // Save the image to the server's file system

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

        [HttpDelete]
        public async Task<ActionResult> RemoveImage([FromBody] StaticModel model)
        {
            await _staticSv.DeleteImage(model.Url!, _xStaticSig);
            return Ok();
        }
    }
}