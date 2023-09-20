using System.Text.Json.Serialization;

namespace Web_BodyApp.Data.AssistClasses
{
    public class PatchStringCreationData
    {
        public string op { get; set; } = "";
        public string path { get; set; } = "";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? value { get; set; } = null;
    }
}
