using bibliotecaMVC.Services;
using Microsoft.EntityFrameworkCore;
using bibliotecaMVC.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework services.
builder.Services.AddDbContext<BibliotecaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BibliotecaConnection")));


// Registro de la dependencia IAutorService -> AutorService con ciclo de vida Scoped.
// Para el Reto (Actividad 5), basta con cambiar AutorService por AutorServiceJson aquí;
// el AutoresController no necesita ninguna modificación.
builder.Services.AddScoped<IAutorService, AutorService>();

// Registro del servicio de Categorías, implementado con ADO.NET puro (sin EF Core).
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();