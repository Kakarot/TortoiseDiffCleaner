using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TortoiseDiffCleaner
{
    /*
     * Class to strip out duplicate sections in a diff file.
     */
    class DeDuplicator
    {
        public string FullFilePath { get; set; }
        public DeDuplicator(string fullFilePath)
        {
            FullFilePath = fullFilePath;
        }

        public void Init()
        {
            this.Parse();
        }

        private void Parse()
        {

            //Grab File name and call it "<FileName>_DeDuped.patch"
            string fileName = Path.GetFileNameWithoutExtension(FullFilePath);
            string newFileName = fileName + "_DeDuped.patch";
#if DEBUG
            Console.ResetColor();
            Console.WriteLine("Fille Name: " + fileName);
            Console.WriteLine("New File Name: " + newFileName);
            Console.WriteLine("Passed in File Path: " + FullFilePath);
            Console.WriteLine("Absolute Path: " + Path.GetFullPath(FullFilePath));
#endif
            //Create the new file.
            //  File.Create(Path.GetFullPath(FullFilePath) + newFileName);

            //Load the file into memory
            //string fullTextFile = File.ReadAllText(FullFilePath);
            StreamReader fullTextFile = new StreamReader(FullFilePath);

            //Load stringbuilder to prepare O(n) operation
            StringBuilder deDuplicatedFileContents = new StringBuilder();

            //Dictionary to hold key for what has already been parsed for ~O(1) lookup
            var alreadyParsed = new Dictionary<string, Boolean>();

            string line = String.Empty;
            string tempDiffComponentName = String.Empty;
            string tempDiffBuffer = String.Empty;
            Boolean copyOverToNewFile = false;
            while ((line = fullTextFile.ReadLine()) != null)
            {
                //First check if the first line contains the Index keyword
                if (line.Length >= 6 && line.StartsWith("Index:"))
                {
                    tempDiffComponentName = Path.GetFileName(line);
                    if (!alreadyParsed.ContainsKey(tempDiffComponentName))
                    {
                        //We have a partial success, but we need the next line to confirm.
                        //Since moving to the next line will advance the StreamReader
                        //we need to store the current line in case the next condition matches.
                        tempDiffBuffer = line;

                        //Now check if the second line contains "======" equal to the length of index
                        if ((line = fullTextFile.ReadLine()) != null && (line.StartsWith("======")))
                        {
                            //If we make it here, we need to store the file name from the
                            //temp string "tempDiffBuffer" into our Dictionary. This way
                            //we know we have already visited this section.
                            alreadyParsed.Add(Path.GetFileName(tempDiffBuffer), true);

                            //Flag to begin copying each line of StreamReader
                            copyOverToNewFile = true;

                            //Explicitly add the previous temporary line before the current line is written.
                            deDuplicatedFileContents.AppendLine(tempDiffBuffer);
                        }
                    }//end if key does not exist in dictionary
                    else
                    {
                        //The key is already in the dictionary, do not copy the following sections.
                        copyOverToNewFile = false;
                    } //end else

                }// end if line is index line
                if (copyOverToNewFile)
                    deDuplicatedFileContents.AppendLine(line);
            }
#if DEBUG
            Console.WriteLine("Finished parsing file, now attempting to write it to:");
            Console.WriteLine(new FileInfo(FullFilePath).Directory.FullName + Path.DirectorySeparatorChar + newFileName);
#endif
            //Now write the file to disk
            File.WriteAllText(new FileInfo(FullFilePath).Directory.FullName + Path.DirectorySeparatorChar + newFileName, deDuplicatedFileContents.ToString());

        }
    }
}