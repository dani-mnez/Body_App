using Web_BodyApp.Data.Models.NutritionData;

namespace Web_BodyApp.Data.DTOs
{
    public class DayTimeIntakesDTO
    {
        public required int Type { get; set; } // 0 - Food, 1 - Meal, 2 - Ambos
 
        public List<string>? FoodIntake { get; set; } = null;

        public List<MealIntake>? MealIntake { get; set; } = null;


        public required int TotalDayTimeIntakesCalories { get; set; } = 0;
        public required FoodMacros TotalDayTimeIntakesMacros { get; set; } = new FoodMacros();
    }
}
