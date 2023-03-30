using static_sv.Models;

namespace static_sv.Services
{
    public class ContentUrl
    {
        
        public static string ToContentUrl(Staticfile staticfile, IConfiguration configuration)
        {
            string url = configuration["ASPNETCORE_DOMAIN_URL"];
            string contentApi = configuration["Static:Api:Content"];
            string fileUrl = Path.Combine(url, contentApi, staticfile.Name!);
            return $"{fileUrl}?id={staticfile.StaticfileId}&filetype={staticfile.Type!.Split("/")[1]}";
        }
    }
}