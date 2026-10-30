using bibliotecaMVC.Services;
using Microsoft.EntityFrameworkCore;
using bibliotecaMVC.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework services con interceptor e Identity
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BibliotecaConnection"))
           .AddInterceptors(new NumericRoundAbortConnectionInterceptor()));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
})
    .AddEntityFrameworkStores<BibliotecaContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Registro de servicios existentes
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

var app = builder.Build();

// Crear roles y usuarios iniciales al arrancar
await RolesInitializer.InicializarAsync(app.Services);

// Configure the HTTP request pipeline.
// En desarrollo se puede activar la pagina de error personalizada con
// "PaginaErrorPersonalizada": true en appsettings.Development.json
bool usarPaginaError = !app.Environment.IsDevelopment()
    || app.Configuration.GetValue<bool>("PaginaErrorPersonalizada");

if (usarPaginaError)
{
    app.UseExceptionHandler("/Home/Error");
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Paginas amigables para 404 y otros codigos sin cuerpo (el 403 lo maneja AccessDeniedPath)
app.UseStatusCodePagesWithReExecute("/Home/EstadoHttp", "?codigo={0}");

app.UseHttpsRedirection();
app.UseRouting();

// Mantenimiento de orden: Autenticación antes de Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();