﻿using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Diagnostics;
using TareasMVC.Models;

namespace TareasMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer<HomeController> localizer;

        public HomeController(ILogger<HomeController> logger,
            IStringLocalizer<HomeController> localizer)
        {
            _logger = logger;
            this.localizer = localizer;
        }

        public IActionResult Index()
        {
            //mensae que se reflejara en un idioma, definido por la localizacion
            ViewBag.Saludo = localizer["Buenos días"];
            return View();
        }

        [HttpPost]
        public IActionResult CambiarIdioma(string cultura, string urlRetorno)
        
        {
            //agregamos la cooki
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultura)),
                new CookieOptions
                {
                    //agregamos 4 anos de expiracion
                    Expires = DateTimeOffset.UtcNow.AddYears(5),
                });
            return LocalRedirect(urlRetorno);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}