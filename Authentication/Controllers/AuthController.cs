using Authentication.Model;
using Authentication.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Controllers
{
    [Authorize]
    [Route("ue-v1-auth/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJWTAuthenticationManager jWTAuthenticationManager;
        private readonly ITokenRefresher tokenRefresher;

        public AuthController(IJWTAuthenticationManager jWTAuthenticationManager, ITokenRefresher tokenRefresher)
        {
            this.jWTAuthenticationManager = jWTAuthenticationManager;
            this.tokenRefresher = tokenRefresher;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserCred userCred)
        {
            var token = jWTAuthenticationManager.Authenticate(userCred.Username, userCred.Password);

            if (token == null)
                return Unauthorized();

            return Ok(token);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshCred refreshCred)
        {
            var token = tokenRefresher.Refresh(refreshCred);

            if (token == null)
                return Unauthorized();

            return Ok(token);
        }
    }
}
