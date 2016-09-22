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
            List<String> doubleCheck = new ArrayList().Cast<String>().ToList();
            DeDuplicator myDeDuplicator;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));

                foreach (string fileLoc in filePaths)
                {


                    // Code to read the contents of the text file
                    if (System.IO.File.Exists(fileLoc))
                    {
                        if (!(System.IO.Path.GetExtension(fileLoc) == ".patch" || System.IO.Path.GetExtension(fileLoc) == ".diff"))
                        {
                            MessageBox.Show(@"I only parse .diff or .patch files generated from ""Create Patch"" menu in TortoiseSVN!");
                            break;
                        }                        

                        doubleCheck.Add(fileLoc);
                        myDeDuplicator = new DeDuplicator(fileLoc);
                        myDeDuplicator.Init();
                    } //end IF for file existing
                    //doubleCheck.ToString();
                }// end foreach

                if (doubleCheck.Count != 0)
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (String str in doubleCheck)
                    {
                        String strippedFile = System.IO.Path.GetFileName(str);
                        builder.AppendLine(strippedFile);

                    }
                    MessageBox.Show("The following files have been converted to .patch files:\n\n" + builder.ToString()
                        + "\n\nThese files are located in the same directory as their respective source file counterparts, which is\n\n" +
                        System.IO.Path.GetDirectoryName(filePaths[0]));
                } //end 'if' statment to ensure valid files were dragged to UI
            }
        }
    }
}
