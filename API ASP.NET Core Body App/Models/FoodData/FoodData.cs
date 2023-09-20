using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using API_ASP.NET_Core_Body_App.Models.NutritionData;

namespace API_ASP.NET_Core_Body_App.Models.FoodData
{
    public class FoodData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public required string FoodName { get; set; }
        public required FoodMacros FoodMacros { get; set; }
        public required int FoodCalories { get; set; }
    }
}
