using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TestStub.Models;
using TestStub.Repository;

namespace TestStub.Controllers
{
    [Route("test/v1/[controller]")]
    [ApiController]
    public class TestAuthController : ControllerBase
    {

        private readonly ITestAuthRepository<IAuthBaseEntity> _TestAuthRepositorCaller;
        private readonly ILogger<TestAuthController> _logger;

        public TestAuthController(ILogger<TestAuthController> logger, ITestAuthRepository<IAuthBaseEntity> ITestAuthRepositorCaller)
        {
            _logger = logger;
            _TestAuthRepositorCaller = ITestAuthRepositorCaller;
        }

        [HttpPost]
        public async Task<AuthResponse> AuthToken(AuthRequest AuthRequest)
        {
            return await _TestAuthRepositorCaller.AuthCallerAsync(AuthRequest);
        }

    }
}
