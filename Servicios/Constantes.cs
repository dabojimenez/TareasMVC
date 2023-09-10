using Microsoft.AspNetCore.Mvc.Rendering;

namespace TareasMVC.Servicios
{
    /// <summary>
    /// Se crea la clase de constantes, para no tener harcodeado los roles
    /// </summary>
    public class Constantes
    {
        public const string RolAdmin = "admin";
        public static readonly SelectListItem[] CulturasSoportadas = new SelectListItem[]
        {
            new SelectListItem
            {
                Value = "es", Text = "Espanol"
            },
            new SelectListItem
            {
                Value = "en", Text = "Ingles"
            }
        };
    }
}
