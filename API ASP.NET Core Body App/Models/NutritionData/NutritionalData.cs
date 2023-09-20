using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace API_ASP.NET_Core_Body_App.Models.NutritionData
{
    public class NutritionalData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonIgnoreIfNull]
        public Dictionary<string, DayTimeIntakes>? DayTimeIntakes { get; set; }

        public int TotalNutDataCalories { get; set; }
        public FoodMacros TotalNutDataMacros { get; set; }


        public NutritionalData GetCopy()
        {
            // Serializa el objeto a un string JSON
            string json = JsonConvert.SerializeObject(this);

            // Deserializa el string JSON de vuelta a un objeto HistoricalData
            return JsonConvert.DeserializeObject<NutritionalData>(json)!;
        }
    }
}