
using MongoDB.Bson.Serialization.Attributes;

namespace API_ASP.NET_Core_Body_App.Models
{
    public class BodyMeasure
    {
        [BsonIgnoreIfNull]
        public BodyFat? BodyFat { get; set; }

        [BsonElement("totalCirc")]
        [BsonIgnoreIfNull]
        public Circunferences? Circunferences { get; set; }
    }
}