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
        private readonly NoyauSihService _noyauService;

        public AuthController(AuthService authService, NoyauSihService noyauService)
        {
            _authService = authService;
            _noyauService = noyauService;
        }

        // GET: api/login
        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginEndpointParams loginEndPointParams)
        {

            var authServiceResponse = await _authService.Login(loginEndPointParams.Username, loginEndPointParams.Password);
            if (authServiceResponse.IsLogedIn) {
                return Ok(authServiceResponse);
            }
            return BadRequest();
        }

        [HttpPost("login-cas")]
        public async Task<ActionResult> LoginCas(LoginEndpointCasParams loginEndPointParams)
        {

            var noyauServiceResponseCas = await _noyauService.VerifyTicketFromCas(loginEndPointParams.CasTicket, loginEndPointParams.Service);
            if (!noyauServiceResponseCas.IsSuccess || noyauServiceResponseCas.UserIdRes == null) 
            {
                return Unauthorized();
            }
            var noyauServiceReponseUser = await _noyauService.GetUserFromNoyauSih(noyauServiceResponseCas.UserIdRes);
            if (!noyauServiceReponseUser.IsSuccess) 
            {
                return Unauthorized();
            }
            var authServiceResponseUser = await _authService.CreateOrUpdateUserFromNoyauSih(noyauServiceReponseUser.NoyauSihUser);
            if (!authServiceResponseUser.IsSuccess) 
            {
                return BadRequest(authServiceResponseUser.Errors);
            }
            var authServiceResponseJwt = _authService.GenerateUserJwt(authServiceResponseUser.User);
            return Ok(authServiceResponseJwt);
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
