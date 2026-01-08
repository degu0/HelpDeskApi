using HelpDeskApi.DTOs;
using HelpDeskApi.Service;
using HelpDeskApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Security.Claims;

namespace HelpDeskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        private readonly AuthService _authService;
        private readonly JwtService _jwtService;
        public UserController(IUserService service, AuthService authService, JwtService jwtService)
        {
            _service = service;
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetId(int id)
        {
            var user = await _service.GetId(id);
            if (user == null)
                return NotFound(new { mensagem = "Usuario não encontrado." });

            return Ok(user);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetUser()
        {
            ClaimsPrincipal currentUser = this.User;

            int id;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            int.TryParse(userId, out id);

            var user = await _service.GetId(id);
            if (user == null)
                return NotFound(new { mensagem = "Usuario não encontrado." });

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreatedUser([FromBody] CreateUserDto dto)
        {
            if (!EmailValido(dto.Email))
                return Unauthorized(new { message = "Email invalido."});

            var user = await _service.CreatedUser(dto);

            return CreatedAtAction(nameof(GetId), new { id = user.Id }, user);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
        {
            if(!EmailValido(dto.Email))
                return Unauthorized(new { message = "Email invalido." });

            var user = await _authService.Login(dto);

            if(user == null)
                return Unauthorized(new { message = "Email ou senha inválidos" });

            var token = _jwtService.GenerateToken(user);


            return Ok(new
            {
               token
            });
        }

        private bool EmailValido(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
