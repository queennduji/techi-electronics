using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Techi.Electronics.ProductAPI.Controllers.v2
{
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/v{version:apiVersion}/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
    }
}
