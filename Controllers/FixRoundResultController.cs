/*using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PowerliftingCompareResult.Models;

namespace PowerliftingCompareResult.Controllers
{
    [ApiController]
    [Route("FixRoundResult")]
    public class FixRoundResultController : ControllerBase
    {
        private readonly ResultContext _context;

        public FixRoundResultController(ResultContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("")]
        public IActionResult ExecuteProcedureFixRound()
        {
            try
            {
               // _context.Database.ExecuteSqlRaw("CALL \"FIX_ROUND_RESULT\"();");
                return Ok("Procedure 'FIX_ROUND_RESULT' was executed successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while executing the procedure: {ex.Message}");
            }
        }
    }

}
*/
using Microsoft.AspNetCore.Mvc;

namespace PowerliftingCompareResult.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FixRoundResultController : ControllerBase
    {
        [HttpPost("ExecuteProcedure")]
        public IActionResult ExecuteProcedure()
        {
            return Ok("Endpoint is working.");
        }
    }
}