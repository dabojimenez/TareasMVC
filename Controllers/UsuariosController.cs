using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TareasMVC.Migrations;
using TareasMVC.Models;

namespace TareasMVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContextClass applicationDbContextClass;

        public UsuariosController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContextClass applicationDbContextClass)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.applicationDbContextClass = applicationDbContextClass;
        }

        [AllowAnonymous]
        public IActionResult Registro()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = new IdentityUser()
            {
                Email = model.Email,
                UserName = model.Email
            };
            //aqui creamos al usuario, enviandole el usuario y el password
            var resultado = await userManager.CreateAsync(usuario, password: model.Password);

            //verificamso el resultado
            if (resultado.Succeeded)
            {
                //pasamos el usuario y la persistencia (isPersistent), que es basicamente que asi este cerrado el navegador
                //el usuario va seguir autenticado en la aplicacion web
                await signInManager.SignInAsync(usuario, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //mostrams los errores al usuario
                foreach (var error in resultado.Errors)
                {
                    //estos son errores al nivel del modelo
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        //agregamos el mensaje de error en caso de ser por fuente externa
        public IActionResult Login(string mensaje = null)
        {
            if (mensaje is not null)
            {
                //pasaremso un viedata, para pasar el mendaje a la vista
                ViewData["mensaje"] = mensaje;
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            var respuesta = await signInManager.PasswordSignInAsync(modelo.Email,
                modelo.Password,
                modelo.Recuerdame,
                //lockoutOnFailure, si el usuario se equivoca muchas veces en ingresar, se le bloqueara, pero en este caso indicaremso que no
                lockoutOnFailure: false);

            if (respuesta.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty,
                    "Nombre de usuario o password incorrecto");
                return View(modelo);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            //para slair le pasamos el esquema de identity
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            //return RedirectToAction("Index", "Home");
            return RedirectToAction("Login");
        }

        /// <summary>
        /// Retornaremos el proveedor y las pro[piedades despues de que se realice la utenticacon en el proveedor
        /// </summary>
        /// <param name="proveedor">Proveedor seleccionado, como microsof, google, etc</param>
        /// <param name="urlRetorno">Direccion de retorno</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        //ChallengeResult, siginifica que vamos a redirigir al usuario a una fuente donde pueda loguearse
        //proveedor, podemos tener varios proveedores de logeo
        //urlRetorno, url que usaremos para retornar, despues de loguearse ene l proveedor
        public ChallengeResult LoginExterno(string proveedor, string urlRetorno = null)
        {
            // RegistrarUsuarioExterno, nombre la accion, que es la cual va a recibir la data del usuario
            var urlRedireccion = Url.Action("RegistrarUsuarioExterno",
                values: new { urlRetorno });

            //ConfigureExternalAuthenticationProperties, configirar las propiedades de autenticacion externa
            var propiedades = signInManager.ConfigureExternalAuthenticationProperties(proveedor,
                urlRedireccion);

            //retornamos el proveedor y las propiedades
            return new ChallengeResult(proveedor, propiedades);
        }

        [AllowAnonymous]
        //remoteError, posible error que nos puede retornar nuestro proveedor de identidad en caso de ocurrir algo mal
        public async Task<IActionResult> RegistrarUsuarioExterno(string urlRetorno = null,
            string remoteError = null)
        {
            //le enviamos al root de la aplicacion, en caso de ser nula
            //urlRetorno = urlRetorno ?? Url.Content("~/");
            urlRetorno = urlRetorno ?? Url.Content("~/Home/Index");
            //variable para mostrarle al usuario
            var mensaje = "";
            if (remoteError is not null)
            {
                mensaje = $"Error del proveedor externo: {remoteError}";
                //le pasamos el error otrogado por el usuario
                return RedirectToAction("login", routeValues: new { mensaje });
            }
            //obtenemso la informacion del proveedor externo
            var info = await signInManager.GetExternalLoginInfoAsync();
            //en caso de ser nula
            if (info is null)
            {
                mensaje = "Error cargando la data de login externo";
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            //hacemso un login, pero usanod la info del login externo
            var resultadoLoginExterno = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey,
                isPersistent: true,
                //nos saltaremos la autenticacion de dos factores
                bypassTwoFactor: true);

            //ya la cuenta existe en este paso
            if (resultadoLoginExterno.Succeeded)
            {
                //como si existe solo lo redirigiremso al usuario de forma local en la aplicacion
                return LocalRedirect(urlRetorno);
            }

            //si no es exitoso, procedemos a crearle una cuenta al usuario
            //empezamos a obtener el email del usuario
            string email = "";
            //obtenemso el correo, verificnaod con los claims
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                mensaje = "Error leyendo el email del usuario del proveedor";
                return RedirectToAction("login", routeValues: new { mensaje });
            }
            //instanciamso nuestro identityUser
            var usuario = new IdentityUser
            {
                Email = email,
                UserName = email
            };
            //creamos el usuario
            var resultadoCrearUsuario = await userManager.CreateAsync(usuario);
            //validamos si fue o no exitosa la creacion del usuario
            if (!resultadoCrearUsuario.Succeeded)
            {
                //simplemente tomaremso el primer error o es lo que le moestraremso al usuario
                mensaje = resultadoCrearUsuario.Errors.First().Description;
                return RedirectToAction("login", routeValues: new { mensaje });
            }

            //si fue exitoso, agregamos el login externo a la tabla de logins
            var resultadoAgregarLogin = await userManager.AddLoginAsync(usuario, info);
            //validmaos el resultado de agregar el usuario
            if (resultadoAgregarLogin.Succeeded)
            {
                //logueamos al usuario
                await signInManager.SignInAsync(usuario, isPersistent: false, info.LoginProvider);
                return LocalRedirect(urlRetorno);
            }
            //si no fue exitosa la creacion, mostraemso un mensaje
            mensaje = "Ha ocurrido un error agregando el Login";
            return RedirectToAction("login", routeValues: new { mensaje });
        }

        [HttpGet]
        public async Task<IActionResult> Listado(string mensaje = null)
        {
            //realizamos un query, pero usaremos dbcontext que creamos. Solo traemos el email
            var usuarios = await applicationDbContextClass.Users
                //proyectamos, los valores obtenidos en (u), proyectamos a una nueva clase de UsuarioViewModel
                .Select(u => new UsuarioViewModel
                {
                    Email = u.Email,
                })
                .ToListAsync();

            var modelo = new UsuarioListaViewModel();
            modelo.Usuarios = usuarios;
            modelo.Mensaje = mensaje;

            return View(modelo);
        }
    }
}
