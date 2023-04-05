using Microsoft.AspNetCore.Mvc;
using static_sv.Interfaces;
using Microsoft.Net.Http.Headers;
using static_sv.DTOs;
using static_sv.Models;
using static_sv.Exceptions;

namespace static_sv.Services
{
    public class ContentService : ControllerBase, IContent
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;

        private readonly IStaticfile _static;
        public ContentService(IHostEnvironment env, IConfiguration configuration, IStaticfile @static)
        {
            _env = env;
            _configuration = configuration;
            _static = @static;
        }
        public PhysicalFileResult GetContent(string name, ContentQueryModel model)
        {
            // Get the file path
            var filePath = Path.Combine(_env.ContentRootPath, _configuration["Static:Name"], _configuration["Static:FilePath"]);

            // Check if the file exists
            string[] localFiles = Directory.GetFiles(filePath, name, SearchOption.AllDirectories);

            if(!localFiles.Any())
            {
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    "File not found in server",
                    new List<Error>()
                );
            }
            string localFile = localFiles[0];

            // if (!System.IO.File.Exists(file))
            // {
            //     throw new ErrorResponseException(
            //         StatusCodes.Status404NotFound,
            //         "File not found",
            //         new List<Error>()
            //     );
            // }

            StaticfileQuery staticfileQuery = new StaticfileQuery{
                StaticfileId=model.Id,
                Is=StaticfileQueryStore.Staticfile
            };

            var files = _static.GetStaticfiles(staticfileQuery);

            if(!files.Any())
            {
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    "File not found in db",
                    new List<Error>()
                );
            }

            var file = files.ElementAt(0);

            // Get the file extension
            // var extension = Path.GetExtension(localFile);

            // // Get the content type based on the extension
            // var contentType = MediaTypeHeaderValue
            //     .Parse(GetMimeType(extension)).ToString();

            // Return the image file

            // using (MemoryStream ms = new MemoryStream(file.FileData!))
            // {
            //     byte[] pngBytes = ms.ToArray();

            //     return File(pngBytes, file.Type!);
            // }
            return PhysicalFile(localFile, file.Type!);
        }

        public string GetMimeType(string extension)
        {
            switch (extension.ToLowerInvariant())
            {
                case ".txt":
                    return "text/plain";
                case ".html":
                case ".htm":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".js":
                    return "text/javascript";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".mpeg":
                case ".mpg":
                    return "audio/mpeg";
                case ".mp4":
                    return "video/mp4";
                case ".pdf":
                    return "application/pdf";
                case ".zip":
                    return "application/zip";
                case ".doc":
                case ".docx":
                    return "application/msword";
                case ".xls":
                case ".xlsx":
                    return "application/vnd.ms-excel";
                case ".ppt":
                case ".pptx":
                    return "application/vnd.ms-powerpoint";
                default:
                    return "application/octet-stream";
            }
        }
    }
}