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

                var betterResults = liftResultModels
                    .Where(lr => LiftHelper.GetValueBasedOnEvent(inputTotal, eventName) > inputTotal)
                    .OrderBy(lr => LiftHelper.GetValueBasedOnEvent(inputTotal, eventName))
                    .Take(5)
                    .ToList();

                // Dodaj wynik użytkownika
                var userResult = new LiftResult
                {
                    Name = "Your Result",
                    Total = inputTotal,
                    Sex = "N/A",
                    Age = 0,
                    WeightClass = "N/A"
                };

                // Pobierz 5 osób z gorszym wynikiem
                var worseResults = liftResultModels
                    .Where(lr => LiftHelper.GetValueBasedOnEvent(inputTotal, eventName) < inputTotal)
                    .OrderByDescending(lr => LiftHelper.GetValueBasedOnEvent(inputTotal, eventName))
                    .Take(5)
                    .ToList();

                // Połącz wyniki
                var results = betterResults.Concat(new List<LiftResult> { userResult }).Concat(worseResults).ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in PostYourResult: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }

        }

    }
}

