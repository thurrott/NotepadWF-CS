using System.Windows.Forms;

namespace NotePadWF_CS
{
    internal static class RichTextBoxExtensions
    {
        /// <summary>
        /// Set's the beeping on a RichTextBox.
        /// </summary>
        /// <remarks>
        /// https://stackoverflow.com/questions/4683663/how-to-remove-annoying-beep-with-richtextbox
        /// </remarks>
        public static void SetBeeping(this RichTextBox richTextBox, bool value)
        {
            if (value)
            {
                richTextBox.KeyDown -= RichTextBox_KeyDown;
            }
            else
            {
                richTextBox.KeyDown += RichTextBox_KeyDown;
            }
        }

        public static bool IsSelectionAtStart(this RichTextBox richTextBox)
        {
            return richTextBox.SelectionStart == 0;
        }

        public static bool IsSelectionAtEnd(this RichTextBox richTextBox)
        {
            return richTextBox.SelectionStart == richTextBox.TextLength;
        }

        public static bool IsSelectionOnFirstLine(this RichTextBox richTextBox)
        {
            return richTextBox.GetLineFromCharIndex(richTextBox.SelectionStart) == 0;
        }

        public static bool IsSelectionOnLastLine(this RichTextBox richTextBox)
        {
            return richTextBox.GetLineFromCharIndex(richTextBox.SelectionStart) == richTextBox.GetLineFromCharIndex(richTextBox.TextLength);
        }

        private static void RichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var richTextBox = (RichTextBox)sender;
            if (richTextBox.IsSelectionOnFirstLine() && e.KeyCode == Keys.Up ||
               richTextBox.IsSelectionOnLastLine() && e.KeyCode == Keys.Down ||
               richTextBox.IsSelectionAtStart() && (e.KeyCode == Keys.Left || e.KeyCode == Keys.Back) ||
               richTextBox.IsSelectionAtEnd() && (e.KeyCode == Keys.Right || e.KeyCode == Keys.Delete))
            {
                e.Handled = true;
            }
        }

    }
}
