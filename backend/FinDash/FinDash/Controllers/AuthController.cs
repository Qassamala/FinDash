using FinDash.Data;
using FinDash.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinDash.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly FinDashDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(FinDashDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("token")]
        public IActionResult GetToken([FromBody] User loginInfo)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == loginInfo.Username);

            if (user == null)
            {
                return Unauthorized();
            }

            // Hash the incoming password with the stored salt
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: loginInfo.PasswordHash,
                salt: Convert.FromBase64String(user.Salt),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            if (hashed != user.PasswordHash)
            {
                return Unauthorized();
            }

            // Generate Token (create claims and sign token)
            var claims = new Claim[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("id", user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("YourSecretKeyHere"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:ValidIssuer"],
                //audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }
}
