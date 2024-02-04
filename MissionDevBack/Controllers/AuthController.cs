using Microsoft.AspNetCore.Mvc;
using MissionDevBack.Models.ControllerParams;
using MissionDevBack.Services;

namespace MissionDevBack.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // GET: api/login
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginEndpointParams loginEndPointParams)
        {

            var authServiceResponse = await _authService.Login(loginEndPointParams.username, loginEndPointParams.password);
            if (authServiceResponse.IsLogedIn) {
                return Ok(authServiceResponse);
            }
            return BadRequest();
        }

        // GET: api/refresh-token
        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken(RefreshTokenEnpointParams refreshTokenEndPointParams)
        {
            var authServiceResponse = await _authService.RefreshToken(refreshTokenEndPointParams.Jwt, refreshTokenEndPointParams.RefreshToken);
            if (authServiceResponse.IsLogedIn) {
                return Ok(authServiceResponse);
            }
            return BadRequest();
        }
    }
}
