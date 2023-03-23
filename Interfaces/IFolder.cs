using static_sv.Models;

namespace static_sv.Interfaces
{
    public interface IFolder
    {
        public Task<Folder> CreateFolder(Folder folder);
        public Task RemoveFolder(long folderId);
    }
}