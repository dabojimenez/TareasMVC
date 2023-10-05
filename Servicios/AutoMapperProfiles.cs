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
            CreateMap<Tarea, TareaDTO>();
        }
    }
}
