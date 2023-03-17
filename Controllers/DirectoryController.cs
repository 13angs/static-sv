using Microsoft.AspNetCore.Mvc;
using static_sv.DTOs;
using static_sv.Interfaces;

namespace static_sv.Controllers
{
    [Route("api/v1/statics/tree")]
    [ApiController]
    public class DirectoryController : ControllerBase
    {
        private readonly IStaticDirectory _directory;

        public DirectoryController(IStaticDirectory directory)
        {
            _directory = directory;
        }

        [HttpGet]
        public ActionResult<StaticDirectoryModel> GetDirectories()
        {
            return _directory.GetDirectories("");
        }

        [HttpGet("{*path}")]
        public ActionResult<StaticDirectoryModel> GetDirectories([FromRoute] string path)
        {
            return _directory.GetDirectories(path);
        }
    }
}