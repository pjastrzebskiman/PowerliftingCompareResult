namespace PowerliftingCompareResult.Models
{
    public class LiftResult
    {
        public string Name { get; set; }
        public string EQ { get; set; }
        public float Age { get; set; }
        public string AgeClass { get; set; }
        public float BodyWeight { get; set; }
        public string WeightClass { get; set; }
        public float Squat { get; set; }
        public float Bench { get; set; }
        public float Deadlift { get; set; }
        public float Total { get; set; }
        public string Sex { get; set; }

        public float GetValueByEventName(string eventName)
        {
            switch (eventName)
            {
                 case "Total":
                    return Total;
                case "Squat":
                    return Squat;
                case "Bench":
                    return Bench;
                case "Deadlift":
                    return Deadlift;
                default:
                    throw new ArgumentException("Invalid event name", nameof(eventName));
            }
        }


    }

}
