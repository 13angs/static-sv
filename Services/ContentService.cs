using Microsoft.AspNetCore.Mvc;
using static_sv.Interfaces;
using static_sv.DTOs;
using static_sv.Exceptions;
using Amazon.S3;
using Amazon.S3.Model;

namespace static_sv.Services
{
    public class ContentService : ControllerBase, IContent
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _env;
        private readonly IStaticfile _static;
        private readonly IAmazonS3 _s3Client;
        public ContentService(IHostEnvironment env, IConfiguration configuration, IStaticfile @static)
        {
            _env = env;
            _configuration = configuration;
            _static = @static;

            var options = new AmazonS3Config
            {
                ServiceURL = _configuration["S3Config:ServiceUrl"]
            };

            // Initialize the S3 client here, or use dependency injection to inject it.
            _s3Client = new AmazonS3Client(_configuration["S3Config:AccessKey"], _configuration["S3Config:SecretKey"], options);
        }
        public async Task<FileContentResult> GetContent(string name, ContentQueryModel model)
        {
            try
            {
                // Create a request to get the object with a specific "Content-Type"
                string objectKey = Path.Combine("files", model!.Directory!, name);
                string bucketName = _configuration["S3Config:BucketName"];
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey,
                };

                // Set the desired "Content-Type" header value
                // request.Headers.ContentType = "image/*";

                // Get the object from S3
                var response = await _s3Client.GetObjectAsync(request);

                // Read the response.ResponseStream into a byte array
                using (var memoryStream = new MemoryStream())
                {
                    await response.ResponseStream.CopyToAsync(memoryStream);
                    var fileContents = memoryStream.ToArray();

                    // Determine the content type for the PhysicalFileResult
                    var contentType = response.Headers.ContentType;

                    // Return the image as a PhysicalFileResult
                    return File(fileContents, contentType, response.Key);
                }
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                // Handle the exception appropriately, e.g., return an error view
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    e.Message,
                    new List<Error>()
                );
            }

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