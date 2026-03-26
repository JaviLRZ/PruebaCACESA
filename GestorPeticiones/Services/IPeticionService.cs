using GestorPeticiones.Models;

namespace GestorPeticiones.Services
{
    /// <summary>
    /// Contrato del servicio de peticiones de material.
    /// Principio Interface Segregation: separado de IUsuarioService.
    /// </summary>
    public interface IPeticionService
    {
        IEnumerable<PeticionMaterial> ObtenerTodas();
        IEnumerable<PeticionMaterial> ObtenerPorUsuario(int idUsuario);

        /// <summary>
        /// Búsqueda con filtros combinados mediante LINQ.
        /// </summary>
        IEnumerable<PeticionMaterial> Buscar(
            int? idUsuario,
            string? descripcion,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            EstadoPeticion? estado);

        void Agregar(PeticionMaterial peticion);
        void ActualizarEstado(int idPeticion, EstadoPeticion nuevoEstado, string comentario);
    }
}
