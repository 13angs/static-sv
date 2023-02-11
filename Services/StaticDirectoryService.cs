using Newtonsoft.Json;
using static_sv.DTOs;
using static_sv.Interfaces;

namespace static_sv.Services
{
    public class StaticDirectoryService : IStaticDirectory
    {
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _configuration;

        private readonly ILogger<StaticDirectoryModel> _logger;
        public StaticDirectoryService(IHostEnvironment env, IConfiguration configuration, ILogger<StaticDirectoryModel> logger)
        {
            _env = env;
            _configuration = configuration;
            _logger = logger;
        }
        public StaticDirectoryModel GetDirectories(string path)
        {
            var root = Path.Combine(_env.ContentRootPath, _configuration["Static:Name"]);
            var directory = new DirectoryInfo(Path.Combine(root, path));
            var subdirectories = directory.GetDirectories();
            var files = directory.GetFiles();

            var directories = new List<DirectoryInfo>();
            foreach (var subdirectory in subdirectories)
            {
                directories.Add(subdirectory);
            }
            StaticDirectoryModel model = new StaticDirectoryModel
            {
                Directories = subdirectories.Select(d => 
                        new DirectoryModel{
                            Name=d.Name,
                            FullName=d.FullName,
                            Extension=d.Extension,
                        }
                    ).ToList(),
                Files = files.Select(d => 
                        new FileModel{
                            Name=d.Name,
                            FullName=d.FullName,
                            Extension=d.Extension,
                        }
                    ).ToList()
            };

            // _logger.LogInformation(JsonConvert.SerializeObject(model));

            return model;
        }

    }
}