using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TareasMVC.Entidades;
using TareasMVC.Migrations;
using TareasMVC.Models;
using TareasMVC.Servicios;

namespace TareasMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasosController : Controller
    {
        private readonly ApplicationDbContextClass context;
        private readonly IServicioUsuarios servicioUsuarios;

        public PasosController(ApplicationDbContextClass context, IServicioUsuarios servicioUsuarios)
        {
            this.context = context;
            this.servicioUsuarios = servicioUsuarios;
        }
        [HttpPost("{tareaId:int}")]
        public async Task<ActionResult<Paso>> Post(int tareaId, [FromBody] PasoCrearDTO pasoCrearDTO)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var tarea = await context.Tareas.FirstOrDefaultAsync(t => t.Id == tareaId);

            if (tarea is null)
            {
                return NotFound();
            }

            if (tarea.UsuarioCreacionId != usuarioId)
            {
                //el usuario no permiso para acceder a dicha tarea, para crear un paso
                return Forbid();
            }
            //verificamos si existen pasos
            var existenPasos = await context.Pasos.AnyAsync(p => p.TareaId == tareaId);
            int ordenMayor = 0;
            if (existenPasos)
            {
                ordenMayor = await context.Pasos.Where(p => p.TareaId == tareaId)
                    .Select(p => p.Orden)
                    .MaxAsync();//el valor maximo
            }

            var paso = new Paso();

            paso.TareaId = tareaId;
            paso.Orden = ordenMayor + 1;
            paso.Descripcion = pasoCrearDTO.Descripcion;
            paso.Realizado = pasoCrearDTO.realizado;

            context.Add(paso);
            await context.SaveChangesAsync();
            //retornamos el paso de donde vamos a sacar el id del paso
            return paso;
        }
        //no colocamos el :int, ya que el id del paso es un GUID
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody] PasoCrearDTO pasoCrearDTO)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var paso = await context.Pasos
                .Include(p => p.Tarea)//traeremos la data de la tarea, ya que en la tarea esta el id del usuario
                .FirstOrDefaultAsync(p => p.Id == id);

            if (paso is null)
            {
                return NotFound();
            }

            if (paso.Tarea.UsuarioCreacionId != usuarioId)
            {
                return Forbid();
            }

            paso.Descripcion = pasoCrearDTO.Descripcion;
            paso.Realizado = pasoCrearDTO.realizado;

            await context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId().ToString();

            Paso paso = await context.Pasos
                .Include(p => p.Tarea)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (paso is null)
            {
                return NotFound();
            }

            if (paso.Tarea.UsuarioCreacionId != usuarioId)
            {
                return Forbid();
            }
            //marcamos la entidad como que va a ser borrada
            context.Remove(paso);
            //borramos la entidad o el paso
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("ordenar/{tareaId:int}")]
        public async Task<IActionResult> Ordenar(int tareaId, [FromBody] Guid[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            Tarea tarea = await context.Tareas .FirstOrDefaultAsync(t => t.Id == tareaId && t.UsuarioCreacionId == usuarioId);
            if (tarea == null)
            {
                return NotFound();
            }
            
            var pasos = await context.Pasos.Where(x => x.TareaId == tareaId).ToListAsync();

            var pasosId = pasos.Select(x => x.Id);

            //revisamos que no nos falte ningun paso
            var idsNoPertenecenALaTarea = ids.Except(pasosId).ToList();
            //si hay alguno que no esta, retonramos bad
            if (idsNoPertenecenALaTarea.Any())
            {
                return BadRequest("No todos los pasos estan presentes");
            }
            //transformamos a un diciconario, donde la key, sera el id del paso (guid)
            var pasosDiccionario = pasos.ToDictionary(p => p.Id);

            for (int i = 0; i < ids.Length; i++)
            {
                var pasoId = ids[i];
                var paso = pasosDiccionario[pasoId];
                paso.Orden = i + 1;
            }

            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
