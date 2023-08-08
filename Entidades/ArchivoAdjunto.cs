using Microsoft.EntityFrameworkCore;

namespace TareasMVC.Entidades
{
    public class ArchivoAdjunto
    {
        public Guid Id { get; set; }
        public int TareaId { get; set; }
        public Tarea Tarea { get; set; }
        [Unicode]
        //no es necesario que el campo para una url, sea nvarchar, basta con que sea varchar, para asi poder ahorrar espacio
        //en esta propiedad no vamos a guardar caracteres especiales, emogis, caracteres chinos, etc
        //si vamos a guardar eso, debemos usar simplemente nvarchar, que vendria a ser quitar ([Unicode])
        public string Url { get; set; }
        public string Titulo { get; set; }
        public int Orden { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
