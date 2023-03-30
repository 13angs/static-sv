using Microsoft.EntityFrameworkCore;
using static_sv.DTOs;
using static_sv.Exceptions;
using static_sv.Interfaces;
using static_sv.Models;
using static_sv.Stores;

namespace static_sv.Services
{
    public class FolderService : IFolder
    {
        private readonly ILogger<FolderService> _logger;
        private readonly StaticContext _context;
        private readonly IConfiguration _configuration;
        public FolderService(ILogger<FolderService> logger, StaticContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }
        public async Task<Folder> CreateFolder(Folder folder)
        {
            // check if folder exist
            Folder? existing = await _context.Folders
                .FirstOrDefaultAsync(f => f.Path == folder.Path);

            if(existing != null)
            {
                _logger.LogInformation($"Folder with path {folder.Path} exist!");
                return existing;
            }
            
            await _context.Folders.AddAsync(folder);
            int result = await _context.SaveChangesAsync();

            if(result > 0)
                return folder;
            
            throw new ErrorResponseException(
                StatusCodes.Status500InternalServerError,
                "Failed saving folder",
                new List<Error>()
            );
        }

        public async Task<Folder> GetFolder(string path)
        {
            Folder? folder = await _context.Folders
                .FirstOrDefaultAsync(f => f.Path == path);

            if(folder == null)
                throw new ErrorResponseException(
                  StatusCodes.Status404NotFound,
                    $"Folder with path {path}",
                    new List<Error>()
                );
            return folder;
        }

        public IEnumerable<Folder> GetFolders(FolderQuery query)
        {
            IEnumerable<Folder> folders = new List<Folder>();
 
            if(query.Is == FolderQueryStore.SubFolder)       
            {
                return _context.Folders
                    .Where(f => f.ParentFolderId == query.FolderId)
                    .AsNoTracking();
            }     

            throw new ErrorResponseException(
                StatusCodes.Status501NotImplemented,
                "Type not implement",
                new List<Error>()
            );
        }

        public async Task RemoveFolder(long folderId)
        {
            Folder? folder = await _context.Folders.FirstOrDefaultAsync(f => f.FolderId == folderId);
            // string folderFullPath = Path.Combine(_configuration["Static:Name"], folder!.Path!);

            // bool localFolder = Directory.Exists(folderFullPath);

            if(folder == null)
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    "Folder not exist in db",
                new List<Error>()
            );
            // if(!localFolder)
            //     throw new ErrorResponseException(
            //         StatusCodes.Status404NotFound,
            //         "Folder not exist in server",
            //     new List<Error>()
            // );

            // remove all the files
            IEnumerable<Staticfile> staticfiles = _context.Staticfiles
                .Where(s => s.FolderId == folder.FolderId);
            
            if(staticfiles.Any())
            {
                _context.Staticfiles.RemoveRange(staticfiles);
            }

            // remove the sub folders

            IEnumerable<Folder> subFolders = _context.Folders
                .Where(f => f.ParentFolderId == folder.FolderId);
            
            if(subFolders.Any())
            {
                _context.Folders.RemoveRange(subFolders);
            }

            _context.Folders.Remove(folder);
            int result = await _context.SaveChangesAsync();
            if(result > 0)
            {
                // Directory.Delete(folderFullPath, true);
                _logger.LogInformation("Successfully remove the folder");
                return;
            }

            throw new ErrorResponseException(
                StatusCodes.Status500InternalServerError,
                "Failed removing folder",
                new List<Error>()
            );
        }
    }
}