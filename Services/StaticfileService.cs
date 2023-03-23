using Newtonsoft.Json;
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

        public string GetStaticPath()
        {
            string contentPath = _env.ContentRootPath;
            string staticPath = _configuration["Static:Name"];
            return Path.Combine(contentPath, staticPath);
            // if (type.ToLower() == StaticTypeStore.Image)
            // {
            // }
            // else if (type.ToLower() == StaticTypeStore.Video)
            // {
            //     string contentPath = _env.ContentRootPath;
            //     string staticPath = _configuration["Static:Name"];
            //     string filePath = _configuration["Static:Types:Video"];
            //     return Path.Combine(contentPath, staticPath, filePath);
            // }
            // else 
            // {
            //     string contentPath = _env.ContentRootPath;
            //     string staticPath = _configuration["Static:Name"];
            //     string filePath = _configuration["Static:Types:File"];
            //     return Path.Combine(contentPath, staticPath, filePath);
            // }
            // throw new ErrorResponseException(
            //     StatusCodes.Status404NotFound,
            //     "Static type not found",
            //     new List<Error>{
            //         new Error{
            //             Field="type",
            //             Message=$"Available type: {StaticTypeStore.Image}"
            //         }
            //     }
            // );
        }

        public async Task<StaticResModel> CreateImage(StaticModel model, string xStaticSig)
        {
            // vlidate the signature
            // vlidate the signature
            object content = new
            {
                type = model.Type,
                name = model.Name,
                folder = model.Folder,
                add_preview_url = model.AddPreviewUrl
            };
            var serverSig = _requestValidator.Validate(content, xStaticSig);

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
            var filePath = GetStaticPath();
            filePath = Path.Combine(filePath, model.Folder!);
            string contentApi = _configuration["Static:Api:Content"];

            // if (staticType == StaticTypes.Image)
            // {
            //     contentApi = Path.Combine(contentApi, _configuration["Static:Types:Image"]);
            // }
            // else if (staticType == StaticTypes.Video)
            // {
            //     filePath = Path.Combine(_configuration["Static:Name"], _configuration["Static:Types:Video"], model.Group!);
            //     contentApi = Path.Combine(contentApi, _configuration["Static:Types:Video"]);
            // }
            // else
            // {
            //     filePath = Path.Combine(_configuration["Static:Name"], _configuration["Static:Types:File"], model.Group!);
            //     contentApi = Path.Combine(contentApi, _configuration["Static:Types:File"]);
            // }


            using (var memoryStream = new MemoryStream(imageBytes))
            {
                // Save the image to the server's file system
                if(!System.IO.Directory.Exists(filePath))
                    System.IO.Directory.CreateDirectory(filePath);
                string fileFullPath = Path.Combine(filePath, fullName);
                await System.IO.File.WriteAllBytesAsync(fileFullPath, memoryStream.ToArray());
                string url = _configuration["ASPNETCORE_DOMAIN_URL"];
                string fileUrl = Path.Combine(url, contentApi, fullName);

                // return Ok(new { filePath = $"images/{fileName}" });
                StaticResModel resModel = new StaticResModel
                {
                    FileUrl = fileUrl,
                    Signature = serverSig,
                    ErrorCode = "SUCCESS"
                };

                if(model.AddPreviewUrl)
                {
                    var prevBytes = Convert.FromBase64String(model.PreviewFile!);
                    var previewName = $"{fileName}_{outputDate}.png";
                    string previewPath = Path.Combine(filePath, previewName);
                    using(var prevStream = new MemoryStream(prevBytes))
                    {
                        await System.IO.File.WriteAllBytesAsync(previewPath, prevStream.ToArray());
                    }

                    string prevUrl = Path.Combine(url, contentApi, previewName);
                    resModel.PreviewUrl=prevUrl;
                }
                return resModel;
            }
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
            // string imgType = "";
            // get the image name from the url
            // Console.WriteLine(JsonConvert.SerializeObject(segments));

            // if(segments[5].Split("/")[0] == (_configuration["Static:Types:Image"]))
            // {
            //     imgType = StaticTypeStore.Image!;
            // }
            // else if(segments[5].Split("/")[0] == (_configuration["Static:Types:Video"]))
            // {
            //     imgType = StaticTypeStore.Video!;
            // }else 
            // {
            //     imgType = StaticTypeStore.File!;
            // }

            string filePath = GetStaticPath();

            // string fullPath = Path.Combine(filePath, imageName);
            // Console.WriteLine(filePath);

            var files = Directory.GetFiles(filePath, imageName, SearchOption.AllDirectories);

            if(!files.Any())
            {
                throw new ErrorResponseException(
                    StatusCodes.Status400BadRequest,
                    $"image name: {imageName} doesn't exist",
                    new List<Error>{
                        new Error{
                            Message=url,
                            Field="url"
                        }
                    }
                );
            }

            string imgPath = files[0];

            if (System.IO.File.Exists(imgPath))
            {
                System.IO.File.Delete(imgPath);
                return Task.CompletedTask;
            }

            throw new ErrorResponseException(
                StatusCodes.Status400BadRequest,
                $"image name: {imageName} doesn't exist",
                new List<Error>{
                    new Error{
                        Message=imgPath,
                        Field="full_name"
                    },
                    new Error{
                        Message=url,
                        Field="url"
                    }
                }
            );
        }

        // public IEnumerable<string> GetImages(StaticQuery queryParams, string xStaticSig)
        // {
        //     // validate the signature
        //     object deleteContent = new
        //     {
        //         query = queryParams.query
        //     };

        //     _requestValidator.Validate(deleteContent, xStaticSig);

        //     string imageDirPath = GetStaticPath(StaticTypeStore.Image!);

        //     // get the query else throw
        //     if (queryParams.query == StaticQueryStore.All)
        //     {
        //         string url = _configuration["ASPNETCORE_DOMAIN_URL"];
        //         var staticPath = _configuration["Static:Name"];
        //         var filePath = _configuration["Static:Types:Image"];

        //         string[] entries = System.IO.Directory.GetFileSystemEntries(imageDirPath);
        //         var fileUrls = entries.Select(e => Path.Combine(url, staticPath, filePath, e.Split("/").Last()));
        //         return fileUrls;
        //     }

        //     throw new ErrorResponseException(
        //         StatusCodes.Status404NotFound,
        //         "Query type not found",
        //         new List<Error>{
        //             new Error{
        //                 Field="query",
        //                 Message=$"Available type: {StaticQueryStore.All}"
        //             }
        //         }
        //     );
        // }
    }
}