using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PowerliftingCompareResult.Models;

namespace PowerliftingCompareResult.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CountryListController : ControllerBase
    {
        private readonly ResultContext _context;

        public CountryListController(ResultContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PopulateCountries()
        {
            var sql = @"
                INSERT INTO ""Countries"" (""Country"")
                SELECT DISTINCT ""Country""
                FROM ""LiftResults""
                WHERE ""Country"" IS NOT NULL
                  AND ""Country"" NOT IN (SELECT ""Country"" FROM ""Countries"")
            ";
            Console.WriteLine(sql);

            await _context.Database.ExecuteSqlRawAsync(sql);

            return Ok("Countries added successfully.");
        }
    }
}
