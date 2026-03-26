using GestorPeticiones.Models;
using GestorPeticiones.Services;

namespace GestorPeticiones
{
    /// <summary>
    /// Datos iniciales de la aplicación.
    /// Crea el usuario supervisor por defecto si la base de datos está vacía.
    /// </summary>
    public static class SeedData
    {
        public static void InicializarDatos(IUsuarioService usuarioService)
        {
            if (usuarioService.ObtenerTodos().Any()) return;

            usuarioService.Agregar(new Usuario
            {
                Nombre        = "Admin",
                Apellidos     = "Sistema",
                Direccion     = "Sede Central",
                NombreUsuario = "admin",
                Contrasena    = "admin123",
                EsSupervisor  = true
            });
        }
    }
}
