using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using API_ASP.NET_Core_Body_App.Models.NutritionData;

namespace API_ASP.NET_Core_Body_App.Models.UserMeal
{
    public class Meal
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Name { get; set; }

        public int TotalCalories { get; set; }
        public FoodMacros TotalMealMacros { get; set; }
        public int TotalWeight { get; set; }
    }
}
