using System.Security.Claims;

namespace TareasMVC.Servicios
{
    public interface IServicioUsuarios
    {
        string ObtenerUsuarioId();
    }

    public class ServicioUsuarios : IServicioUsuarios
    {
        private HttpContext httpContext;

        public ServicioUsuarios(IHttpContextAccessor httpContextAccessor)
        {
            httpContext = httpContextAccessor.HttpContext;
        }
        public string ObtenerUsuarioId()
        {
            //verificamos si esta autenticado el usuario ene l sistema
            if (httpContext.User.Identity.IsAuthenticated)
            {
                //buscamos el calim (imformacion del usuario)
                //ClaimTypes.NameIdentifier, el id del usuario
                var idClaim = httpContext.User.Claims.Where( u => u.Type == ClaimTypes.NameIdentifier)
                    .FirstOrDefault();

                return idClaim.Value;
            }
            else
            {
                //en teorica nunca se debe llegar aqui, pero se lo agrega
                throw new Exception("El usuario no exta autenticado");
            }
        }
    }
}
