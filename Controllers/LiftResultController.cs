using Microsoft.AspNetCore.Mvc;
using PowerliftingCompareResult.Models;
using System.Linq;

namespace PowerliftingCompareResult.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LiftResultController : ControllerBase
    {
        private static readonly IEnumerable<LiftResult> liftResultModels = new List<LiftResult>
      {
        new LiftResult
        {
          Name = "John Doe",
          Total = 500,
          Squat = 200,
          Bench = 150
        }
      };

        [HttpPost]
        public IActionResult Post([FromBody] LiftInput input)
        {
            try
            {

                string eventName;
                float inputTotal;
                if (input.Total > 0)
                {
                    eventName = "Total";
                    inputTotal = input.Total;
                }
                else if (input.Squat > 0)
                {
                    eventName = "Squat";
                    inputTotal = input.Squat;
                }
                else if (input.Bench > 0)
                {
                    eventName = "Bench";
                    inputTotal = input.Bench;
                }
                else if (input.Deadlift > 0)
                {
                    eventName = "Deadlift";
                    inputTotal = input.Deadlift;
                }
                else
                {
                    return BadRequest("No valid input provided.");
                }
                // Pobierz 5 osób z lepszym wynikiem
                var betterResults = GetBetterResults(eventName, inputTotal);
                // Dodaj samego siebie poki co puste bo wszytsko leci w total i tpye
                var userResult = new LiftResult
                {
                    Name = "Your Result",
                    Total = input.Total,
                    Squat = input.Squat,
                    Bench = input.Bench,
                    Deadlift = input.Deadlift
                };

                // Pobierz 5 osób z gorszym wynikiem
                var worseResults = GetWorstResults(eventName, inputTotal);

                // Połącz wyniki
                var results = betterResults.Concat(new List<LiftResult> { userResult }).Concat(worseResults).ToList();


                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostYourResult: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }
        private IEnumerable<LiftResult> GetBetterResults(string eventName, float inputThreshold)
        {
            var betterResults = liftResultModels
                .Where(lr => lr.GetValueByEventName(eventName) >= inputThreshold)
                .OrderBy(lr => lr.GetValueByEventName(eventName))
                .Take(5)
                .ToList();

            return betterResults;
        }
        private IEnumerable<LiftResult> GetWorstResults(string eventName, float inputThreshold)
        {
            var betterResults = liftResultModels
                .Where(lr => lr.GetValueByEventName(eventName) < inputThreshold)
                .OrderBy(lr => lr.GetValueByEventName(eventName))
                .Take(5)
                .ToList();

            return betterResults;
        }




    }
}

