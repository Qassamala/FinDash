using FinDash.Data;
using FinDash.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinDash.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class FinancialController : ControllerBase
    {

        private readonly InstrumentService _instrumentService;
        public FinancialController(InstrumentService instrumentService)
        {
            _instrumentService= instrumentService;
        }

        [HttpPost("AddStaticStockData")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddStaticStockData()
        {
            try
            {
                await _instrumentService.AddStaticStockData();
                return Ok("Static stock data added successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception details here for debugging
                return StatusCode(500, new { Message = "An error occurred while adding static stock data.", Error = ex.Message });
            }
        }


        // Test endpoint so far to test Admin in token and only accessing data related to id found in token
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

            return Ok(new { Message = "Accessed admin data successfully!" });
        }
    }

}
