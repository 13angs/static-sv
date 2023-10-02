using Microsoft.AspNetCore.Mvc;
using static_sv.DTOs;

namespace static_sv.Interfaces
{
    public interface IContent
    {
        public Task<FileContentResult> GetContent(string name, ContentQueryModel model);
        public string GetMimeType(string extension);
    }
}