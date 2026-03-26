namespace GestorPeticiones.Models
{
    /// <summary>
    /// Entidad que representa un usuario del sistema.
    /// </summary>
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public bool EsSupervisor { get; set; }

        public string NombreCompleto => $"{Nombre} {Apellidos}";

        public override string ToString() => NombreCompleto;
    }
}
