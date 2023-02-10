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

        public string GetStaticPath(string type)
        {
            if(type.ToLower() == StaticTypeStore.Image)
            {
                string contentPath = _env.ContentRootPath;
                string staticPath = _configuration["Static:Name"];
                string imagePath = _configuration["Static:Types:Image"];
                return Path.Combine(contentPath, staticPath, imagePath);
            }
            throw new ErrorResponseException(
                StatusCodes.Status404NotFound,
                "Static type not found",
                new List<Error>{
                    new Error{
                        Field="type",
                        Message=$"Available type: {StaticTypeStore.Image}"
                    }
                }
            );
        }

        public async Task<StaticResModel> CreateImage(StaticModel model, string xStaticSig)
        {
            // vlidate the signature
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
            // validate the signature
            object deleteContent = new
            {
                url = url
            };

            _requestValidator.Validate(deleteContent, xStaticSig);
            Uri imageUri = new Uri(url);
            string[] segments = imageUri.Segments;
            string imageName = segments.Last().Replace("/", "");
            // get the image name from the url
            string imagePath = GetStaticPath(StaticTypeStore.Image!);

            string fullPath = Path.Combine(imagePath, imageName);

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

        public IEnumerable<string> GetImages(StaticQuery queryParams, string xStaticSig)
        {
            // validate the signature
            object deleteContent = new
            {
                query = queryParams.query
            };

            _requestValidator.Validate(deleteContent, xStaticSig);
            
            string imageDirPath = GetStaticPath(StaticTypeStore.Image!);

            // get the query else throw
            if(queryParams.query == StaticQueryStore.All)
            {
                string url = _configuration["ASPNETCORE_DOMAIN_URL"];
                var staticPath = _configuration["Static:Name"];
                var imagePath = _configuration["Static:Types:Image"];

                string[] entries = System.IO.Directory.GetFileSystemEntries(imageDirPath);
                var imageUrls = entries.Select(e => Path.Combine(url, staticPath, imagePath, e.Split("/").Last()));
                return imageUrls;
            }

            throw new ErrorResponseException(
                StatusCodes.Status404NotFound,
                "Query type not found",
                new List<Error>{
                    new Error{
                        Field="query",
                        Message=$"Available type: {StaticQueryStore.All}"
                    }
                }
            );
        }
    }
}