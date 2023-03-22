using Microsoft.AspNetCore.Mvc;

namespace static_sv.Interfaces
{
    public interface IContent
    {
        public PhysicalFileResult GetContent(string name);
        public string GetMimeType(string extension);
    }
}