using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API_ASP.NET_Core_Body_App.Models.HistoricData
{
    public class HistoricalData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public DateTime Date { get; set; }

        // Para PhysicalData
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public string? PhysicalData { get; set; }


        // Para NutritionalData
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public string? NutritionalData { get; set; }
    }
}