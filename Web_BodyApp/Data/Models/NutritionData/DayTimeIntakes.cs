using Web_BodyApp.Data.DTOs;

namespace Web_BodyApp.Data.Models.NutritionData
{
    public class DayTimeIntakes
    {
        public required int Type { get; set; } // 0 - Food, 1 - Meal, 2 - Ambos


        public List<FoodData>? FoodIntake { get; set; } = null;

        public List<MealIntake>? MealIntake { get; set; } = null;


        public required int TotalDayTimeIntakesCalories { get; set; } = 0;
        public required FoodMacros TotalDayTimeIntakesMacros { get; set; } = new FoodMacros();


        public DayTimeIntakesDTO ToDTO()
        {
            List<string>? foodIntakes = null;
            if (this.FoodIntake != null)
            {
                foodIntakes = this.FoodIntake.Where(x => x.Id != null).Select(x => x.Id!).ToList();
            }
            return new()
            {
                Type = this.Type,
                FoodIntake = foodIntakes,
                MealIntake = this.MealIntake,
                TotalDayTimeIntakesCalories = this.TotalDayTimeIntakesCalories,
                TotalDayTimeIntakesMacros = this.TotalDayTimeIntakesMacros
            };
        }
    }
}
