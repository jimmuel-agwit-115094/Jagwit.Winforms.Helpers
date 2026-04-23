using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Jagwit.Winforms.Helpers.Utilities
{
    /// <summary>
    /// Provides extension and static methods for formatting, configuring, and querying <see cref="DataGridView"/> controls.
    /// </summary>
    public static class DgFormatHelper
    {
        // Cached fonts to prevent memory leaks from repeated Font allocations
        private static readonly Font HeaderFont = new Font("Segoe UI", 9.75f, FontStyle.Bold);
        private static readonly Font CellFont = new Font("Consolas", 9.75f);
        private static readonly Font ZeroCellFont = new Font("Consolas", 9.75f, FontStyle.Bold);
        private static readonly Regex CamelCaseRegex = new Regex("(?<!^)(?=[A-Z])", RegexOptions.Compiled);

        /// <summary>
        /// Automatically aligns and formats each column based on its value type
        /// (DateTime, decimal/double, int/long, string, enum).
        /// </summary>
        public static void AutoFormat(this DataGridView dataGridView)
        {
            if (dataGridView == null || dataGridView.Rows.Count == 0)
                return;

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (column.ValueType == typeof(DateTime))
                {
                    column.DefaultCellStyle.Format = "MMM. dd, yyyy h:mm tt";
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
                else if (column.ValueType == typeof(decimal) || column.ValueType == typeof(double))
                {
                    column.DefaultCellStyle.Format = "N2";
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                else if (column.ValueType == typeof(int) || column.ValueType == typeof(long))
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                else if (column.ValueType == typeof(string))
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
                else if (column.ValueType != null && column.ValueType.IsEnum)
                {
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                }
            }
        }

        /// <summary>
        /// Styles the column at <paramref name="columnIndex"/> as a blue underlined link with a zero-padded
        /// numeric format, and wires hover cursor behaviour. Moves the column to position 0.
        /// </summary>
        public static void SetupLinkId(DataGridView dataGridView, int columnIndex)
        {
            if (dataGridView == null || dataGridView.Rows.Count == 0)
                return;

            Font underlinedFont = new Font(dataGridView.DefaultCellStyle.Font, FontStyle.Underline);

            dataGridView.Columns[columnIndex].DefaultCellStyle = new DataGridViewCellStyle
            {
                ForeColor = Color.Blue,
                Font = underlinedFont,
                SelectionForeColor = Color.Blue,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Format = "000000000"
            };

            dataGridView.Columns[columnIndex].DisplayIndex = 0;

            SetupLinkColumnHandlers(dataGridView, columnIndex, underlinedFont);
        }

        /// <summary>
        /// Styles the column at <paramref name="columnIndex"/> as a blue underlined link for string values.
        /// </summary>
        public static void SetupLinkColumnsForString(DataGridView dataGridView, int columnIndex)
        {
            if (dataGridView == null)
                return;

            Font underlinedFont = new Font(dataGridView.DefaultCellStyle.Font, FontStyle.Underline);

            dataGridView.Columns[columnIndex].DefaultCellStyle = new DataGridViewCellStyle
            {
                ForeColor = Color.Blue,
                Font = underlinedFont,
                SelectionForeColor = Color.Blue,
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            SetupLinkColumnHandlers(dataGridView, columnIndex, underlinedFont);
        }

        private static void SetupLinkColumnHandlers(DataGridView dataGridView, int columnIndex, Font underlinedFont)
        {
            DataGridViewCellEventHandler cellMouseEnterHandler = (sender, e) =>
            {
                if (e.ColumnIndex == columnIndex && e.RowIndex >= 0)
                {
                    dataGridView.Cursor = Cursors.Hand;
                    dataGridView.Rows[e.RowIndex].Cells[columnIndex].Style.ForeColor = Color.FromArgb(0, 0, 192);
                }
            };

            DataGridViewCellEventHandler cellMouseLeaveHandler = (sender, e) =>
            {
                if (e.ColumnIndex == columnIndex && e.RowIndex >= 0)
                {
                    dataGridView.Cursor = Cursors.Default;
                    dataGridView.Rows[e.RowIndex].Cells[columnIndex].Style.ForeColor = Color.Blue;
                }
            };

            dataGridView.CellMouseEnter += cellMouseEnterHandler;
            dataGridView.CellMouseLeave += cellMouseLeaveHandler;

            dataGridView.Disposed += (sender, e) =>
            {
                dataGridView.CellMouseEnter -= cellMouseEnterHandler;
                dataGridView.CellMouseLeave -= cellMouseLeaveHandler;
                underlinedFont?.Dispose();
            };
        }

        /// <summary>
        /// Applies standard grid styling: locks header selection colour, applies fonts, disables column
        /// sorting, sets row selection highlight, and suppresses F3 and header double-click.
        /// </summary>
        public static void BasicGridFormat(DataGridView dataGridView)
        {
            if (dataGridView == null)
                return;

            dataGridView.ColumnHeadersDefaultCellStyle.SelectionBackColor =
                dataGridView.ColumnHeadersDefaultCellStyle.BackColor;

            SetDataGridStyles(dataGridView);

            foreach (DataGridViewColumn column in dataGridView.Columns)
                column.SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 224, 192);

            KeyEventHandler keyDownHandler = (s, ev) =>
            {
                if (ev.KeyCode == Keys.F3)
                    ev.Handled = true;
            };

            MouseEventHandler mouseDoubleClickHandler = (s, ev) =>
            {
                // Suppress column header double-click
            };

            dataGridView.KeyDown += keyDownHandler;
            dataGridView.MouseDoubleClick += mouseDoubleClickHandler;

            dataGridView.Disposed += (sender, e) =>
            {
                dataGridView.KeyDown -= keyDownHandler;
                dataGridView.MouseDoubleClick -= mouseDoubleClickHandler;
            };
        }

        /// <summary>
        /// Hides all columns except those listed in <paramref name="fieldsToShow"/>, and converts their
        /// header text from camelCase to spaced words.
        /// </summary>
        public static void ShowOnlyField(DataGridView dataGridView, params string[] fieldsToShow)
        {
            if (dataGridView == null)
                throw new ArgumentNullException(nameof(dataGridView));

            if (fieldsToShow == null || fieldsToShow.Length == 0)
                throw new ArgumentException("At least one field must be specified.", nameof(fieldsToShow));

            var fieldsToShowSet = new HashSet<string>(fieldsToShow, StringComparer.OrdinalIgnoreCase);

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                if (fieldsToShowSet.Contains(column.DataPropertyName ?? string.Empty) || fieldsToShowSet.Contains(column.Name ?? string.Empty))
                {
                    column.Visible = true;
                    column.HeaderText = AddSpacesToCamelCase(column.DataPropertyName ?? column.Name ?? string.Empty);
                }
                else
                {
                    column.Visible = false;
                }
            }
        }

        /// <summary>
        /// Filters rows so that only those containing <paramref name="searchTextBox"/>'s text
        /// (case-insensitive, any cell) remain visible.
        /// </summary>
        public static void SearchOnGrid(DataGridView dataGridView, TextBox searchTextBox)
        {
            if (dataGridView == null || searchTextBox == null)
                return;

            try
            {
                string searchText = searchTextBox.Text;
                dataGridView.CurrentCell = null;

                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    bool visible = false;
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.Value != null &&
                            (cell.Value.ToString() ?? string.Empty).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            visible = true;
                            break;
                        }
                    }
                    row.Visible = visible;
                }
            }
            catch (Exception ex)
            {
                MessageHandler.ShowError("Error on search on datagrid: " + ex.Message);
            }
        }

        /// <summary>
        /// Returns the integer cell value from the clicked row/column, or 0 if the click was
        /// on a different column or the value is invalid.
        /// </summary>
        public static int GetSelectedId(DataGridView dgv, DataGridViewCellEventArgs e, string columnName)
        {
            try
            {
                if (e.ColumnIndex == dgv.Columns[columnName].Index && e.RowIndex >= 0)
                {
                    object cellValue = dgv.Rows[e.RowIndex].Cells[columnName].Value;
                    if (cellValue != null && int.TryParse(cellValue.ToString(), out int selectedId) && selectedId != 0)
                        return selectedId;
                }
                return 0;
            }
            catch (Exception ex)
            {
                MessageHandler.ShowError($"Error getting selected id: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Returns the integer value of <paramref name="columnName"/> from the currently selected row.
        /// Throws if no row is selected, the column is missing, or the value is not a positive integer.
        /// </summary>
        public static int GetSelectedIdOnSelectionChange(DataGridView dgv, string columnName)
        {
            if (dgv == null)
                throw new ArgumentNullException(nameof(dgv));

            if (string.IsNullOrEmpty(columnName))
                throw new ArgumentException("Column name cannot be null or empty.", nameof(columnName));

            if (!dgv.Columns.Contains(columnName))
                throw new ArgumentException($"Column '{columnName}' does not exist in the DataGridView.", nameof(columnName));

            try
            {
                if (dgv.CurrentRow == null)
                    throw new InvalidOperationException("No row is currently selected.");

                var cellValue = dgv.CurrentRow.Cells[columnName].Value;

                if (cellValue != null && int.TryParse(cellValue.ToString(), out int selectedId))
                {
                    if (selectedId > 0)
                        return selectedId;

                    throw new InvalidOperationException($"Invalid ID value: {selectedId}. ID must be greater than 0.");
                }

                throw new InvalidOperationException("Cell value is null or not a valid integer.");
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error fetching selected ID: {ex.Message}.";
                MessageHandler.ShowError(errorMessage);
                throw new ApplicationException(errorMessage, ex);
            }
        }

        /// <summary>Applies the cached header and cell fonts to all columns.</summary>
        public static void SetDataGridStyles(DataGridView dataGrid)
        {
            if (dataGrid == null)
                return;

            foreach (DataGridViewColumn column in dataGrid.Columns)
                column.HeaderCell.Style.Font = HeaderFont;

            dataGrid.DefaultCellStyle.Font = CellFont;
        }

        /// <summary>Returns the integer value of <paramref name="columnToConvertAsId"/> from the first selected row, or 0.</summary>
        public static int GetSelectedRowId(DataGridView dataGridView, string columnToConvertAsId)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var cellValue = dataGridView.SelectedRows[0].Cells[columnToConvertAsId].Value;
                if (cellValue != null)
                    return Convert.ToInt32(cellValue);
            }
            return 0;
        }

        /// <summary>Returns the string value of <paramref name="columnToGet"/> from the first selected row, or <see cref="string.Empty"/>.</summary>
        public static string GetSelectedRowString(DataGridView dataGridView, string columnToGet)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var cellValue = dataGridView.SelectedRows[0].Cells[columnToGet].Value;
                if (cellValue != null)
                    return cellValue.ToString() ?? string.Empty;
            }
            return string.Empty;
        }

        /// <summary>Suppresses the Enter key when the grid has rows, preventing the default beep.</summary>
        public static void HandleEnterKey(KeyEventArgs e, DataGridView dataGridView)
        {
            if (e.KeyCode == Keys.Enter && dataGridView.Rows.Count > 0)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// Makes <paramref name="columnName"/> read-only, non-sortable, and shows a <see cref="Cursors.No"/>
        /// cursor on hover. Cleans up event handlers when the grid is disposed.
        /// </summary>
        public static void DisableColumnClick(DataGridView dataGridView, string columnName)
        {
            if (dataGridView == null || string.IsNullOrWhiteSpace(columnName))
                throw new ArgumentException("Invalid column name or DataGridView.");

            DataGridViewColumn column = dataGridView.Columns[columnName]
                ?? throw new ArgumentException($"Column '{columnName}' not found in the DataGridView.");

            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            column.ReadOnly = true;

            int columnIndex = column.Index;

            DataGridViewCellEventHandler cellMouseEnterHandler = (sender, e) =>
            {
                if (e.ColumnIndex == columnIndex)
                    dataGridView.Cursor = Cursors.No;
            };

            DataGridViewCellEventHandler cellMouseLeaveHandler = (sender, e) =>
                dataGridView.Cursor = Cursors.Default;

            DataGridViewCellEventHandler cellClickHandler = (sender, e) =>
            {
                if (e.ColumnIndex == columnIndex && sender is DataGridView dgv)
                    dgv.CurrentCell = null;
            };

            dataGridView.CellMouseEnter += cellMouseEnterHandler;
            dataGridView.CellMouseLeave += cellMouseLeaveHandler;
            dataGridView.CellClick += cellClickHandler;

            dataGridView.Disposed += (sender, e) =>
            {
                dataGridView.CellMouseEnter -= cellMouseEnterHandler;
                dataGridView.CellMouseLeave -= cellMouseLeaveHandler;
                dataGridView.CellClick -= cellClickHandler;
            };
        }

        /// <summary>Selects the row that owns the current cell.</summary>
        public static void SelectCurrentRow(DataGridView dataGridView)
        {
            if (dataGridView.CurrentCell != null)
            {
                int rowIndex = dataGridView.CurrentCell.RowIndex;
                dataGridView.Rows[rowIndex].Selected = true;
            }
        }

        /// <summary>Disables the grid and applies a greyed-out visual style to all rows.</summary>
        public static void DisableDatagrid(DataGridView dataGridView)
        {
            dataGridView.Enabled = false;
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                row.DefaultCellStyle.BackColor = SystemColors.Control;
                row.DefaultCellStyle.ForeColor = Color.DarkGray;
            }
        }

        /// <summary>
        /// Highlights cells in <paramref name="columnToFormat"/> that have a value of zero
        /// by colouring them red and applying bold font.
        /// </summary>
        public static void ZeroCellValuesFormat(DataGridView dataGridView, string columnToFormat)
        {
            if (dataGridView == null || string.IsNullOrWhiteSpace(columnToFormat))
                return;

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.Cells[columnToFormat] is DataGridViewCell cell &&
                    decimal.TryParse(cell.Value?.ToString(), out decimal cellValue) && cellValue == 0)
                {
                    cell.Style.ForeColor = Color.Red;
                    cell.Style.Font = ZeroCellFont;
                    cell.Style.SelectionForeColor = Color.Red;
                    cell.Style.SelectionBackColor = cell.Style.BackColor;
                }
            }
        }

        private static string AddSpacesToCamelCase(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return CamelCaseRegex.Replace(input, " ");
        }
    }
}
