using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Kopy
{
    class FileManip
    {
        public void FileSum(FileObj sumFile, uint sumLevel, out String checkSum)
        {
            // initialize variable
            checkSum = string.Empty;
            System.Security.Cryptography.HashAlgorithm cryptoProvider;
            
            switch (sumLevel)
            {
                case 1: cryptoProvider = new SHA1Managed();
                    break;
                case 5: cryptoProvider = new MD5Cng();
                    break;
                case 256: cryptoProvider = new SHA256Managed();
                    break;
                case 512: cryptoProvider = new SHA512Managed();
                    break;
                default: throw new ArgumentException("Invalid MD/SHA Level Specified");
            }

            // Instantiating the file like this should allow other operations to continue on it without issue
            using(FileStream fs = new FileStream(sumFile.NamedPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using(BufferedStream bs = new BufferedStream(fs))
                {
                    try
                    {
                        byte[] hashBuilder = cryptoProvider.ComputeHash(bs);
                        StringBuilder formattedHash = new StringBuilder(2 * hashBuilder.Length);
                        foreach (byte b in hashBuilder)
                            formattedHash.AppendFormat("{0:X2}", b);
                    }
                    catch(Exception e)
                    {
                        throw e;
                    }
                }
            }
        }
        public void PerformCopyBasic(FileObj inputFile, bool overwrite, String copyTo, out FileObj outputFile)
        {
            String newFile = copyTo + "\\" + inputFile.Name;
            File.Copy(inputFile.NamedPath, newFile, overwrite);
            outputFile = new FileObj(inputFile.Name, copyTo);
        }
        public void PerformCopyByte(FileObj inputFile, bool overwrite, String copyTo, int attemptCount, bool ignoreBadDisk, bool verifyFile, out FileObj outputFile)
        {

        }
    }
}
