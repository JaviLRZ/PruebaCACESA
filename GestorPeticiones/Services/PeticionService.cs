using GestorPeticiones.Models;
using GestorPeticiones.Repositories;

namespace GestorPeticiones.Services
{
    /// <summary>
    /// Lógica de negocio para la gestión de peticiones de material.
    /// Principio Single Responsibility: solo gestiona la lógica de peticiones.
    /// Usa LINQ encadenado para los filtros de búsqueda.
    /// </summary>
    public class PeticionService : IPeticionService
    {
        private readonly JsonRepository<PeticionMaterial> _repositorio;

        public PeticionService(JsonRepository<PeticionMaterial> repositorio)
        {
            _repositorio = repositorio;
        }

        public IEnumerable<PeticionMaterial> ObtenerTodas()
            => _repositorio.ObtenerTodos();

        public IEnumerable<PeticionMaterial> ObtenerPorUsuario(int idUsuario)
            => _repositorio.ObtenerTodos()
                           .Where(p => p.IdUsuario == idUsuario);

        /// <summary>
        /// Búsqueda combinada con LINQ. Cada filtro es opcional (null = sin filtro).
        /// </summary>
        public IEnumerable<PeticionMaterial> Buscar(
            int? idUsuario,
            string? descripcion,
            DateTime? fechaDesde,
            DateTime? fechaHasta,
            EstadoPeticion? estado)
        {
            var query = _repositorio.ObtenerTodos().AsQueryable();

            if (idUsuario.HasValue)
                query = query.Where(p => p.IdUsuario == idUsuario.Value);

            if (!string.IsNullOrWhiteSpace(descripcion))
                query = query.Where(p => p.Descripcion
                                          .Contains(descripcion, StringComparison.OrdinalIgnoreCase));

            if (fechaDesde.HasValue)
                query = query.Where(p => p.FechaSolicitud.Date >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
                query = query.Where(p => p.FechaSolicitud.Date <= fechaHasta.Value.Date);

            if (estado.HasValue)
                query = query.Where(p => p.Estado == estado.Value);

            return query.OrderByDescending(p => p.FechaSolicitud).ToList();
        }

        public void Agregar(PeticionMaterial peticion)
        {
            peticion.IdPeticion = _repositorio.SiguienteId();
            peticion.FechaSolicitud = DateTime.Now;
            peticion.Estado = EstadoPeticion.Pendiente;
            _repositorio.Agregar(peticion);
        }

        public void ActualizarEstado(int idPeticion, EstadoPeticion nuevoEstado, string comentario)
        {
            var peticion = _repositorio.ObtenerPorId(idPeticion)
                ?? throw new InvalidOperationException($"No se encontró la petición con id {idPeticion}.");

            peticion.Estado = nuevoEstado;
            peticion.Comentario = comentario;
            _repositorio.Actualizar(peticion);
        }
    }
}
