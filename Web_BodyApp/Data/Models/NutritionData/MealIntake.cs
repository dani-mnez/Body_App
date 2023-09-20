using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Web_BodyApp.Data.Models.NutritionData
{
    public class MealIntake
    {
        public string? MealId { get; set; }

        public required FoodServing MealServing { get; set; }
        public required int TotalMealIntakeCalories { get; set; }
        public required FoodMacros TotalMealIntakeMacros { get; set; }

        public MealIntake GetCopy()
        {
            string json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<MealIntake>(json)!;
        }
    }
}
