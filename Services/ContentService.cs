using Microsoft.AspNetCore.Mvc;
using static_sv.Interfaces;
using Microsoft.Net.Http.Headers;
using static_sv.Exceptions;

namespace static_sv.Services
{
    public class ContentService : ControllerBase, IContent
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;
        public ContentService(IHostEnvironment env, IConfiguration configuration)
        {
            _env = env;
            _configuration = configuration;
        }
        public PhysicalFileResult GetContent(string name)
        {
            // Get the file path
            var typePath = Path.Combine(_env.ContentRootPath, _configuration["Static:Name"]);

            // Check if the file exists
            string[] files = Directory.GetFiles(typePath, name, SearchOption.AllDirectories);

            if(!files.Any())
            {
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    "File not found",
                    new List<Error>()
                );
            }
            string file = files[0];

            if (!System.IO.File.Exists(file))
            {
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    "File not found",
                    new List<Error>()
                );
            }

            // Get the file extension
            var extension = Path.GetExtension(file);

            // Get the content type based on the extension
            var contentType = MediaTypeHeaderValue
                .Parse(GetMimeType(extension)).ToString();

            // Return the image file
            return PhysicalFile(file, contentType);
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