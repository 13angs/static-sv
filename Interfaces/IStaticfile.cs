using static_sv.DTOs;

namespace static_sv.Interfaces
{
    public interface IStaticfile
    {
        public Task<StaticResModel> CreateImage(StaticModel model, string xStaticSig);
        public Task DeleteImage(string url, string xStaticSig);
    }
}