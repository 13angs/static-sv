using System.Net;
using System.Web;
using Microsoft.EntityFrameworkCore;
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
        private readonly IObjectStorage _objSv;

        public StaticfileService(IConfiguration configuration, IRequestValidator requestValidator, IHostEnvironment env, IFolder folder, StaticContext context, ILogger<StaticfileService> logger, IObjectStorage objSv)
        {
            _configuration = configuration;
            _requestValidator = requestValidator;
            _env = env;
            _folder = folder;
            _context = context;
            _logger = logger;
            _objSv = objSv;
        }

        public string GetStaticPath()
        {
            string contentPath = _env.ContentRootPath;
            string staticPath = _configuration["Static:Name"];
            string filePath = _configuration["Static:FilePath"];
            string fullPath = Path.Combine(contentPath, staticPath, filePath);

            if (!Directory.Exists(fullPath))
                Directory.CreateDirectory(fullPath);
            return fullPath;
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

            // Save the image to the server's file system
            Folder folder = new Folder();
            folder.Path = model.Folder;
            folder = await _folder.CreateFolder(folder);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }


            long fileTs = DateConverter.ToTimestamp(now);
            var fullName = $"{fileName}_{fileTs}.{fileType}";
            string fileFullPath = Path.Combine(filePath, fullName);

            using (var memoryStream = new MemoryStream(fileBytes))
            {
                await System.IO.File.WriteAllBytesAsync(fileFullPath, memoryStream.ToArray());
            }

            Staticfile staticfile = new Staticfile
            {
                Name = fullName,
                FolderId = folder.FolderId,
                Path = Path.Combine(model.Folder!, fullName),
                Type = model.Type,
                Size = fileBytes.Length,
                Timestamp = fileTs,
                // FileData=fileBytes
            };

            await _context.Staticfiles.AddAsync(staticfile);
            int result = await _context.SaveChangesAsync();

            if (result <= 0)
            {
                throw new ErrorResponseException(
                    StatusCodes.Status500InternalServerError,
                    "Failed saving Staticfile",
                    new List<Error>()
                );
            }


            // string url = _configuration["ASPNETCORE_DOMAIN_URL"];
            // string fileUrl = Path.Combine(url, contentApi, staticfile.Name);

            // return Ok(new { filePath = $"images/{fileName}" });
            StaticResModel resModel = new StaticResModel
            {
                // FileUrl = ContentUrl.ToContentUrl(staticfile, _configuration),
                Signature = serverSig,
                ErrorCode = "SUCCESS"
            };

            if (model.AddPreviewUrl)
            {
                var prevBytes = Convert.FromBase64String(model.PreviewFile!);
                // now = now.AddSeconds(1);
                // long prevTs = DateConverter.ToTimestamp(now);
                var previewName = $"{fileName}_{fileTs}.png";
                string previewPath = Path.Combine(filePath, previewName);
                using (var prevStream = new MemoryStream(prevBytes))
                {
                    await System.IO.File.WriteAllBytesAsync(previewPath, prevStream.ToArray());
                }
                Staticfile relatedFile = new Staticfile
                {
                    Name = previewName,
                    FolderId = folder.FolderId,
                    Path = Path.Combine(model.Folder!, previewName),
                    Type = "image/png",
                    Size = prevBytes.Length,
                    Timestamp = fileTs,
                    // FileData=prevBytes,
                    ParentFileId = staticfile.StaticfileId
                };

                await _context.Staticfiles.AddAsync(relatedFile);
                result = await _context.SaveChangesAsync();

                // using(var prevStream = new MemoryStream(prevBytes))
                // {
                // }


                if (result <= 0)
                {
                    throw new ErrorResponseException(
                        StatusCodes.Status500InternalServerError,
                        "Failed saving Staticfile",
                        new List<Error>()
                    );
                }

                // string prevUrl = Path.Combine(url, contentApi, relatedFile.Name);
                // resModel.PreviewUrl=ContentUrl.ToContentUrl(relatedFile, _configuration);
            }
            return resModel;
            // using (var memoryStream = new MemoryStream(fileBytes))
            // {
            // }
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
            string query = imageUri.PathAndQuery;
            string dirId = HttpUtility.ParseQueryString(imageUri.Query).Get("dir")!;
            // Int64.TryParse(strId, out staticId);
            string fileName = Path.GetFileName(imageUri.LocalPath);
            string key = Path.Combine("files", dirId, fileName);
            // string imageName = segments.Last().Replace("/", "");

            // string filePath = GetStaticPath();
            // long timestamp;
            // string strTs = imageName.Split("_")[1].Split(".")[0];
            // Int64.TryParse(strTs, out timestamp);

            // string fullPath = Path.Combine(filePath, imageName);
            // Console.WriteLine(filePath);
            // StaticfileQuery staticQuery = new StaticfileQuery{
            //     Is=StaticfileQueryStore.Staticfile,
            //     StaticfileId=staticId
            //     // Name=imageName
            // };

            // var staticfiles = GetStaticfiles(staticQuery);
            // Staticfile? staticfile = new Staticfile();
            // string filePath = String.Empty;

            // if(staticfiles.Any())
            // {
            //     // Console.WriteLine("getting files");
            //     // return Task.CompletedTask;
            //     staticfile=staticfiles.FirstOrDefault();

            //     // find the related file
            //     staticQuery = new StaticfileQuery{
            //         Is=StaticfileQueryStore.RelatedFile,
            //         StaticfileId=staticfile!.StaticfileId
            //     };

            //     var relatedFiles = GetStaticfiles(staticQuery);

            //     if(relatedFiles.Any())
            //     {
            //         foreach(var relatedFile in relatedFiles)
            //         {
            //             filePath = Path.Combine(_env.ContentRootPath, _configuration["Static:Name"], _configuration["Static:FilePath"], relatedFile!.Path!);
            //             File.Delete(filePath);
            //         }
            //         _context.Staticfiles.RemoveRange(relatedFiles);
            //     }

            //     filePath = Path.Combine(_env.ContentRootPath, _configuration["Static:Name"], _configuration["Static:FilePath"], staticfile!.Path!);

            //     System.IO.File.Delete(filePath);

            //     _context.Staticfiles.Remove(staticfile!);
            //     await _context.SaveChangesAsync();
            //     return;
            // }
            var response = await _objSv.RemoveFile(key);

            if (response.HttpStatusCode != HttpStatusCode.NoContent)
            {
                throw new ErrorResponseException(
                    StatusCodes.Status400BadRequest,
                    $"image with id: {key} doesn't exist",
                    new List<Error>{
                    new Error{
                        Message=key,
                        Field="id"
                    }
                    }
                );
            }
            return;
        }

        public IEnumerable<Staticfile> GetStaticfiles(StaticfileQuery query)
        {
            IEnumerable<Staticfile> staticfiles = new List<Staticfile>();

            if (query.Is == StaticfileQueryStore.Folder)
            {
                return staticfiles = _context.Staticfiles
                    .Include(s => s.RelatedFiles)
                    .Where(s => s.FolderId == query.FolderId &&
                        (!String.IsNullOrEmpty(query.Type) ? s.Type!.Contains(query.Type) : true) &&
                        (!String.IsNullOrEmpty(query.Name) ? s.Name!.Contains(query.Name) : true)
                        )
                    .Take(query.Limit)
                    .AsNoTracking();
            }

            if (query.Is == StaticfileQueryStore.Staticfile)
            {
                return staticfiles = _context.Staticfiles
                    .Where(f => f.StaticfileId == query.StaticfileId ||
                            f.Name == query.Name
                        )
                    .AsNoTracking();
            }

            if (query.Is == StaticfileQueryStore.RelatedFile)
            {
                return staticfiles = _context.Staticfiles
                    .Include(s => s.RelatedFiles)
                    .Where(f => f.ParentFileId == query.StaticfileId ||
                            f.Name == query.Name
                        )
                    .AsNoTracking();
            }

            throw new ErrorResponseException(
                StatusCodes.Status501NotImplemented,
                $"Type not implement",
                new List<Error> { }
            );

        }

        public async Task<StaticResModel> CreateFileData(StaticModel model, string xStaticSig)
        {
            // vlidate the signature
            // vlidate the signature
            object content = new { };
            var serverSig = _requestValidator.Validate(content, xStaticSig);

            // Decode the Base64 encoded image data
            byte[] fileBytes = null!;

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
            // filePath = Path.Combine(filePath, model.Folder!);
            string contentApi = _configuration["Static:Api:Content"];

            // Save the image to the server's file system
            // Folder folder = new Folder();
            // folder.Path=model.Folder;
            // folder = await _folder.CreateFolder(folder);    

            // if(!Directory.Exists(filePath))
            // {
            //     Directory.CreateDirectory(filePath);
            // }

            // if(!System.IO.Directory.Exists(filePath))
            // {
            //     System.IO.Directory.CreateDirectory(filePath);
            // }
            long fileTs = DateConverter.ToTimestamp(now);
            var fullName = $"{fileName}_{fileTs}.{fileType}";
            string fileFullPath = Path.Combine(filePath, fullName);
            // await System.IO.File.WriteAllBytesAsync(fileFullPath, memoryStream.ToArray());
            Console.WriteLine(model.AddPreviewUrl);

            using (var ms = new MemoryStream())
            {
                await model.FileData!.CopyToAsync(ms);
                fileBytes = ms.ToArray();
                await System.IO.File.WriteAllBytesAsync(fileFullPath, fileBytes);
            }

            // Staticfile staticfile = new Staticfile{
            //     Name=fullName,
            //     FolderId=folder.FolderId,
            //     Path=Path.Combine(model.Folder!, fullName),
            //     Type=model.Type,
            //     Size=fileBytes.Length,
            //     Timestamp=fileTs,
            //     // FileData=fileBytes
            // };

            // await _context.Staticfiles.AddAsync(staticfile);
            // int result = await _context.SaveChangesAsync();

            // if(result <= 0)
            // {
            //     throw new ErrorResponseException(
            //         StatusCodes.Status500InternalServerError,
            //         "Failed saving Staticfile",
            //         new List<Error>()
            //     );
            // }


            // string url = _configuration["ASPNETCORE_DOMAIN_URL"];
            // string fileUrl = Path.Combine(url, contentApi, staticfile.Name);
            model.LocalPath = fileFullPath;
            model.Name = fullName;

            var response = await _objSv.UploadFile(model);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                _logger.LogError("Failed saving Staticfile");
                throw new ErrorResponseException(
                   StatusCodes.Status500InternalServerError,
                   "Failed saving Staticfile",
                   new List<Error>()
               );
            }

            // return Ok(new { filePath = $"images/{fileName}" });
            StaticResModel resModel = new StaticResModel
            {
                FileUrl = ContentUrl.GetUrl(model, _configuration),
                Signature = serverSig,
                ErrorCode = "SUCCESS"
            };
            Console.Write($"AddPreviewFile: {model.Folder}");
            // Console.Write($"AddPreviewFile: {model.AddPreviewFile}");
            // if(model.AddPreviewUrl)
            // {
            //     if(String.IsNullOrEmpty(model.PreviewFile))
            //     {
            //         throw new ErrorResponseException(
            //             StatusCodes.Status400BadRequest,
            //             $"PreviewFile can not be null",
            //             new List<Error>()
            //         );
            //     }

            //     byte[] prevBytes = Convert.FromBase64String(model.PreviewFile);

            //     // using(var prevMs = new MemoryStream())
            //     // {
            //     //     await model.PreviewFileData!.CopyToAsync(prevMs);
            //     //     prevBytes = prevMs.ToArray();
            //     // }
            //     // now = now.AddSeconds(1);
            //     // long prevTs = DateConverter.ToTimestamp(now);
            //     var previewName = $"{fileName}_{fileTs}.png";
            //     string previewPath = Path.Combine(filePath, previewName);
            //     await System.IO.File.WriteAllBytesAsync(previewPath, prevBytes);
            //     Staticfile relatedFile = new Staticfile{
            //         Name=previewName,
            //         FolderId=folder.FolderId,
            //         Path=Path.Combine(model.Folder!, previewName),
            //         Type="image/png",
            //         Size=prevBytes.Length,
            //         Timestamp=fileTs,
            //         // FileData=prevBytes,
            //         ParentFileId=staticfile.StaticfileId
            //     };

            //     await _context.Staticfiles.AddAsync(relatedFile);
            //     result = await _context.SaveChangesAsync();

            //     // using(var prevStream = new MemoryStream(prevBytes))
            //     // {
            //     // }


            //     if(result <= 0)
            //     {
            //         throw new ErrorResponseException(
            //             StatusCodes.Status500InternalServerError,
            //             "Failed saving Staticfile",
            //             new List<Error>()
            //         );
            //     }

            //     // string prevUrl = Path.Combine(url, contentApi, relatedFile.Name);
            //     resModel.PreviewUrl=ContentUrl.ToContentUrl(relatedFile, _configuration);
            // }
            return resModel;
            // using (var memoryStream = new MemoryStream(fileBytes))
            // {
            // }
        }
    }
}