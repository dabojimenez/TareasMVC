using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivosController : ControllerBase
    {
        private readonly ApplicationDbContextClass context;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly string contenedor = "archivosadjuntos";

        public ArchivosController(ApplicationDbContextClass context, IAlmacenadorArchivos almacenadorArchivos,
            IServicioUsuarios servicioUsuarios)
        {
            this.context = context;
            this.almacenadorArchivos = almacenadorArchivos;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpPost("{tareaId:int}")]
        //FromForm, nos permite utilizar un formato especial a traves del cual podemos recibir data en una peticion http
        //peroe sta data sera en formato de trasnmicion de archivos, o colecciones de archivos
        public async Task<ActionResult<IEnumerable<ArchivoAdjunto>>> Post(int tareaId, [FromForm] IEnumerable<IFormFile> archivos)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await context.Tareas.FirstOrDefaultAsync(t => t.Id == tareaId);
            if (tarea is null)
            {
                return NotFound();
            }

            if (tarea.UsuarioCreacionId != usuarioId)
            {
                return Forbid();
            }

            //AnyAsync, preguntaos si existe alguna rchvio que pertenece a la tarea que estoy recibiendo
            var existenArchviosAdjuntos = await context.ArchivoAdjunto.AnyAsync(a => a.TareaId == tareaId);
            var ordenMayor = 0;
            if (existenArchviosAdjuntos)
            {
                //si existen, buscamos cual es el orden mayor
                ordenMayor = await context.ArchivoAdjunto.Where(a => a.TareaId == tareaId)
                    .Select(a => a.Orden)
                    .MaxAsync();
            }

            //almacenaremos todos los archivos
            var resultados = await almacenadorArchivos.Almacenar(contenedor, archivos);

            //indice, este de aqui nos permitira determinar en queelemnto de ienumerable estamos
            var archivosAdjuntos = resultados.Select((resultado, indice) => new ArchivoAdjunto
            {
                TareaId = tareaId,
                FechaCreacion = DateTime.UtcNow,
                Url = resultado.URL,
                Titulo = resultado.Titulo,
                Orden = ordenMayor + indice + 1
            }).ToList();
            //permite agregar de una sola, un conjunto de entidades
            context.AddRange(archivosAdjuntos);
            await context.SaveChangesAsync();
            return archivosAdjuntos.ToList();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody] string titulo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            //traemos el arhcivo adjunto de la base de datos
            var archivoAdjunto = await context.ArchivoAdjunto
                .Include(a => a.Tarea)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (archivoAdjunto == null)
            {
                return NotFound();
            }

            if (archivoAdjunto.Tarea.UsuarioCreacionId != usuarioId)
            {
                return Forbid();
            }
            //acctualizamos en memoria
            archivoAdjunto.Titulo = titulo;
            //salvamos el contexto, para cambiar en la base de datos
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
