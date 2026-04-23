using System.Windows.Forms;

namespace Jagwit.Winforms.Helpers.Utilities
{
    /// <summary>
    /// Provides a one-call setup method that composes multiple <see cref="DgFormatHelper"/> operations
    /// on a <see cref="DataGridView"/>.
    /// </summary>
    public static class DgExtensions
    {
        /// <summary>
        /// Applies basic formatting, optionally styles a link column, filters visible columns,
        /// and toggles a "not found" label based on whether the grid has rows.
        /// </summary>
        /// <param name="dataGrid">The <see cref="DataGridView"/> to configure.</param>
        /// <param name="setUpIdLink">When <see langword="true"/>, calls <see cref="DgFormatHelper.SetupLinkId"/> on <paramref name="columnIndex"/>.</param>
        /// <param name="columnIndex">The column index to style as a link. Ignored when <paramref name="setUpIdLink"/> is <see langword="false"/>.</param>
        /// <param name="notFoundLabel">Label shown when the grid has no rows.</param>
        /// <param name="fieldsToShow">Column names to display; all others are hidden.</param>
        public static void ConfigureDataGrid(
            DataGridView dataGrid,
            bool setUpIdLink,
            int columnIndex,
            Label notFoundLabel,
            params string[] fieldsToShow)
        {
            DgFormatHelper.BasicGridFormat(dataGrid);
            dataGrid.AutoFormat();

            if (setUpIdLink)
                DgFormatHelper.SetupLinkId(dataGrid, columnIndex);

            DgFormatHelper.ShowOnlyField(dataGrid, fieldsToShow);

            notFoundLabel.Visible = dataGrid.Rows.Count == 0;
        }
    }
}
