using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static SuperFramework.WindowsAPI.Kernel32API;
namespace SuperFramework.SuperFile
{
    /// <summary>
    /// 日 期:2014-09-22
    /// 作 者:不良帥
    /// 描 述:文件操作辅助类
    /// </summary>
    public static class FileHelper
    {
        #region 判断文件的编码格式

        /// <summary>
        /// 判断文件的编码类型
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>返回文件的编码类型</returns>
        public static Encoding GetType(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型。
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>返回文件的编码类型</returns>
        public static Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;

        }

        #endregion

        #region 判断是否是不带 BOM 的 UTF8 格式
        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data">字节</param>
        /// <returns>返回true为不带BOM的UTF8格式，false为带BOM的UTF8格式</returns>
        public static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                            charByteCounter++;
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                            return false;
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                        return false;
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }
        #endregion

        #region 获得不带非法字符的文件名
        /// <summary>
        /// 获得不带非法字符的文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetCorrectFileName(string fileName)
        {
            return fileName.Replace(@"\", "").Replace(@"/", "").Replace(@"?", "").Replace(@":", "").Replace(@"*", "")
                .Replace("\"", "").Replace(@">", "").Replace(@"<", "").Replace(@"|", "");
        }
        #endregion

        #region 获得合法文件名和路径
        /// <summary>
        /// 获得合法文件名和路径
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="path">文件路径</param>
        /// <returns>合法文件名称或者路径</returns>
        public static string[] GetFileNameOrPath(string fileName = "", string path = "")
        {
            StringBuilder rBuilder;
            List<string> name = new List<string>();
            if (!string.IsNullOrEmpty(fileName) && string.IsNullOrEmpty(path))
            {
                rBuilder = new StringBuilder(fileName);
                foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
                    rBuilder.Replace(rInvalidChar.ToString(), string.Empty);
                name.Add(rBuilder.ToString());
            }
            else if (string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(path))
            {
                rBuilder = new StringBuilder(path);
                foreach (char rInvalidChar in Path.GetInvalidPathChars())
                    rBuilder.Replace(rInvalidChar.ToString(), string.Empty);
                name.Add(rBuilder.ToString());
            }
            else if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(path))
            {
                rBuilder = new StringBuilder(fileName);
                foreach (char rInvalidChar in Path.GetInvalidFileNameChars())
                    rBuilder.Replace(rInvalidChar.ToString(), string.Empty);
                name.Add(rBuilder.ToString());

                rBuilder = new StringBuilder(path);
                foreach (char rInvalidChar in Path.GetInvalidPathChars())
                    rBuilder.Replace(rInvalidChar.ToString(), string.Empty);
                name.Add(rBuilder.ToString());
            }
            return name.ToArray();
        }
        #endregion

        #region 取得文件后缀名
        /****************************************
          * 函数名称：GetPostfixStr
          * 功能说明：取得文件后缀名
          * 参     数：filename:文件名称
          * 调用示列：
          *            string filename = "aaa.aspx";        
          *            string s = EC.FileObj.GetPostfixStr(filename);         
         *****************************************/
        /// <summary>
        /// 取后缀名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>返回文件后缀</returns>
        public static string GetPostfixStr(string filename)
        {
            int start = filename.LastIndexOf(".");
            int length = filename.Length;
            string postfix = filename.Substring(start, length - start);
            return postfix;
        }
        #endregion

        #region 创建预先占用一定磁盘空间的文件
        /// <summary>
        /// 创建预先占用一定磁盘空间的文件
        /// </summary>
        /// <param name="path">文件路径</param>
        public static void CreateNullFile(string path)
        {
            using (FileStream fs = File.Create(path))
            {
                long size = int.MaxValue;
                long offset = fs.Seek(10 * size - 1, SeekOrigin.Begin);
                fs.WriteByte(new byte());
            }
        }
        #endregion

        #region 提取文件名
        /// <summary>
        /// 提取文件名
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns>返回文件名</returns>
        public static string GetFileName(string FilePath,bool isGetExt=true)
        {
            if (isGetExt)
                return Path.GetFileName(FilePath);
            else
            {
                string s = Path.GetFileName(FilePath);
                return s.Substring(0, s.LastIndexOf(".")-1);
            }

        }

        #endregion

        #region 检查文件,如果文件不存在则创建。
        /// <summary>
        /// 检查文件,如果文件不存在则创建。
        /// </summary>
        /// <param name="FilePath">路径,包括文件名</param>
        public static void ExistsFile(string FilePath)
        {
            if (!File.Exists(FilePath))
            {
                FileStream fs = File.Create(FilePath);
                fs.Close();
            }
        }
        #endregion

        #region 获取文本文件的行数
        /// <summary>
        /// 获取文本文件的行数
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param> 
        /// <returns>返回文本行数</returns>
        public static int GetLineCount(string filePath)
        {
            //将文本文件的各行读到一个字符串数组中
            string[] rows = File.ReadAllLines(filePath);

            //返回行数
            return rows.Length;
        }
        #endregion

        #region 获取指定文件详细属性
        /****************************************
         * 函数名称：GetFileAttibe(string filePath)
         * 功能说明：获取指定文件详细属性
         * 参    数：filePath:文件详细路径
         * 调用示列：
         *           string file = Server.MapPath("robots.txt");  
         *            Response.Write(DotNet.Utilities.FileOperate.GetFileAttibe(file));         
        *****************************************/
        /// <summary>
        /// 获取指定文件详细属性
        /// </summary>
        /// <param name="filePath">文件详细路径</param>
        /// <param name="fileattribute">文件属性结构对象</param>
        /// <returns>返回属性字符串</returns>
        /// <exception cref="FileNotFoundException">文件不存在</exception>
        public static string GetFileAttibe(string filePath,ref FileInfoClass.FileAttribute fileattribute)
        {
            string str;
            try
            {
                if (IsExistFile(filePath))
                {
                    FileInfo objFI = new FileInfo(filePath);
                    str = string.Format("详细路径:{0}；文件名称:{1}；文件长度:{2}字节；创建时间{3}；最后访问时间:{4}；修改时间:{5}；所在目录:{6}；扩展名:{7}", objFI.FullName, objFI.Name, objFI.Length, objFI.CreationTime, objFI.LastAccessTime, objFI.LastWriteTime, objFI.DirectoryName, objFI.Extension);
                    fileattribute.CreationTime = objFI.CreationTime;
                    fileattribute.DirectoryName = objFI.DirectoryName;
                    fileattribute.Extension = objFI.Extension;
                    fileattribute.FullName = objFI.FullName;
                    fileattribute.LastAccessTime = objFI.LastAccessTime;
                    fileattribute.LastWriteTime = objFI.LastWriteTime;
                    fileattribute.Length = objFI.Length;
                    fileattribute.Name = objFI.Name;
                }
                else
                { return ""; }
            }
            catch (FileNotFoundException ex) { throw new FileNotFoundException(ex.Message); }
            return str;
        }
        #endregion

        #region 检测指定文件是否存在
        /// <summary>
        /// 检测指定文件是否存在
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
        #endregion

        #region 磁盘剩余空间计算
        /****************************************
          * 函数名称：DiskFreeSpace
          * 功能说明：磁盘剩余空间计算
          * 参    数：TargetDisk:目标驱动器
          * 调用示列：
          *            string TargetDisk = "C";
          *            string showStr=EC.FileObj.DiskFreeSpace(TargetDisk);
         *****************************************/
        /// <summary>
        /// 磁盘剩余空间计算
        /// </summary>
        /// <param name="targetDisk">目标驱动器</param>
        /// <param name="unit">使用大小单位</param>
        /// <returns>返回磁盘可用空间量</returns>
        public static double DiskFreeSpace(string targetDisk, FileInfoClass.SizeUnit unit)
        {
            if (!Directory.Exists(targetDisk))
                return 0;
            long x = new DriveInfo(targetDisk).AvailableFreeSpace;
            const long KB = 1024;
            const long MB = 1024 * KB;
            const long GB = 1024 * MB;
            const long TB = 1024 * GB;
            double showsize;
            switch (unit)
            {
                case FileInfoClass.SizeUnit.Bytes:
                    showsize = Convert.ToDouble(x);
                    break;
                case FileInfoClass.SizeUnit.KB:
                    showsize = Convert.ToDouble(((double)x / KB).ToString("#0.00"));
                    break;
                case FileInfoClass.SizeUnit.MB:
                    showsize = Convert.ToDouble(((double)x / MB).ToString("#0.0000"));
                    break;
                case FileInfoClass.SizeUnit.GB:
                    showsize = Convert.ToDouble(((double)x / GB).ToString("#0.0000"));
                    break;
                case FileInfoClass.SizeUnit.TB:
                    showsize = Convert.ToDouble(((double)x / TB).ToString("#0.0000"));
                    break;
                default:
                    showsize = 1;
                    break;
            }
            return showsize;
        }

        #endregion

        #region 将文件移动到指定目录
        /// <summary>
        /// 将文件移动到指定目录
        /// </summary>
        /// <param name="sourceFilePath">需要移动的源文件的绝对路径</param>
        /// <param name="descDirectoryPath">移动到的目录的绝对路径</param>
        /// <returns>成功返回true，异常返回false</returns>
        public static bool MoveFile(string sourceFilePath, string descDirectoryPath)
        {
            try
            {
                //获取源文件的名称
                string sourceFileName = GetFileName(sourceFilePath);
                if (DirOperate.IsExistDirectory(descDirectoryPath))
                {
                    //如果目标中存在同名文件,则删除
                    if (IsExistFile(string.Format("{0}\\{1}", descDirectoryPath, sourceFileName)))
                        File.Delete(string.Format("{0}\\{1}", descDirectoryPath, sourceFileName));
                    //将文件移动到指定目录
                    File.Move(sourceFilePath, string.Format("{0}\\{1}", descDirectoryPath, sourceFileName));
                }
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region 分割文件
        /// <summary>
        /// 分割文件
        /// </summary>
        /// <param name="openPath">源文件路径</param>
        /// <param name="savePath">保存文件路径</param>
        /// <param name="size">分割大小</param>
        /// <returns>成功返回true，失败返回flase</returns>
        public static bool SplitFile(string openPath, string savePath, int size)
        {
            try
            {
                using (FileStream fs = new FileStream(openPath, FileMode.Open))
                {
                    size *= 1024;
                    byte[] by = new byte[size];
                    int i = 0;
                    while (true)
                    {
                        i++;
                        int f = fs.Read(by, 0, by.Length);
                        if (f == 0)
                            break;
                        FileStream fsm = new FileStream(savePath + "\\" + i.ToString() + ".part", FileMode.Create);
                        fsm.Write(by, 0, f);
                        fsm.Flush();
                        fsm.Close();
                    }
                    return true;
                }
            }
            catch { return false; }
        }
        #endregion

        #region 合并文件
        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="openPath">源文件夹路径</param>
        /// <param name="savePath">保存文件路径</param>
        /// <returns>成功返回true，失败返回flase</returns>
        public static bool MergeFile(string openPath, string savePath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(openPath);
                FileInfo[] files = dir.GetFiles();
                using (FileStream fs = new FileStream(savePath, FileMode.Create))
                {
                    foreach (FileInfo item in files)
                    {
                        using (FileStream f = new FileStream(item.FullName, FileMode.Open))
                        {
                            byte[] by = new byte[f.Length];
                            int x = f.Read(by, 0, by.Length);
                            fs.Write(by, 0, x);
                            fs.Flush();
                        }
                    }
                    return true;
                }
            }
            catch { return false; }
        }
        #endregion

        #region 取得一个文件全名
        /// <summary>
        /// 获取一个文件的全名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>返回生成文件的完整路径名</returns>
        public static string GetFullName(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);
            return fi.FullName;
        }
        #endregion

        #region 批量重命名文件名称
        /// <summary>
        /// 批量重命名文件夹下的指定文件
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <param name="fileName">新文件名称</param>
        /// <param name="newFixStr">新文件后缀</param>
        /// <param name="oldFixStr">指定文件后缀</param>
        /// <param name="isNextDirectory">是否递归下一级目录</param>
        /// <returns>返回true为成功修改，返回false修改出现异常</returns>
        public static bool BatchRename(string dirPath, string fileName, string newFixStr = null, string oldFixStr = "*.*", bool isNextDirectory = false)
        {
            bool b = true;
            try
            {
                if (!oldFixStr.Contains("."))
                    oldFixStr = "." + oldFixStr;
                if (oldFixStr.Substring(0, 1) != "*")
                    oldFixStr = "*" + oldFixStr;
                if (!newFixStr.Contains("."))
                    newFixStr = "." + newFixStr;
                DirectoryInfo di = new DirectoryInfo(dirPath);
                FileInfo[] filelist = di.GetFiles(oldFixStr);
                string strFileFolder = dirPath;
                int i = 0;
                int TotalFiles = 1;
                foreach (FileInfo fi in filelist)
                {
                    if (string.IsNullOrWhiteSpace(newFixStr))
                        newFixStr = "." + fi.Extension;
                    string strNewFileName = fileName + TotalFiles + newFixStr;
                    string strNewFilePath = @strFileFolder + "\\" + strNewFileName;
                    filelist[i].MoveTo(@strNewFilePath);
                    TotalFiles += 1;
                    i += 1;
                }
                if (isNextDirectory)
                {
                    DirectoryInfo[] fs = di.GetDirectories();
                    foreach (DirectoryInfo item in fs)
                    {
                        try
                        {
                            BatchRename(item.FullName, fileName, newFixStr, oldFixStr, isNextDirectory);
                        }
                        catch { continue; }
                    }
                }
            }
            catch
            {
                b = false;
            }
            return b;
        }
        #endregion

        #region 批量重命名文件夹下的所有文件后缀
        /// <summary>
        /// 批量重命名文件夹下的所有文件后缀
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <param name="newFixStr">新文件后缀</param>
        /// <param name="oldFixStr">旧的后缀，*.*代表所有文件</param>
        /// <param name="isNextDirectory">是否递归下一级目录</param>
        /// <returns>返回true为成功修改，返回false修改出现异常</returns>
        public static bool BatchReFix(string dirPath, string newFixStr, string oldFixStr = "*.*", bool isNextDirectory = false)
        {
            bool b = true;
            try
            {
                if (newFixStr == "")
                    return false;
                if (!oldFixStr.Contains("."))
                    oldFixStr = "." + oldFixStr;
                if (oldFixStr.Substring(0, 1) != "*")
                    oldFixStr = "*" + oldFixStr;
                if (!newFixStr.Contains("."))
                    newFixStr = "." + newFixStr;
                DirectoryInfo di = new DirectoryInfo(dirPath);
                FileInfo[] filelist = di.GetFiles(oldFixStr);
                string strFileFolder = dirPath;
                int i = 0;

                foreach (FileInfo fi in filelist)
                {

                    string strNewFileName = fi.Name.Substring(0, fi.Name.LastIndexOf(".")) + newFixStr;
                    string strNewFilePath = string.Format("{0}\\{1}", @strFileFolder, strNewFileName);
                    filelist[i].MoveTo(@strNewFilePath);

                }
                if (isNextDirectory)
                {
                    DirectoryInfo[] fs = di.GetDirectories();
                    foreach (DirectoryInfo item in fs)
                    {
                        try
                        {
                            BatchReFix(item.FullName, newFixStr, oldFixStr, isNextDirectory);
                        }
                        catch { continue; }
                    }
                }
            }
            catch
            {
                b = false;
            }
            return b;
        }
        #endregion

        #region 转换文件大小

        /// <summary>
        /// 转换文件大小 
        /// </summary>
        /// <param name="Size">初始文件大小,Size为字节大小</param>
        /// <remarks>保留两位小数</remarks>
        /// <returns>返回文件大小和单位</returns>
        public static string CountSize(long Size)
        {
            string m_strSize = "";
            long FactSize = Size;
            if (FactSize < 1024.00)
                m_strSize = FactSize.ToString("F2") + " Byte";
            else if (FactSize >= 1024.00 && FactSize < 1048576)
                m_strSize = (FactSize / 1024.00).ToString("F2") + " KB";
            else if (FactSize >= 1048576 && FactSize < 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00).ToString("F2") + " MB";
            else if (FactSize >= 1073741824)
                m_strSize = (FactSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " GB";
            else if (FactSize >= 1099511627776)
                m_strSize = (FactSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " TB";
            return m_strSize;
        }
        #endregion

        #region 获取一个文件的长度
        /// <summary>
        /// 获取一个文件的长度,单位为Byte
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>    
        /// <returns>返回文本Byte长度</returns>
        static long GetFileSize(string filePath)
        {
            long size = 0;
            if (File.Exists(filePath))
                size = new FileInfo(filePath).Length;//创建一个文件对象
            //获取文件的大小
            return size;
        }
        
        /// <summary>
        /// 获取一批文件的大小
        /// </summary>
        /// <param name="sFilePath">文件所在的路径</param>
        /// <param name="sMask">文件名称含通配符</param>
        /// <returns></returns>
        public static double GetFilesSize(string sFilePath, string sMask)
        {
            double lSize = 0;
            if (sMask.Trim() == "")
                return lSize;
            DirectoryInfo pDirectoryInfo = new DirectoryInfo(sFilePath);
            if (pDirectoryInfo.Exists == false)
                return lSize;
            FileInfo[] pFileInfos = pDirectoryInfo.GetFiles(sMask, SearchOption.TopDirectoryOnly);
            foreach (FileInfo e in pFileInfos)
            {
                lSize += GetFileSize(e.FullName);
            }
            return lSize;
        }
        #endregion

        #region 检测文件被占用
        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public static readonly IntPtr HFILE_ERROR = new IntPtr(-1);
        /// <summary>
        /// 检测文件被占用
        /// </summary>
        /// <param name="FileNames">要检测的文件路径</param>
        /// <returns>返回 false 表示被占用</returns>
        public static bool CheckFiles(string FileNames)
        {
            if (!File.Exists(FileNames))
            {
                //文件不存在
                return true;
            }
            IntPtr vHandle = _lopen(FileNames, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (vHandle == HFILE_ERROR)
            {
                //文件被占用
                return false;
            }
            //文件没被占用
            CloseHandle(vHandle);
            return true;
        }
        #endregion
    }
}
