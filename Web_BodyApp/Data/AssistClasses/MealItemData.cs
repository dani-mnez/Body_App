using Web_BodyApp.Data.Models.NutritionData;

namespace Web_BodyApp.Data
{
    public class MealItemData
    {
        public required string HistoricalDataId { get; set; }
        public required string NutritionalDataId { get; set; }
        public required Dictionary<int, DayTimeIntakes> DayTimeIntakesWithMeal { get; set; }
    }
}
