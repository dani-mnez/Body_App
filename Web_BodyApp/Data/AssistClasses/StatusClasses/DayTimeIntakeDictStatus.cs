namespace Web_BodyApp.Data.AssistClasses.StatusClasses
{
    public class DayTimeIntakeDictStatus
    {
        public bool LastDayTimeKey { get; set; } = false;
        public Dictionary<int, DayTimeIntakeStatus> DayTimeIntakeStatus { get; set; }
    }
}
