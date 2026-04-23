using System;
using System.Globalization;
using System.Windows.Forms;

namespace Jagwit.Winforms.Helpers.Utilities
{
    /// <summary>
    /// Provides static methods for attaching input formatting and validation behaviour to <see cref="TextBox"/> controls.
    /// All methods wire event handlers and clean them up when the control is disposed.
    /// </summary>
    public static class TextBoxHelper
    {
        /// <summary>
        /// Configures <paramref name="textBox"/> for decimal currency input.
        /// On focus, raw digits are shown. On blur, the value is formatted as <c>N2</c> with thousand separators.
        /// </summary>
        public static void FormatDecimalTextbox(TextBox textBox)
        {
            if (textBox == null)
                return;

            textBox.Text = string.Empty;

            EventHandler enterHandler = (sender, e) =>
            {
                if (decimal.TryParse(textBox.Text, out decimal value))
                {
                    textBox.Text = value == 0 ? string.Empty : value.ToString("0.##");
                    if (value != 0) textBox.SelectAll();
                }
                else
                {
                    textBox.Text = string.Empty;
                }
            };

            KeyPressEventHandler keyPressHandler = (sender, e) =>
            {
                char input = e.KeyChar;

                if (char.IsControl(input))
                    return;

                if (!char.IsDigit(input) && input != '.')
                {
                    e.Handled = true;
                    return;
                }

                if (input == '.' && (textBox.Text.Contains(".") || textBox.SelectionStart == 0))
                    e.Handled = true;
            };

            EventHandler leaveHandler = (sender, e) =>
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                    textBox.Text = "0.00";
                else if (decimal.TryParse(textBox.Text, out decimal value))
                    textBox.Text = string.Format("{0:N2}", value);
                else
                    textBox.Text = "0.00";
            };

            textBox.Enter += enterHandler;
            textBox.KeyPress += keyPressHandler;
            textBox.Leave += leaveHandler;

            textBox.Disposed += (sender, e) =>
            {
                textBox.Enter -= enterHandler;
                textBox.KeyPress -= keyPressHandler;
                textBox.Leave -= leaveHandler;
            };
        }

        /// <summary>
        /// Configures <paramref name="textBox"/> for whole-number percentage input (0–100).
        /// Rejects non-digit characters and clamps the value on blur.
        /// </summary>
        public static void FormatPercentageTextbox(TextBox textBox)
        {
            if (textBox == null)
                return;

            textBox.Text = "0";

            KeyPressEventHandler keyPressHandler = (sender, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };

            EventHandler leaveHandler = (sender, e) =>
            {
                if (int.TryParse(textBox.Text, out int value))
                {
                    if (value < 0) textBox.Text = "0";
                    else if (value > 100) textBox.Text = "100";
                }
                else
                {
                    textBox.Text = "0";
                }
            };

            textBox.KeyPress += keyPressHandler;
            textBox.Leave += leaveHandler;

            textBox.Disposed += (sender, e) =>
            {
                textBox.KeyPress -= keyPressHandler;
                textBox.Leave -= leaveHandler;
            };
        }

        /// <summary>
        /// Configures <paramref name="textBox"/> for non-negative integer input.
        /// On focus, zero values are cleared. Rejects non-digit characters.
        /// </summary>
        public static void FormatIntegerTextbox(TextBox textBox)
        {
            if (textBox == null)
                return;

            textBox.Text = "0";

            EventHandler enterHandler = (sender, e) =>
            {
                if (int.TryParse(textBox.Text, out int value))
                {
                    if (value == 0) textBox.Text = string.Empty;
                    else textBox.SelectAll();
                }
                else
                {
                    textBox.Text = string.Empty;
                }
            };

            KeyPressEventHandler keyPressHandler = (sender, e) =>
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };

            EventHandler leaveHandler = (sender, e) =>
            {
                textBox.Text = int.TryParse(textBox.Text, out int value) ? value.ToString() : "0";
            };

            textBox.Enter += enterHandler;
            textBox.KeyPress += keyPressHandler;
            textBox.Leave += leaveHandler;

            textBox.Disposed += (sender, e) =>
            {
                textBox.Enter -= enterHandler;
                textBox.KeyPress -= keyPressHandler;
                textBox.Leave -= leaveHandler;
            };
        }

        /// <summary>
        /// Configures <paramref name="textBox"/> for barcode input (digits and <c>/</c> only).
        /// Clears the value on blur if it contains invalid characters.
        /// </summary>
        public static void FormatBarcode(TextBox textBox)
        {
            if (textBox == null)
                return;

            textBox.Text = string.Empty;

            KeyPressEventHandler keyPressHandler = (sender, e) =>
            {
                char input = e.KeyChar;
                if (char.IsControl(input))
                    return;

                if (!char.IsDigit(input) && input != '/')
                    e.Handled = true;
            };

            EventHandler leaveHandler = (sender, e) =>
            {
                if (!string.IsNullOrEmpty(textBox.Text) &&
                    !System.Text.RegularExpressions.Regex.IsMatch(textBox.Text, @"^[0-9/]+$"))
                {
                    textBox.Text = string.Empty;
                }
            };

            textBox.KeyPress += keyPressHandler;
            textBox.Leave += leaveHandler;

            textBox.Disposed += (sender, e) =>
            {
                textBox.KeyPress -= keyPressHandler;
                textBox.Leave -= leaveHandler;
            };
        }

        /// <summary>
        /// Configures <paramref name="textBox"/> to accept letters only.
        /// </summary>
        public static void FormatStringTextbox(TextBox textBox)
        {
            if (textBox == null)
                return;

            KeyPressEventHandler keyPressHandler = (sender, e) =>
            {
                if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar))
                    e.Handled = true;
            };

            textBox.KeyPress += keyPressHandler;

            textBox.Disposed += (sender, e) =>
            {
                textBox.KeyPress -= keyPressHandler;
            };
        }

        /// <summary>
        /// If <paramref name="textBox"/> is empty, sets it to <c>"0"</c> and selects all text.
        /// Call this in a <c>Leave</c> handler after <see cref="FormatDecimalTextbox"/> to guard against blank submission.
        /// </summary>
        public static void HandleEmptyDecimalTextbox(TextBox textBox)
        {
            if (textBox.Text == string.Empty)
            {
                textBox.Text = "0";
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Parses <paramref name="text"/> as a decimal, respecting thousand separators and the current culture's decimal point.
        /// Returns <c>0</c> when the input is blank or unparseable.
        /// </summary>
        public static decimal ParseDecimalTextbox(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0m;

            return decimal.TryParse(text, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out decimal result)
                ? result
                : 0m;
        }
    }
}
