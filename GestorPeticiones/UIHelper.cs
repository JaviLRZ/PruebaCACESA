using System.Drawing.Drawing2D;

namespace GestorPeticiones
{
    /// <summary>
    /// Utilidades para mejorar la estética de la interfaz WinForms.
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// Aplica un diseño plano y cuadrado a un botón.
        /// </summary>
        public static void FormatearBoton(Button btn, int radioUnused = 0)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            // Se elimina el código de redondeo por diseño más limpio y nativo
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
