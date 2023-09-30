using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinDash.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class FinancialController : ControllerBase
    {
        [HttpGet("GetFinancialData")]
        public IActionResult GetFinancialData()
        {
            Console.WriteLine("was here");
            // ...logic here...
            return Ok(new { Message = "Accessed data successfully!" });
        }
    }

}
