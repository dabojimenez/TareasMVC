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
    }
}
