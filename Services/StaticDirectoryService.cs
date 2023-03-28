using Newtonsoft.Json;
using static_sv.DTOs;
using static_sv.Interfaces;
using static_sv.Models;
using static_sv.Stores;

namespace static_sv.Services
{
    public class StaticDirectoryService : IStaticDirectory
    {
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _configuration;

        private readonly ILogger<StaticDirectoryModel> _logger;
        private readonly IRequestValidator _reqVal;
        private readonly IFolder _folder;
        private readonly IStaticfile _static;
        public StaticDirectoryService(IHostEnvironment env, IConfiguration configuration, ILogger<StaticDirectoryModel> logger, IRequestValidator reqVal, IFolder folder, IStaticfile @static)
        {
            _env = env;
            _configuration = configuration;
            _logger = logger;
            _reqVal = reqVal;
            _folder = folder;
            _static = @static;
        }
        public async Task<StaticDirectoryModel> GetDirectories(string path, string query, string signature)
        {
            var content = new{};
            _reqVal.Validate(content, signature);
            Console.WriteLine(query);
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            // Split the string into key-value pairs
            string[] parts = query.Split(' ');
            foreach (string part in parts)
            {
                string[] pair = part.Split(':');
                if (pair.Length == 2)
                {
                    keyValuePairs.Add(pair[0], pair[1]);
                }
            }

            // Deserialize the key-value pairs into an object
            string json = JsonConvert.SerializeObject(keyValuePairs);
            StaticQuery staticQuery = JsonConvert.DeserializeObject<StaticQuery>(json)!;

            // if(String.IsNullOrEmpty(staticQuery.GroupId) && staticQuery.Is == QueryTypeStore.Group)
            // {
            //     throw new ErrorResponseException(
            //         StatusCodes.Status404NotFound,
            //         "GroupId not found or is empty",
            //         new List<Error>()
            //     );
            // }

            // find folder in db
            Folder folder = await _folder.GetFolder(path);

            // get the nested/sub folders
            FolderQuery folderQuery = new FolderQuery{
                Is=FolderQueryStore.SubFolder,
                FolderId=folder.FolderId
            };
            var subFolders = _folder.GetFolders(folderQuery);

            var root = Path.Combine(_env.ContentRootPath, _configuration["Static:Name"]);
            string dirPath = Path.Combine(root, path);
            var directory = new DirectoryInfo(dirPath);

            // get all the files
            StaticfileQuery staticfileQuery = new StaticfileQuery{
                Is=StaticfileQueryStore.Folder,
                FolderId=folder.FolderId,
                Type=staticQuery.Type,
                Limit=staticQuery.Limit,
                Name=staticQuery.Name
            };
            var staticfiles = _static.GetStaticfiles(staticfileQuery);
            // var subdirectories = directory.GetDirectories();

            // var files = directory.GetFiles().Take(staticQuery.Limit);
            // if(!String.IsNullOrEmpty(staticQuery.Name))
            // {
            //     files = files.Where(f => f.Name.Contains(staticQuery.Name)).ToArray();
            // }

            // var directories = new List<DirectoryInfo>();
            // foreach (var subdirectory in subdirectories)
            // {
            //     directories.Add(subdirectory);
            // }
            string strStatic = JsonConvert.SerializeObject(staticfiles);
            IEnumerable<StaticfileModel>? staticfileModels = JsonConvert.DeserializeObject<IEnumerable<StaticfileModel>>(strStatic);
            IEnumerable<StaticfileModel>? updatedStatics = new List<StaticfileModel>();
            if(staticfileModels!.Any())
            {
                updatedStatics = staticfileModels!.Select(s => new StaticfileModel{
                    StaticfileId=s.StaticfileId,
                    Name=s.Name,
                    Path=s.Path,
                    Type=s.Type,
                    Size=s.Size,
                    FolderId=s.FolderId,
                    Timestamp=s.Timestamp,
                    Url=Path.Combine(_configuration["ASPNETCORE_DOMAIN_URL"], _configuration["Static:Api:Content"], s.Name!)
                });
            }

            StaticDirectoryModel model = new StaticDirectoryModel
            {
                Folders = subFolders.ToList(),
                Staticfiles = updatedStatics!.ToList()
            };

            // _logger.LogInformation(JsonConvert.SerializeObject(model));

            return model;
        }

    }
}