namespace Web_BodyApp.Data.Models.PhysicData
{
    public class BodyStats
    {
        public required double Imc { get; set; }

        public double? FatPerc { get; set; }

        public double? MusclePerc { get; set; }

        public WeightComposition? WeightComposition { get; set; }

        public double? TotalCircunferences { get; set; }
    }
}