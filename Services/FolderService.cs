using Microsoft.EntityFrameworkCore;
using static_sv.Exceptions;
using static_sv.Interfaces;
using static_sv.Models;

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

        public async Task RemoveFolder(long folderId)
        {
            Folder? folder = await _context.Folders.FirstOrDefaultAsync(f => f.FolderId == folderId);
            string folderFullPath = Path.Combine(_configuration["Static:Name"], folder!.Path!);

            bool localFolder = Directory.Exists(folderFullPath);

            if(folder == null)
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    "Folder not exist in db",
                new List<Error>()
            );
            if(!localFolder)
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    "Folder not exist in server",
                new List<Error>()
            );

            _context.Folders.Remove(folder);
            int result = await _context.SaveChangesAsync();
            if(result > 0)
            {
                Directory.Delete(folderFullPath, true);
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