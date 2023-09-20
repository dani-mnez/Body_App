using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace API_ASP.NET_Core_Body_App.Models
{
    public class PhysicalData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public double Weight { get; set; }

        [BsonIgnoreIfNull]
        public BodyMeasure? BodyMeasure { get; set; }

        public ComputedData Computed { get; set; }

        public PhysicalData GetCopy()
        {
            // Serializa el objeto a un string JSON
            string json = JsonConvert.SerializeObject(this);

            // Deserializa el string JSON de vuelta a un objeto HistoricalData
            return JsonConvert.DeserializeObject<PhysicalData>(json)!;
        }
    }
}
