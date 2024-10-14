using Microsoft.AspNetCore.Mvc;
using PowerliftingCompareResult.Models;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PowerliftingCompareResult.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LiftResultController : ControllerBase
    {
        private readonly ResultContext _context;

        public LiftResultController(ResultContext context)
        {
            _context = context;
        }

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

                var betterResults = GetBetterResults(eventName, inputTotal,input.Gender,input.Country);

                var userResult = new LiftResult
                {
                    Name = "Your Result",
                    Total = input.Total,
                    Squat = input.Squat,
                    Bench = input.Bench,
                    Deadlift = input.Deadlift                };

                var worseResults = GetWorstResults(eventName, inputTotal,input.Gender,input.Country);

                var results = betterResults.Concat(new List<LiftResult> { userResult }).Concat(worseResults).ToList();

                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostYourResult: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        private Expression<Func<LiftResult, float?>> GetSelector(string eventName)
        {
            switch (eventName)
            {
                case "Total":
                    return lr => lr.Total;
                case "Squat":
                    return lr => lr.Squat;
                case "Bench":
                    return lr => lr.Bench;
                case "Deadlift":
                    return lr => lr.Deadlift;
                default:
                    throw new ArgumentException("Invalid event name");
            }
        }

        private Expression<Func<LiftResult, bool>> GetBetterPredicate(string eventName, float inputThreshold,string gender, string country)
        {
            switch (eventName)
            {
                case "Total":
                    return lr => lr.Total >= inputThreshold && lr.Sex== gender &&(string.IsNullOrEmpty(country) || lr.Country == country);
                case "Squat":
                    return lr => lr.Squat >= inputThreshold && lr.Sex == gender && (string.IsNullOrEmpty(country) || lr.Country == country); ;
                case "Bench":
                    return lr => lr.Bench >= inputThreshold && lr.Sex == gender && (string.IsNullOrEmpty(country) || lr.Country == country); ;
                case "Deadlift":
                    return lr => lr.Deadlift >= inputThreshold && lr.Sex == gender && (string.IsNullOrEmpty(country) || lr.Country == country); ;
                default:
                    throw new ArgumentException("Invalid event name");
            }
        }

        private Expression<Func<LiftResult, bool>> GetWorstPredicate(string eventName, float inputThreshold,string gender, string country)
        {
            switch (eventName)
            {
                case "Total":
                    return lr => lr.Total < inputThreshold && lr.Sex == gender && (string.IsNullOrEmpty(country) || lr.Country == country); ;
                case "Squat":
                    return lr => lr.Squat < inputThreshold && lr.Sex == gender && (string.IsNullOrEmpty(country) || lr.Country == country); ;
                case "Bench":
                    return lr => lr.Bench < inputThreshold && lr.Sex == gender && (string.IsNullOrEmpty(country) || lr.Country == country); ;
                case "Deadlift":
                    return lr => lr.Deadlift < inputThreshold && lr.Sex == gender && (string.IsNullOrEmpty(country) || lr.Country == country); ;
                default:
                    throw new ArgumentException("Invalid event name");
            }
        }

        private IEnumerable<LiftResult> GetBetterResults(string eventName, float inputThreshold,string gender, string country)
        {
            var selector = GetSelector(eventName);
            var predicate = GetBetterPredicate(eventName, inputThreshold,gender,country);

            var betterResults = _context.LiftResults
                .Where(predicate)
                .OrderBy(selector)
                .Take(5)
                .ToList();

            return betterResults;
        }

        private IEnumerable<LiftResult> GetWorstResults(string eventName, float inputThreshold, string gender,string country)
        {
            var selector = GetSelector(eventName);
            var predicate = GetWorstPredicate(eventName, inputThreshold,gender,country);

            var worseResults = _context.LiftResults
                .Where(predicate)
                .OrderByDescending(selector)
                .Take(5)
                .ToList();

            return worseResults;
        }
        [HttpGet("GetCountries")]
        public IActionResult GetCountries()
        {
            try
            {
                var countries = _context.LiftResults
      .Where(lr => lr.Country != null && lr.Country != "")
      .Select(lr => lr.Country)
      .Distinct()
      .OrderBy(c => c)
      .ToList();


                return Ok(countries);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetCountries: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
