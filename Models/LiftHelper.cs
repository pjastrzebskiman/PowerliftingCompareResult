namespace PowerliftingCompareResult.Models
{
    public class LiftHelper
    {
        public static float GetValueBasedOnEvent(float lr, string eventName)
        {
            switch (eventName)
            {
                case "Total": return lr;
                case "Squat": return lr;
                case "Bench": return lr;
                case "Deadlift": return lr;
                default: throw new InvalidOperationException("Invalid event name");
            }
        }
    }
}
