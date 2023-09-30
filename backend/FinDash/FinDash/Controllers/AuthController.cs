using FinDash.Data;
using FinDash.DTOs;
using FinDash.Models;
using FinDash.Services;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FinDash.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly FinDashDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;

        public AuthController(FinDashDbContext context, PasswordService passwordService, TokenService tokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        [HttpPost("token")]
        public IActionResult GetToken([FromBody] UserDTO userDTO)  // Assume UserDTO contains just Username and Password
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == userDTO.Username);

            if (user == null)
            {
                return Unauthorized();
            }

            // Validate the password using PasswordService
            if (!_passwordService.VerifyPassword(user.PasswordHash, user.Salt, userDTO.Password))
            {
                return Unauthorized();
            }

            // Generate the token using TokenService
            var token = _tokenService.GenerateToken(user);

            Console.WriteLine(user.Id);

            return Ok(new
            {
                token,
                user.IsAdmin,
                user.Id,
                message = "User authenticated successfully"
            });
        }
    }
}
