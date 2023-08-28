using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Error.Requerido")]
        [EmailAddress(ErrorMessage = "Error.Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Error.Requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        //si el usuario cierra el navegador va a seguir logueado en nuestra aplicacio o de lo contrario
        //nuevamente tiene q ingresar al login, para eso es la implementacion de esta propiedad
        [Display(Name = "Recuérdame")]
        public bool Recuerdame { get; set; }
    }
}
