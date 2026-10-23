namespace bibliotecaMVC.Models
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Bibliotecario = "Bibliotecario";
        public const string Usuario = "Usuario";

        // Roles que pueden agregar, editar y eliminar libros
        public const string GestionLibros = Admin + "," + Bibliotecario;

        public static readonly string[] Todos = { Admin, Bibliotecario, Usuario };
    }
}
