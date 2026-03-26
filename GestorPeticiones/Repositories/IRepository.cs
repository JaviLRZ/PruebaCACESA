namespace GestorPeticiones.Repositories
{
    /// <summary>
    /// Interfaz genérica de repositorio (patrón Repository).
    /// Principio Open/Closed: se pueden añadir implementaciones sin modificar esta interfaz.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad gestionada.</typeparam>
    public interface IRepository<T>
    {
        IEnumerable<T> ObtenerTodos();
        T? ObtenerPorId(int id);
        void Agregar(T entidad);
        void Actualizar(T entidad);
        void Eliminar(int id);
    }
}
