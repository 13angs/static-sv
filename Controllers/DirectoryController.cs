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
        private readonly string _signature;
        private readonly IConfiguration _configuration;

        public DirectoryController(IStaticDirectory directory, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _directory = directory;
            _configuration = configuration;
            _signature = contextAccessor.HttpContext!
                .Request.Headers[_configuration["Static:Header"]].ToString();
        }

        // [HttpGet]
        // public ActionResult<StaticDirectoryModel> GetDirectories()
        // {
        //     return _directory.GetDirectories("");
        // }

        [HttpGet]
        public ActionResult<StaticDirectoryModel> GetDirectories([FromQuery] string query)
        {
            return _directory.GetDirectories(query, _signature);
        }
    }
}