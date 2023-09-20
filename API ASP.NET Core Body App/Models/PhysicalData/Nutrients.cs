namespace API_ASP.NET_Core_Body_App.Models
{
    public class Nutrients
    {
        public required int DiaryTMB { get; set; }

        public required int MaintainKcals { get; set; }

        public required int FatLossKcals { get; set; }


        public required ObjectiveMacros Macros { get; set; }
    }
}