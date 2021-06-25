using Microsoft.AspNetCore.Mvc;

namespace TestStub.Controllers
{
    [Route("rating/v1/[controller]")]
    [ApiController]
    public class TestRatingController : ControllerBase
    {
        [HttpGet]
        public string Ping()
        {
            return "Service Check Pinned at Test Stub";
        }

    }
}
