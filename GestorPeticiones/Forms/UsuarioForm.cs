using GestorPeticiones.Models;
using GestorPeticiones.Services;

namespace GestorPeticiones.Forms
{
    /// <summary>
    /// Formulario de gestión de usuarios (solo accesible por el supervisor).
    /// Permite al supervisor crear, editar y eliminar usuarios del sistema.
    /// </summary>
    public class UsuarioForm : Form
    {
        private readonly IUsuarioService _usuarioService;
        private Usuario? _usuarioSeleccionado;

        // ── Controles ─────────────────────────────────────────────────────────
        private DataGridView dgvUsuarios = null!;
        private Button btnNuevo = null!;
        private Button btnEditar = null!;
        private Button btnEliminar = null!;
        private GroupBox grpDetalle = null!;
        private TextBox txtNombre = null!;
        private TextBox txtApellidos = null!;
        private TextBox txtDireccion = null!;
        private TextBox txtNombreUsuario = null!;
        private TextBox txtContrasena = null!;
        private CheckBox chkEsSupervisor = null!;
        private Button btnGuardar = null!;
        private Button btnCancelar = null!;

        public UsuarioForm(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            InicializarComponentes();
            CargarUsuarios();
        }

        private void InicializarComponentes()
        {
            Text            = "Gestión de Usuarios";
            Size            = new Size(900, 520);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            BackColor       = Color.FromArgb(245, 247, 250);
            Font            = new Font("Segoe UI", 9.5f);
            MinimumSize     = new Size(900, 520);

            // ── DataGridView ──────────────────────────────────────────────────
            dgvUsuarios = new DataGridView
            {
                Location          = new Point(10, 10),
                Size              = new Size(450, 440),
                ReadOnly          = true,
                SelectionMode     = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect       = false,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor   = Color.White,
                BorderStyle       = BorderStyle.None
            };
            dgvUsuarios.SelectionChanged += DgvUsuarios_SelectionChanged;
            UIHelper.FormatearGrid(dgvUsuarios);

            // ── Botones de acción ─────────────────────────────────────────────
            btnNuevo   = CrearBoton("➕ Nuevo",   new Point(470, 10),  Color.FromArgb(30, 80, 160));
            btnEditar  = CrearBoton("✏️ Editar",  new Point(470, 55),  Color.FromArgb(0, 140, 90));
            btnEliminar = CrearBoton("🗑 Eliminar", new Point(470, 100), Color.FromArgb(200, 60, 60));
            btnNuevo.Click    += BtnNuevo_Click;
            btnEditar.Click   += BtnEditar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnEditar.Enabled   = false;
            btnEliminar.Enabled = false;

            UIHelper.FormatearBoton(btnNuevo);
            UIHelper.FormatearBoton(btnEditar);
            UIHelper.FormatearBoton(btnEliminar);

            // ── Panel de detalle / edición ────────────────────────────────────
            grpDetalle = new GroupBox
            {
                Text      = "Datos del usuario",
                Location  = new Point(470, 155),
                Size      = new Size(400, 305),
                ForeColor = Color.FromArgb(30, 80, 160),
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Visible   = false
            };

            var labels = new[] { "Nombre:", "Apellidos:", "Dirección:", "Usuario:", "Contraseña:" };
            txtNombre       = CrearTexto(grpDetalle, labels[0], 0);
            txtApellidos    = CrearTexto(grpDetalle, labels[1], 1);
            txtDireccion    = CrearTexto(grpDetalle, labels[2], 2);
            txtNombreUsuario = CrearTexto(grpDetalle, labels[3], 3);
            txtContrasena   = CrearTexto(grpDetalle, labels[4], 4);
            txtContrasena.PasswordChar = '●';

            chkEsSupervisor = new CheckBox
            {
                Text     = "Es Supervisor",
                Location = new Point(10, 220),
                AutoSize = true,
                Font     = new Font("Segoe UI", 9.5f)
            };
            grpDetalle.Controls.Add(chkEsSupervisor);

            btnGuardar  = CrearBoton("💾 Guardar",   new Point(15, 260),  Color.FromArgb(30, 80, 160), grpDetalle);
            btnCancelar = CrearBoton("✖ Cancelar",  new Point(215, 260), Color.FromArgb(120, 120, 120), grpDetalle);
            btnGuardar.Click  += BtnGuardar_Click;
            btnCancelar.Click += (s, e) => MostrarDetalle(false);
            UIHelper.FormatearBoton(btnGuardar);
            UIHelper.FormatearBoton(btnCancelar);

            Controls.AddRange(new Control[]
            {
                dgvUsuarios, btnNuevo, btnEditar, btnEliminar, grpDetalle
            });
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private Button CrearBoton(string texto, Point pos, Color color, Control? padre = null)
        {
            var btn = new Button
            {
                Text      = texto,
                Location  = pos,
                Size      = new Size(170, 35),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 9.5f)
            };
            btn.FlatAppearance.BorderSize = 0;
            (padre ?? (Control)this).Controls.Add(btn);
            return btn;
        }

        private TextBox CrearTexto(GroupBox padre, string etiqueta, int fila)
        {
            int y = 25 + fila * 38;
            padre.Controls.Add(new Label { Text = etiqueta, Location = new Point(10, y), AutoSize = true });
            var txt = new TextBox { Location = new Point(10, y + 18), Size = new Size(360, 22) };
            padre.Controls.Add(txt);
            return txt;
        }

        // ── Carga de datos ────────────────────────────────────────────────────

        private void CargarUsuarios()
        {
            var usuarios = _usuarioService.ObtenerTodos()
                .Select(u => new
                {
                    u.IdUsuario,
                    u.Nombre,
                    u.Apellidos,
                    u.Direccion,
                    u.NombreUsuario,
                    Rol = u.EsSupervisor ? "Supervisor" : "Usuario"
                }).ToList();

            dgvUsuarios.DataSource = usuarios;

            if (dgvUsuarios.Columns.Contains("IdUsuario"))
                dgvUsuarios.Columns["IdUsuario"]!.HeaderText = "ID";
        }

        // ── Eventos ──────────────────────────────────────────────────────────

        private void DgvUsuarios_SelectionChanged(object? sender, EventArgs e)
        {
            bool haySeleccion = dgvUsuarios.SelectedRows.Count > 0;
            btnEditar.Enabled   = haySeleccion;
            btnEliminar.Enabled = haySeleccion;
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            _usuarioSeleccionado = null;
            LimpiarFormulario();
            MostrarDetalle(true);
        }

        private void BtnEditar_Click(object? sender, EventArgs e)
        {
            int id = (int)dgvUsuarios.SelectedRows[0].Cells["IdUsuario"].Value;
            _usuarioSeleccionado = _usuarioService.ObtenerTodos().FirstOrDefault(u => u.IdUsuario == id);
            if (_usuarioSeleccionado == null) return;

            txtNombre.Text        = _usuarioSeleccionado.Nombre;
            txtApellidos.Text     = _usuarioSeleccionado.Apellidos;
            txtDireccion.Text     = _usuarioSeleccionado.Direccion;
            txtNombreUsuario.Text = _usuarioSeleccionado.NombreUsuario;
            txtContrasena.Text    = _usuarioSeleccionado.Contrasena;
            chkEsSupervisor.Checked = _usuarioSeleccionado.EsSupervisor;
            MostrarDetalle(true);
        }

        private void BtnEliminar_Click(object? sender, EventArgs e)
        {
            int id = (int)dgvUsuarios.SelectedRows[0].Cells["IdUsuario"].Value;
            var resultado = MessageBox.Show(
                "¿Seguro que deseas eliminar este usuario?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (resultado == DialogResult.Yes)
            {
                _usuarioService.Eliminar(id);
                CargarUsuarios();
                MostrarDetalle(false);
            }
        }

        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text)    ||
                string.IsNullOrWhiteSpace(txtNombreUsuario.Text) ||
                string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                MessageBox.Show("Nombre, usuario y contraseña son obligatorios.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (_usuarioSeleccionado == null)
                {
                    // Nuevo usuario
                    var nuevo = new Models.Usuario
                    {
                        Nombre         = txtNombre.Text.Trim(),
                        Apellidos      = txtApellidos.Text.Trim(),
                        Direccion      = txtDireccion.Text.Trim(),
                        NombreUsuario  = txtNombreUsuario.Text.Trim(),
                        Contrasena     = txtContrasena.Text,
                        EsSupervisor   = chkEsSupervisor.Checked
                    };
                    _usuarioService.Agregar(nuevo);
                }
                else
                {
                    // Actualizar existente
                    _usuarioSeleccionado.Nombre        = txtNombre.Text.Trim();
                    _usuarioSeleccionado.Apellidos     = txtApellidos.Text.Trim();
                    _usuarioSeleccionado.Direccion     = txtDireccion.Text.Trim();
                    _usuarioSeleccionado.NombreUsuario = txtNombreUsuario.Text.Trim();
                    _usuarioSeleccionado.Contrasena    = txtContrasena.Text;
                    _usuarioSeleccionado.EsSupervisor  = chkEsSupervisor.Checked;
                    _usuarioService.Actualizar(_usuarioSeleccionado);
                }

                CargarUsuarios();
                MostrarDetalle(false);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarFormulario()
        {
            txtNombre.Clear();
            txtApellidos.Clear();
            txtDireccion.Clear();
            txtNombreUsuario.Clear();
            txtContrasena.Clear();
            chkEsSupervisor.Checked = false;
        }

        private void MostrarDetalle(bool visible)
        {
            grpDetalle.Visible = visible;
        }
    }
}
