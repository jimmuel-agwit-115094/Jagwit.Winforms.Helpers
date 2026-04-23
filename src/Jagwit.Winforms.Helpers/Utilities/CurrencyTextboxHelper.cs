using System.Windows.Forms;

namespace Jagwit.Winforms.Helpers.Utilities
{
    /// <summary>
    /// Provides utilities for configuring <see cref="NumericUpDown"/> controls used as currency inputs.
    /// </summary>
    public static class CurrencyTextboxHelper
    {
        /// <summary>
        /// Iterates all controls inside <paramref name="container"/> and hides the spin buttons on any
        /// <see cref="NumericUpDown"/> whose <c>Tag</c> is set to <c>"IsNumeric"</c>.
        /// </summary>
        /// <param name="container">The parent control whose children are inspected.</param>
        public static void ApplyNumericProperty(Control container)
        {
            foreach (Control control in container.Controls)
            {
                if (control.Tag != null && control.Tag.ToString() == "IsNumeric")
                {
                    if (control is NumericUpDown numericUpDown)
                        HideNumericUpDownButton(numericUpDown);
                }
            }
        }

        private static void HideNumericUpDownButton(NumericUpDown numericUpDown)
        {
            if (numericUpDown.Controls.Count > 0)
                numericUpDown.Controls[0].Visible = false;
        }
    }
}
