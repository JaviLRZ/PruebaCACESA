using GestorPeticiones.Models;
using GestorPeticiones.Services;

namespace GestorPeticiones.Forms
{
    /// <summary>
    /// Formulario de peticiones de material.
    /// - Usuario: crea nuevas peticiones y ve las suyas con filtros.
    /// - Supervisor: ve TODAS las peticiones, puede filtrar también por nombre de usuario,
    ///               y puede aceptar o rechazar peticiones pendientes.
    /// </summary>
    public class PeticionForm : Form
    {
        private readonly Usuario _usuarioActual;
        private readonly IPeticionService _peticionService;
        private readonly IUsuarioService _usuarioService;

        // ── Controles de búsqueda ──────────────────────────────────────────────
        private GroupBox grpFiltros = null!;
        private TextBox txtDescripcionFiltro = null!;
        private DateTimePicker dtpDesde = null!;
        private CheckBox chkUsarDesde = null!;
        private DateTimePicker dtpHasta = null!;
        private CheckBox chkUsarHasta = null!;
        private ComboBox cmbEstadoFiltro = null!;
        private TextBox txtNombreUsuarioFiltro = null!;
        private Label lblNombreUsuarioFiltro = null!;
        private Button btnBuscar = null!;
        private Button btnLimpiar = null!;

        // ── Controles principales ─────────────────────────────────────────────
        private DataGridView dgvPeticiones = null!;
        private GroupBox grpNueva = null!;
        private TextBox txtDescripcionNueva = null!;
        private Button btnCrearPeticion = null!;

        // ── Acciones supervisor ───────────────────────────────────────────────
        private GroupBox grpAccion = null!;
        private TextBox txtComentario = null!;
        private Button btnAceptar = null!;
        private Button btnRechazar = null!;

        public PeticionForm(Usuario usuarioActual, IPeticionService peticionService,
                              IUsuarioService usuarioService)
        {
            _usuarioActual  = usuarioActual;
            _peticionService = peticionService;
            _usuarioService  = usuarioService;

            InicializarComponentes();
            EjecutarBusqueda();
        }

        private void InicializarComponentes()
        {
            Text            = "Peticiones de Material";
            Size            = new Size(980, 620);
            MinimumSize     = new Size(980, 620);
            StartPosition   = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            BackColor       = Color.FromArgb(245, 247, 250);
            Font            = new Font("Segoe UI", 9.5f);

            // ── Panel de filtros ──────────────────────────────────────────────
            grpFiltros = new GroupBox
            {
                Text     = "🔍  Búsqueda / Filtros",
                Location = new Point(10, 8),
                Size     = new Size(955, 110),
                Font     = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 160)
            };

            // Descripción
            grpFiltros.Controls.Add(new Label { Text = "Descripción:", Location = new Point(10, 22), AutoSize = true });
            txtDescripcionFiltro = new TextBox { Location = new Point(10, 42), Size = new Size(180, 22) };
            grpFiltros.Controls.Add(txtDescripcionFiltro);

            // Desde
            chkUsarDesde = new CheckBox { Text = "Desde:", Location = new Point(205, 22), AutoSize = true };
            dtpDesde = new DateTimePicker { Location = new Point(205, 42), Size = new Size(140, 22), Enabled = false, Format = DateTimePickerFormat.Short };
            chkUsarDesde.CheckedChanged += (s, e) => dtpDesde.Enabled = chkUsarDesde.Checked;
            grpFiltros.Controls.AddRange(new Control[] { chkUsarDesde, dtpDesde });

            // Hasta
            chkUsarHasta = new CheckBox { Text = "Hasta:", Location = new Point(355, 22), AutoSize = true };
            dtpHasta = new DateTimePicker { Location = new Point(355, 42), Size = new Size(140, 22), Enabled = false, Format = DateTimePickerFormat.Short };
            chkUsarHasta.CheckedChanged += (s, e) => dtpHasta.Enabled = chkUsarHasta.Checked;
            grpFiltros.Controls.AddRange(new Control[] { chkUsarHasta, dtpHasta });

            // Estado
            grpFiltros.Controls.Add(new Label { Text = "Estado:", Location = new Point(505, 22), AutoSize = true });
            cmbEstadoFiltro = new ComboBox
            {
                Location     = new Point(505, 42),
                Size         = new Size(130, 22),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbEstadoFiltro.Items.AddRange(new object[] { "(Todos)", "Pendiente", "Aceptada", "Rechazada" });
            cmbEstadoFiltro.SelectedIndex = 0;
            grpFiltros.Controls.Add(cmbEstadoFiltro);

            // Nombre usuario (solo supervisor)
            lblNombreUsuarioFiltro = new Label
            {
                Text     = "Nombre usuario:",
                Location = new Point(645, 22),
                AutoSize = true,
                Visible  = _usuarioActual.EsSupervisor
            };
            txtNombreUsuarioFiltro = new TextBox
            {
                Location = new Point(645, 42),
                Size     = new Size(140, 22),
                Visible  = _usuarioActual.EsSupervisor
            };
            grpFiltros.Controls.AddRange(new Control[] { lblNombreUsuarioFiltro, txtNombreUsuarioFiltro });

            // Botones búsqueda
            btnBuscar  = CrearBotonPequeno("🔍 Buscar",  new Point(800, 38), Color.FromArgb(30, 80, 160));
            btnLimpiar = CrearBotonPequeno("✖ Limpiar", new Point(880, 38), Color.FromArgb(120, 120, 120));
            grpFiltros.Controls.AddRange(new Control[] { btnBuscar, btnLimpiar });
            btnBuscar.Click  += (s, e) => EjecutarBusqueda();
            btnLimpiar.Click += BtnLimpiar_Click;

            Controls.Add(grpFiltros);

            // ── DataGridView ──────────────────────────────────────────────────
            dgvPeticiones = new DataGridView
            {
                Location          = new Point(10, 130),
                Size              = new Size(955, 260),
                ReadOnly          = true,
                SelectionMode     = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect       = false,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor   = Color.White,
                BorderStyle       = BorderStyle.None
            };
            dgvPeticiones.SelectionChanged += DgvPeticiones_SelectionChanged;
            Controls.Add(dgvPeticiones);

            // ── Nueva petición ────────────────────────────────────────────────
            grpNueva = new GroupBox
            {
                Text      = "➕  Nueva petición",
                Location  = new Point(10, 400),
                Size      = new Size(500, 75),
                ForeColor = Color.FromArgb(0, 140, 90),
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            grpNueva.Controls.Add(new Label { Text = "Descripción:", Location = new Point(10, 22), AutoSize = true });
            txtDescripcionNueva = new TextBox { Location = new Point(10, 42), Size = new Size(370, 22) };
            btnCrearPeticion = CrearBotonPequeno("Enviar", new Point(390, 40), Color.FromArgb(0, 140, 90));
            grpNueva.Controls.AddRange(new Control[] { txtDescripcionNueva, btnCrearPeticion });
            btnCrearPeticion.Click += BtnCrearPeticion_Click;
            Controls.Add(grpNueva);

            // ── Acciones supervisor ───────────────────────────────────────────
            grpAccion = new GroupBox
            {
                Text      = "✅  Resolución (solo supervisor)",
                Location  = new Point(520, 400),
                Size      = new Size(445, 75),
                Visible   = _usuarioActual.EsSupervisor,
                ForeColor = Color.FromArgb(30, 80, 160),
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            grpAccion.Controls.Add(new Label { Text = "Comentario:", Location = new Point(10, 22), AutoSize = true });
            txtComentario = new TextBox { Location = new Point(90, 20), Size = new Size(180, 22) };
            btnAceptar   = CrearBotonPequeno("✔ Aceptar",  new Point(280, 40), Color.FromArgb(0, 140, 90));
            btnRechazar  = CrearBotonPequeno("✖ Rechazar", new Point(360, 40), Color.FromArgb(200, 60, 60));
            btnAceptar.Enabled  = false;
            btnRechazar.Enabled = false;
            grpAccion.Controls.AddRange(new Control[] { txtComentario, btnAceptar, btnRechazar });
            btnAceptar.Click  += BtnAceptar_Click;
            btnRechazar.Click += BtnRechazar_Click;
            Controls.Add(grpAccion);
        }

        // ── Helper ────────────────────────────────────────────────────────────

        private Button CrearBotonPequeno(string texto, Point pos, Color color, Control? padre = null)
        {
            var btn = new Button
            {
                Text      = texto,
                Location  = pos,
                Size      = new Size(75, 26),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 8.5f)
            };
            btn.FlatAppearance.BorderSize = 0;
            (padre ?? (Control)this).Controls.Add(btn);
            return btn;
        }

        // ── Lógica ────────────────────────────────────────────────────────────

        private void EjecutarBusqueda()
        {
            // Determinar idUsuario según rol
            int? idUsuario = _usuarioActual.EsSupervisor ? null : (int?)_usuarioActual.IdUsuario;

            // Filtro por nombre de usuario (supervisor): busca el id del usuario
            if (_usuarioActual.EsSupervisor && !string.IsNullOrWhiteSpace(txtNombreUsuarioFiltro.Text))
            {
                var usuarioEncontrado = _usuarioService.ObtenerTodos()
                    .FirstOrDefault(u => u.NombreCompleto
                        .Contains(txtNombreUsuarioFiltro.Text, StringComparison.OrdinalIgnoreCase));
                idUsuario = usuarioEncontrado?.IdUsuario;
            }

            EstadoPeticion? estado = cmbEstadoFiltro.SelectedIndex switch
            {
                1 => EstadoPeticion.Pendiente,
                2 => EstadoPeticion.Aceptada,
                3 => EstadoPeticion.Rechazada,
                _ => null
            };

            var resultados = _peticionService.Buscar(
                idUsuario,
                txtDescripcionFiltro.Text,
                chkUsarDesde.Checked ? dtpDesde.Value : null,
                chkUsarHasta.Checked ? dtpHasta.Value : null,
                estado
            );

            var vista = resultados.Select(p => new
            {
                p.IdPeticion,
                p.NombreUsuario,
                p.Descripcion,
                Fecha  = p.FechaSolicitud.ToString("dd/MM/yyyy HH:mm"),
                Estado = p.EstadoTexto,
                p.Comentario
            }).ToList();

            dgvPeticiones.DataSource = vista;

            if (dgvPeticiones.Columns.Contains("IdPeticion"))
                dgvPeticiones.Columns["IdPeticion"]!.HeaderText = "ID";
            if (dgvPeticiones.Columns.Contains("NombreUsuario"))
                dgvPeticiones.Columns["NombreUsuario"]!.HeaderText = "Usuario";

            ActualizarBotonesAccion();
        }

        private void BtnLimpiar_Click(object? sender, EventArgs e)
        {
            txtDescripcionFiltro.Clear();
            txtNombreUsuarioFiltro.Clear();
            chkUsarDesde.Checked = false;
            chkUsarHasta.Checked = false;
            cmbEstadoFiltro.SelectedIndex = 0;
            EjecutarBusqueda();
        }

        private void DgvPeticiones_SelectionChanged(object? sender, EventArgs e)
            => ActualizarBotonesAccion();

        private void ActualizarBotonesAccion()
        {
            if (!_usuarioActual.EsSupervisor) return;

            bool haySeleccion = dgvPeticiones.SelectedRows.Count > 0;
            if (!haySeleccion)
            {
                btnAceptar.Enabled = btnRechazar.Enabled = false;
                return;
            }

            string estado = dgvPeticiones.SelectedRows[0].Cells["Estado"].Value?.ToString() ?? "";
            bool esPendiente = estado == EstadoPeticion.Pendiente.ToString();
            btnAceptar.Enabled  = esPendiente;
            btnRechazar.Enabled = esPendiente;
        }

        private void BtnCrearPeticion_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDescripcionNueva.Text))
            {
                MessageBox.Show("Introduce una descripción para la petición.",
                                "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var peticion = new PeticionMaterial
            {
                IdUsuario    = _usuarioActual.IdUsuario,
                NombreUsuario = _usuarioActual.NombreCompleto,
                Descripcion  = txtDescripcionNueva.Text.Trim()
            };

            _peticionService.Agregar(peticion);
            txtDescripcionNueva.Clear();
            MessageBox.Show("Petición enviada correctamente.", "OK",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
            EjecutarBusqueda();
        }

        private void BtnAceptar_Click(object? sender, EventArgs e)
            => ResolverPeticion(EstadoPeticion.Aceptada);

        private void BtnRechazar_Click(object? sender, EventArgs e)
            => ResolverPeticion(EstadoPeticion.Rechazada);

        private void ResolverPeticion(EstadoPeticion nuevoEstado)
        {
            int id = (int)dgvPeticiones.SelectedRows[0].Cells["IdPeticion"].Value;
            _peticionService.ActualizarEstado(id, nuevoEstado, txtComentario.Text.Trim());
            txtComentario.Clear();
            EjecutarBusqueda();
        }
    }
}
