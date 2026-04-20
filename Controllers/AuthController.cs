using Microsoft.AspNetCore.Mvc;
using SchoolManagementAPI.DTOs;
using SchoolManagementAPI.Services;

namespace SchoolManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            return Ok(_auth.Register(dto));
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var token = _auth.Login(dto);
            if (token == null) return Unauthorized();

            return Ok(new { token });
        }
    }
}