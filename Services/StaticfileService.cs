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

        public Task DeleteImage(string url, string xStaticSig)
        {
            object deleteContent = new {
                url=url
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

            if(System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
                return Task.CompletedTask;
            }

            throw new ErrorResponseException(
                StatusCodes.Status204NoContent,
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