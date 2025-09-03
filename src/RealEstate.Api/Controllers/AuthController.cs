using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Application.Services;

namespace RealEstate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _auth;

        public AuthController(IConfiguration config, IAuthService auth)
        {
            _config = config;
            _auth = auth;
        }

        public record LoginRequest(string Username, string Password);

        [HttpPost("token")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateToken([FromBody] LoginRequest req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
                return BadRequest("username y password son requeridos.");

            var user = await _auth.ValidateAsync(req.Username, req.Password, ct);
            if (user is null) return Unauthorized("Credenciales inválidas.");

            var secret = _config["Jwt:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
                return StatusCode(500, "Falta Jwt:Secret en configuración.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Username),
                new(JwtRegisteredClaimNames.UniqueName, user.Username),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, user.Role)
            };

            var expires = DateTime.UtcNow.AddHours(1);
            var token = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { access_token = jwt, token_type = "Bearer", expires_in = (int)(expires - DateTime.UtcNow).TotalSeconds });
        }
    }
}