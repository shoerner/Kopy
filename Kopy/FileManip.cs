using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Kopy
{
    class FileManip
    {
        /// <summary>
        /// Calls file checksum evaluation
        /// </summary>
        /// <param name="sumFile">Object representing file to be processed</param>
        /// <param name="sumLevel">Sum to be made. Valid options: 1 (SHA1), 5 (MD5), 256 (SHA256), 512 (SHA512)</param>
        /// <param name="checkSum">Output checksum</param>
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
        public void PerformCopySimple(FileObj inputFile, bool overwrite, String copyTo, out FileObj outputFile)
        {
            String newFile = copyTo + "\\" + inputFile.Name;
            File.Copy(inputFile.NamedPath, newFile, overwrite);
            outputFile = new FileObj(inputFile.Name, copyTo);
        }
        /// <summary>
        /// Expensive process used to copy each byte of the file individually. Each call of readbyte is allocating a new mini array.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="overwrite"></param>
        /// <param name="copyTo"></param>
        /// <param name="attemptCount"></param>
        /// <param name="ignoreBadDisk"></param>
        /// <param name="verifyFile"></param>
        /// <param name="outputFile"></param>
        public void PerformCopyByte(FileObj inputFile, bool overwrite, String copyTo, int attemptCount, bool ignoreBadDisk, bool verifyFile, out FileObj outputFile)
        {
            String newFile = copyTo + "\\" + inputFile.Name;
            FileMode mode;
            using(FileStream inputStream = new FileStream(inputFile.NamedPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // Attempt to read for 30 seconds. Throw exception after that.
                inputStream.ReadTimeout = 30000;
                // Note this will overwrite a destination file 
                // TODO: Insert logic to ask about overwrite at destination
                if(overwrite)
                {
                    mode = FileMode.Create;
                }
                else if (!overwrite && File.Exists(newFile))
                {
                    LocalLog.Instance.MakeEvent("File Exists on byte level copy, overwrite not set", "FileManip:PerformCopyByte", "WARNING");
                    throw new IOException("FileExists");
                }
                else
                {
                    mode = FileMode.CreateNew;
                }
                using(FileStream outputStream = new FileStream(newFile, mode, FileAccess.ReadWrite, FileShare.None))
                {
                    for(int readByte = 0; readByte < inputStream.Length; readByte++)
                    {
                        byte[] bytes = new byte[1];
                        int output = inputStream.Read(bytes, readByte, readByte + 1);
                        if (output == 0)
                            break;
                        outputStream.WriteByte(bytes[0]);
                    }
                }
            }

            outputFile = new FileObj(inputFile.Name, copyTo);
        }
        public bool PerformMoveSimple(FileObj inputFile, bool overwrite, String destinationPath)
        {
            try
            {
                File.Copy(inputFile.NamedPath, (destinationPath + "\\" + inputFile.Name), overwrite);
                inputFile.Path = destinationPath;
                return true;
            }
            catch (IOException ioe)
            {
                LocalLog.Instance.MakeEvent("IOException caught: " + ioe.Message, "FileManip:PerformMoveSimple", "ERROR");
                return false;
            }
            catch(Exception e)
            {
                LocalLog.Instance.MakeEvent("Exception caught on file move: " + e.StackTrace, "FileManip:PerformMoveSimple", "ERROR");
                return false;
            }
        }
    }
}
