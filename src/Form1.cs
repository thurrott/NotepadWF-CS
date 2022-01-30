using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace NotePadWF_CS
{
    public partial class Form1 : Form
    {
        private Boolean TextHasChanged = false;
        private String DocumentName = "";
        private Font MasterFont;
        private String FindTextString = "";
        private int FindLastIndexFound = 0;
        private string PrintText;
        private int StartPage;
        private int NumPages;
        private int PageNumber;

        public RichTextBoxScrollBars Both { get; private set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {        
            Text = Application.ProductName;
            aboutToolStripMenuItem.Text = "About " + Application.ProductName;

            Location = Properties.Settings.Default.MyLocation;
            if (!Properties.Settings.Default.MySize.IsEmpty)
            {
                Size = Properties.Settings.Default.MySize;
            }

            richTextBox1.Font = Properties.Settings.Default.MyFont;
            richTextBox1.ForeColor = Properties.Settings.Default.MyTextColor;
            richTextBox1.BackColor = Properties.Settings.Default.MyBackgroundColor;
            richTextBox1.WordWrap = Properties.Settings.Default.MyWordWrap;
            wordWrapToolStripMenuItem.Checked = Properties.Settings.Default.MyWordWrap;
            if (richTextBox1.WordWrap == true)
            {
                richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
            }
            else
            {
                richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
            }
            statusStrip1.Visible = Properties.Settings.Default.MyStatusBar;
            statusBarToolStripMenuItem.Checked = Properties.Settings.Default.MyStatusBar;

            autoSaveToolStripMenuItem.Checked = Properties.Settings.Default.MyAutoSave;
            if (autoSaveToolStripMenuItem.Checked)
            {
                timer1.Enabled = true;
                autoSaveToolStripStatusLabel.Text = "Auto Save: On";
            }
            else
            {
                timer1.Enabled = false;
                autoSaveToolStripStatusLabel.Text = "Auto Save: Off";
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;

            Properties.Settings.Default.MyLocation = Location;
            Properties.Settings.Default.MySize = Size;
            Properties.Settings.Default.MyFont = richTextBox1.Font;
            Properties.Settings.Default.MyTextColor = richTextBox1.ForeColor;
            Properties.Settings.Default.MyBackgroundColor = richTextBox1.BackColor;
            Properties.Settings.Default.MyStatusBar = statusStrip1.Visible;
            Properties.Settings.Default.MyWordWrap = wordWrapToolStripMenuItem.Checked;
            Properties.Settings.Default.MyAutoSave = autoSaveToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();

            if (TextHasChanged)
            {
                DialogResult SavePrompt = MessageBox.Show("Do you want to save changes?", Application.ProductName, MessageBoxButtons.YesNoCancel);
                switch(SavePrompt)
                {
                    case DialogResult.Yes:
                        saveFileDialog1.ShowDialog();
                        break;
                    case DialogResult.No:
                        break;
                    default:
                        e.Cancel = true;
                        break;
                }
            }
            else
            { 
                Application.Exit();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (TextHasChanged == false)
            {
                Text = "*" + Text;
                TextHasChanged = true;
            }

            int Count = System.Text.RegularExpressions.Regex.Matches(richTextBox1.Text, @"[\S]+").Count;
            wordCountToolStripStatusLabel.Text = Count.ToString() + " word";
            if (Count > 1)
            {
                wordCountToolStripStatusLabel.Text += "s";
            }
 
            ChangePositionToolStripStatusLabel();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ChangePositionToolStripStatusLabel()
        {
            positionToolStripStatusLabel.Text = " Ln " +
                (richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart) + 1).ToString()
                + ", Col " +
                (richTextBox1.SelectionStart - richTextBox1.GetFirstCharIndexFromLine(richTextBox1.GetLineFromCharIndex(richTextBox1.SelectionStart)) + 1).ToString();
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.ZoomFactor < 64.5)
            {
                richTextBox1.ZoomFactor += .1f;
            }
            zoomToolStripStatusLabel.Text = Math.Round(richTextBox1.ZoomFactor * 100).ToString() + "%";
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.ZoomFactor > 0.515625)
            {
                richTextBox1.ZoomFactor -= .1f;
            }
            zoomToolStripStatusLabel.Text = Math.Round(richTextBox1.ZoomFactor * 100).ToString() + "%";
        }

        private void restoreDefaultZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ZoomFactor = 1;
            zoomToolStripStatusLabel.Text = "100%";
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TextHasChanged)
            {
                DialogResult SavePrompt = MessageBox.Show("Do you want to save changes?", Application.ProductName, MessageBoxButtons.YesNoCancel);
                if (SavePrompt == DialogResult.Yes)
                    saveFileDialog1.ShowDialog();
                else if (SavePrompt == DialogResult.No)
                    richTextBox1.Text = "";
            }
            else
            {
                richTextBox1.Text = "";
            }
            TextHasChanged = false;
            DocumentName = "";
            Text = "Untitled - " + Application.ProductName;
        }

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult OpenFile = openFileDialog1.ShowDialog();
            if (OpenFile == DialogResult.OK)
            {
                try
                {
                    richTextBox1.Text = File.ReadAllText(openFileDialog1.FileName);
                    Text = openFileDialog1.SafeFileName + " - " + Application.ProductName;
                    TextHasChanged = false;
                    DocumentName = openFileDialog1.FileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void wordWrapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.WordWrap == true)
            {
                richTextBox1.WordWrap = false;
                richTextBox1.ScrollBars = RichTextBoxScrollBars.Both;
                wordWrapToolStripMenuItem.Checked = false;
            }
            else
            {
                richTextBox1.WordWrap = true;
                richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
                wordWrapToolStripMenuItem.Checked = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DocumentName != "")
            {
                try
                {
                    File.WriteAllText(DocumentName, richTextBox1.Text);
                    TextHasChanged = false;
                    Text = Text.Replace("*", "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            { 
                saveAsToolStripMenuItem_Click(sender, e);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, EventArgs e)
        {
            try
            {
                DocumentName = saveFileDialog1.FileName;
                Text = Path.GetFileNameWithoutExtension(DocumentName) + "- " + Application.ProductName;
                TextHasChanged = false;
                File.WriteAllText(DocumentName, richTextBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = richTextBox1.Font;
            // DialogResult Result = fontDialog1.ShowDialog();

            // if (Result == DialogResult.OK)
            if (fontDialog1.ShowDialog() == DialogResult.OK)
                {
                richTextBox1.Font = fontDialog1.Font;
                // restoreDefaultZoomToolStripMenuItem_Click(sender, e);
            }
        }

        private void statusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (statusBarToolStripMenuItem.Checked)
            {
                statusStrip1.Hide();
                statusBarToolStripMenuItem.Checked = false;
            }
            else
            {
                statusStrip1.Show();
                statusBarToolStripMenuItem.Checked = true;
            }
        }

        private void blackOnWhiteDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ForeColor = Color.Black;
            richTextBox1.BackColor = Color.White;
        }

        private void blackOnLightGrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ForeColor = Color.Black;
            richTextBox1.BackColor = Color.LightGray;
        }

        private void amberOnBlackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ForeColor = Color.Orange;
            richTextBox1.BackColor = Color.Black;
        }

        private void greenOnBlackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.ForeColor = Color.LightGreen;
            richTextBox1.BackColor = Color.Black;
        }

        private void selectTextColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // DialogResult Result = colorDialog1.ShowDialog();
            // if (Result == DialogResult.OK)
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.ForeColor = colorDialog1.Color;
            }
        }

        private void selectBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // DialogResult Result = colorDialog1.ShowDialog();
            // if (Result == DialogResult.OK)
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.BackColor = colorDialog1.Color;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
            about.Dispose();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                richTextBox1.SelectedText = Clipboard.GetText(TextDataFormat.Text);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("{DEL}");
        }

        private void searchWithBingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.SelectedText.Length != 0)
                {
                    Process.Start("https://www.bing.com/search?q=" + richTextBox1.SelectedText);
                }
                else
                {
                    Process.Start("https://www.bing.com");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int LineNum = Convert.ToInt32(Interaction.InputBox("Line number:", "Go to line", "", Location.X + 200, Location.Y + 300));
                if (LineNum <= richTextBox1.Lines.Length)
                {
                    richTextBox1.SelectionStart = richTextBox1.GetFirstCharIndexFromLine(LineNum - 1);
                    richTextBox1.SelectionLength = 0;
                    richTextBox1.ScrollToCaret();
                }
                else
                {
                    MessageBox.Show("The line number is beyond the total number of lines", "Go to line");
                    goToToolStripMenuItem_Click(this, e);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void insertTimeDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectedText = DateTime.Now.ToShortTimeString() + " " + DateTime.Now.ToShortDateString();
        }

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pageSetupDialog1.ShowDialog();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            printDialog1.AllowSelection = richTextBox1.SelectionLength > 0;

            // DialogResult Result = printDialog1.ShowDialog();
            // if (Result == DialogResult.OK)
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.DocumentName = richTextBox1.Text;
                switch (printDialog1.PrinterSettings.PrintRange)
                {
                    case System.Drawing.Printing.PrintRange.AllPages:
                        PrintText = richTextBox1.Text;
                        StartPage = 1;
                        NumPages = printDialog1.PrinterSettings.MaximumPage;
                        break;
                    case System.Drawing.Printing.PrintRange.Selection:
                        PrintText = richTextBox1.SelectedText;
                        StartPage = 1;
                        NumPages = printDialog1.PrinterSettings.MaximumPage;
                        break;
                    case System.Drawing.Printing.PrintRange.SomePages:
                        PrintText = richTextBox1.Text;
                        StartPage = printDialog1.PrinterSettings.FromPage;
                        NumPages = printDialog1.PrinterSettings.ToPage - StartPage + 1;
                        break;
                    default:
                        break;
                }
                PageNumber = 1;
                printDocument1.Print();
            }
        }

        private int charsInLines(string PrintText, int NumLines)
        {
            int Index = 0;
            int X = 0;

            while (X < NumLines)
            {
                Index = 1 + PrintText.IndexOf(Environment.NewLine, Index);
                if (Index == 0)
                    return PrintText.Length;
            }

        return Index;
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics Grfx = e.Graphics;
            Font Font = richTextBox1.Font;
            float CyFont = Font.GetHeight(Grfx);
            StringFormat StrFmt = new StringFormat();
            RectangleF RectfFull, RectfText;
            int Chars, Lines;

            // Calculate RectangleF for header and footer
            if (Grfx.VisibleClipBounds.X < 0)
            {
                RectfFull = e.MarginBounds;
            }
            else 
            {
                RectfFull = new RectangleF(
                    e.MarginBounds.Left - (e.PageBounds.Width -
                              Grfx.VisibleClipBounds.Width) / 2,
                    e.MarginBounds.Top - (e.PageBounds.Height -
                              Grfx.VisibleClipBounds.Height) / 2,
                    e.MarginBounds.Width, e.MarginBounds.Height);
            }

            // Calculate RectangleF for text
            RectfText = RectangleF.Inflate(RectfFull, 0, -2 * CyFont);

            int DisplayLines = (int)Math.Floor(RectfText.Height / CyFont);
            RectfText.Height = DisplayLines * CyFont;

            // Set up StringFormat object for rectanglar display of text
            if (richTextBox1.WordWrap)
            { 
                StrFmt.Trimming = StringTrimming.Word;
            }
            else
            {
                StrFmt.Trimming = StringTrimming.EllipsisCharacter;
                StrFmt.FormatFlags |= StringFormatFlags.NoWrap;
            }

            // For "some pages" get to the first page
            while ((PageNumber < StartPage) && (PrintText.Length > 0))
            {
                if (richTextBox1.WordWrap)
                {
                    Grfx.MeasureString(PrintText, Font, RectfText.Size,
                                           StrFmt, out Chars, out Lines);
                }
                else
                {
                    Chars = charsInLines(PrintText, DisplayLines);
                }

            PrintText = PrintText.Substring(Chars);
            PageNumber += 1;
            }

            // If we've prematurely run out of text, cancel the print job
            if (PrintText.Length == 0)
            {
                e.Cancel = true;
                return;
            }

            // Display text for this page
            Grfx.DrawString(PrintText, Font, Brushes.Black, RectfText, StrFmt);

            // Get text for next page
            if (richTextBox1.WordWrap)
            {
                Grfx.MeasureString(PrintText, Font, RectfText.Size, StrFmt, out Chars, out Lines);
            }
            else
            {
                Chars = charsInLines(PrintText, DisplayLines);
            }

            PrintText = PrintText.Substring(Chars);

            // Reset StringFormat display header and footer
            StrFmt = new StringFormat();

            // Decide whether to print another page
            PageNumber++;
            e.HasMorePages = (PrintText.Length > 0) && (PageNumber < StartPage + NumPages);

            // Reinitialize variables for printing from preview form
            if (!e.HasMorePages)
            {
                PrintText = richTextBox1.Text;
                StartPage = 1;
                NumPages = printDialog1.PrinterSettings.MaximumPage;
                PageNumber = 1;
            }

            StrFmt.Dispose();
        }

        private void autoSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (autoSaveToolStripMenuItem.Checked)
            {
                timer1.Enabled = false;
                // Uncheck it, disable auto save, disable timer
                // DialogResult Result = MessageBox.Show("Click OK to disable Auto Save.", "Disable Auto Save", MessageBoxButtons.OKCancel);
                // if (Result == DialogResult.OK)
                if (MessageBox.Show("Click OK to disable Auto Save.", "Disable Auto Save", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    autoSaveToolStripMenuItem.Checked = false;
                    autoSaveToolStripStatusLabel.Text = "Auto Save: Off";
                }
                else
                {
                    timer1.Enabled = true;
                }
            }
            else
            {
                // Check it, enable auto save, reset the timer
                // DialogResult Result = MessageBox.Show("Click OK to automatically save your document every 30 seconds.", "Enable Auto Save", MessageBoxButtons.OKCancel);
                // if (Result == DialogResult.OK)
                if (MessageBox.Show("Click OK to automatically save your document every 30 seconds.", "Enable Auto Save", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                    autoSaveToolStripMenuItem.Checked = true;
                    autoSaveToolStripStatusLabel.Text = "Auto Save: On";
                    timer1.Enabled = true;
                }
            }
        }

        private void checkTimerInterval()
        {
            if ((TextHasChanged = true) && (DocumentName == "") && (autoSaveToolStripMenuItem.Checked))
            {
                timer1.Enabled = false;
            }
        }

        private void resetTimerInterval()
        {
            if ((TextHasChanged = true) && (DocumentName == "") && (autoSaveToolStripMenuItem.Checked))
            {
                timer1.Enabled = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (TextHasChanged)
            {
                saveToolStripMenuItem_Click(sender, e);
            }
        }

        private void autoSaveToolStripStatusLabel_Click(object sender, EventArgs e)
        {
            autoSaveToolStripMenuItem_Click(sender, e);
        }

        private void FindTextIndex(int FindFromIndex, Boolean FindPreviousIndex)
        {
            string Text = richTextBox1.Text;

            if (FindPreviousIndex == false)
            {
                FindLastIndexFound = Text.IndexOf(FindTextString, FindFromIndex);
                if (FindLastIndexFound == -1)
                {
                    // If text is not found, try searching from the beginning
                    FindLastIndexFound = Text.IndexOf(FindTextString, 0);
                }
            }
            else
            {
                FindLastIndexFound = Text.LastIndexOf(FindTextString, FindFromIndex);
                if (FindLastIndexFound == -1)
                {
                    // If text is not found, try searching from the end
                    FindLastIndexFound = Text.LastIndexOf(FindTextString, Text.Length - 1);
                }
            }
        }

        private void FindTheText()
        {
            if (FindLastIndexFound > -1)
                richTextBox1.Select(FindLastIndexFound, FindTextString.Length);
            else
                MessageBox.Show("Cannot find '" + FindTextString + "'", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindTextString = Interaction.InputBox("Find what:", "Find", richTextBox1.SelectedText, Location.X + 200, Location.Y + 300);
            // Find the text from the current cursor position
            FindTextIndex(richTextBox1.SelectionStart, false);
            FindTheText();
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindTextIndex(FindLastIndexFound + FindTextString.Length, false);
            FindTheText();
        }

        private void findPreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindTextIndex(FindLastIndexFound, true);
            FindTheText();
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string FindWhat = Interaction.InputBox("Find what:", "Replace", richTextBox1.SelectedText, Location.X + 200, Location.Y + 300);
            string ReplaceWith = Interaction.InputBox("Replace with:", "Replace", "", Location.X + 200, Location.Y + 300);

            FindTextString = FindWhat;
            // Find text from current cursor position
            FindTextIndex(richTextBox1.SelectionStart, false);

            if (FindLastIndexFound > -1)
                richTextBox1.Text = richTextBox1.Text.Substring(0, FindLastIndexFound) + ReplaceWith + richTextBox1.Text.Substring(FindLastIndexFound + FindTextString.Length);
            else
                MessageBox.Show("Cannot find '" + FindTextString + "'", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void replaceAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string FindWhat = Interaction.InputBox("Find what:", "Replace all", richTextBox1.SelectedText, Location.X + 200, Location.Y + 300);
            string ReplaceWith = Interaction.InputBox("Replace with:", "Replace all", "", Location.X + 200, Location.Y + 300);

            FindTextString = FindWhat;
            FindTextIndex(0, false);

            if (FindLastIndexFound > -1)
            {
                string NewText = Strings.Replace(richTextBox1.Text, FindTextString, ReplaceWith, 1);
                richTextBox1.Text = NewText;
            }
            else
                MessageBox.Show("Cannot find '" + FindTextString + "'", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
