using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContextClass context;
        private readonly IServicioUsuarios servicioUsuarios;

        public TareasController(ApplicationDbContextClass context, IServicioUsuarios servicioUsuarios)
        {
            this.context = context;
            this.servicioUsuarios = servicioUsuarios;
        }

        [HttpPost]
        public async Task<ActionResult<Tarea>> Post([FromBody] string titulo)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            //nos retorna un valor booleano, si en caso de esistir tareas del usuario id, que se ha logueado
            var existenTareas = await context.Tareas.AnyAsync(t => t.UsuarioCreacionId == usuarioId);

            int ordenMayor = 0;
            if (existenTareas)
            {
                //si esxisten tareas buscaremos el numero mayor de las tareas
                ordenMayor = await context.Tareas.Where(t => t.UsuarioCreacionId == usuarioId)
                    //obtenemos solo el campo orden
                    .Select(t => t.Orden)
                    //el maximo valor del campo orden
                    .MaxAsync();
            }

            var tarea = new Tarea
            {
                Titulo = titulo,
                UsuarioCreacionId = usuarioId,
                FechaCreacion = DateTime.UtcNow,
                Orden = ordenMayor + 1,
            };

            //**********agregamos un registro usando EFCore
            //Add, marca como que va a ser agregada, al momento de guardar los cambios
            context.Add(tarea);
            //SaveChangesAsync, guardamos los cambios en la base de datos, los que han sido marcados con add
            await context.SaveChangesAsync();

            return tarea;
        }
    }
}
