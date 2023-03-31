using Microsoft.AspNetCore.Mvc;
using static_sv.DTOs;
using static_sv.Interfaces;

namespace static_sv.Controllers
{
    [ApiController]
    [Route("api/v1/statics")]
    public class StaticController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IRequestValidator requestValidator;
        private readonly IStaticfile _staticSv;
        private readonly string _xStaticSig;

        public StaticController(IConfiguration configuration, IHttpContextAccessor contextAccessor, IRequestValidator requestValidator, IStaticfile staticSv)
        {
            this.configuration = configuration;
            this.contextAccessor = contextAccessor;
            this.requestValidator = requestValidator;
            _staticSv = staticSv;

            _xStaticSig = contextAccessor.HttpContext!
                .Request.Headers[configuration["Static:Header"]].ToString();
        }

        // [HttpGet]
        // public ActionResult<IEnumerable<string>> GetImages([FromQuery]StaticQuery queryParams)
        // {
        //     return Ok(_staticSv.GetImages(queryParams, _xStaticSig));
        // }

        [HttpPost]
        [RequestSizeLimit(268_435_456)]
        public async Task<ActionResult<StaticResModel>> CreateImage([FromBody]StaticModel model)
        {
            return await _staticSv.CreateFile(model, _xStaticSig);
        }
    
        [HttpDelete]
        public async Task<ActionResult> RemoveImage([FromBody] StaticModel model)
        {
            await _staticSv.DeleteFile(model.Url!, _xStaticSig);
            return Ok();
        }
    }
}