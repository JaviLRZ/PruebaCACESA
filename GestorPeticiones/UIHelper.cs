using System.Drawing.Drawing2D;

namespace GestorPeticiones
{
    /// <summary>
    /// Utilidades para mejorar la estética de la interfaz WinForms.
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// Aplica un diseño redondeado a un botón.
        /// </summary>
        public static void HacerRedondeado(Button btn, int radio = 15)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            
            // Evento para redibujar el borde redondeado al cambiar el tamaño
            btn.Resize += (s, e) => ActualizarRegionRedondeada(btn, radio);
            ActualizarRegionRedondeada(btn, radio);
        }

        private static void ActualizarRegionRedondeada(Button btn, int radio)
        {
            Rectangle rect = new Rectangle(0, 0, btn.Width, btn.Height);
            GraphicsPath path = new GraphicsPath();
            
            int d = radio * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            
            btn.Region = new Region(path);
        }

        /// <summary>
        /// Aplica un estilo moderno a un DataGridView.
        /// </summary>
        public static void FormatearGrid(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 80, 160);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(30, 80, 160);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 35;
            
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 240, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.RowTemplate.Height = 30;
            dgv.GridColor = Color.FromArgb(240, 240, 240);
        }
    }
}
