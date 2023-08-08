namespace TareasMVC.Entidades
{
    public class Paso
    {
        public Guid Id { get; set; } //oefrsf88-adsc-dfr554gt-fdfdf
        //establecemos la relacion entre Tarea y Paso, usnaod la convencion
        //esta compuesta por la table (Tarea) y su PK (ID)
        public int TareaId { get; set; }
        //Configuramos la propiedad de navegacion, ya que esta nos llevara directamente a la entidad Tarea
        public Tarea Tarea { get; set; }
        public string Descripcion { get; set; }
        public bool Realizado { get; set; }
        public int Orden { get; set; }
    }
}
