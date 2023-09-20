using Newtonsoft.Json;
using Web_BodyApp.Data.DTOs;

namespace Web_BodyApp.Data.Models.NutritionData
{
    public class NutritionalData
    {
        public string? Id { get; set; } = null;
        public Dictionary<int, DayTimeIntakes>? DayTimeIntakes { get; set; }
        public required int TotalNutDataCalories { get; set; }
        public required FoodMacros TotalNutDataMacros { get; set; }


        public NutritionalData GetCopy()
        {
            // Serializa el objeto a un string JSON
            string json = JsonConvert.SerializeObject(this);

            // Deserializa el string JSON de vuelta a un objeto HistoricalData
            return JsonConvert.DeserializeObject<NutritionalData>(json)!;
        }

        public NutritionalDataDTO ToDTO()
        {
            NutritionalDataDTO nutritionalDataDTO = new()
            {
                Id = this.Id,
                TotalNutDataCalories = this.TotalNutDataCalories,
                TotalNutDataMacros = this.TotalNutDataMacros
            };

            if (this.DayTimeIntakes != null)
            {
                Dictionary<int, DayTimeIntakesDTO> tempDTIDTO = new();

                foreach ((int dayTime, DayTimeIntakes intakes) in this.DayTimeIntakes)
                {
                    List<string>? foodIntakes = null;
                    if (intakes.FoodIntake != null)
                    {
                        foodIntakes = intakes.FoodIntake!.Select(x => x.Id).ToList();
                    }
                    tempDTIDTO.Add(
                        dayTime,
                        new() { 
                            Type = intakes.Type,
                            FoodIntake = foodIntakes,
                            MealIntake = intakes.MealIntake,
                            TotalDayTimeIntakesCalories = intakes.TotalDayTimeIntakesCalories,
                            TotalDayTimeIntakesMacros = intakes.TotalDayTimeIntakesMacros
                        }
                    );
                }

                nutritionalDataDTO.DayTimeIntakes = tempDTIDTO;
            }

            return nutritionalDataDTO;
        }
    }
}
