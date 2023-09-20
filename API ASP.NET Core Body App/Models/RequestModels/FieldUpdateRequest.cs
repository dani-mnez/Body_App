namespace API_ASP.NET_Core_Body_App.Models.RequestModels
{
    public class FieldUpdateRequest
    {
        public required string Id { get; set; }
        public required string ClassName { get; set; }
        public required dynamic ClassContent { get; set; }
    }
}
