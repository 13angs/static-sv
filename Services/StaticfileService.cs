using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static_sv.DTOs;
using static_sv.Exceptions;
using static_sv.Interfaces;
using static_sv.Models;

namespace static_sv.Services
{
    public class StaticfileService : IStaticfile
    {
        private readonly IConfiguration _configuration;
        private readonly IRequestValidator _requestValidator;
        private readonly IHostEnvironment _env;
        private readonly IFolder _folder;
        private readonly StaticContext _context;
        private readonly ILogger<StaticfileService> _logger;

        public StaticfileService(IConfiguration configuration, IRequestValidator requestValidator, IHostEnvironment env, IFolder folder, StaticContext context, ILogger<StaticfileService> logger)
        {
            _configuration = configuration;
            _requestValidator = requestValidator;
            _env = env;
            _folder = folder;
            _context = context;
            _logger = logger;
        }

        public string GetStaticPath()
        {
            string contentPath = _env.ContentRootPath;
            string staticPath = _configuration["Static:Name"];
            return Path.Combine(contentPath, staticPath);
        }

        public async Task<StaticResModel> CreateFile(StaticModel model, string xStaticSig)
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
            var fileBytes = Convert.FromBase64String(model.Base64EncodedFile!);

            // Create a MemoryStream from the image bytes
            string[] allTypes = model.Type!.Split('/');
            string staticType = allTypes.ElementAt(0);

            string fileType = allTypes.ElementAt(1);


            bool isStaticType = !string.IsNullOrEmpty(StaticTypes.AllTypes.FirstOrDefault(x => x == staticType));

            if (!isStaticType)
                throw new ErrorResponseException(
                    StatusCodes.Status400BadRequest,
                    $"{staticType} not Available",
                    new List<Error>()
                );

            // Create a new unique file name

            DateTime now = DateTime.Now;
            // string outputDate = now.ToString("yyyy-MM-dd_HH-mm-ss");

            string fileName = model.Name!.Replace(" ", "-")
                            .Replace("--", "-");

            var filePath = GetStaticPath();
            filePath = Path.Combine(filePath, model.Folder!);
            string contentApi = _configuration["Static:Api:Content"];

            using (var memoryStream = new MemoryStream(fileBytes))
            {
                // Save the image to the server's file system
                Folder folder = new Folder();
                folder.Path=model.Folder;
                folder = await _folder.CreateFolder(folder);    

                if(!System.IO.Directory.Exists(filePath))
                {
                    System.IO.Directory.CreateDirectory(filePath);
                }
                long fileTs = DateConverter.ToTimestamp(now);
                var fullName = $"{fileName}_{fileTs}.{fileType}";
                string fileFullPath = Path.Combine(filePath, fullName);
                await System.IO.File.WriteAllBytesAsync(fileFullPath, memoryStream.ToArray());

                Staticfile staticfile = new Staticfile{
                    Name=fullName,
                    FolderId=folder.FolderId,
                    Path=Path.Combine(model.Folder!, fullName),
                    Type=model.Type,
                    Size=memoryStream.Length,
                    Timestamp=fileTs
                };

                await _context.Staticfiles.AddAsync(staticfile);
                int result = await _context.SaveChangesAsync();

                if(result <= 0)
                {
                    throw new ErrorResponseException(
                        StatusCodes.Status500InternalServerError,
                        "Failed saving Staticfile",
                        new List<Error>()
                    );
                }


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
                    now = now.AddSeconds(1);
                    long prevTs = DateConverter.ToTimestamp(now);
                    var previewName = $"{fileName}_{prevTs}.png";
                    string previewPath = Path.Combine(filePath, previewName);
                    using(var prevStream = new MemoryStream(prevBytes))
                    {
                        await System.IO.File.WriteAllBytesAsync(previewPath, prevStream.ToArray());
                        staticfile = new Staticfile{
                            Name=previewName,
                            FolderId=folder.FolderId,
                            Path=Path.Combine(model.Folder!, previewName),
                            Type="image/png",
                            Size=prevStream.Length,
                            Timestamp=prevTs
                        };

                        await _context.Staticfiles.AddAsync(staticfile);
                        result = await _context.SaveChangesAsync();
                    }


                    if(result <= 0)
                    {
                        throw new ErrorResponseException(
                            StatusCodes.Status500InternalServerError,
                            "Failed saving Staticfile",
                            new List<Error>()
                        );
                    }

                    string prevUrl = Path.Combine(url, contentApi, previewName);
                    resModel.PreviewUrl=prevUrl;
                }
                return resModel;
            }
        }

        public async Task DeleteFile(string url, string xStaticSig)
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

            // string filePath = GetStaticPath();
            // long timestamp;
            // string strTs = imageName.Split("_")[1].Split(".")[0];
            // Int64.TryParse(strTs, out timestamp);

            // string fullPath = Path.Combine(filePath, imageName);
            // Console.WriteLine(filePath);
            StaticfileQuery staticQuery = new StaticfileQuery{
                Is=StaticfileQueryStore.Staticfile,
                Name=imageName
            };

            var staticfiles = GetStaticfiles(staticQuery);
            Staticfile? staticfile = new Staticfile();
            string filePath = String.Empty;

            if(staticfiles.Any())
            {
                // Console.WriteLine("getting files");
                // return Task.CompletedTask;
                staticfile=staticfiles.FirstOrDefault();
                filePath = Path.Combine(_env.ContentRootPath, _configuration["Static:Name"], staticfile!.Path!);

                if(!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning("File doesn't exist. Deleting from db...");
                    _context.Staticfiles.Remove(staticfile);
                    await _context.SaveChangesAsync();
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

                System.IO.File.Delete(filePath);

                _context.Staticfiles.Remove(staticfile);
                await _context.SaveChangesAsync();
                return;
            }

            throw new ErrorResponseException(
                StatusCodes.Status400BadRequest,
                $"image name: {imageName} doesn't exist both in db and server",
                new List<Error>{
                    new Error{
                        Message=url,
                        Field="url"
                    }
                }
            );
            // var files = Directory.GetFiles(filePath, imageName, SearchOption.AllDirectories);



            // if(!files.Any())
            // {
            // }

            

            // return Task.CompletedTask;
            // string imgPath = files[0];

            // if (System.IO.File.Exists(imgPath))
            // {
            // }

            // throw new ErrorResponseException(
            //     StatusCodes.Status400BadRequest,
            //     $"image name: {imageName} doesn't exist",
            //     new List<Error>{
            //         new Error{
            //             Message=imgPath,
            //             Field="full_name"
            //         },
            //         new Error{
            //             Message=url,
            //             Field="url"
            //         }
            //     }
            // );
        }

        public IEnumerable<Staticfile> GetStaticfiles(StaticfileQuery query)
        {
            IEnumerable<Staticfile> staticfiles = new List<Staticfile>();

            if(query.Is == StaticfileQueryStore.Folder)
            {
                return staticfiles=_context.Staticfiles
                    .Where(s => s.FolderId == query.FolderId && 
                        (!String.IsNullOrEmpty(query.Type) ? s.Type!.Contains(query.Type) : true) &&
                        (!String.IsNullOrEmpty(query.Name) ? s.Name!.Contains(query.Name) : true)
                        )
                    .Take(query.Limit)
                    .AsNoTracking();
            }

            if(query.Is == StaticfileQueryStore.Staticfile)
            {
                return staticfiles=_context.Staticfiles
                    .Where(f => f.StaticfileId == query.StaticfileId ||
                            f.Name == query.Name
                        )
                    .AsNoTracking();
                
                // foreach(Staticfile s in staticfiles)
                // {
                //     long dbTs = DateConverter.ToTimestamp(s.CreatedDate);
                //     Console.WriteLine($"s: {dbTs} - q; {query.Timestamp}");
                //     if(dbTs == query.Timestamp)
                //     {
                //         var result = new List<Staticfile>();
                //         result.Add(s);
                //         return result;
                //     }
                // }
                
            }

            throw new ErrorResponseException(
                StatusCodes.Status501NotImplemented,
                $"Type not implement",
                new List<Error>{}
            );

        }
    }
}