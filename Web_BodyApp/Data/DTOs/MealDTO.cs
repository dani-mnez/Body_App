namespace Web_BodyApp.Data.DTOs
{
    public class MealDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> FoodData { get; set; }
        public int TotalCalories { get; set; }
    }
}
