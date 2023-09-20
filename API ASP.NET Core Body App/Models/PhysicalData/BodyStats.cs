using MongoDB.Bson.Serialization.Attributes;

namespace API_ASP.NET_Core_Body_App.Models
{
    public class BodyStats
    {
        public required double Imc { get; set; }

        [BsonIgnoreIfNull]
        public double? FatPerc { get; set; }

        [BsonIgnoreIfNull]
        public double? MusclePerc { get; set; }

        [BsonIgnoreIfNull]
        public WeightComposition? WeightComposition { get; set; }

        [BsonIgnoreIfNull]
        public double? TotalCircunferences { get; set; }
    }
}