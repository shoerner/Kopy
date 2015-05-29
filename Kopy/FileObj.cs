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
        public String NamedPath 
        { 
            get
            {
                return Name + "\\" + Path;
            }
        }
        public FileObj(String FileName, String FilePath)
        {
            this.Name = FileName;
            this.Path = FilePath;

            try
            {
                this.Size = new System.IO.FileInfo(this.NamedPath).Length;
            }
            catch(FileNotFoundException)
            {
                LocalLog.Instance.MakeEvent("FileNotFoundException Raised on " + NamedPath, "FileObj:FileObj", "ERROR");
                // null everything out
                Name = String.Empty;
                Path = String.Empty;
            }
            catch(IOException ioe)
            {
                LocalLog.Instance.MakeEvent("Generic IOException Raised on " + NamedPath + "InnerText: " + ioe.StackTrace, "FileObj:FileObj", "ERROR");
                // null everything out
                Name = String.Empty;
                Path = String.Empty;
            }

        }
    }
}
