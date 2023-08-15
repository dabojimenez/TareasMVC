using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TareasMVC.Entidades
{
    public class Tarea
    {
        public int Id { get; set; }
        [StringLength(250)]
        [Required]
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
        public DateTime FechaCreacion { get; set; }
        //agregamos la llaveforanea, agregando la relacion entre tarea y usuario
        //creamos el id, del usuario que a creado la tarea
        public string UsuarioCreacionId { get; set; }
        //adicionamos, la propiedad de navegacion de identityuser
        public IdentityUser UsuarioCreacion { get; set; }

        //propiedad de navegacion, obteniendo el detalle o los registros hijos
        public List<Paso> Pasos { get; set; }
        public List<ArchivoAdjunto> ArchivosAdjuntos { get; set; }
    }
}
