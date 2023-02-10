using static_sv.DTOs;
using static_sv.Exceptions;
using static_sv.Interfaces;

namespace static_sv.Services
{
    public class StaticfileService : IStaticfile
    {
        private readonly IConfiguration _configuration;
        private readonly IRequestValidator _requestValidator;
        private readonly IHostEnvironment _env;

        public StaticfileService(IConfiguration configuration, IRequestValidator requestValidator, IHostEnvironment env)
        {
            _configuration = configuration;
            _requestValidator = requestValidator;
            _env = env;
        }

        public async Task<StaticResModel> CreateImage(StaticModel model, string xStaticSig)
        {
            object content = new
            {
                type = model.Type,
                name = model.Name
            };
            var serverSig = _requestValidator.Validate(content, xStaticSig);

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
                    var imagePath = Path.Combine(_configuration["Static:Name"], _configuration["Static:Types:Image"], fullName);
                    await System.IO.File.WriteAllBytesAsync(imagePath, memoryStream.ToArray());

                    string url = _configuration["ASPNETCORE_DOMAIN_URL"];
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

        public Task DeleteImage(string url, string xStaticSig)
        {
            object deleteContent = new
            {
                url = url
            };

            // validate the signature
            _requestValidator.Validate(deleteContent, xStaticSig);
            Uri imageUri = new Uri(url);
            string[] segments = imageUri.Segments;
            string imageName = segments.Last().Replace("/", "");
            // get the image name from the url

            string contentPath = _env.ContentRootPath;
            string staticPath = _configuration["Static:Name"];
            string imagePath = _configuration["Static:Types:Image"];
            string fullPath = Path.Combine(contentPath, staticPath, imagePath, imageName);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return Task.CompletedTask;
            }

            throw new ErrorResponseException(
                StatusCodes.Status400BadRequest,
                $"image name: {imageName} doesn't exist",
                new List<Error>{
                    new Error{
                        Message=fullPath,
                        Field="full_name"
                    },
                    new Error{
                        Message=url,
                        Field="url"
                    }
                }
            );
        }
    }
}