using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Jagwit.Winforms.Helpers.Utilities
{
    /// <summary>
    /// Provides a utility for displaying a status icon and message inside a <see cref="Panel"/>
    /// that contains a <see cref="PictureBox"/> and a <see cref="Label"/>.
    /// </summary>
    public static class StatusIconHelper
    {
        /// <summary>
        /// Sets the label text and picture box image inside <paramref name="statusPanel"/>,
        /// then makes both controls visible.
        /// </summary>
        /// <param name="statusPanel">
        /// A <see cref="Panel"/> that must contain exactly one <see cref="PictureBox"/> and one <see cref="Label"/>.
        /// </param>
        /// <param name="statusMessage">The text to display in the label.</param>
        /// <param name="statusImage">
        /// The icon to display in the picture box. Pass <see langword="null"/> to hide the picture box.
        /// </param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the panel does not contain both a <see cref="PictureBox"/> and a <see cref="Label"/>.
        /// </exception>
        public static void ShowStatus(Panel statusPanel, string statusMessage, Image? statusImage = null)
        {
            PictureBox? pictureBox = statusPanel.Controls.OfType<PictureBox>().FirstOrDefault();
            Label? label = statusPanel.Controls.OfType<Label>().FirstOrDefault();

            if (pictureBox == null || label == null)
                throw new InvalidOperationException("The panel must contain a PictureBox and a Label.");

            label.Text = statusMessage;
            label.Visible = true;

            if (statusImage != null)
            {
                pictureBox.Image = statusImage;
                pictureBox.Visible = true;
            }
            else
            {
                pictureBox.Visible = false;
            }
        }
    }
}
