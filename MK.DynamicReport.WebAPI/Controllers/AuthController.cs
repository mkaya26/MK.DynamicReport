using Microsoft.AspNetCore.Mvc;
using MK.DynamicReport.Application.Interfaces;
using MK.DynamicReport.Domain.DTOs;

namespace MK.DynamicReport.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var token = await _authService.AuthenticateAsync(request);
            if (token == null)
                return Unauthorized(new { message = "Geçersiz kullanıcı adı veya şifre." });

            return Ok(new { Token = token });
        }
    }
}
