using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;
using TareasMVC.Models;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TareasController : ControllerBase
    {
        private readonly ApplicationDbContextClass context;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IMapper mapper;

        public TareasController(ApplicationDbContextClass context, IServicioUsuarios servicioUsuarios,
            IMapper mapper)
        {
            this.context = context;
            this.servicioUsuarios = servicioUsuarios;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<TareaDTO>>> obtenerTareas()
        {
            //obtenemos el usuario que esta realizando la solicitud
            string usuarioId = servicioUsuarios.ObtenerUsuarioId();
            List<TareaDTO> tareas = await context.Tareas
                .Where(t => t.UsuarioCreacionId == usuarioId)
                //ordenamos por el campo orden
                //de forma descendente podemos usar (OrderByDescending)
                .OrderBy(t => t.Orden)
                ////seleccionamos los campos a mostrar con select, pero es de tipo anonimo 
                //.Select(t => new TareaDTO
                //{
                //    Id = t.Id,
                //    Titulo = t.Titulo
                //})
                //-----Usaremos AUTOMAPPER
                .ProjectTo<TareaDTO>(mapper.ConfigurationProvider)
                .ToListAsync();
            //Quitamos el IActionResult, ya que e smuy peligroso, al momento de devolver la información
            return tareas;
        }
        //definimos una variable llamada id, y de tipo de dato entero
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tarea>> Get(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            Tarea tarea = await context.Tareas
                //similar a un join (include)
                .Include(t => t.Pasos //con esta función incluimos los pasos de la tarea, ya que la tarea tiene su propiedad de listado de pasos list<pasos>
                    .OrderBy(p => p.Orden) //ordenamos los pasos por elc ampo orden
                    )
                .FirstOrDefaultAsync(t => t.Id == id
                                && t.UsuarioCreacionId == usuarioId);
            if (tarea is null)
            {
                return NotFound();
            }
            return tarea;
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

        [HttpPost("ordenar")]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioid = servicioUsuarios.ObtenerUsuarioId();
            var tareas = await context.Tareas
                .Where(t => t.UsuarioCreacionId == usuarioid).ToListAsync();
            var tareasId = tareas.Select(t => t.Id);
            //verificamso que no pertenencen al usuario
            var idsTareasNoPertenecenAlUsuario = ids.Except(tareasId).ToList();
            if (idsTareasNoPertenecenAlUsuario.Any())
            {
                //retornamos prohibido
                return Forbid();
            }
            //creamos un diccionario donde la llave va hacer el id
            var tareasDiccionario = tareas.ToDictionary(x => x.Id);
            for (int i = 0; i < ids.Length; i++)
            {
                var id = ids[i];
                var tarea = tareasDiccionario[id];
                tarea.Orden = i + 1;
            }

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditarTarea(int id, [FromBody] TareaEditarDTO tareaEditarDTO)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tarea = await context.Tareas.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioCreacionId == usuarioId);
            if (tarea is null)
            {
                return NotFound();
            }

            //procedemos a actualizar la tarea
            tarea.Titulo = tareaEditarDTO.Titulo;
            tarea.Descripcion = tareaEditarDTO.Descripcion;

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tarea = await context.Tareas.FirstOrDefaultAsync(t => t.Id == id && t.UsuarioCreacionId == usuarioId);
            if (tarea is null)
            {
                return NotFound();
            }
            //marcmaos con el estadtus que esa tarea sera removida
            context.Remove(tarea);
            //aqui removemos la tarea
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
