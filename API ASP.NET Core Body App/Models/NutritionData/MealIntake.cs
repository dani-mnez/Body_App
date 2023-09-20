using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace API_ASP.NET_Core_Body_App.Models.NutritionData
{
    public class MealIntake
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public required string MealId { get; set; }

        public required FoodServing MealServing { get; set; }
        public required int TotalMealIntakeCalories { get; set; }
        public required FoodMacros TotalMealIntakeMacros { get; set; }
    }
}
