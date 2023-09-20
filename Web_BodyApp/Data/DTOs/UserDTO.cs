using Newtonsoft.Json;

namespace Web_BodyApp.Data.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Mail { get; set; }

        public string Password { get; set; }

        public DateTime BirthDate { get; set; }

        [JsonIgnore]
        public int Age { get; set; }

        public int Height { get; set; }

        public int? BodyType { get; set; }

        public int? ActivityLevel { get; set; }

        public int? Goal { get; set; }

        public int Sex { get; set; }

        public int? FatLooseRate { get; set; }

        public UserDTO GetCopy()
        {
            // Serializa el objeto a un string JSON
            string json = JsonConvert.SerializeObject(this);

            // Deserializa el string JSON de vuelta a un objeto HistoricalData
            return JsonConvert.DeserializeObject<UserDTO>(json)!;
        }
    }
}
