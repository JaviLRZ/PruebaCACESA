using GestorPeticiones.Models;

namespace GestorPeticiones.Services
{
    /// <summary>
    /// Contrato del servicio de usuarios.
    /// Principio Interface Segregation: interfaz específica, no monolítica.
    /// </summary>
    public interface IUsuarioService
    {
        IEnumerable<Usuario> ObtenerTodos();
        Usuario? Autenticar(string nombreUsuario, string contrasena);
        void Agregar(Usuario usuario);
        void Actualizar(Usuario usuario);
        void Eliminar(int idUsuario);
        bool ExisteNombreUsuario(string nombreUsuario, int? idExcluir = null);
    }
}
