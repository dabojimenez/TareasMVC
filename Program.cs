using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TareasMVC;
using Microsoft.AspNetCore.Mvc.Razor;
using TareasMVC.Servicios;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//creamos la politica de autorizacion, esto para aplicar el atributo [Authorize] a todos los controladores
var politicaUsuariosAutenticados = new AuthorizationPolicyBuilder()
    //requreriremos la auternticacion de los usuarios
    .RequireAuthenticatedUser()
    //construiremos la politica de autenticacion de usuarios
    .Build();

// Add services to the container.
builder.Services.AddControllersWithViews(opciones =>
{
    //buscamos la opcion de filtro
    opciones.Filters
    //agregamos la politica que cosntruimos
        .Add(new AuthorizeFilter(politicaUsuariosAutenticados));
})
//AddViewLocalization, esto nos ayudara a traducir las paginas de html
.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
//la siguiente linea es para el uso compartido de los mensjaes en distintos idiomas, pero usando los dataanotations
.AddDataAnnotationsLocalization(opciones =>
{
    //tecnica, que nos permitira utilizar un unico archivo de recursos para traducir las anotaciones de datos
    //(_)descartamos el tipo
    opciones.DataAnnotationLocalizerProvider = (_, factoria) =>
        factoria.Create(typeof(RecursoCompartido));
})
//con AddJsonOptions, para modificar la configuracion de serializacion de json
.AddJsonOptions(opciones =>
{
    opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;//puede ignorar las referencias ciclicas de nuestro codigo
});

builder.Services.AddDbContext<ApplicationDbContextClass>(opciones =>
    opciones.UseSqlServer("name=DefaultConnection"));

//configuracion de identity
builder.Services.AddAuthentication()
    //adicionamos, el login usando una cuenta en Microsoft, pasandole el clientid y el secretId
    .AddMicrosoftAccount(opciones =>
    {
        opciones.ClientId = builder.Configuration["MicrosoftClientId"];
        opciones.ClientSecret = builder.Configuration["MicrosoftSecretId"];
    });
builder.Services.AddIdentity<IdentityUser, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = false;
}).AddEntityFrameworkStores<ApplicationDbContextClass>()
.AddDefaultTokenProviders();
// la autenticacion y uso de los formularios propios, mas no de los de identity
builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, opciones =>
{
    opciones.LogoutPath = "/usuarios/login";
    opciones.AccessDeniedPath = "/usuarios/login";
});

//haremos uso de IStringLocalizer
builder.Services.AddLocalization(opciones =>
{
    //agregamos el archivo de recursos, que creamos
    opciones.ResourcesPath = "Recursos";
});

builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();
//agrrgamos la dependencia de automaper, indicancole que trabajara sobre el poryecto/assembly sobre el proyecto en el que estamos
builder.Services.AddAutoMapper(typeof(Program));
////implemetacion de la interface para subir archivos
//Para usar almacenamiento en Azure
//builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();
//Para usar almacenamiento de forma local
builder.Services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>(); 

var app = builder.Build();

//agregando las culturas soportadas, es espanol y en ingles
//var culturasUISoportadas = new[] { "es", "en" };


//le pasamos las culturas que hemos definidas a soportar
app.UseRequestLocalization(opciones =>
{
    //agregamos la cultura soportada por defecto
    opciones.DefaultRequestCulture = new RequestCulture("es");
    //ahora convertiremso nuestro arreglo de culturas soportadas, a una lista, la cual proyectaremos a cultura informacion
    //opciones.SupportedUICultures = culturasUISoportadas.Select(cultura => new CultureInfo(cultura))
    //                                .ToList();
    //ahora que cremaos en la clase Constantes el modelo de espanol e ingles aplicamos aqui
    opciones.SupportedUICultures = Constantes.CulturasSoportadas .Select(cultura => new CultureInfo(cultura.Value))
                                    .ToList();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=usuarios}/{action=login}/{id?}");

app.Run();
