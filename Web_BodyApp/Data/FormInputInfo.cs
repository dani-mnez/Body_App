using Microsoft.AspNetCore.Components;

namespace Web_BodyApp.Data
{
    public class FormInputInfo
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public Func<double?> GetValue { get; set; }
        public Action<double?> SetValue { get; set; }
        public int Sex { get; set; } = 2; // 0: mujeres, 1: hombres, 2: ambos
    
        
        public FormInputInfo GetCopy()
        {
            return new FormInputInfo
            {
                Id = this.Id,
                Label = this.Label,
                GetValue = this.GetValue,
                SetValue = this.SetValue,
                Sex = this.Sex
            };
        } 
    }

}
