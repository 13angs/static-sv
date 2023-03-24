using static_sv.DTOs;
using static_sv.Models;

namespace static_sv.Interfaces
{
    public interface IFolder
    {
        public Task<Folder> CreateFolder(Folder folder);
        public Task RemoveFolder(long folderId);

        public Task<Folder> GetFolder(string path);
        public IEnumerable<Folder> GetFolders(FolderQuery query);
    }
}