using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinDash.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class FinancialController : ControllerBase
    {
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetFinancialData(int id)
        {
            // Retrieve the user ID from the token claims
            var userIdClaim = User.Claims.First(c => c.Type == "id");

            if (userIdClaim == null)
            {
                // Handle error - claim not found
                return Unauthorized(new { Message = "Invalid token" });
            }

            var userIdFromToken = int.Parse(userIdClaim.Value);

            // Check if the ID in the request matches the ID in the token
            if (id != userIdFromToken)
            {
                return NotFound();
            }

            // Still need to apply additional logic to check authorized data

            return Ok(new { Message = "Accessed data successfully!" });
        }
    }

}
