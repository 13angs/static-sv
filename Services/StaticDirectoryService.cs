using System.Globalization;
using Newtonsoft.Json;
using static_sv.DTOs;
using static_sv.Exceptions;
using static_sv.Interfaces;
using static_sv.Stores;

namespace static_sv.Services
{
    public class StaticDirectoryService : IStaticDirectory
    {
        private readonly IHostEnvironment _env;
        private readonly IConfiguration _configuration;

        private readonly ILogger<StaticDirectoryModel> _logger;
        private readonly IRequestValidator _reqVal;
        public StaticDirectoryService(IHostEnvironment env, IConfiguration configuration, ILogger<StaticDirectoryModel> logger, IRequestValidator reqVal)
        {
            _env = env;
            _configuration = configuration;
            _logger = logger;
            _reqVal = reqVal;
        }
        public StaticDirectoryModel GetDirectories(string query, string signature)
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

            if(String.IsNullOrEmpty(staticQuery.GroupId) && staticQuery.Is == QueryTypeStore.Group)
            {
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    "GroupId not found or is empty",
                    new List<Error>()
                );
            }

            var root = Path.Combine(_env.ContentRootPath, _configuration["Static:Name"]);
            string dirPath = Path.Combine(root, staticQuery.Type!, staticQuery.GroupId!);
            var directory = new DirectoryInfo(dirPath);
            var subdirectories = directory.GetDirectories();

            var files = directory.GetFiles().Take(staticQuery.Limit);
            if(!String.IsNullOrEmpty(staticQuery.Name))
            {
                files = files.Where(f => f.Name.Contains(staticQuery.Name)).ToArray();
            }

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
                            Name=d.Name.Split("_")[0].Replace("-", " "),
                            FullName=d.FullName,
                            Extension=d.Extension,
                            Type=staticQuery.Type,
                            CreatedDate=DateTime.ParseExact(d.Name.Split("_")[1].Split(".")[0], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                            Url=Path.Combine(_configuration["ASPNETCORE_DOMAIN_URL"], _configuration["Static:Api:Content"], staticQuery.Type!, d.Name)
                        }
                    ).ToList()
            };

            // _logger.LogInformation(JsonConvert.SerializeObject(model));

            return model;
        }

    }
}