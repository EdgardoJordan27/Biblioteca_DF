using bibliotecaMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace bibliotecaMVC.Data
{
    public static class RolesInitializer
    {
        // Nombres usados en la semana 13, se renombran para no dejar roles duplicados
        private static readonly Dictionary<string, string> RolesAnteriores = new()
        {
            { "Administrador", Roles.Admin },
            { "Lector", Roles.Usuario }
        };

        public static async Task InicializarAsync(IServiceProvider servicios)
        {
            using var scope = servicios.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RolesInitializer");

            await RenombrarRolesAnterioresAsync(roleManager, logger);

            foreach (var nombreRol in Roles.Todos)
            {
                if (await roleManager.RoleExistsAsync(nombreRol))
                {
                    continue;
                }

                var resultado = await roleManager.CreateAsync(new IdentityRole(nombreRol));
                if (!resultado.Succeeded)
                {
                    logger.LogError("No se pudo crear el rol {Rol}: {Errores}", nombreRol, Describir(resultado));
                }
            }

            var passwordInicial = config["UsuariosIniciales:Password"] ?? "Biblioteca123";

            await CrearUsuarioAsync(userManager, logger, "admin", "admin@biblioteca.com", passwordInicial, Roles.Admin);
            await CrearUsuarioAsync(userManager, logger, "bibliotecario", "bibliotecario@biblioteca.com", passwordInicial, Roles.Bibliotecario);
            await CrearUsuarioAsync(userManager, logger, "usuario", "usuario@biblioteca.com", passwordInicial, Roles.Usuario);

            // Usuarios existentes sin ningun rol quedan como Usuario
            foreach (var usuario in userManager.Users.ToList())
            {
                var rolesActuales = await userManager.GetRolesAsync(usuario);
                if (rolesActuales.Count == 0)
                {
                    await AsignarRolAsync(userManager, logger, usuario, Roles.Usuario);
                }
            }
        }

        private static async Task RenombrarRolesAnterioresAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
        {
            foreach (var (anterior, nuevo) in RolesAnteriores)
            {
                var rol = await roleManager.FindByNameAsync(anterior);
                if (rol == null || await roleManager.RoleExistsAsync(nuevo))
                {
                    continue;
                }

                rol.Name = nuevo;
                var resultado = await roleManager.UpdateAsync(rol);
                if (!resultado.Succeeded)
                {
                    logger.LogError("No se pudo renombrar el rol {Anterior} a {Nuevo}: {Errores}", anterior, nuevo, Describir(resultado));
                }
            }
        }

        private static async Task CrearUsuarioAsync(
            UserManager<IdentityUser> userManager, ILogger logger,
            string nombre, string correo, string password, string rol)
        {
            var usuario = await userManager.FindByEmailAsync(correo);

            if (usuario == null)
            {
                usuario = new IdentityUser
                {
                    UserName = nombre,
                    Email = correo,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(usuario, password);
                if (!resultado.Succeeded)
                {
                    logger.LogError("No se pudo crear el usuario {Usuario}: {Errores}", nombre, Describir(resultado));
                    return;
                }
            }

            await AsignarRolAsync(userManager, logger, usuario, rol);
        }

        private static async Task AsignarRolAsync(
            UserManager<IdentityUser> userManager, ILogger logger, IdentityUser usuario, string rol)
        {
            if (await userManager.IsInRoleAsync(usuario, rol))
            {
                return;
            }

            var resultado = await userManager.AddToRoleAsync(usuario, rol);
            if (!resultado.Succeeded)
            {
                logger.LogError("No se pudo asignar el rol {Rol} a {Usuario}: {Errores}", rol, usuario.UserName, Describir(resultado));
            }
        }

        private static string Describir(IdentityResult resultado)
        {
            return string.Join("; ", resultado.Errors.Select(e => e.Description));
        }
    }
}
