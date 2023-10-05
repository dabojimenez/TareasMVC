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
        public async Task<List<TareaDTO>> obtenerTareas()
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
