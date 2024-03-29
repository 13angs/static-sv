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
        private readonly IObjectStorage _objSv;
        public StaticDirectoryService(IHostEnvironment env, IConfiguration configuration, ILogger<StaticDirectoryModel> logger, IRequestValidator reqVal, IFolder folder, IStaticfile @static, IObjectStorage objSv)
        {
            _env = env;
            _configuration = configuration;
            _logger = logger;
            _reqVal = reqVal;
            _folder = folder;
            _static = @static;
            _objSv = objSv;
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
            staticQuery.Directory = path;

            // // find folder in db
            // Folder folder = await _folder.GetFolder(path);

            // // get the nested/sub folders
            // FolderQuery folderQuery = new FolderQuery{
            //     Is=FolderQueryStore.SubFolder,
            //     FolderId=folder.FolderId
            // };
            // var subFolders = _folder.GetFolders(folderQuery);

            // // var root = Path.Combine(_env.ContentRootPath, _configuration["Static:Name"]);
            // // string dirPath = Path.Combine(root, path);
            // // var directory = new DirectoryInfo(dirPath);

            // // get all the files
            // StaticfileQuery staticfileQuery = new StaticfileQuery{
            //     Is=StaticfileQueryStore.Folder,
            //     FolderId=folder.FolderId,
            //     Type=staticQuery.Type,
            //     Limit=staticQuery.Limit,
            //     Name=staticQuery.Name
            // };
            // var staticfiles = _static.GetStaticfiles(staticfileQuery);

            // string strStatic = JsonConvert.SerializeObject(staticfiles);
            // IEnumerable<StaticfileModel>? staticfileModels = JsonConvert.DeserializeObject<IEnumerable<StaticfileModel>>(strStatic);
            // IEnumerable<StaticfileModel>? updatedStatics = new List<StaticfileModel>();
            // if(staticfileModels!.Any())
            // {
            //     foreach(StaticfileModel s in staticfileModels!)
            //     {
            //         s.Url=ContentUrl.ToContentUrl(s, _configuration);
            //         string strRelFiles = JsonConvert.SerializeObject(s.RelatedFiles);
            //         s.Files = JsonConvert.DeserializeObject<IEnumerable<StaticfileModel>>(strRelFiles);

            //         if(s.Files != null)
            //         {
            //             foreach(StaticfileModel rf in s.Files!)
            //             {
            //                 rf.Url=ContentUrl.ToContentUrl(rf, _configuration);
            //             }
            //         }
            //         s.RelatedFiles=null;
            //     }
            //     // updatedStatics = staticfileModels!.Select(s => new StaticfileModel{
            //     //     StaticfileId=s.StaticfileId,
            //     //     Name=s.Name,
            //     //     Path=s.Path,
            //     //     Type=s.Type,
            //     //     Size=s.Size,
            //     //     FolderId=s.FolderId,
            //     //     Timestamp=s.Timestamp,
            //     //     Url=ContentUrl.ToContentUrl(s, _configuration),
            //     //     ParentFileId=s.ParentFileId,
            //     //     RelatedFiles=s.RelatedFiles
            //     // });
            // }

            // StaticDirectoryModel model = new StaticDirectoryModel
            // {
            //     Folders = subFolders.ToList(),
            //     Staticfiles = staticfileModels!.ToList()
            // };

            StaticDirectoryModel model = await _objSv.GetFiles(
                staticQuery
            );

            return model;
        }

    }
}