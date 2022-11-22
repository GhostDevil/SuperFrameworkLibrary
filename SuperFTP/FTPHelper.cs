using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using static SuperFramework.SuperFTP.FTPWinAPI;

namespace SuperFramework.SuperFTP
{
    /// <summary>
    /// 日期:2014-11-26
    /// 作者:不良帥
    /// 描述:FTP辅助方法类
    /// </summary>
    public class FTPHelper
    {
        #region  字段 
        string ftpURI;
        string ftpUserID;
        string ftpServerIP;
        string ftpPassword;
        string ftpRemotePath;
        /// <summary>
        /// 下载文件的实时大小
        /// </summary>
        public long downLoadedSize = 0;
        /// <summary>
        /// 文件大小
        /// </summary>
        public long contentLength = 0;
        #endregion

        #region  构造函数 
        /// <summary>  
        /// 构造函数
        /// </summary>  
        /// <param name="FtpServerIP">FTP连接地址</param>  
        /// <param name="FtpRemotePath">指定FTP连接成功后的当前目录, 如果不指定即默认为根目录</param>  
        /// <param name="FtpUserID">用户名</param>  
        /// <param name="FtpPassword">密码</param>  
        public FTPHelper(string FtpServerIP, string FtpRemotePath, string FtpUserID, string FtpPassword)
        {
            ftpServerIP = FtpServerIP;
            ftpRemotePath = FtpRemotePath;
            ftpUserID = FtpUserID;
            ftpPassword = FtpPassword;
            ftpURI = string.Format("ftp://{0}/{1}", ftpServerIP, ftpRemotePath);
        }
        #endregion

        #region  上传文件 
        /// <summary>  
        /// 上传文件
        /// </summary>
        /// <param name="fileFullName">文件名称</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool Upload(string fileFullName)
        {
            FileInfo fileInf = new(fileFullName);
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(string.Format("{0}/{1}", ftpURI, fileInf.Name)));
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.KeepAlive = false;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            FileStream fs = fileInf.OpenRead();
            try
            {
                Stream strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region  下载文件 
        /// <summary>  
        /// 下载文件
        /// </summary>  
        /// <param name="savePath">保存路径</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="saveName">保存名称</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool DownloadFile(string savePath, string fileName,string saveName)
        {
            try
            {
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);
                FileStream outputStream = new(string.Format("{0}\\{1}", savePath, saveName), FileMode.Create);
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(string.Format("{0}/{1}", ftpURI, fileName)));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                contentLength = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    downLoadedSize += readCount;
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region  下载文件夹 
        /// <summary>
        /// 下载文件夹
        /// </summary>
        /// <param name="ftpads">FTP路径</param>
        /// <param name="name">需要下载文件路径</param>
        /// <param name="Myads">保存的本地路径</param>
        public void DownloadDir(string ftpads, string name, string Myads)
        {
            string downloadDir = Myads + name;
            string ftpdir = ftpads + name;

            string[] fullname = Ftp(ftpads, name, WebRequestMethods.Ftp.ListDirectoryDetails);
            string[] onlyname = Ftp(ftpads, name, WebRequestMethods.Ftp.ListDirectory);

            Dictionary<string, string> dic_full_only = new();
            for (int i = 0; i < fullname.Length; i++)
            {
                if (fullname[i] != "" && onlyname[i] != "")
                    dic_full_only.Add(fullname[i], onlyname[i]);
            }

            //判断是否为单个文件 
            if (fullname.Length <= 2)
            {
                if (fullname[fullname.Length - 1] == "")
                {
                    DownloadSingleFile(string.Format("{0}/{1}", downloadDir, name), string.Format("{0}{1}/{1}", ftpads, name));
                }
            }
            else
            {
                if (!Directory.Exists(downloadDir))
                {
                    Directory.CreateDirectory(downloadDir);
                }
                foreach (KeyValuePair<string, string> pair in dic_full_only)
                {
                    //判断是否具有文件夹标识<DIR>
                    if (pair.Key.Contains("<DIR>"))
                    {
                        string olname = pair.Key.Split(new string[] { "<DIR>" },
                        StringSplitOptions.None)[1].Trim();
                        DownloadDir(ftpdir, "\\" + olname, downloadDir);
                    }
                    else
                    {
                        DownloadSingleFile(string.Format("{0}\\{1}", downloadDir, dic_full_only[pair.Key]), string.Format("{0}{1}\\{2}", ftpads, name, dic_full_only[pair.Key]));
                    }
                }
            }

        }
        /// <summary>
        /// 建立FTP连接
        /// </summary>
        /// <param name="ftpads">FTP地址路径</param>
        /// <param name="name">文件或者文件夹名字</param>
        /// <param name="type">要发送到FTP服务器的命令</param>
        /// <returns></returns>
        private string[] Ftp(string ftpads, string name, string type)
        {
            StreamReader ftpFileListReader = null;
            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(new Uri(ftpads + name));
                ftpRequest.Method = type;
                WebResponse webresp = ftpRequest.GetResponse();
                ftpFileListReader = new StreamReader(webresp.GetResponseStream(), Encoding.Default);
            }
            catch (Exception ex)
            {
                ex.ToString();

            }
            StringBuilder str = new();
            string line = ftpFileListReader.ReadLine();
            while (line != null)
            {
                str.Append(line);
                str.Append("\n");
                line = ftpFileListReader.ReadLine();
            }
            string[] fen = str.ToString().Split('\n');
            return fen;
        }

        /// <summary>
        /// 单个文件下载方法
        /// </summary>
        /// <param name="adss">保存文件的本地路径</param>
        /// <param name="ftpadss">下载文件的FTP路径</param>
        private void DownloadSingleFile(string adss, string ftpadss)
        {
            //FileMode常数确定如何打开或创建文件,指定操作系统应创建新文件。
            //FileMode.Create如果文件已存在，它将被改写
            FileStream outputStream = new(adss, FileMode.Create);
            FtpWebRequest downRequest = (FtpWebRequest)WebRequest.Create(new Uri(ftpadss));
            //设置要发送到 FTP 服务器的命令
            downRequest.Method = WebRequestMethods.Ftp.DownloadFile;
            FtpWebResponse response = (FtpWebResponse)downRequest.GetResponse();
            Stream ftpStream = response.GetResponseStream();
            long cl = response.ContentLength;
            int bufferSize = 2048;
            int readCount;
            byte[] buffer = new byte[bufferSize];
            readCount = ftpStream.Read(buffer, 0, bufferSize);
            downLoadedSize += readCount;
            while (readCount > 0)
            {
                outputStream.Write(buffer, 0, readCount);
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                downLoadedSize += readCount;
            }
            ftpStream.Close();
            outputStream.Close();
            response.Close();
        }
        #endregion

        #region  删除当前目录下文件 
        /// <summary>  
        /// 删除当前目录下文件  
        /// </summary> 
        /// <param name="fileName">文件名称</param>
        /// <returns>成功返回true，失败返回flase</returns>
        public bool DeleteFile(string fileName)
        {
            try
            {
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + "/" + fileName));
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
                reqFTP.KeepAlive = false;
                string result = string.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
        #endregion

        #region  获取当前目录下明细(包含文件和文件夹) 
        /// <summary>  
        /// 获取当前目录下明细(包含文件和文件夹)  
        /// </summary>  
        /// <returns>返回文件列表数组</returns>
        /// <exception cref="Exception">1.URL错误 2.文件不存在</exception>
        public FileStruct[] GetFilesDetailList()
        {
            try
            {
                StringBuilder result = new();
                FtpWebRequest ftp;
                ftp = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                ftp.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                ftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = ftp.GetResponse();
                StreamReader reader = new(response.GetResponseStream());
                List<FileStruct> fs = new();
                string line = reader.ReadLine();
                while (line != null)
                {
                    FileStruct f = new();
                    f = GetList(line);
                    string fileName = f.Name;                     //排除非文件夹 
                    line = reader.ReadLine();
                    fs.Add(f);
                }

                return fs.ToArray();
                //string line = reader.ReadLine();
                //while (line != null)
                //{
                //    result.Append(line);
                //    result.Append("\n");
                //    line = reader.ReadLine();
                //}
                //result.Remove(result.ToString().LastIndexOf("\n"), 1);
                //reader.Close();
                //response.Close();
                //return result.ToString().Split('\n');
            }
            catch (Exception e)
            {
                return null;
            }
        }
        #endregion

        #region  获取文件列表 
        /// <summary>  
        /// 获取FTP文件列表(包括文件夹)
        /// </summary> 
        /// <returns>返回文件列表数组</returns>
        /// <exception cref="Exception">未知错误，详见错误信息</exception>
        private string[] GetAllList()
        {
            List<string> list = new();
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(new Uri(ftpURI));
            req.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            req.Method = WebRequestMethods.Ftp.ListDirectory;
            req.UseBinary = true;
            req.UsePassive = true;
            try
            {
                using (FtpWebResponse res = (FtpWebResponse)req.GetResponse())
                {
                    using (StreamReader sr = new(res.GetResponseStream()))
                    {
                        string s;
                        while ((s = sr.ReadLine()) != null)
                        {
                            list.Add(s);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return list.ToArray();
        }

        /// <summary>  
        /// 获取FTP文件列表(不包括文件夹)  
        /// </summary> 
        /// <returns>返回文件列表数组</returns>
        /// <exception cref="Exception">URL错误</exception>
        public string[] GetFileList()
        {
            StringBuilder result = new();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {

                    if (line.IndexOf("<DIR>") == -1)
                    {
                        result.Append(Regex.Match(line, @"[\S]+ [\S]+", RegexOptions.IgnoreCase).Value.Split(' ')[1]);
                        result.Append("\n");
                    }
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
            }
            catch (Exception e)
            {
                throw;
            }
            return result.ToString().Split('\n');
        }
        #endregion

        #region  判断当前目录下指定的文件是否存在 
        /// <summary>  
        /// 判断当前目录下指定的文件是否存在  
        /// </summary>  
        /// <param name="remoteFileName">远程文件名</param> 
        /// <returns>存在返回true，不存在返回false</returns>

        public bool FileExist(string remoteFileName)
        {
            string[] fileList = GetFileList();
            foreach (string str in fileList)
            {
                if (str.Trim() == remoteFileName.Trim())
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region  创建文件夹 
        /// <summary>  
        /// 创建文件夹  
        /// </summary>   
        /// <param name="dirName">文件夹名称</param>
        /// <exception cref="Exception">1.URL错误 2.文件夹命名不规范</exception>

        public void MakeDir(string dirName)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(string.Format("{0}/{1}", ftpURI, dirName)));
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception e)
            { throw; }
        }
        #endregion

        #region   删除文件夹 
        /// <summary>
        /// 删除空文件夹
        /// </summary>
        /// <param name="dirName">文件夹名称或路径</param>
        /// <exception cref="Exception">1.URL错误 2.文件夹不存在 3.文件夹下不为空</exception>
        public void DelDir(string dirName)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(string.Format("{0}/{1}", ftpURI, dirName)));
                reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception e)
            {
                throw;
            }

        }
        /// <summary>
        /// 删除非空文件夹
        /// </summary>
        /// <param name="path"></param>
        public void DeleteDir(string path)
        {
            try
            {
                string[] folderArray = GetDeleteFolderArray(path);
                string[] fileArray = GetDeleteFileArray(path);
                ArrayList folderArrayList = new();
                ArrayList fileArrayList = new();
                //重新构造存放文件夹的数组(用动态数组实现) 
                if (folderArray != null)
                {
                    for (int i = 0; i < folderArray.Length; i++)
                    {
                        if (folderArray[i] == "." || folderArray[i] == ".." || folderArray[i] == "")
                            continue;
                        else
                            folderArrayList.Add(folderArray[i]);
                    }
                }
                if (fileArray != null)
                {
                    //重新构造存放文件的数组(用动态数组实现) 
                    for (int i = 0; i < fileArray.Length; i++)
                    {
                        if (fileArray[i] == "")
                            continue;
                        else
                            fileArrayList.Add(fileArray[i]);
                    }
                }
                if (folderArrayList.Count == 0 && fileArrayList.Count == 0)
                {
                    DelDir(path);
                }
                else if (folderArrayList.Count == 0 && fileArrayList.Count != 0)
                {
                    for (int i = 0; i < fileArrayList.Count; i++)
                    {
                        string fileUri = string.Format("{0}/{1}", path, fileArrayList[i]);
                        DeleteFile(fileUri);
                    }
                    DelDir(path);
                }
                else if (folderArrayList.Count != 0 && fileArrayList.Count != 0)
                {
                    for (int i = 0; i < fileArrayList.Count; i++)
                    {
                        string fileUri = string.Format("{0}/{1}", path, fileArrayList[i]);
                        DeleteFile(fileUri);
                    }
                    for (int i = 0; i < folderArrayList.Count; i++)
                    {
                        string dirUri = string.Format("{0}/{1}", path, folderArrayList[i]);
                        DeleteDir(dirUri);
                    }
                    DelDir(path);
                }
                else if (folderArrayList.Count != 0 && fileArrayList.Count == 0)
                {
                    for (int i = 0; i < folderArrayList.Count; i++)
                    {
                        string dirUri = string.Format("{0}/{1}", path, folderArrayList[i]);
                        DeleteDir(dirUri);
                    }
                    DelDir(path);
                }
            }
            catch
            {
                throw;
            }
        }
        /// <summary>
        /// 获取子文件夹数组 
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <returns>返回文件夹数组</returns>

        private string[] GetDeleteFolderArray(string path)
        {
            string[] deleteFolders;
            StringBuilder result = new();
            FtpWebRequest reqFTP;
            try
            {

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + "/" + path));
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Encoding encoding = Encoding.GetEncoding("GB2312");
                StreamReader reader = new(response.GetResponseStream(), encoding);
                string line = reader.ReadLine();
                bool flag = false;
                while (line != null)
                {
                    FileStruct f = new();
                    f = GetList(line);
                    string fileName = f.Name;
                    if (f.IsDirectory)
                    {
                        result.Append(fileName);
                        result.Append("\n");
                        flag = true;
                        line = reader.ReadLine();
                        continue;
                    }
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
                if (flag)
                {
                    result.Remove(result.ToString().LastIndexOf("\n"), 1);
                    return result.ToString().Split('\n');
                }
                else
                {
                    deleteFolders = null;
                    return deleteFolders;
                }
            }
            catch
            {

                deleteFolders = null;
                return deleteFolders;
            }
        }
        /// <summary>
        /// 获取子文件数组
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <returns>返回文件数组</returns>

        private string[] GetDeleteFileArray(string path)
        {
            string[] DeleteFiles;
            StringBuilder result = new();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + "/" + path));
                reqFTP.UseBinary = true;
                reqFTP.UsePassive = false;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Encoding encoding = Encoding.GetEncoding("GB2312");
                StreamReader reader = new(response.GetResponseStream(), encoding);
                string line = reader.ReadLine();
                bool flag = false;
                while (line != null)
                {
                    FileStruct f = new();
                    f = GetList(line);
                    string fileName = f.Name;                     //排除非文件夹 
                    if (!f.IsDirectory)
                    {
                        result.Append(fileName);
                        result.Append("\n");
                        flag = true;
                        line = reader.ReadLine();
                        continue;
                    }
                    line = reader.ReadLine();
                }
                reader.Close();
                response.Close();
                if (flag)
                {
                    result.Remove(result.ToString().LastIndexOf("\n"), 1);
                    return result.ToString().Split('\n');
                }

                else
                {
                    DeleteFiles = null;
                    return DeleteFiles;
                }
            }
            catch
            {

                DeleteFiles = null;
                return DeleteFiles;
            }
        }

        #endregion

        #region  获取指定文件大小 
        /// <summary>  
        /// 获取指定文件大小  
        /// </summary>  
        /// <param name="filename">文件名称</param>
        /// <returns>返回文件大小</returns>
        /// <exception cref="Exception">未知错误，详见错误信息</exception>
        public long GetFileSize(string filename)
        {
            FtpWebRequest reqFTP;
            long fileSize;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + "/" + filename));
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                fileSize = response.ContentLength;
                ftpStream.Close();
                response.Close();
            }
            catch (Exception e)
            { throw; }
            return fileSize;
        }
        #endregion

        #region  更改文件名 
        /// <summary>  
        /// 更改文件名  
        /// </summary> 
        /// <param name="currentFileName">源文件名称</param>
        /// <param name="newFileName">新文件名称</param>
        /// <exception cref="Exception">未知错误，详见错误信息</exception>
        public void ReName(string currentFileName, string newFileName)
        {
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(ftpURI + "/" + currentFileName));
                reqFTP.Method = WebRequestMethods.Ftp.Rename;
                reqFTP.RenameTo = newFileName;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
            }
            catch (Exception e)
            { throw; }
        }
        #endregion

        #region  移动文件 
        /// <summary>  
        /// 移动文件  
        /// </summary>  
        /// <param name="currentFilename">源文件名称</param>
        /// <param name="newDirectory">新文件路径和文件名（文件夹/234.txt）</param>
        public void MovieFile(string currentFilename, string newDirectory)
        {
            ReName(currentFilename, newDirectory);

        }
        #endregion

        #region  切换当前目录 
        /// <summary>  
        /// 切换当前目录  
        /// </summary>  
        /// <param name="directoryName">切换到的路径</param>
        /// <param name="isRoot">true:绝对路径 false:相对路径</param>
        public void GotoDirectory(string directoryName, bool isRoot)
        {
            if (isRoot)
                ftpRemotePath = directoryName;
            else
                ftpRemotePath = string.Format("{0}/{1}", ftpRemotePath, directoryName);
            ftpURI = string.Format("ftp://{0}/{1}", ftpServerIP, ftpRemotePath);
        }
        #endregion

        #region  获取文件、文件夹信息 
        /// <summary>  
        /// 列出FTP服务器上面当前目录的所有文件信息
        /// </summary>  
        private FileStruct[] ListFiles(FileStruct[] listAll)
        {
            List<FileStruct> listFile = new();
            foreach (FileStruct file in listAll)
                if (!file.IsDirectory)
                    listFile.Add(file);
            return listFile.ToArray();
        }
        /// <summary>  
        /// 列出FTP服务器上面当前目录的所有的目录信息 
        /// </summary>  
        private FileStruct[] ListDirectories(FileStruct[] listAll)
        {
            List<FileStruct> listDirectory = new();
            foreach (FileStruct file in listAll)
                if (file.IsDirectory)
                    listDirectory.Add(file);
            return listDirectory.ToArray();
        }

        /// <summary>
        /// 获得文件或目录信息
        /// </summary>
        /// <param name="datastring">FTP返回的列表字符信息</param>
        /// <returns></returns>
        private FileStruct GetList(string datastring)
        {
            FileStruct f = new();
            string[] dataRecords = datastring.Split('\n');
            FTPEnum.FileListStyle _directoryListStyle = GuessFileListStyle(dataRecords);
            if (_directoryListStyle != FTPEnum.FileListStyle.Unknown && datastring != "")
            {
                switch (_directoryListStyle)
                {
                    case FTPEnum.FileListStyle.UnixStyle:
                        f = ParseFileStructFromWindowsStyleRecord(datastring);
                        break;
                    case FTPEnum.FileListStyle.WindowsStyle:
                        f = ParseFileStructFromWindowsStyleRecord(datastring);
                        break;
                }
            }
            return f;
        }

        /// <summary>
        /// 从Windows格式中返回文件信息
        /// </summary>
        /// <param name="Record">文件信息</param>
        /// <returns></returns>
        private FileStruct ParseFileStructFromWindowsStyleRecord(string Record)
        {
            FileStruct f = new();
            string processstr = Record.Trim();
            string dateStr = processstr.Substring(0, 8);
            processstr = (processstr.Substring(8, processstr.Length - 8)).Trim();
            string timeStr = processstr.Substring(0, 7);
            processstr = (processstr.Substring(7, processstr.Length - 7)).Trim();
            DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
            myDTFI.ShortTimePattern = "t";
            f.CreateTime = DateTime.Parse(string.Format("{0} {1}", dateStr, timeStr), myDTFI);
            if (processstr.Substring(0, 5) == "<DIR>")
            {
                f.IsDirectory = true;
                processstr = (processstr.Substring(5, processstr.Length - 5)).Trim();
            }
            else
            {
                //string[] strs = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);// true);
                //string[] strs = processstr.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                string str = processstr.Substring(0, processstr.IndexOf(" "));
                f.FileSize = long.Parse(str);
                processstr = processstr.Substring(processstr.IndexOf(" ") + 1);
                f.IsDirectory = false;
            }
            f.Name = processstr;
            return f;
        }

        /// <summary>
        /// 判断文件列表的方式Window方式还是Unix方式
        /// </summary>
        /// <param name="recordList">文件信息列表</param>
        /// <returns></returns>
        private FTPEnum.FileListStyle GuessFileListStyle(string[] recordList)
        {
            foreach (string s in recordList)
            {
                if (s.Length > 10 && Regex.IsMatch(s.Substring(0, 10), "(-|d)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)(-|r)(-|w)(-|x)"))
                    return FTPEnum.FileListStyle.UnixStyle;
                else if (s.Length > 8 && Regex.IsMatch(s.Substring(0, 8), "[0-9][0-9]-[0-9][0-9]-[0-9][0-9]"))
                    return FTPEnum.FileListStyle.WindowsStyle;
            }
            return FTPEnum.FileListStyle.Unknown;
        }

        /// <summary>
        /// 从Unix格式中返回文件信息
        /// </summary>
        /// <param name="Record">文件信息</param>
        /// <returns></returns>
        private FileStruct ParseFileStructFromUnixStyleRecord(string Record)
        {
            FileStruct f = new();
            string processstr = Record.Trim();
            f.Flags = processstr.Substring(0, 10);
            f.IsDirectory = (f.Flags[0] == 'd');
            processstr = (processstr.Substring(11)).Trim();
            CutSubstringFromStringWithTrim(ref processstr, ' ', 0);  //跳过一部分             
            f.Owner = CutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            f.Group = CutSubstringFromStringWithTrim(ref processstr, ' ', 0);
            CutSubstringFromStringWithTrim(ref processstr, ' ', 0);  //跳过一部分 
            string yearOrTime = processstr.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[2];
            if (yearOrTime.IndexOf(":") >= 0)  //time             
                processstr = processstr.Replace(yearOrTime, DateTime.Now.Year.ToString());
            f.CreateTime = DateTime.Parse(CutSubstringFromStringWithTrim(ref processstr, ' ', 8));
            f.Name = processstr;  //最后就是名称             
            return f;
        }

        /// <summary>
        /// 按照一定的规则进行字符串截取
        /// </summary>
        /// <param name="s">截取的字符串</param>
        /// <param name="c">查找的字符</param>
        /// <param name="startIndex">查找的位置</param>
        /// <returns></returns>
        private string CutSubstringFromStringWithTrim(ref string s, char c, int startIndex)
        {
            int pos1 = s.IndexOf(c, startIndex);
            string retString = s.Substring(0, pos1);
            s = (s.Substring(pos1)).Trim();
            return retString;
        }
        #endregion

        
    }
}