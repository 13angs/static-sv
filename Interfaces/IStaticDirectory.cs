using static_sv.DTOs;

namespace static_sv.Interfaces
{
    public interface IStaticDirectory
    {
        public Task<StaticDirectoryModel> GetDirectories(string path, string query, string signature);
    }
}