using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TareasMVC.Migrations;
using TareasMVC.Models;

namespace TareasMVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        public UsuariosController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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
        public IActionResult Login()
        {
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
    }
}
