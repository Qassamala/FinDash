using FinDash.Data;
using FinDash.DTOs;
using FinDash.Helpers;
using FinDash.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            _instrumentService = instrumentService;
        }

        [HttpDelete("RemoveStock/{id}")]
        public async Task<IActionResult> DeleteStock(int id)
        {
            //Check authorization need to be DRY, create helper
            var userIdClaim = User.Claims.First(c => c.Type == "id");

            if (userIdClaim == null)
            {
                // Handle error - claim not found
                return Unauthorized(new { Message = "Invalid token" });
            }

            try
            {
                await _instrumentService.RemoveSavedStock(id);
                return Ok("Stock removed succesfully.");
            }
            catch (Exception e)
            {

                return StatusCode(500, new { Message = "An error occurred while removing stock.", Error = e.Message });
            }
        }

        [HttpPost("SaveStock")]
        public async Task<IActionResult> SaveStock([FromBody] AddStockDTO addStockDTO)
        {

            var userIdClaim = User.Claims.First(c => c.Type == "id");

            if (userIdClaim == null)
            {
                // Handle error - claim not found
                return Unauthorized(new { Message = "Invalid token" });
            }

            var userIdFromToken = int.Parse(userIdClaim.Value);

            // Check if the ID in the request matches the ID in the token
            if (addStockDTO.UserId != userIdFromToken)
            {
                return Unauthorized(new { Message = "Token mismatch, will be reported." });
            }

            try
            {
                await _instrumentService.AddStockToUser(addStockDTO);
                return Ok("Stock added succesfully.");
            }
            catch (Exception e)
            {

                return StatusCode(500, new { Message = "An error occurred while loading personal stock data.", Error = e.Message });
            }
        }


        [HttpGet("SavedStocks/{id}")]
        public IActionResult GetSavedStocks(int id)
        {

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

            try
            {
                List<StockViewDTO> savedStocks = _instrumentService.LoadSavedStocks(id);

                return Ok(savedStocks);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { Message = "An error occurred while loading personal stock data.", Error = ex.Message });
            }
        }

        [HttpGet("LoadStocks")]
        public IActionResult LoadStocks()
        {
            try
            {
                // Fetch the stocks from your database or external API
                List<StockViewDTO> stocks = _instrumentService.LoadStocks();

                return Ok(stocks);

            }
            catch (Exception ex)
            {
                // Log the exception details here for debugging
                return StatusCode(500, new { Message = "An error occurred while loading stocks data.", Error = ex.Message });
            }
        }

        [HttpPost("AddStaticStockData")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddStaticStockData([FromQuery] string region)
        {
            if (Enum.IsDefined(typeof(Region), region.ToUpper()))
            {
                try
                {
                    await _instrumentService.AddStaticStockData(region.ToUpper());
                    return Ok("Static stock data added successfully.");
                }
                catch (Exception ex)
                {
                    // Log the exception details here for debugging
                    return StatusCode(500, new { Message = "An error occurred while adding static stock data.", Error = ex.Message });
                }
            }
            else
            {
                return StatusCode(500, new { Message = "Invalid region provided." });
            }
        }

        [HttpPost("UpdateStockPrices")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStockPrices([FromQuery] string region)
        {
            if (Enum.IsDefined(typeof(Region), region.ToUpper()))
            {
                try
                {
                    await _instrumentService.GetStockPrices(region.ToUpper());
                    return Ok("Stock prices added successfully.");
                }
                catch (Exception ex)
                {
                    // Log the exception details here for debugging
                    return StatusCode(500, new { Message = "An error occurred while retrieving and updating stock prices.", Error = ex.Message });
                }
            }
            else
            {
                return StatusCode(500, new { Message = "Invalid region provided." });
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
