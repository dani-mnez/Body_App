using System.ComponentModel.DataAnnotations;

namespace Web_BodyApp.Data
{
    public class LoginFormModel
    {
        [Required(ErrorMessage = "El mail es obligatorio.")]
        [EmailAddress]
        public string Mail { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; }

        public bool Remember { get; set; } = false;
    }
}
