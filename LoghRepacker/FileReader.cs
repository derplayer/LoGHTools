using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LoghRepacker
{
    class FileReader
    {

        string rootDirectory;
        public void setRootDirectory(string rootDirectory = "./")
        {
            this.rootDirectory = rootDirectory;
        }

        public List<string> getFileList()
        {
            List<string> fileNames = new List<string>();
            fileNames = Directory.GetFiles(this.rootDirectory, "*", SearchOption.AllDirectories).ToList();
            return fileNames;
        }

        public List<string> getFileNamesForARC()
        {
            List<string> fileNamesForARC = new List<string>();
            foreach (string fullFileName in this.getFileList())
            {
               fileNamesForARC.Add(fullFileName.Replace(this.rootDirectory,""));
            }
            return fileNamesForARC;
        }

    }
}
