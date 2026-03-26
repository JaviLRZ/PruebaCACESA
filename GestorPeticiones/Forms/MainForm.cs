using GestorPeticiones.Models;
using GestorPeticiones.Services;

namespace GestorPeticiones.Forms
{
    /// <summary>
    /// Formulario principal (menú). Muestra opciones según el rol del usuario autenticado.
    /// Supervisor: puede gestionar usuarios Y ver todas las peticiones.
    /// Usuario normal: solo puede gestionar sus propias peticiones.
    /// </summary>
    public class MainForm : Form
    {
        private readonly Usuario _usuarioActual;
        private readonly IUsuarioService _usuarioService;

        private Label lblBienvenida = null!;
        private Label lblRol = null!;
        private Button btnUsuarios = null!;
        private Button btnPeticiones = null!;
        private Button btnCerrarSesion = null!;
        private Panel pnlBotones = null!;

        public MainForm(Usuario usuarioActual, IUsuarioService usuarioService)
        {
            _usuarioActual  = usuarioActual;
            _usuarioService = usuarioService;
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            Text            = "Gestor de Peticiones – Menú Principal";
            Size            = new Size(420, 320);
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            BackColor       = Color.FromArgb(245, 247, 250);
            Font            = new Font("Segoe UI", 10f);

            // ── Bienvenida ────────────────────────────────────────────────────
            lblBienvenida = new Label
            {
                Text      = $"Bienvenido/a, {_usuarioActual.NombreCompleto}",
                Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 160),
                AutoSize  = true,
                Location  = new Point(30, 25)
            };

            lblRol = new Label
            {
                Text      = _usuarioActual.EsSupervisor ? "Rol: Supervisor" : "Rol: Usuario",
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                ForeColor = _usuarioActual.EsSupervisor ? Color.DarkGreen : Color.Gray,
                AutoSize  = true,
                Location  = new Point(30, 55)
            };

            // ── Panel de botones ──────────────────────────────────────────────
            pnlBotones = new Panel
            {
                Location  = new Point(30, 90),
                Size      = new Size(350, 150),
                BackColor = Color.Transparent
            };

            // Gestión de Usuarios (solo supervisor)
            btnUsuarios = new Button
            {
                Text      = "👥  Gestión de Usuarios",
                Size      = new Size(310, 45),
                Location  = new Point(0, 0),
                BackColor = Color.FromArgb(30, 80, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10.5f),
                Cursor    = Cursors.Hand,
                Visible   = _usuarioActual.EsSupervisor
            };
            btnUsuarios.FlatAppearance.BorderSize = 0;
            btnUsuarios.Click += BtnUsuarios_Click;

            // Peticiones de Material
            btnPeticiones = new Button
            {
                Text      = "📋  Peticiones de Material",
                Size      = new Size(310, 45),
                Location  = new Point(0, _usuarioActual.EsSupervisor ? 60 : 0),
                BackColor = Color.FromArgb(0, 140, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10.5f),
                Cursor    = Cursors.Hand
            };
            btnPeticiones.FlatAppearance.BorderSize = 0;
            btnPeticiones.Click += BtnPeticiones_Click;

            pnlBotones.Controls.AddRange(new Control[] { btnUsuarios, btnPeticiones });

            // ── Cerrar sesión ─────────────────────────────────────────────────
            btnCerrarSesion = new Button
            {
                Text      = "Cerrar sesión",
                Location  = new Point(290, 250),
                Size      = new Size(110, 30),
                BackColor = Color.FromArgb(200, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand
            };
            btnCerrarSesion.FlatAppearance.BorderSize = 0;
            btnCerrarSesion.Click += (s, e) => Close();

            Controls.AddRange(new Control[]
            {
                lblBienvenida, lblRol, pnlBotones, btnCerrarSesion
            });
        }

        private void BtnUsuarios_Click(object? sender, EventArgs e)
        {
            using var form = new UsuarioForm(_usuarioService);
            form.ShowDialog(this);
        }

        private void BtnPeticiones_Click(object? sender, EventArgs e)
        {
            // PeticionService se instancia aquí para mantener la inyección limpia
            var peticionRepo = ServiceLocator.CrearRepoPeticiones();
            var peticionService = new Services.PeticionService(peticionRepo);

            using var form = new PeticionForm(_usuarioActual, peticionService, _usuarioService);
            form.ShowDialog(this);
        }
    }
}
