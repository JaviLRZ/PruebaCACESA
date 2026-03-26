using GestorPeticiones.Models;
using GestorPeticiones.Repositories;

namespace GestorPeticiones.Services
{
    /// <summary>
    /// Lógica de negocio para la gestión de usuarios.
    /// Principio Single Responsibility: solo gestiona la lógica de usuarios.
    /// Principio Dependency Inversion: depende de IRepository, no de la implementación concreta.
    /// </summary>
    public class UsuarioService : IUsuarioService
    {
        private readonly JsonRepository<Usuario> _repositorio;

        public UsuarioService(JsonRepository<Usuario> repositorio)
        {
            _repositorio = repositorio;
        }

        public IEnumerable<Usuario> ObtenerTodos()
            => _repositorio.ObtenerTodos();

        public Usuario? Autenticar(string nombreUsuario, string contrasena)
            => _repositorio.ObtenerTodos()
                           .FirstOrDefault(u => u.NombreUsuario == nombreUsuario
                                             && u.Contrasena == contrasena);

        public void Agregar(Usuario usuario)
        {
            if (ExisteNombreUsuario(usuario.NombreUsuario))
                throw new InvalidOperationException($"El usuario '{usuario.NombreUsuario}' ya existe.");

            usuario.IdUsuario = _repositorio.SiguienteId();
            _repositorio.Agregar(usuario);
        }

        public void Actualizar(Usuario usuario)
        {
            if (ExisteNombreUsuario(usuario.NombreUsuario, usuario.IdUsuario))
                throw new InvalidOperationException($"El usuario '{usuario.NombreUsuario}' ya está en uso.");

            _repositorio.Actualizar(usuario);
        }

        public void Eliminar(int idUsuario)
            => _repositorio.Eliminar(idUsuario);

        public bool ExisteNombreUsuario(string nombreUsuario, int? idExcluir = null)
            => _repositorio.ObtenerTodos()
                           .Any(u => u.NombreUsuario == nombreUsuario
                                  && u.IdUsuario != (idExcluir ?? -1));
    }
}
