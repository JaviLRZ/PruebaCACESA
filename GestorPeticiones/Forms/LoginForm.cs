using GestorPeticiones.Models;
using GestorPeticiones.Services;

namespace GestorPeticiones.Forms
{
    /// <summary>
    /// Formulario de inicio de sesión.
    /// Valida credenciales y abre el menú principal con el usuario autenticado.
    /// </summary>
    public class LoginForm : Form
    {
        private readonly IUsuarioService _usuarioService;

        // ── Controles ─────────────────────────────────────────────────────────
        private Label lblTitulo = null!;
        private Label lblUsuario = null!;
        private Label lblContrasena = null!;
        private TextBox txtUsuario = null!;
        private TextBox txtContrasena = null!;
        private Button btnEntrar = null!;
        private Label lblError = null!;

        public LoginForm(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            // ── Formulario ────────────────────────────────────────────────────
            Text            = "Gestor de Peticiones – Iniciar sesión";
            Size            = new Size(380, 320); // Aumentado el alto para dar aire al botón
            StartPosition   = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox     = false;
            MinimizeBox     = false;
            BackColor       = Color.FromArgb(245, 247, 250);
            Font            = new Font("Segoe UI", 9.5f);

            // ── Título ────────────────────────────────────────────────────────
            lblTitulo = new Label
            {
                Text      = "🗂  Gestor de Peticiones",
                Font      = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 160),
                AutoSize  = true,
                Location  = new Point(75, 20) // Centrado aproximado para el texto
            };

            // ── Usuario ───────────────────────────────────────────────────────
            lblUsuario = new Label  { Text = "Usuario:", Location = new Point(45, 80), AutoSize = true };
            txtUsuario = new TextBox
            {
                Location = new Point(45, 100),
                Size     = new Size(290, 24),
                Name     = "txtUsuario"
            };

            // ── Contraseña ────────────────────────────────────────────────────
            lblContrasena = new Label { Text = "Contraseña:", Location = new Point(45, 135), AutoSize = true };
            txtContrasena = new TextBox
            {
                Location     = new Point(45, 155),
                Size         = new Size(290, 24),
                PasswordChar = '●',
                Name         = "txtContrasena"
            };
            txtContrasena.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnEntrar_Click(s, e); };

            // ── Error ─────────────────────────────────────────────────────────
            lblError = new Label
            {
                Text      = string.Empty,
                ForeColor = Color.Red,
                AutoSize  = true,
                Location  = new Point(45, 190)
            };

            // ── Botón Entrar ──────────────────────────────────────────────────
            btnEntrar = new Button
            {
                Text      = "Entrar",
                Location  = new Point(135, 225), // Movido un poco más abajo
                Size      = new Size(110, 33),
                BackColor = Color.FromArgb(30, 80, 160),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnEntrar.FlatAppearance.BorderSize = 0;
            btnEntrar.Click += BtnEntrar_Click;
            UIHelper.FormatearBoton(btnEntrar);

            // ── Añadir controles ──────────────────────────────────────────────
            Controls.AddRange(new Control[]
            {
                lblTitulo, lblUsuario, txtUsuario,
                lblContrasena, txtContrasena,
                lblError, btnEntrar
            });
        }

        private void BtnEntrar_Click(object? sender, EventArgs e)
        {
            lblError.Text = string.Empty;

            if (string.IsNullOrWhiteSpace(txtUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                lblError.Text = "Introduce usuario y contraseña.";
                return;
            }

            Usuario? usuario = _usuarioService.Autenticar(txtUsuario.Text.Trim(),
                                                           txtContrasena.Text);
            if (usuario == null)
            {
                lblError.Text = "Usuario o contraseña incorrectos.";
                txtContrasena.Clear();
                return;
            }

            // Ocultar login y abrir menú principal
            Hide();
            using var mainForm = new MainForm(usuario, _usuarioService);
            mainForm.ShowDialog();
            Close();
        }
    }
}
