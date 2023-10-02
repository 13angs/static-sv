using Amazon.S3.Model;
using static_sv.DTOs;

namespace static_sv.Interfaces
{
    public interface IObjectStorage
    {
        public Task<PutObjectResponse> UploadFile(StaticModel model);
        public Task<StaticDirectoryModel> GetFiles(StaticQuery model);
    }
}