using Amazon.S3;
using Amazon.S3.Model;
using Newtonsoft.Json;
using static_sv.DTOs;
using static_sv.Exceptions;
using static_sv.Interfaces;

namespace static_sv.Services
{
    public class ObjectStorageService : IObjectStorage
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ObjectStorageService> _logger;
        public ObjectStorageService(IConfiguration configuration, ILogger<ObjectStorageService> logger)
        {
            _configuration = configuration;
            var options = new AmazonS3Config
            {
                ServiceURL = _configuration["S3Config:ServiceUrl"]
            };
            // Initialize the S3 client here, or use dependency injection to inject it.
            _s3Client = new AmazonS3Client(_configuration["S3Config:AccessKey"], _configuration["S3Config:SecretKey"], options);
            _logger = logger;
        }

        public async Task<StaticDirectoryModel> GetFiles(StaticQuery model)
        {
            // Create an Amazon S3 client with your credentials and region.
            StaticDirectoryModel dirModel = new StaticDirectoryModel();
            List<StaticfileModel> staticModels = new List<StaticfileModel>();
            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = _configuration["S3Config:BucketName"],
                    Prefix = Path.Combine("files", model.Directory!) + "/",
                    // MaxKeys = 5
                };

                ListObjectsV2Response response;
                do
                {
                    response = await _s3Client.ListObjectsV2Async(request);

                    foreach (var obj in response.S3Objects)
                    {
                        if (staticModels.Count() >= model.Limit)
                        {
                            break;
                        }

                        // Console.WriteLine($"Object Key: {obj.Key.Split("/").Last()}");
                        var metadataResponse = await _s3Client.GetObjectMetadataAsync(_configuration["S3Config:BucketName"], obj.Key);
                        var formattedKey = new StaticfileModel
                        {
                            Name = obj.Key.Split("/").Last(),
                            Url = ContentUrl.GetUrl(
                                    new StaticModel
                                    {
                                        Name = obj.Key.Split("/").Last(),
                                        Folder = model.Directory
                                    },
                                    _configuration
                                ),
                            ModifiedDate = obj.LastModified,
                            Size = obj.Size,
                            Type = metadataResponse.Headers.ContentType
                        };

                        if (!String.IsNullOrEmpty(model.Name))
                        {
                            if (obj.Key.Contains(model.Name))
                            {
                                // Console.WriteLine($"Matched Object Key: {obj.Key}");
                                staticModels.Add(formattedKey);
                            }
                            continue;
                        }
                        staticModels.Add(formattedKey);
                    }

                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                Console.WriteLine($"S3 Error: {amazonS3Exception.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
            // Console.WriteLine(JsonConvert.SerializeObject(staticModels));
            dirModel.Staticfiles = staticModels;
            return dirModel;
        }

        public async Task<DeleteObjectResponse> RemoveFile(string key)
        {
            try
            {
                var deleteObjectRequest = new DeleteObjectRequest
                {
                    BucketName = _configuration["S3Config:BucketName"],
                    Key = key, // Specify the key (file name) of the object to delete
                };

                var response = await _s3Client.DeleteObjectAsync(deleteObjectRequest);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    _logger.LogInformation($"Object with key '{key}' deleted successfully.");
                    return response;
                }
                else
                {
                    _logger.LogError($"Failed to delete object with key '{key}'.");
                    throw new ErrorResponseException(
                        StatusCodes.Status500InternalServerError,
                        $"Failed to delete object with key '{key}'.",
                        new List<Error>()
                    );
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                _logger.LogError($"S3 Error: {amazonS3Exception.Message}");
                throw new ErrorResponseException(
                    StatusCodes.Status500InternalServerError,
                    $"S3 Error: {amazonS3Exception.Message}",
                    new List<Error>()
                );
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception: {e.Message}");
                throw new ErrorResponseException(
                    StatusCodes.Status500InternalServerError,
                    $"Exception: {e.Message}",
                    new List<Error>()
                );
            }
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