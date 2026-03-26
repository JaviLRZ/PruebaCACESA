using System.Text.Json;

namespace GestorPeticiones.Repositories
{
    /// <summary>
    /// Implementación genérica de repositorio con persistencia en fichero JSON.
    /// Usa LINQ para las búsquedas sobre la colección en memoria.
    /// Principio Liskov: puede sustituir a IRepository&lt;T&gt; sin romper el contrato.
    /// </summary>
    /// <typeparam name="T">Tipo de entidad con propiedad Id de tipo int.</typeparam>
    public class JsonRepository<T> : IRepository<T> where T : class
    {
        private readonly string _rutaFichero;
        private readonly Func<T, int> _obtenerIdFn;
        private List<T> _datos;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        public JsonRepository(string rutaFichero, Func<T, int> obtenerIdFn)
        {
            _rutaFichero = rutaFichero;
            _obtenerIdFn = obtenerIdFn;
            _datos = CargarDesdeFichero();
        }

        // ── Implementación de IRepository<T> ─────────────────────────────────

        public IEnumerable<T> ObtenerTodos() => _datos.ToList();

        public T? ObtenerPorId(int id)
            => _datos.FirstOrDefault(e => _obtenerIdFn(e) == id);

        public void Agregar(T entidad)
        {
            _datos.Add(entidad);
            GuardarEnFichero();
        }

        public void Actualizar(T entidad)
        {
            int indice = _datos.FindIndex(e => _obtenerIdFn(e) == _obtenerIdFn(entidad));
            if (indice < 0)
                throw new InvalidOperationException("Entidad no encontrada para actualizar.");

            _datos[indice] = entidad;
            GuardarEnFichero();
        }

        public void Eliminar(int id)
        {
            int eliminados = _datos.RemoveAll(e => _obtenerIdFn(e) == id);
            if (eliminados == 0)
                throw new InvalidOperationException($"No se encontró la entidad con id {id}.");

            GuardarEnFichero();
        }

        // ── Métodos privados de persistencia ──────────────────────────────────

        private List<T> CargarDesdeFichero()
        {
            if (!File.Exists(_rutaFichero))
                return new List<T>();

            string json = File.ReadAllText(_rutaFichero);
            return JsonSerializer.Deserialize<List<T>>(json, _jsonOptions) ?? new List<T>();
        }

        private void GuardarEnFichero()
        {
            string directorio = Path.GetDirectoryName(_rutaFichero)!;
            if (!Directory.Exists(directorio))
                Directory.CreateDirectory(directorio);

            string json = JsonSerializer.Serialize(_datos, _jsonOptions);
            File.WriteAllText(_rutaFichero, json);
        }

        /// <summary>
        /// Genera el siguiente Id disponible (max actual + 1).
        /// </summary>
        public int SiguienteId()
            => _datos.Any() ? _datos.Max(_obtenerIdFn) + 1 : 1;
    }
}
