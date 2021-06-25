using Microsoft.AspNetCore.Mvc;

namespace TestStub.Controllers
{
    [Route("test/v1/[controller]")]
    [ApiController]
    public class TestClientController : ControllerBase
    {
        // GET api/<SeviceCheckController>/5
        [HttpGet]
        public string Ping()
        {
            return "Service Check Pinned at Test Stub";
        }



    }
}
