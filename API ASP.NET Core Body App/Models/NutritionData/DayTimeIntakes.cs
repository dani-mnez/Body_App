using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API_ASP.NET_Core_Body_App.Models.NutritionData
{
    public class DayTimeIntakes
    {
        public int Type { get; set; } // 0 - Food, 1 - Meal, 2 - Ambos

        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string>? FoodIntake { get; set; } = null;

        public List<MealIntake>? MealIntake { get; set; } = null;


        public int TotalDayTimeIntakesCalories { get; set; }
        public FoodMacros TotalDayTimeIntakesMacros { get; set; }
    }
}
