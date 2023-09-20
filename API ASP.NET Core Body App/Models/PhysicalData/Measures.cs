
using MongoDB.Bson.Serialization.Attributes;

namespace API_ASP.NET_Core_Body_App.Models
{
    public class Measures
    {
        [BsonIgnoreIfNull]
        public double? Chest { get; set; }

        [BsonIgnoreIfNull]
        public double? Abdominal { get; set; }

        [BsonIgnoreIfNull]
        public double? Thigh { get; set; }

        [BsonIgnoreIfNull]
        public double? Tricep { get; set; }

        [BsonIgnoreIfNull]
        public double? Subscapular { get; set; }

        [BsonIgnoreIfNull]
        public double? Suprailiac { get; set; }

        [BsonIgnoreIfNull]
        public double? MidAxilary { get; set; }

        [BsonIgnoreIfNull]
        public double? Bicep { get; set; }

        [BsonIgnoreIfNull]
        public double? LowerBack { get; set; }

        [BsonIgnoreIfNull]
        public double? Calf { get; set; }


        [BsonIgnoreIfNull]
        public double? TapeNeck { get; set; }
        
        [BsonIgnoreIfNull]
        public double? TapeAbdomen { get; set; }

        [BsonIgnoreIfNull]
        public double? TapeWaist { get; set; }    

        [BsonIgnoreIfNull]
        public double? TapeHip { get; set; }
    }
}