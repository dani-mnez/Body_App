namespace Web_BodyApp.Data
{
    public class User
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Mail { get; set; }

        public string Password { get; set; }

        public DateTime BirthDate { get; set; }

        public int Height { get; set; }

        public int? BodyType { get; set; }

        public int? ActivityLevel { get; set; }

        public int? Goal { get; set; }

        public int Sex { get; set; }

        public int? FatLooseRate { get; set; }

        public List<string> HistoricalData { get; set; }

        public List<string> CustomMeals { get; set; }
    }
}
