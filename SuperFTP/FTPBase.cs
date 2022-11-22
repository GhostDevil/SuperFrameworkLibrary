using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using Microsoft.Win32;
using System.Drawing;
using static SuperFramework.SuperFTP.FTPWinAPI;

namespace SuperFramework.SuperFTP
{
    /// <summary>
    /// 日期:2015-05-12
    /// 作者:不良帥
    /// 描述:FTP辅助方法类，含基础信息。
    /// </summary>
    class FTPBase
    {

        #region  获取服务器图标
        /// <summary>
        /// 获取文件图标
        /// </summary>
        /// <param name="fileType">文件后缀.*</param>
        /// <param name="isLarge"></param>
        /// <returns>给出文件扩展名（.*），返回相应图标,若不以"."开头则返回文件夹的图标。</returns>
        public Icon GetIconByFileType(string fileType,bool isLarge)
        {
            if(fileType == null || fileType.Equals(string.Empty)) return null;
            string regIconString = null;
            string systemDirectory = Environment.SystemDirectory + "\\";
            if(fileType[0] == '.')
            {
                //读系统注册表中文件类型信息
                RegistryKey regVersion = Registry.ClassesRoot.OpenSubKey(fileType, true);
                if (regVersion != null)
                {
                    string regFileType = regVersion.GetValue("") as string;
                    regVersion.Close();
                    regVersion = Registry.ClassesRoot.OpenSubKey(regFileType + @"\DefaultIcon" , true);
                    if(regVersion != null)
                    {
                        regIconString = regVersion.GetValue("") as string;
                        regVersion.Close();
                    }
                }
                if(regIconString == null)
                {
                    //没有读取到文件类型注册信息，指定为未知文件类型的图标
                    regIconString = systemDirectory +"shell32.dll,0";
                }
            }
            else
            {
                //直接指定为文件夹图标
                regIconString = systemDirectory +"shell32.dll,3";
            }
            string[] fileIcon = regIconString.Split(new char[]{','});
            if(fileIcon.Length != 2)
            {
                //系统注册表中注册的标图不能直接提取，则返回可执行文件的通用图标
                fileIcon = new string[]{systemDirectory +"shell32.dll","2"};
            }
            Icon resultIcon;
            try
            {
                //调用API方法读取图标
                int[] phiconLarge = new int[1];
                int[] phiconSmall = new int[1];
                uint count = FTPWinAPI.ExtractIconEx(fileIcon[0], int.Parse(fileIcon[1]),phiconLarge,phiconSmall,1);
                IntPtr IconHnd = new(isLarge?phiconLarge[0]:phiconSmall[0]);
                resultIcon = Icon.FromHandle(IconHnd);
            }

            catch
            {
                fileIcon = new string[] { systemDirectory + "shell32.dll", "2" };
                                //调用API方法读取图标
                int[] phiconLarge = new int[1];
                int[] phiconSmall = new int[1];
                uint count = FTPWinAPI.ExtractIconEx(fileIcon[0], int.Parse(fileIcon[1]),phiconLarge,phiconSmall,1);
                IntPtr IconHnd = new(isLarge?phiconLarge[0]:phiconSmall[0]);
                resultIcon = Icon.FromHandle(IconHnd);
            }
            return resultIcon;
        }
        #endregion

        #region  文件夹的复制
        /// <summary>
        /// 文件夹的复制
        /// </summary>
        /// <param Ddir="string">要复制的目的路径</param>
        /// <param Sdir="string">要复制的原路径</param>
        public void FilesCopy(string Ddir, string Sdir)
        {
            DirectoryInfo dir = new(Sdir);
            try
            {
                if (!dir.Exists)//判断所指的文件或文件夹是否存在
                {
                    return;
                }
                DirectoryInfo dirD = dir as DirectoryInfo;//如果给定参数不是文件夹则退出
                string UpDir = UpAndDownDir(Ddir);
                if (dirD == null)//判断文件夹是否为空
                {
                    Directory.CreateDirectory(string.Format("{0}\\{1}", UpDir, dirD.Name));//如果为空，创建文件夹并退出
                    return;
                }
                else
                {
                    Directory.CreateDirectory(string.Format("{0}\\{1}", UpDir, dirD.Name));
                }
                string SbuDir = string.Format("{0}\\{1}\\", UpDir, dirD.Name);
                FileSystemInfo[] files = dirD.GetFileSystemInfos();//获取文件夹中所有文件和文件夹
                //对单个FileSystemInfo进行判断,如果是文件夹则进行递归操作
                foreach (FileSystemInfo FSys in files)
                {
                    FileInfo file = FSys as FileInfo;
                    if (file != null)//如果是文件的话，进行文件的复制操作
                    {
                        FileInfo SFInfo = new(string.Format("{0}\\{1}", file.DirectoryName, file.Name));//获取文件所在的原始路径
                        SFInfo.CopyTo(string.Format("{0}\\{1}", SbuDir, file.Name), true);//将文件复制到指定的路径中
                    }
                    else
                    {
                        //string pp = FSys.Name;//获取当前搜索到的文件夹名称
                        FilesCopy(SbuDir + FSys, string.Format("{0}\\{1}", Sdir, FSys));//如果是文件，则进行递归调用
                    }
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region  返回上一级目录
        /// <summary>
        /// 返回上一级目录
        /// </summary>
        /// <param dir="string">目录</param>
        /// <returns>返回String对象</returns>
        public string UpAndDownDir(string dir)
        {
            string Change_dir = Directory.GetParent(dir).FullName;
            return Change_dir;
        }
        #endregion

        /// <summary>
        /// 获取ftp文件ICO
        /// </summary>
        /// <param name="ftpip">ftp Ip地址</param>
        /// <param name="user">用户</param>
        /// <param name="pwd">密码</param>
        /// <param name="path">路径</param>
        /// <param name="il">ImageList对象</param>
        /// <param name="lv">ListView对象</param>
        public void GetFTPServerICO(string ftpip,string  user,string  pwd, string path ,ref ImageList il, ref ListView lv)//获取服务器的图标
        {
            try
            {
                string[] a;
                lv.Items.Clear();
                il.Images.Clear();
                if(path.Length==0)
                    a = GetFileList(ftpip, user, pwd);
                else
                    a= GetFileList(string.Format("{0}/{1}", ftpip, path.Remove(path.LastIndexOf("/"))), user, pwd);
                if (a != null)
                {
                    for (int i = 0; i < a.Length; i++)
                    {
                        
                        string[] b = a[i].ToString().Split(' ');
                        string filename = b[b.Length-1];
                        string filetype="";
                        if (a[i].IndexOf("DIR") != -1)
                        {
                            filetype = filename;
                        }
                        else
                        {
                            filetype = filename.Substring(filename.LastIndexOf("."), filename.Length - filename.LastIndexOf("."));
                        }
                        il.Images.Add(GetIconByFileType(filetype, true));
                        string[] info = new string[4];
                        FileInfo fi = new(filename);
                        info[0] = fi.Name;
                        info[1] = GetFileSize(filename, ftpip, user, pwd, path).ToString();
                        if (a[i].IndexOf("DIR") != -1)
                        {
                            info[2] = "";
                            info[1] = "文件夹";
                        }
                        else
                        {
                            info[2] = GetFileSize(filename, ftpip, user, pwd, path).ToString();
                            info[1] = fi.Extension.ToString();
                        }
                        ListViewItem item = new(info, i);
                        lv.Items.Add(item);
                    }
                }
            }
            catch(Exception){}
        }

        public void ListFolders(ToolStripComboBox tscb)//获取本地磁盘目录
        {
            string[] logicdrives = System.IO.Directory.GetLogicalDrives();
            for (int i = 0; i < logicdrives.Length; i++)
            {
                tscb.Items.Add(logicdrives[i]);
                tscb.SelectedIndex = 0;
            }
        }

        int k = 0;
        public void GoBack(ListView lv,ImageList il,string path)
        {

            if (AllPath.Length != 3)
            {
                string NewPath = AllPath.Remove(AllPath.LastIndexOf("\\")).Remove(AllPath.Remove(AllPath.LastIndexOf("\\")).LastIndexOf("\\")) + "\\";
                lv.Items.Clear();
                GetListViewItem(NewPath, il, lv);
                AllPath = NewPath;
            }
            else
            {
                if (k == 0)
                {
                    lv.Items.Clear();
                    GetListViewItem(path, il, lv);
                    k++;
                }
            }
        }
        public string Mpath()
        {
            string path=AllPath;
            return path;
        }

        private static string AllPath = "";//---------
        public void GetPath(string path, ImageList imglist, ListView lv,int ppath)//-------
        {
            if (ppath == 0)
                {
                    if (AllPath != path)
                    {
                        lv.Items.Clear();
                        AllPath = path;
                        GetListViewItem(AllPath, imglist, lv);
                    }
                }
                else
                {
                string uu = AllPath + path;
                if (Directory.Exists(uu))
                    {
                        AllPath = string.Format("{0}{1}\\", AllPath, path);
                    string pp = AllPath.Substring(0, AllPath.Length - 1);
                    lv.Items.Clear();
                        GetListViewItem(pp, imglist, lv);
                    }
                    else
                    {
                        uu = AllPath + path;
                        System.Diagnostics.Process.Start(uu);
                    }
                }
        }

        public void GetListViewItem(string path, ImageList imglist, ListView lv)//获取指定路径下所有文件及其图标
        {
            lv.Items.Clear();
            SHFILEINFO shfi = new();
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                for (int i = 0; i < dirs.Length; i++)
                {
                    string[] info = new string[4];
                    DirectoryInfo dir = new(dirs[i]);
                    if (dir.Name == "RECYCLER" || dir.Name == "RECYCLED" || dir.Name == "Recycled" || dir.Name == "System Volume Information")
                    { }
                    else
                    {
                        //获得图标
                        FTPWinAPI.SHGetFileInfo(dirs[i],
                                            (uint)0x80,
                                            ref shfi,
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                                            (uint)(0x100 | 0x400)); //取得Icon和TypeName
                        //添加图标
                        imglist.Images.Add(dir.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                        info[0] = dir.Name;
                        info[1] = "";
                        info[2] = "文件夹";
                        info[3] = dir.LastWriteTime.ToString();
                        ListViewItem item = new(info, dir.Name);
                        lv.Items.Add(item);
                        //销毁图标
                        FTPWinAPI.DestroyIcon(shfi.hIcon);
                    }
                }
                for (int i = 0; i < files.Length; i++)
                {
                    string[] info = new string[4];
                    FileInfo fi = new(files[i]);
                    string Filetype = fi.Name.Substring(fi.Name.LastIndexOf(".") + 1, fi.Name.Length - fi.Name.LastIndexOf(".") - 1);
                    string newtype = Filetype.ToLower();
                    if (newtype == "sys" || newtype == "ini" || newtype == "bin" || newtype == "log" || newtype == "com" || newtype == "bat" || newtype == "db")
                    { }
                    else
                    {


                        //获得图标
                        FTPWinAPI.SHGetFileInfo(files[i],
                                            (uint)0x80,
                                            ref shfi,
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                                            (uint)(0x100 | 0x400)); //取得Icon和TypeName
                        //添加图标
                        imglist.Images.Add(fi.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                        info[0] = fi.Name;
                        info[1] = fi.Length.ToString();
                        info[2] = fi.Extension.ToString();
                        info[3] = fi.LastWriteTime.ToString();
                        ListViewItem item = new(info, fi.Name);
                        lv.Items.Add(item);
                        //销毁图标
                        FTPWinAPI.DestroyIcon(shfi.hIcon);
                    }
                }
            }
            catch
            {
            }
        }

        FtpWebRequest reqFTP;
        /// <summary>
        /// 验证登录用户是否合法
        /// </summary>
        /// <param name="DomainName">Ftp访问路径</param>
        /// <param name="FtpUserName">登录用户名</param>
        /// <param name="FtpUserPwd">登录密码</param>
        /// <returns></returns>
        public bool CheckFtp(string DomainName, string FtpUserName, string FtpUserPwd)//验证登录用户是否合法
        {
            bool ResultValue = true;
            try
            {
                FtpWebRequest ftprequest = (FtpWebRequest)WebRequest.Create("ftp://" + DomainName);//创建FtpWebRequest对象
                ftprequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);//设置FTP登陆信息
                ftprequest.Method = WebRequestMethods.Ftp.ListDirectory;//发送一个请求命令
                FtpWebResponse ftpResponse = (FtpWebResponse)ftprequest.GetResponse();//响应一个请求
                ftpResponse.Close();//关闭请求
            }
            catch
            {
                ResultValue = false;
            }
            return ResultValue;
        }
        /// <summary>
        /// 获取文件大小
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="ftpserver">Ftp地址</param>
        /// <param name="ftpUserID">登录用户</param>
        /// <param name="ftpPassword">登录密码</param>
        /// <param name="path">访问路径</param>
        /// <returns>返回文件大小</returns>
        public long GetFileSize(string filename, string ftpserver,string ftpUserID, string ftpPassword,string path)
        {
            try
            {
                FileInfo fi = new(filename);
                string uri;
                if(path.Length==0)
                    uri = "ftp://" + ftpserver + "/" + fi.Name;
                else
                    uri = "ftp://" + ftpserver + "/" +path+ fi.Name;
                Connect(uri, ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long filesize = response.ContentLength;
                return filesize;
            }
            catch
            {
                return 0;
            }
        }

       
        /// <summary>
        /// 从ftp服务器上获得文件列表
        /// </summary>
        /// <param name="ftpServerIP">Ftp地址</param>
        /// <param name="ftpUserID">登录用户</param>
        /// <param name="ftpPassword">登录密码</param>
        /// <returns>返回文件列表数组</returns>
        public string[] GetFileList(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            string[] downloadFiles;
            StringBuilder result = new();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new(response.GetResponseStream(), Encoding.GetEncoding("GB2312"));
                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                    
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch
            {
                downloadFiles = null;
                return downloadFiles;
            }
        }
        /// <summary>
        /// 从ftp服务器上获得指定路径的文件列表
        /// </summary>
        /// <param name="ftpServerIP">Ftp地址</param>
        /// <param name="ftpUserID">登录用户</param>
        /// <param name="ftpPassword">登录密码</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="path">访问路径</param>
        /// <returns>返回文件列表数组</returns>
        public string[] GetFileListByPath(string ftpServerIP, string ftpUserID, string ftpPassword,string fileName,string path)//指定路径的文件列表
        {
            if (path == null)
                path = "";
            if (path.Length == 0)
            {
                string[] downloadFiles;
                StringBuilder result = new();
                FtpWebRequest reqFTP;
                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(string.Format("ftp://{0}/{1}", ftpServerIP, fileName)));
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                    reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                    WebResponse response = reqFTP.GetResponse();
                    StreamReader reader = new(response.GetResponseStream(), Encoding.GetEncoding("GB2312"));

                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        result.Append(line);
                        result.Append("\n");
                        line = reader.ReadLine();
                    }
                    result.Remove(result.ToString().LastIndexOf('\n'), 1);
                    reader.Close();
                    response.Close();
                    return result.ToString().Split('\n');
                }
                catch
                {
                    downloadFiles = null;
                    return downloadFiles;
                }
            }
            else
            {
                string[] downloadFiles;
                StringBuilder result = new();
                FtpWebRequest reqFTP;
                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + ftpServerIP + "/" +path+ fileName));
                    reqFTP.UseBinary = true;
                    reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                    reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                    WebResponse response = reqFTP.GetResponse();
                    StreamReader reader = new(response.GetResponseStream(), Encoding.GetEncoding("GB2312"));

                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        result.Append(line);
                        result.Append("\n");
                        line = reader.ReadLine();
                    }
                    result.Remove(result.ToString().LastIndexOf('\n'), 1);
                    reader.Close();
                    response.Close();
                    return result.ToString().Split('\n');
                }
                catch
                {
                    downloadFiles = null;
                    return downloadFiles;
                }
            }
        }

        //去除空格
        private string QCKG(string str)
        {
            string a = "";
            CharEnumerator CEnumerator = str.GetEnumerator();
            while (CEnumerator.MoveNext())
            {
                byte[] array = new byte[1];
                array = System.Text.Encoding.ASCII.GetBytes(CEnumerator.Current.ToString());
                int asciicode = (short)(array[0]);
                if (asciicode != 32)
                {
                    a += CEnumerator.Current.ToString();
                }
            }
            return a;
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filename">文件完全限定名或者相对文件名</param>
        /// <param name="ftpServerIP">Ftp地址</param>
        /// <param name="ftpUserID">登录用户</param>
        /// <param name="ftpPassword">登录密码</param>
        /// <param name="pb">进度栏控件</param>
        /// <param name="path">上传路径</param>
        /// <returns>true表示成功，否则失败</returns>
        public bool UploadFile(string filename, string ftpServerIP, string ftpUserID, string ftpPassword, ToolStripProgressBar pb,string path)
        {
            if (path == null)
                path = "";
            bool success = true;
            FileInfo fileInf = new(filename);
            int allbye = (int)fileInf.Length;
            int startbye = 0;
            pb.Maximum = allbye;
            pb.Minimum = 0;
            string newFileName;
            if (fileInf.Name.IndexOf("#") == -1)
            {
                newFileName =QCKG(fileInf.Name);
            }
            else
            {
                newFileName = fileInf.Name.Replace("#", "＃");
                newFileName = QCKG(newFileName);
            }
            string uri;
            if (path.Length == 0)
                uri = string.Format("ftp://{0}/{1}", ftpServerIP, newFileName);
            else
                uri = string.Format("ftp://{0}/{1}{2}", ftpServerIP, path, newFileName);
            FtpWebRequest reqFTP;
            // 根据uri创建FtpWebRequest对象 
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            // ftp用户名和密码 
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            // 默认为true，连接不会被关闭 
            // 在一个命令之后被执行 
            reqFTP.KeepAlive = false;
            // 指定执行什么命令 
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // 指定数据传输类型 
            reqFTP.UseBinary = true;
            // 上传文件时通知服务器文件的大小 
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;// 缓冲大小设置为2kb 
            byte[] buff = new byte[buffLength];
            // 打开一个文件流 (System.IO.FileStream) 去读上传的文件 
            FileStream fs= fileInf.OpenRead();
            try
            {
                // 把上传的文件写入流 
                Stream strm = reqFTP.GetRequestStream();
                // 每次读文件流的2kb 
                int contentLen = fs.Read(buff, 0, buffLength);
                // 流内容没有结束 
                while (contentLen != 0)
                {
                    // 把内容从file stream 写入 upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    startbye += contentLen;
                    pb.Value = startbye;
                }
                // 关闭两个流 
                strm.Close();
                fs.Close();
             }
             catch(Exception)
             {
                 success = false;
             }
             return success;
        }
        /// <summary>
        /// 连接FTP服务器
        /// </summary>
        /// <param name="path">Ftp地址</param>
        /// <param name="ftpUserID">登录用户</param>
        /// <param name="ftpPassword">登录密码</param>
        public void Connect(string path, string ftpUserID, string ftpPassword)//连接ftp
        {
            // 根据uri创建FtpWebRequest对象
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
            // 指定数据传输类型
            reqFTP.UseBinary = true;
            // ftp用户名和密码
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        }

        
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="ftpServerIP">Ftp地址</param>
        /// <param name="ftpUserID">登录用户</param>
        /// <param name="ftpPassword">登录密码</param>
        /// <param name="path">删除路径</param>
        public void DeleteFileName(string fileName, string ftpServerIP, string ftpUserID, string ftpPassword,string path)
        {
           try
           {
               string uri;
               if(path.Length==0)
                   uri= string.Format("ftp://{0}/{1}", ftpServerIP, fileName);
               else
                   uri = string.Format("ftp://{0}/{1}{2}", ftpServerIP, path, fileName);
               // 根据uri创建FtpWebRequest对象
               reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
               // 指定数据传输类型
               reqFTP.UseBinary = true;
               // ftp用户名和密码
               reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
               // 默认为true，连接不会被关闭
               // 在一个命令之后被执行
               reqFTP.KeepAlive = false;
               // 指定执行什么命令
               reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
               FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
               response.Close();
           }
           catch (Exception)
           {
             
           }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath">存储路径</param>
        /// <param name="fileName">文件名</param>
        /// <param name="ftpServerIP">Ftp地址</param>
        /// <param name="ftpUserID">登录用户</param>
        /// <param name="ftpPassword">登录密码</param>
        /// <param name="path">下载路径</param>
        /// <returns>返回true表示成功，否则失败</returns>
        public bool Download(string filePath, string fileName, string ftpServerIP, string ftpUserID, string ftpPassword,string path)
        {
            bool check = true;
            FtpWebRequest reqFTP;
            string uri;
            if (path.Length == 0)
                uri = string.Format("ftp://{0}/{1}", ftpServerIP, fileName);
            else
                uri = string.Format("ftp://{0}/{1}{2}", ftpServerIP, path, fileName);
            try
            {
                FileStream outputStream = new(string.Format("{0}\\{1}", filePath, fileName), FileMode.Create);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                ftpStream.Close();
                outputStream.Close();
                response.Close();
            }
            catch(Exception)
            {
                check = false;
            }
            return check;
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="dirName">目录名称</param>
        /// <param name="ftpServerIP">ftp地址</param>
        /// <param name="ftpUserID">登录用户</param>
        /// <param name="ftpPassword">登录密码</param>
        public void MakeDir(string dirName, string ftpServerIP,string ftpUserID, string ftpPassword)
        {
            try
            {
                string uri = string.Format("ftp://{0}/{1}", ftpServerIP, dirName);
                Connect(uri, ftpUserID, ftpPassword);//连接       
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch(Exception){}
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dirName">目录名称</param>
        /// <param name="ftpServerIP">ftp地址</param>
        /// <param name="ftpUserID">登录用户</param>
        /// <param name="ftpPassword">登录密码</param>
        public void DelDir(string dirName, string ftpServerIP, string ftpUserID, string ftpPassword)
        {
             try
             {
                 string uri = string.Format("ftp://{0}/{1}", ftpServerIP, dirName);
                 Connect(uri, ftpUserID, ftpPassword);//连接      
                 reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;//向服务器发送删除文件夹的命令
                 FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                 response.Close();
             }
             catch (Exception)
             {
             }
        }

        /// <summary>
        /// 获取指定路径的文件列表
        /// </summary>
        /// <param name="ftpServerIP">Ftp地址</param>
        /// <param name="ftpUserID">登录用户</param>
        /// <param name="ftpPassword">登录密码</param>
        /// <param name="path">访问路径</param>
        /// <returns>返回文件列表数组</returns>
        public string[] GetFTPList(string ftpServerIP, string ftpUserID, string ftpPassword, string path)//指定路径的文件列表
        {
            if (path == null)
            path = "";
            string[] downloadFiles;
            StringBuilder result = new();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(string.Format("ftp://{0}/{1}", ftpServerIP, path.Remove(path.LastIndexOf("/")))));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new(response.GetResponseStream(), Encoding.GetEncoding("GB2312"));

                string line = reader.ReadLine();
                while (line != null)
                {
                    result.Append(line);
                    result.Append("\n");
                    line = reader.ReadLine();
                }
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                return result.ToString().Split('\n');
            }
            catch
            {
                downloadFiles = null;
                return downloadFiles;
            }
        }
    }
}
