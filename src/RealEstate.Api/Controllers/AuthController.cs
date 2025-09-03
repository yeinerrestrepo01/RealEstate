using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstate.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config) => _config = config;

        public record LoginRequest(string Username, string? Role);

        [HttpPost("token")]
        [AllowAnonymous]
        public IActionResult CreateToken([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Username))
                return BadRequest("username requerido.");

            var secret = _config["Jwt:Secret"];
            if (string.IsNullOrWhiteSpace(secret))
                return StatusCode(500, "Falta Jwt:Secret en configuración.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, req.Username),
                new(JwtRegisteredClaimNames.UniqueName, req.Username),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            if (!string.IsNullOrWhiteSpace(req.Role))
                claims.Add(new Claim(ClaimTypes.Role, req.Role!));

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
