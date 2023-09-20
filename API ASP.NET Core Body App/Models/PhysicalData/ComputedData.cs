using MongoDB.Bson.Serialization.Attributes;

namespace API_ASP.NET_Core_Body_App.Models
{
    public class ComputedData
    {
        [BsonIgnoreIfNull]
        public Nutrients? Nutrients { get; set; }

        public required BodyStats BodyStats { get; set; }
    }
}