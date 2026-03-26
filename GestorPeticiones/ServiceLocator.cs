using GestorPeticiones.Models;
using GestorPeticiones.Repositories;

namespace GestorPeticiones
{
    /// <summary>
    /// Localizador de servicios simple para composición de dependencias.
    /// Centraliza la creación de repositorios y resuelve las rutas de los ficheros JSON.
    /// En una app mayor se usaría un contenedor DI como Microsoft.Extensions.DependencyInjection.
    /// </summary>
    public static class ServiceLocator
    {
        /// <summary>
        /// Directorio donde se guardan los ficheros de datos JSON.
        /// Usa la carpeta 'Data' junto al ejecutable.
        /// </summary>
        private static readonly string DirectorioDatos =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

        public static JsonRepository<Usuario> CrearRepoUsuarios()
            => new(Path.Combine(DirectorioDatos, "usuarios.json"),
                   u => u.IdUsuario);

        public static JsonRepository<PeticionMaterial> CrearRepoPeticiones()
            => new(Path.Combine(DirectorioDatos, "peticiones.json"),
                   p => p.IdPeticion);
    }
}
