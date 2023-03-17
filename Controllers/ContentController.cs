using Microsoft.AspNetCore.Mvc;
using static_sv.Interfaces;

namespace static_sv.Controllers
{
    [Route("api/v1/statics/contents")]
    public class ContentController : Controller
    {
        private readonly ILogger<ContentController> _logger;
        private readonly IContent _content;

        public ContentController(ILogger<ContentController> logger, IContent content)
        {
            _logger = logger;
            _content = content;
        }

        // [HttpGet("{*path}")]
        [HttpGet("{type}/{name}")]
        public IActionResult Index([FromRoute] string type, [FromRoute] string name)
        {
            return _content.GetContent(type, name);
        }
    }
}