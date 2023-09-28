using Amazon.S3;
using Amazon.S3.Model;
using static_sv.DTOs;
using static_sv.Interfaces;

namespace static_sv.Services
{
    public class ObjectStorageService : IObjectStorage
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        public ObjectStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
            var options = new AmazonS3Config
            {
                ServiceURL = _configuration["S3Config:ServiceUrl"]
            };
            // Initialize the S3 client here, or use dependency injection to inject it.
            _s3Client = new AmazonS3Client(_configuration["S3Config:AccessKey"], _configuration["S3Config:SecretKey"], options);
        }
        public async Task<PutObjectResponse> UploadFile(StaticModel model)
        {
            // string url = $"https://{_objSetting.Value.ServiceUrl}";

            // string[] separateDir = new string[]{
            //     MessageEventTypes.Image,
            //     MessageEventTypes.Audio,
            //     MessageEventTypes.Video,
            // };

            // string[] mediaTypes = model.ContentType!.Split("/");
            // string fileType = string.Empty;

            // if (separateDir.Contains(mediaTypes.First()))
            // {
            //     fileType = mediaTypes.First();
            // }
            // else
            // {
            //     fileType = "file";
            // }

            // url = Path.Combine($"https://{_objSetting.Value.ServiceUrl}", fileType);
            

            var request = new PutObjectRequest
            {
                BucketName = _configuration["S3Config:BucketName"],
                Key = Path.Combine("files", model.Folder!, model.Name!),
                ContentType = model.Type,
                FilePath = model.LocalPath,
                
                // CannedACL = S3CannedACL.PublicRead,
            };

            var response = await _s3Client.PutObjectAsync(request);

            return response;
        }
    }
}