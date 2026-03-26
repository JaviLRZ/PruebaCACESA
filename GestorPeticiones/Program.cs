using GestorPeticiones.Forms;
using GestorPeticiones.Services;

namespace GestorPeticiones;

static class Program
{
    /// <summary>
    /// Punto de entrada de la aplicación.
    /// Aquí se componen las dependencias (Composition Root) y se abre el formulario de login.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Composition Root: creamos repositorios y servicios una sola vez
        var repoUsuarios = ServiceLocator.CrearRepoUsuarios();
        var usuarioService = new UsuarioService(repoUsuarios);

        // SeedData: asegurarse de que existe al menos un supervisor de ejemplo
        SeedData.InicializarDatos(usuarioService);

        Application.Run(new LoginForm(usuarioService));
    }
}
