using AutoMapper;
using TareasMVC.Entidades;
using TareasMVC.Models;

namespace TareasMVC.Servicios
{
    public class AutoMapperProfiles: Profile
    {
        //constructor
        public AutoMapperProfiles()
        {
            //creamos los mapeados del orgin, hacia el destino
            CreateMap<Tarea, TareaDTO>()
                //para los atrivutos de TareaDTO (PasosRealizados,PasosTotal)
                .ForMember( //para el miembro
                dto => dto.PasosTotal,
                //de donde vamos a sacar dicha data (ent) entidad, que en este caso nuestra entidad es Tarea
                //MapFrom = mapear desde
                ent => ent.MapFrom(x => x.Pasos.Count())
                )
                .ForMember(dto => dto.PasosRealizados, 
                ent => 
                    ent.MapFrom(x => x.Pasos.Where(p => p.Realizado).Count()));
        }
    }
}
