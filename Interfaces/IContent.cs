using Microsoft.AspNetCore.Mvc;
using static_sv.DTOs;

namespace static_sv.Interfaces
{
    public interface IContent
    {
        public PhysicalFileResult GetContent(string name, ContentQueryModel model);
        public string GetMimeType(string extension);
    }
}