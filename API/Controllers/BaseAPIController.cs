using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{    
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[Controller]")] 
    public class BaseApiController : ControllerBase
    {
        
    }
}