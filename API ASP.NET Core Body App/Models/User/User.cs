using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace API_ASP.NET_Core_Body_App.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Mail { get; set; }

        public string Password { get; set; }

        public DateTime BirthDate { get; set; }

        public int Height { get; set; }

        [BsonIgnoreIfNull]
        public int? BodyType { get; set; }

        [BsonIgnoreIfNull]
        public int? ActivityLevel { get; set; }

        public int Sex { get; set; }

        [BsonIgnoreIfNull]
        public int? FatLooseRate { get; set; }

        [BsonIgnoreIfNull]
        public int? Goal { get; set; }


        // Para HistoricalData
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public List<string> HistoricalData { get; set; }


        // Para Meals
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfNull]
        public List<string> CustomMeals { get; set; }


        public User GetCopy()
        {
            // Serializa el objeto a un string JSON
            string json = JsonConvert.SerializeObject(this);

            // Deserializa el string JSON de vuelta a un objeto HistoricalData
            return JsonConvert.DeserializeObject<User>(json)!;
        }
    }
}