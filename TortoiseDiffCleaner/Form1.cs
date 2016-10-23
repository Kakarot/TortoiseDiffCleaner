using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TortoiseDiffCleaner
{
    public partial class TortoiseDiffCleaner : Form
    {
        public TortoiseDiffCleaner()
        {
            InitializeComponent();
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                //Make it a thread so we don't hang UI
                Task parseFiles = new Task(() => cleanDuplicates(filePaths));
                parseFiles.Start();
            }
        }

        private void TortoiseDiffCleaner_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void cleanDuplicates(string[] filePaths)
        {
            List<String> doubleCheck = new ArrayList().Cast<String>().ToList();
            DeDuplicator myDeDuplicator;


            foreach (string fileLoc in filePaths)
            {
                // Code to read the contents of the text file
                if (System.IO.File.Exists(fileLoc))
                {
                    if (!(System.IO.Path.GetExtension(fileLoc) == ".patch" || System.IO.Path.GetExtension(fileLoc) == ".diff"))
                    {
                        MessageBox.Show(@"I only parse .diff or .patch files generated from ""Create Patch"" menu in TortoiseSVN!" +
                            "\nCannot Parse: " + fileLoc);
                        break;
                    }

                    doubleCheck.Add(fileLoc);
                    myDeDuplicator = new DeDuplicator(fileLoc);
                    myDeDuplicator.Init();
                } //end IF for file existing                 
            }// end foreach

            if (doubleCheck.Count != 0)
            {
                StringBuilder builder = new StringBuilder();
                foreach (String str in doubleCheck)
                {
                    String strippedFile = System.IO.Path.GetFileNameWithoutExtension(str) + "_DeDuped" + System.IO.Path.GetExtension(str);
                    builder.AppendLine(strippedFile);

                }
                MessageBox.Show("Cleanse successful! The following files have been created as .patch files:\n\n" + builder.ToString()
                    + "\n\nThese files are located in the same directory as their respective source file counterparts.", "Success!");
            } //end 'if' statment to ensure valid files were dragged to UI
        }

        private void websiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout formAbout = new FormAbout();
            Enabled = false;
            formAbout.ShowDialog();
            Enabled = true;
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
@"This is a very basic GUI application that addresses the occasional issue where TortoiseSVN creates patches with duplicate entries.

Drag and Drop your diff or patch files onto the UI.
A new file will automatically be generated (without the duplicate entries) into the same directory as the source file. It will not overwrite the original diff.

Selecting and dropping multiple files onto the UI works as well.

The target platform is:

Windows 64 bit architecture with minimum.Net Framework 4.5.1", "Instructions:");
        }
    }
}
