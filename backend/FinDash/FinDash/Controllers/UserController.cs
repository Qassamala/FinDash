using FinDash.Data;
using FinDash.DTOs;
using FinDash.Models;
using FinDash.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinDash.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly FinDashDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly TokenService _tokenService;

        public UserController(FinDashDbContext context, PasswordService passwordService, TokenService tokenService)
        {
            _context = context;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] UserDTO userDTO)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == userDTO.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists.");
            }

            var salt = _passwordService.GenerateSalt();
            var passwordHash = _passwordService.HashPassword(userDTO.Password, salt);

            // Create new User
            var user = new User
            {
                Username = userDTO.Username,
                PasswordHash = passwordHash,
                Salt = salt
            };

            // Add to database
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.InnerException?.Message);
                // Handle exception (e.g., unique constraint violation)
                return Conflict("Could not create user.");
            }

            string token = _tokenService.GenerateToken(user.Username, user.Id);

            return Ok(new
            {
                token,
                message = "User created successfully"
            });
        }
    }


}
