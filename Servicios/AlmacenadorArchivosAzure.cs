using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using TareasMVC.Models;

namespace TareasMVC.Servicios
{
    public class AlmacenadorArchivosAzure: IAlmacenadorArchivos
    {
        private readonly string conecctionString;
        public AlmacenadorArchivosAzure(IConfiguration configuration)
        {

            conecctionString = configuration.GetConnectionString("AzureStorage");

        }

        public async Task<AlmacenarArchivoResultado[]> Almacenar(string contenedor, IEnumerable<IFormFile> archivos)
        {
            //instanciar al ciente de azure storages
            var cliente = new BlobContainerClient(conecctionString, contenedor);
            //si no existe lo creara
            await cliente.CreateIfNotExistsAsync();
            //la politica de acceso sera a nivel de blob, osea a nivel de archivo
            cliente.SetAccessPolicy(PublicAccessType.Blob);
            //el arreglo de archvios lo subiremos de forma simultanea
            var tareas = archivos.Select(async archivo =>
            {
                //nombre del archivo original para la base de datos
                var nombreArchivoOriginal = Path.GetFileName(archivo.FileName);
                //la extencion del archivo
                var extension = Path.GetExtension(archivo.FileName);
                //nombre dela rchivo, que colocaremos el azure storage, sera un guid (aleatorio), para que no exitsa problema con otros archivos
                var nombreArchivo = $"${Guid.NewGuid()}{extension}";
                var blob = cliente.GetBlobClient(nombreArchivo);
                
                //configuraciones
                var blobHttpHeaders = new BlobHttpHeaders();
                blobHttpHeaders.ContentType = archivo.ContentType;
                //hacemos la carga del archivo
                await blob.UploadAsync(archivo.OpenReadStream(), blobHttpHeaders);
                return new AlmacenarArchivoResultado
                {
                    URL = blob.Uri.ToString(),
                    Titulo = nombreArchivoOriginal,
                };
            });
            //para ejecutar simultaneamente el ienumerable de tareas
            var resultados = await Task.WhenAll(tareas);
            return resultados;
        }

        public async Task Borrar(string ruta, string contenedor)
        {
            if (string.IsNullOrEmpty(ruta))
            {
                return;
            }

            var cliente = new BlobContainerClient(conecctionString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            var nombreArchivo = Path.GetFileName(ruta);
            var blob = cliente.GetBlobClient(nombreArchivo);
            //borramos si existe el archivo
            await blob.DeleteIfExistsAsync();
        }
    }
}
