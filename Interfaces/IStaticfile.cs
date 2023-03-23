using static_sv.DTOs;
using static_sv.Models;

namespace static_sv.Interfaces
{
    public interface IStaticfile
    {
        public Task<StaticResModel> CreateFile(StaticModel model, string xStaticSig);
        public Task DeleteImage(string url, string xStaticSig);
        // public IEnumerable<string> GetImages(StaticQuery queryParams, string xStaticSig);
        public string GetStaticPath();
        public IEnumerable<Staticfile> GetStaticfiles(StaticfileQuery query);
    }
}