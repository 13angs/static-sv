using static_sv.DTOs;

namespace static_sv.Interfaces
{
    public interface IStaticDirectory
    {
        public StaticDirectoryModel GetDirectories(string path, string signature);
    }
}