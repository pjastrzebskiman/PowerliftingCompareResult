﻿using Microsoft.AspNetCore.Mvc;
using PowerliftingCompareResult.Models;
using System.Linq;
using System.Linq.Expressions;

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

                var betterResults = GetBetterResults(eventName, inputTotal);

                var userResult = new LiftResult
                {
                    Name = "Your Result",
                    Total = input.Total,
                    Squat = input.Squat,
                    Bench = input.Bench,
                    Deadlift = input.Deadlift
                };

                var worseResults = GetWorstResults(eventName, inputTotal);

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

        private Expression<Func<LiftResult, bool>> GetBetterPredicate(string eventName, float inputThreshold)
        {
            switch (eventName)
            {
                case "Total":
                    return lr => lr.Total >= inputThreshold;
                case "Squat":
                    return lr => lr.Squat >= inputThreshold;
                case "Bench":
                    return lr => lr.Bench >= inputThreshold;
                case "Deadlift":
                    return lr => lr.Deadlift >= inputThreshold;
                default:
                    throw new ArgumentException("Invalid event name");
            }
        }

        private Expression<Func<LiftResult, bool>> GetWorstPredicate(string eventName, float inputThreshold)
        {
            switch (eventName)
            {
                case "Total":
                    return lr => lr.Total < inputThreshold;
                case "Squat":
                    return lr => lr.Squat < inputThreshold;
                case "Bench":
                    return lr => lr.Bench < inputThreshold;
                case "Deadlift":
                    return lr => lr.Deadlift < inputThreshold;
                default:
                    throw new ArgumentException("Invalid event name");
            }
        }

        private IEnumerable<LiftResult> GetBetterResults(string eventName, float inputThreshold)
        {
            var selector = GetSelector(eventName);
            var predicate = GetBetterPredicate(eventName, inputThreshold);

            var betterResults = _context.LiftResults
                .Where(predicate)
                .OrderBy(selector)
                .Take(5)
                .ToList();

            return betterResults;
        }

        private IEnumerable<LiftResult> GetWorstResults(string eventName, float inputThreshold)
        {
            var selector = GetSelector(eventName);
            var predicate = GetWorstPredicate(eventName, inputThreshold);

            var worseResults = _context.LiftResults
                .Where(predicate)
                .OrderByDescending(selector)
                .Take(5)
                .ToList();

            return worseResults;
        }
    }
}
