using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using bibliotecaMVC.Models;

namespace bibliotecaMVC.Data
{
    public class BibliotecaContext : IdentityDbContext<IdentityUser>
    {
        public BibliotecaContext(DbContextOptions<BibliotecaContext> options) : base(options)
        {
        }

        public DbSet<Libro> Libros { get; set; }
    }
}