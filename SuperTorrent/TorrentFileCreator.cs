
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace SuperFramework.SuperTorrent
{
    public delegate void FileIODelegate(ulong numByteRead);
    public class TorrentFileCreator
    {
        /// <summary>
        /// Encode a single file
        /// </summary>
        /// <param name="fileName">filename include path</param>
        /// <param name="announceUrl">announce url</param>
        public static void EncodeFile(string fileName, 
            string announceUrl )
        {
            string torrentFileName = string.Format("{0}.torrent",
                Path.GetFileNameWithoutExtension(fileName));
            CreateTorrentFile(new string[] { fileName }, 
                announceUrl, torrentFileName, string.Empty);
        }

        /// <summary>
        /// Encode a whole directory
        /// </summary>
        /// <param name="directoryPath">path of the directory</param>
        /// <param name="announceUrl">announce url</param>
        public static void EncodeDirectory(string directoryPath,
            string announceUrl)
        {
            string[] fileNames = Directory.GetFiles(directoryPath);
            string directoryName = TorrentUtility.GetClosestDirectoryName(
                directoryPath);
            string torrentFileName = string.Format("{0}.torrent",
               Path.GetFileNameWithoutExtension(directoryName));
            CreateTorrentFile(fileNames,announceUrl,
                torrentFileName, directoryName);
        }
       

        /// <summary>
        /// Main entry
        /// </summary>
        /// <param name="fileName">Array of the files to be encoded in a torrent file</param>
        /// <param name="announceUrl">announceUrl</param>
        /// <param name="torrentFileName">output torrent filename</param>
        /// <param name="torrentDirectoryName"></param>
        private static void CreateTorrentFile(string[] fileName, 
            string announceUrl, string torrentFileName, 
            string torrentDirectoryName)
        {
            string[] sortedFileName = TorrentUtility.OrdinalSortStringArray(fileName);
            // 1. create the torrent tree
            BenDictionary root = new BenDictionary();
            // 1.1 announce URL
            root.Add("announce",new BenStringFormString(announceUrl));
            // 1.2 info block
            BenDictionary info = new BenDictionary();
            // 1.2.1 name
            bool IsDirectory = !string.IsNullOrEmpty(torrentDirectoryName);
            string strNameValue;
            if (IsDirectory)
            {
                strNameValue = torrentDirectoryName;
            }
            else
            {
                strNameValue = Path.GetFileName(sortedFileName[0]);
            }
            BenStringFormString nameValue = 
                new BenStringFormString(strNameValue);
            info.Add("name", nameValue);
            // 1.2.2 piece length
            BenInt pieceLength = new BenInt(1024 * 1024);
            info.Add("piece length", pieceLength);
            // 1.2.3 pieces
            BenBinaryFormString pieces =
                new BenBinaryFormString(Get1MBasedHashFromFiles(sortedFileName));
            info.Add("pieces", pieces);
            // 1.2.4 single file
            if (IsDirectory)
            {
                BenList files = GetFilesBlock(sortedFileName);
                info.Add("files", files);
            }
            else
            {
                BenInt length =
                    new BenInt(
                    TorrentUtility.GetFileLength(sortedFileName[0])
                    );
                info.Add("length", length);
            }
            root.Add("info", info);
            // 2. write to the file
            using (FileStream torrentFileStream = new FileStream(torrentFileName,
                FileMode.Create, FileAccess.Write))
            {
                byte[] torrentFileByte = root.ToByteArray();
                torrentFileStream.Write(torrentFileByte,
                    0, torrentFileByte.Length);
            }
        }

        static private byte[] Get1MBasedHashFromFiles(string[] fileName)
        {
            const int blockSize = 1024 * 1024;
            List<byte> hashList = new List<byte>();
            SHA1 sha1 = SHA1.Create();
            byte[] hash;
            FileBlockReader blockReader =
                new FileBlockReader(fileName, blockSize);
            byte[] buffer = new byte[blockSize];
            while (!blockReader.EOF)
            {
                int byteReaded = blockReader.ReadBlock(buffer);
                hash = sha1.ComputeHash(buffer, 0, byteReaded);
                hashList.AddRange(hash);
                // report the progress
                FireProgress(Convert.ToUInt64(byteReaded));
            }
            return hashList.ToArray();
        }

        static BenList GetFilesBlock(string[] fileName)
        {
            BenList files = new BenList();
            foreach (string file in fileName)
            {
                BenDictionary fileEntry =
                    new BenDictionary();
                // 1. length
                BenInt length = 
                    new BenInt(TorrentUtility.GetFileLength(file));
                fileEntry.Add("length", length);
                // 2. path
                BenList path = new BenList();
                string fileNameSegment = Path.GetFileName(file);
                path.Add(new BenStringFormString(fileNameSegment));
                fileEntry.Add("path", path);
                files.Add(fileEntry);
            }
            return files;
        }

        static private void FireProgress(ulong numByteRead)
        {
            if (OnProgress != null)
            {
                OnProgress(numByteRead);
            }
        }

        static public FileIODelegate OnProgress;
    }

    class FileBlockReader
    {
        public FileBlockReader(string[] fileName, int blockSize)
        {
            if (fileName.Length == 0)
                throw new ApplicationException("Should have at least one file");
            fs = new FileStream(fileName[0], FileMode.Open, FileAccess.Read);
            m_fileName = fileName;
            m_blockSize = blockSize;
            index = 0;
            EOF = false;
        }
        public int ReadBlock(byte[] buffer)
        {
            int byteToRead = m_blockSize;
            while (byteToRead > 0)
            {
                int currentRead = fs.Read(buffer,
                    m_blockSize - byteToRead, byteToRead);
                byteToRead -= currentRead;
                //eof reached
                if (byteToRead > 0)
                {
                    // still have files to read
                    if (index < m_fileName.Length - 1)
                    {
                        fs.Close();
                        ++index;
                        fs = new FileStream(m_fileName[index], 
                            FileMode.Open, FileAccess.Read);
                    }
                    else
                    {
                        // reach the tail
                        fs.Close();
                        EOF = true;
                        break;
                    }
                }
            }
            int byteReaded = m_blockSize - byteToRead;
            return byteReaded;
        }

        private FileStream fs;
        private string[] m_fileName;
        private int m_blockSize;
        private int index;
        public bool EOF;
    }
}
