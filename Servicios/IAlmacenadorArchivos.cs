using TareasMVC.Models;

namespace TareasMVC.Servicios
{
    //esta interface, tendra dos implementaciones, uno que guardara el archivo en azure y otro que guardara en el proyecto
    public interface IAlmacenadorArchivos
    {
        //contenedor, la carpeta donde se encuentra nuestro archivo adjunto
        Task Borrar(string ruta, string contenedor);
        //IFormFile, representacion de tipo de dato para un archivo cualquiera
        Task<AlmacenarArchivoResultado[]> Almacenar(string contenedor, IEnumerable<IFormFile> archivos);
    }
}
