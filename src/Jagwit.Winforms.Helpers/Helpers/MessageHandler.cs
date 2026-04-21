using System.Windows.Forms;

namespace Jagwit.Winforms.Helpers.Helpers
{
    /// <summary>
    /// Provides static methods for displaying WinForms message dialogs.
    /// </summary>
    public static class MessageHandler
    {
        /// <summary>Displays an error dialog with the given message.</summary>
        /// <param name="message">The error message to display.</param>
        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>Displays a warning dialog with the given message.</summary>
        /// <param name="message">The warning message to display.</param>
        public static void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>Displays an informational dialog with the given message.</summary>
        /// <param name="message">The information message to display.</param>
        public static void ShowInfo(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>Displays a Yes/No confirmation dialog.</summary>
        /// <param name="message">The question to ask the user.</param>
        /// <param name="title">The dialog title. Defaults to "Confirm".</param>
        /// <returns><see cref="DialogResult.Yes"/> or <see cref="DialogResult.No"/>.</returns>
        public static DialogResult ShowConfirmation(string message, string title = "Confirm")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }
    }
}
