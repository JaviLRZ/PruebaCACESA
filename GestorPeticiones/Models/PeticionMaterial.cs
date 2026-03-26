namespace GestorPeticiones.Models
{
    /// <summary>
    /// Estado posible de una petición de material.
    /// </summary>
    public enum EstadoPeticion
    {
        Pendiente,
        Aceptada,
        Rechazada
    }

    /// <summary>
    /// Entidad que representa una solicitud de material realizada por un usuario.
    /// </summary>
    public class PeticionMaterial
    {
        public int IdPeticion { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;
        public EstadoPeticion Estado { get; set; } = EstadoPeticion.Pendiente;
        public string Comentario { get; set; } = string.Empty;

        public string EstadoTexto => Estado.ToString();
    }
}
