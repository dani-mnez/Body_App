namespace Web_BodyApp.Data.Models.PhysicData
{
    public class Nutrients
    {
        public required int DiaryTMB { get; set; }

        public required int MaintainKcals { get; set; }

        public required int FatLossKcals { get; set; }


        public required Macros Macros { get; set; }
    }
}