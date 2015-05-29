using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Kopy
{
    class FileObj
    {
        public String Name { get; set; }
        public String Path { get; set; }
        public Int64 Size { get; set; }
        public String Sum { get; set; }
        public String NamedPath { get; set; }
        public FileObj(String FileName, String FilePath)
        {
            this.Name = FileName;
            this.Path = FilePath;
            this.NamedPath = FilePath + "\\" + FileName;

            try
            {
                this.Size = new System.IO.FileInfo(this.NamedPath).Length;
            }
            catch(FileNotFoundException)
            {
                // null everything out
            }
            catch(IOException)
            {
                
            }

        }
    }
}
