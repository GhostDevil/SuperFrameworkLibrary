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
    /// ����:2015-05-12
    /// ����:������
    /// ����:FTP���������࣬��������Ϣ��
    /// </summary>
    class FTPBase
    {

        #region  ��ȡ������ͼ��
        /// <summary>
        /// ��ȡ�ļ�ͼ��
        /// </summary>
        /// <param name="fileType">�ļ���׺.*</param>
        /// <param name="isLarge"></param>
        /// <returns>�����ļ���չ����.*����������Ӧͼ��,������"."��ͷ�򷵻��ļ��е�ͼ�ꡣ</returns>
        public Icon GetIconByFileType(string fileType,bool isLarge)
        {
            if(fileType == null || fileType.Equals(string.Empty)) return null;
            string regIconString = null;
            string systemDirectory = Environment.SystemDirectory + "\\";
            if(fileType[0] == '.')
            {
                //��ϵͳע������ļ�������Ϣ
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
                    //û�ж�ȡ���ļ�����ע����Ϣ��ָ��Ϊδ֪�ļ����͵�ͼ��
                    regIconString = systemDirectory +"shell32.dll,0";
                }
            }
            else
            {
                //ֱ��ָ��Ϊ�ļ���ͼ��
                regIconString = systemDirectory +"shell32.dll,3";
            }
            string[] fileIcon = regIconString.Split(new char[]{','});
            if(fileIcon.Length != 2)
            {
                //ϵͳע�����ע��ı�ͼ����ֱ����ȡ���򷵻ؿ�ִ���ļ���ͨ��ͼ��
                fileIcon = new string[]{systemDirectory +"shell32.dll","2"};
            }
            Icon resultIcon;
            try
            {
                //����API������ȡͼ��
                int[] phiconLarge = new int[1];
                int[] phiconSmall = new int[1];
                uint count = FTPWinAPI.ExtractIconEx(fileIcon[0], int.Parse(fileIcon[1]),phiconLarge,phiconSmall,1);
                IntPtr IconHnd = new(isLarge?phiconLarge[0]:phiconSmall[0]);
                resultIcon = Icon.FromHandle(IconHnd);
            }

            catch
            {
                fileIcon = new string[] { systemDirectory + "shell32.dll", "2" };
                                //����API������ȡͼ��
                int[] phiconLarge = new int[1];
                int[] phiconSmall = new int[1];
                uint count = FTPWinAPI.ExtractIconEx(fileIcon[0], int.Parse(fileIcon[1]),phiconLarge,phiconSmall,1);
                IntPtr IconHnd = new(isLarge?phiconLarge[0]:phiconSmall[0]);
                resultIcon = Icon.FromHandle(IconHnd);
            }
            return resultIcon;
        }
        #endregion

        #region  �ļ��еĸ���
        /// <summary>
        /// �ļ��еĸ���
        /// </summary>
        /// <param Ddir="string">Ҫ���Ƶ�Ŀ��·��</param>
        /// <param Sdir="string">Ҫ���Ƶ�ԭ·��</param>
        public void FilesCopy(string Ddir, string Sdir)
        {
            DirectoryInfo dir = new(Sdir);
            try
            {
                if (!dir.Exists)//�ж���ָ���ļ����ļ����Ƿ����
                {
                    return;
                }
                DirectoryInfo dirD = dir as DirectoryInfo;//����������������ļ������˳�
                string UpDir = UpAndDownDir(Ddir);
                if (dirD == null)//�ж��ļ����Ƿ�Ϊ��
                {
                    Directory.CreateDirectory(string.Format("{0}\\{1}", UpDir, dirD.Name));//���Ϊ�գ������ļ��в��˳�
                    return;
                }
                else
                {
                    Directory.CreateDirectory(string.Format("{0}\\{1}", UpDir, dirD.Name));
                }
                string SbuDir = string.Format("{0}\\{1}\\", UpDir, dirD.Name);
                FileSystemInfo[] files = dirD.GetFileSystemInfos();//��ȡ�ļ����������ļ����ļ���
                //�Ե���FileSystemInfo�����ж�,������ļ�������еݹ����
                foreach (FileSystemInfo FSys in files)
                {
                    FileInfo file = FSys as FileInfo;
                    if (file != null)//������ļ��Ļ��������ļ��ĸ��Ʋ���
                    {
                        FileInfo SFInfo = new(string.Format("{0}\\{1}", file.DirectoryName, file.Name));//��ȡ�ļ����ڵ�ԭʼ·��
                        SFInfo.CopyTo(string.Format("{0}\\{1}", SbuDir, file.Name), true);//���ļ����Ƶ�ָ����·����
                    }
                    else
                    {
                        //string pp = FSys.Name;//��ȡ��ǰ���������ļ�������
                        FilesCopy(SbuDir + FSys, string.Format("{0}\\{1}", Sdir, FSys));//������ļ�������еݹ����
                    }
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region  ������һ��Ŀ¼
        /// <summary>
        /// ������һ��Ŀ¼
        /// </summary>
        /// <param dir="string">Ŀ¼</param>
        /// <returns>����String����</returns>
        public string UpAndDownDir(string dir)
        {
            string Change_dir = Directory.GetParent(dir).FullName;
            return Change_dir;
        }
        #endregion

        /// <summary>
        /// ��ȡftp�ļ�ICO
        /// </summary>
        /// <param name="ftpip">ftp Ip��ַ</param>
        /// <param name="user">�û�</param>
        /// <param name="pwd">����</param>
        /// <param name="path">·��</param>
        /// <param name="il">ImageList����</param>
        /// <param name="lv">ListView����</param>
        public void GetFTPServerICO(string ftpip,string  user,string  pwd, string path ,ref ImageList il, ref ListView lv)//��ȡ��������ͼ��
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
                            info[1] = "�ļ���";
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

        public void ListFolders(ToolStripComboBox tscb)//��ȡ���ش���Ŀ¼
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

        public void GetListViewItem(string path, ImageList imglist, ListView lv)//��ȡָ��·���������ļ�����ͼ��
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
                        //���ͼ��
                        FTPWinAPI.SHGetFileInfo(dirs[i],
                                            (uint)0x80,
                                            ref shfi,
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                                            (uint)(0x100 | 0x400)); //ȡ��Icon��TypeName
                        //���ͼ��
                        imglist.Images.Add(dir.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                        info[0] = dir.Name;
                        info[1] = "";
                        info[2] = "�ļ���";
                        info[3] = dir.LastWriteTime.ToString();
                        ListViewItem item = new(info, dir.Name);
                        lv.Items.Add(item);
                        //����ͼ��
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


                        //���ͼ��
                        FTPWinAPI.SHGetFileInfo(files[i],
                                            (uint)0x80,
                                            ref shfi,
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                                            (uint)(0x100 | 0x400)); //ȡ��Icon��TypeName
                        //���ͼ��
                        imglist.Images.Add(fi.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                        info[0] = fi.Name;
                        info[1] = fi.Length.ToString();
                        info[2] = fi.Extension.ToString();
                        info[3] = fi.LastWriteTime.ToString();
                        ListViewItem item = new(info, fi.Name);
                        lv.Items.Add(item);
                        //����ͼ��
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
        /// ��֤��¼�û��Ƿ�Ϸ�
        /// </summary>
        /// <param name="DomainName">Ftp����·��</param>
        /// <param name="FtpUserName">��¼�û���</param>
        /// <param name="FtpUserPwd">��¼����</param>
        /// <returns></returns>
        public bool CheckFtp(string DomainName, string FtpUserName, string FtpUserPwd)//��֤��¼�û��Ƿ�Ϸ�
        {
            bool ResultValue = true;
            try
            {
                FtpWebRequest ftprequest = (FtpWebRequest)WebRequest.Create("ftp://" + DomainName);//����FtpWebRequest����
                ftprequest.Credentials = new NetworkCredential(FtpUserName, FtpUserPwd);//����FTP��½��Ϣ
                ftprequest.Method = WebRequestMethods.Ftp.ListDirectory;//����һ����������
                FtpWebResponse ftpResponse = (FtpWebResponse)ftprequest.GetResponse();//��Ӧһ������
                ftpResponse.Close();//�ر�����
            }
            catch
            {
                ResultValue = false;
            }
            return ResultValue;
        }
        /// <summary>
        /// ��ȡ�ļ���С
        /// </summary>
        /// <param name="filename">�ļ���</param>
        /// <param name="ftpserver">Ftp��ַ</param>
        /// <param name="ftpUserID">��¼�û�</param>
        /// <param name="ftpPassword">��¼����</param>
        /// <param name="path">����·��</param>
        /// <returns>�����ļ���С</returns>
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
        /// ��ftp�������ϻ���ļ��б�
        /// </summary>
        /// <param name="ftpServerIP">Ftp��ַ</param>
        /// <param name="ftpUserID">��¼�û�</param>
        /// <param name="ftpPassword">��¼����</param>
        /// <returns>�����ļ��б�����</returns>
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
        /// ��ftp�������ϻ��ָ��·�����ļ��б�
        /// </summary>
        /// <param name="ftpServerIP">Ftp��ַ</param>
        /// <param name="ftpUserID">��¼�û�</param>
        /// <param name="ftpPassword">��¼����</param>
        /// <param name="fileName">�ļ�����</param>
        /// <param name="path">����·��</param>
        /// <returns>�����ļ��б�����</returns>
        public string[] GetFileListByPath(string ftpServerIP, string ftpUserID, string ftpPassword,string fileName,string path)//ָ��·�����ļ��б�
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

        //ȥ���ո�
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
        /// �ϴ��ļ�
        /// </summary>
        /// <param name="filename">�ļ���ȫ�޶�����������ļ���</param>
        /// <param name="ftpServerIP">Ftp��ַ</param>
        /// <param name="ftpUserID">��¼�û�</param>
        /// <param name="ftpPassword">��¼����</param>
        /// <param name="pb">�������ؼ�</param>
        /// <param name="path">�ϴ�·��</param>
        /// <returns>true��ʾ�ɹ�������ʧ��</returns>
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
                newFileName = fileInf.Name.Replace("#", "��");
                newFileName = QCKG(newFileName);
            }
            string uri;
            if (path.Length == 0)
                uri = string.Format("ftp://{0}/{1}", ftpServerIP, newFileName);
            else
                uri = string.Format("ftp://{0}/{1}{2}", ftpServerIP, path, newFileName);
            FtpWebRequest reqFTP;
            // ����uri����FtpWebRequest���� 
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
            // ftp�û��������� 
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            // Ĭ��Ϊtrue�����Ӳ��ᱻ�ر� 
            // ��һ������֮��ִ�� 
            reqFTP.KeepAlive = false;
            // ָ��ִ��ʲô���� 
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            // ָ�����ݴ������� 
            reqFTP.UseBinary = true;
            // �ϴ��ļ�ʱ֪ͨ�������ļ��Ĵ�С 
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;// �����С����Ϊ2kb 
            byte[] buff = new byte[buffLength];
            // ��һ���ļ��� (System.IO.FileStream) ȥ���ϴ����ļ� 
            FileStream fs= fileInf.OpenRead();
            try
            {
                // ���ϴ����ļ�д���� 
                Stream strm = reqFTP.GetRequestStream();
                // ÿ�ζ��ļ�����2kb 
                int contentLen = fs.Read(buff, 0, buffLength);
                // ������û�н��� 
                while (contentLen != 0)
                {
                    // �����ݴ�file stream д�� upload stream 
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                    startbye += contentLen;
                    pb.Value = startbye;
                }
                // �ر������� 
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
        /// ����FTP������
        /// </summary>
        /// <param name="path">Ftp��ַ</param>
        /// <param name="ftpUserID">��¼�û�</param>
        /// <param name="ftpPassword">��¼����</param>
        public void Connect(string path, string ftpUserID, string ftpPassword)//����ftp
        {
            // ����uri����FtpWebRequest����
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(path));
            // ָ�����ݴ�������
            reqFTP.UseBinary = true;
            // ftp�û���������
            reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
        }

        
        /// <summary>
        /// ɾ���ļ�
        /// </summary>
        /// <param name="fileName">�ļ�����</param>
        /// <param name="ftpServerIP">Ftp��ַ</param>
        /// <param name="ftpUserID">��¼�û�</param>
        /// <param name="ftpPassword">��¼����</param>
        /// <param name="path">ɾ��·��</param>
        public void DeleteFileName(string fileName, string ftpServerIP, string ftpUserID, string ftpPassword,string path)
        {
           try
           {
               string uri;
               if(path.Length==0)
                   uri= string.Format("ftp://{0}/{1}", ftpServerIP, fileName);
               else
                   uri = string.Format("ftp://{0}/{1}{2}", ftpServerIP, path, fileName);
               // ����uri����FtpWebRequest����
               reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));
               // ָ�����ݴ�������
               reqFTP.UseBinary = true;
               // ftp�û���������
               reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
               // Ĭ��Ϊtrue�����Ӳ��ᱻ�ر�
               // ��һ������֮��ִ��
               reqFTP.KeepAlive = false;
               // ָ��ִ��ʲô����
               reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;
               FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
               response.Close();
           }
           catch (Exception)
           {
             
           }
        }

        /// <summary>
        /// �����ļ�
        /// </summary>
        /// <param name="filePath">�洢·��</param>
        /// <param name="fileName">�ļ���</param>
        /// <param name="ftpServerIP">Ftp��ַ</param>
        /// <param name="ftpUserID">��¼�û�</param>
        /// <param name="ftpPassword">��¼����</param>
        /// <param name="path">����·��</param>
        /// <returns>����true��ʾ�ɹ�������ʧ��</returns>
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
        /// ����Ŀ¼
        /// </summary>
        /// <param name="dirName">Ŀ¼����</param>
        /// <param name="ftpServerIP">ftp��ַ</param>
        /// <param name="ftpUserID">��¼�û�</param>
        /// <param name="ftpPassword">��¼����</param>
        public void MakeDir(string dirName, string ftpServerIP,string ftpUserID, string ftpPassword)
        {
            try
            {
                string uri = string.Format("ftp://{0}/{1}", ftpServerIP, dirName);
                Connect(uri, ftpUserID, ftpPassword);//����       
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                response.Close();
            }
            catch(Exception){}
        }

        /// <summary>
        /// ɾ��Ŀ¼
        /// </summary>
        /// <param name="dirName">Ŀ¼����</param>
        /// <param name="ftpServerIP">ftp��ַ</param>
        /// <param name="ftpUserID">��¼�û�</param>
        /// <param name="ftpPassword">��¼����</param>
        public void DelDir(string dirName, string ftpServerIP, string ftpUserID, string ftpPassword)
        {
             try
             {
                 string uri = string.Format("ftp://{0}/{1}", ftpServerIP, dirName);
                 Connect(uri, ftpUserID, ftpPassword);//����      
                 reqFTP.Method = WebRequestMethods.Ftp.RemoveDirectory;//�����������ɾ���ļ��е�����
                 FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                 response.Close();
             }
             catch (Exception)
             {
             }
        }

        /// <summary>
        /// ��ȡָ��·�����ļ��б�
        /// </summary>
        /// <param name="ftpServerIP">Ftp��ַ</param>
        /// <param name="ftpUserID">��¼�û�</param>
        /// <param name="ftpPassword">��¼����</param>
        /// <param name="path">����·��</param>
        /// <returns>�����ļ��б�����</returns>
        public string[] GetFTPList(string ftpServerIP, string ftpUserID, string ftpPassword, string path)//ָ��·�����ļ��б�
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
