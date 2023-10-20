﻿using TareasMVC.Models;

namespace TareasMVC.Servicios
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment env;
        private readonly HttpContextAccessor httpContextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment env,
            HttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            //para poder acceder al contexto de http
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<AlmacenarArchivoResultado[]> Almacenar(string contenedor, IEnumerable<IFormFile> archivos)
        {
            var tareas = archivos.Select(async archivo =>
            {
                var nombreArchivoOriginal = Path.GetFileName(archivo.FileName);
                var extension = Path.GetExtension(archivo.FileName);
                var nombreArchivo = $"${Guid.NewGuid()}{extension}";
                //combinacion d ela ruta en donde se encontrara nuestro archivo
                string folder = Path.Combine(env.WebRootPath, contenedor);
                
                if (!Directory.Exists(folder))
                {
                    //si no existe lo creamos
                    Directory.CreateDirectory(folder);
                }
                //ruta final donde estara el archivo subido por el usuario
                string ruta = Path.Combine(folder, nombreArchivo);

                //
                using (var ms = new MemoryStream())
                {
                    await archivo.CopyToAsync(ms);
                    var contenido = ms.ToArray();
                    await File.WriteAllBytesAsync(ruta, contenido);
                }

                //retornamos el titulo y url del archivo
                var url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
                //remplazmaos el \\, por / porque estamos guardando una url
                var urlArchivo = Path.Combine(url, contenedor, nombreArchivo).Replace("\\", "/");

                return new AlmacenarArchivoResultado
                {
                    URL = urlArchivo,
                    Titulo = nombreArchivoOriginal
                };
            });

            var resultados = await Task.WhenAll(tareas);
            return resultados;
        }

        public Task Borrar(string ruta, string contenedor)
        {
            if (string.IsNullOrWhiteSpace(ruta))
            {
                return Task.CompletedTask;
            }

            var nombreArchivo = Path.GetFileName(ruta);
            var directorioArchivo = Path.Combine(env.WebRootPath, contenedor, nombreArchivo);
            if (File.Exists(directorioArchivo))
            {
                File.Delete(directorioArchivo);
            }

            return Task.CompletedTask;
        }
    }
}
