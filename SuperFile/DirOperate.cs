using System;
using System.Collections.Generic;
using System.IO;

namespace SuperFramework.SuperFile
{
    /// <summary>
    /// 日 期:2015-06-24
    /// 作 者:不良帥
    /// 描 述:目录操作辅助类
    /// </summary>
    public static class DirOperate
    {
        #region 获取指定目录及子目录中所有文件列表
        /// <summary>
        /// 获取指定目录及子目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>  
        /// <returns>返回文件名称列表数组</returns>  
        /// <exception cref="DirectoryNotFoundException">目录不存在</exception>    
        public static string[] GetFileNames(string directoryPath)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new DirectoryNotFoundException();
            }

            //获取文件列表
            return Directory.GetFiles(directoryPath);
        }
        /// <summary>
        /// 获取指定目录及子目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        /// <exception cref="DirectoryNotFoundException">目录不存在</exception>
        /// <exception cref="IOException">I/O发生错误</exception>
        /// <returns>返回文件路径数组</returns>
        public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild = true)
        {
            //如果目录不存在，则抛出异常
            if (!IsExistDirectory(directoryPath))
            {
                throw new DirectoryNotFoundException();
            }

            try
            {
                if (isSearchChild)
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw;
            }
        }
        #endregion

        #region 计算文件夹大小所用私有方法
        private static long FolderFileSize(string path)
        {
            long size = 0;
            try
            {
                FileInfo[] files = (new DirectoryInfo(path)).GetFiles();
                foreach (FileInfo file in files)
                {
                    size += file.Length;
                }
            }
            catch
            {
                throw;
            }
            return size;
        }
        /// <summary>
        /// 计算目录下总文件大小
        /// </summary>
        /// <param name="path">文件夹路径</param>
        /// <returns>文件夹大小</returns>
        private static long FolderSize(string path)
        {
            long Fsize;
            try
            {
                Fsize = FolderFileSize(path);
                DirectoryInfo[] folders = (new DirectoryInfo(path)).GetDirectories();
                foreach (DirectoryInfo folder in folders)
                    Fsize += FolderSize(folder.FullName);
            }
            catch
            {
                throw;
            }
            return Fsize;
        }
        #endregion

        #region 计算文件夹的大小
        /****************************************
          * 函数名称：FolderSize
          * 功能说明：计算文件夹的大小
          * 参    数：TargetFolder:目标文件夹
          * 调用示列：
          *            string TargetFolder = "C:\\Temp";
          *            string showStr=EC.FileObj.FolderSize(TargetFolder);
         *****************************************/
        /// <summary>
        /// 计算文件夹的大小
        /// </summary>
        /// <param name="TargetFolder">目标文件夹</param>
        /// <param name="unit">使用大小单位</param>
        /// <returns>返回文件夹大小</returns>
        public static double GetFolderSize(string TargetFolder, FileInfoClass.SizeUnit unit)
        {
            const double KB = 1024;
            const double MB = 1024 * KB;
            const double GB = 1024 * MB;
            const double TB = 1024 * GB;
            const double PB = 1024 * TB;
            const double EB = 1024 * PB;
            const double ZB = 1024 * EB;
            const double YB = 1024 * ZB;
            const double BB = 1024 * YB;
            const double NB = 1024 * BB;
            const double DB = 1024 * NB;
            long filesize = FolderSize(TargetFolder);
            double showsize;
            switch (unit)
            {
                case FileInfoClass.SizeUnit.Bytes:
                    showsize = Convert.ToDouble(filesize);
                    break;
                case FileInfoClass.SizeUnit.KB:
                    showsize = Convert.ToDouble(((double)filesize / KB).ToString("#0.00"));
                    break;
                case FileInfoClass.SizeUnit.MB:
                    showsize = Convert.ToDouble(((double)filesize / MB).ToString("#0.0000"));
                    break;
                case FileInfoClass.SizeUnit.GB:
                    showsize = Convert.ToDouble(((double)filesize / GB).ToString("#0.0000"));
                    break;
                case FileInfoClass.SizeUnit.TB:
                    showsize = Convert.ToDouble(((double)filesize / TB).ToString("#0.0000"));
                    break;
                case FileInfoClass.SizeUnit.PB:
                    showsize = Convert.ToDouble(((double)filesize / PB).ToString("#0.0000"));
                    break;
                case FileInfoClass.SizeUnit.EB:
                    showsize = Convert.ToDouble(((double)filesize / EB).ToString("#0.0000"));
                    break;
                case FileInfoClass.SizeUnit.ZB:
                    showsize = Convert.ToDouble(((double)filesize / ZB).ToString("#0.0000"));
                    break;
                case FileInfoClass.SizeUnit.YB:
                    showsize = Convert.ToDouble(((double)filesize / YB).ToString("#0.0000"));
                    break;
                case FileInfoClass.SizeUnit.BB:
                    showsize = Convert.ToDouble(((double)filesize / BB).ToString("#0.0000"));
                    break;
                case FileInfoClass.SizeUnit.NB:
                    showsize = Convert.ToDouble(((double)filesize / NB).ToString("#0.0000"));
                    break;
                case FileInfoClass.SizeUnit.DB:
                    showsize = Convert.ToDouble(((double)filesize / DB).ToString("#0.0000"));
                    break;
                default:
                    showsize = 1;
                    break;
            }
            return showsize;
        }

        #endregion

        #region 以一个文件夹的框架在另一个目录创建文件夹和空文件
        /****************************************
          * 函数名称：FolderBuild
          * 功能说明：创建文件夹框架
          * 参    数：OrignFolder:源路径,NewFolder:目标路径
          * 调用示列：
          *            string orignFolder = "C:\\Temp";     
          *            string NewFolder = "D:\\";
          *            EC.FileObj.FolderBuild(orignFolder, NewFolder);
         *****************************************/
        /// <summary>
        /// 以一个文件夹的框架在另一个目录创建文件夹和空文件
        /// </summary>
        /// <param name="orignFolder">源路径</param>
        /// <param name="NewFolder">目标路径</param>
        /// <returns>成功返回true，异常返回false</returns>
        public static bool FolderBuild(string orignFolder, string NewFolder)
        {
            try
            {
                bool b = false;
                string path = (NewFolder.LastIndexOf("\\") == NewFolder.Length - 1) ? NewFolder : NewFolder + "\\";
                string parent = Path.GetDirectoryName(orignFolder);
                Directory.CreateDirectory(path + Path.GetFileName(orignFolder));
                DirectoryInfo dir = new DirectoryInfo((orignFolder.LastIndexOf("\\") == orignFolder.Length - 1) ? orignFolder : orignFolder + "\\");
                FileSystemInfo[] fileArr = dir.GetFileSystemInfos();
                Queue<FileSystemInfo> Folders = new Queue<FileSystemInfo>(dir.GetFileSystemInfos());
                while (Folders.Count > 0)
                {
                    FileSystemInfo tmp = Folders.Dequeue();
                    FileInfo f = tmp as FileInfo;
                    if (f == null)
                    {
                        DirectoryInfo d = tmp as DirectoryInfo;
                        Directory.CreateDirectory(d.FullName.Replace((parent.LastIndexOf("\\") == parent.Length - 1) ? parent : parent + "\\", path));
                        foreach (FileSystemInfo fi in d.GetFileSystemInfos())
                        {
                            Folders.Enqueue(fi);
                        }
                    }
                    else
                    {
                        if (b) File.Create(f.FullName.Replace(parent, path));
                    }
                }
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region 拷贝文件夹
        /****************************************
          * 函数名称：FolderCoppy
          * 功能说明：拷贝文件夹
          * 参    数：OrignFolder:源路径,NewFolder:目标路径
          * 调用示列：
          *            string orignFolder = "C:\\Temp";     
          *            string NewFolder = "D:\\";
          *            EC.FileObj.FolderCoppy(orignFolder, NewFolder);
         *****************************************/
        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="orignFolder">源路径</param>
        /// <param name="newFolder">目标路径</param>
        public static void FolderCoppy(string orignFolder, string newFolder)
        {
            string path = (newFolder.LastIndexOf("\\") == newFolder.Length - 1) ? newFolder : newFolder + "\\";
            string parent = Path.GetDirectoryName(orignFolder);
            Directory.CreateDirectory(path + Path.GetFileName(orignFolder));
            DirectoryInfo dir = new DirectoryInfo((orignFolder.LastIndexOf("\\") == orignFolder.Length - 1) ? orignFolder : orignFolder + "\\");
            FileSystemInfo[] fileArr = dir.GetFileSystemInfos();
            Queue<FileSystemInfo> Folders = new Queue<FileSystemInfo>(dir.GetFileSystemInfos());
            while (Folders.Count > 0)
            {
                FileSystemInfo tmp = Folders.Dequeue();
                FileInfo f = tmp as FileInfo;
                if (f == null)
                {
                    DirectoryInfo d = tmp as DirectoryInfo;
                    Directory.CreateDirectory(d.FullName.Replace((parent.LastIndexOf("\\") == parent.Length - 1) ? parent : parent + "\\", path));
                    foreach (FileSystemInfo fi in d.GetFileSystemInfos())
                    {
                        Folders.Enqueue(fi);
                    }
                }
                else
                {
                    f.CopyTo(f.FullName.Replace(parent, path));
                }
            }
        }

        #endregion

        #region 移动文件夹
        /****************************************
          * 函数名称：FolderMove
          * 功能说明：拷贝文件夹
          * 参    数：OrignFolder:源路径,NewFolder:目标路径
          * 调用示列：
          *            string orignFolder = "C:\\Temp";     
          *            string NewFolder = "D:\\";
          *            EC.FileObj.FolderCoppy(orignFolder, NewFolder);
         *****************************************/
        /// <summary>
        /// 移动文件夹
        /// </summary>
        /// <param name="orignFolder">源路径</param>
        /// <param name="NewFolder">目标路径</param>
        public static void FolderMove(string orignFolder, string NewFolder)
        {
            string filename = Path.GetFileName(orignFolder);
            string path = (NewFolder.LastIndexOf("\\") == NewFolder.Length - 1) ? NewFolder : NewFolder + "\\";
            if (Path.GetPathRoot(orignFolder) == Path.GetPathRoot(NewFolder))
                Directory.Move(orignFolder, path + filename);
            else
            {
                string parent = Path.GetDirectoryName(orignFolder);
                Directory.CreateDirectory(path + Path.GetFileName(orignFolder));
                DirectoryInfo dir = new DirectoryInfo((orignFolder.LastIndexOf("\\") == orignFolder.Length - 1) ? orignFolder : orignFolder + "\\");
                FileSystemInfo[] fileArr = dir.GetFileSystemInfos();
                Queue<FileSystemInfo> Folders = new Queue<FileSystemInfo>(dir.GetFileSystemInfos());
                while (Folders.Count > 0)
                {
                    FileSystemInfo tmp = Folders.Dequeue();
                    FileInfo f = tmp as FileInfo;
                    if (f == null)
                    {
                        DirectoryInfo d = tmp as DirectoryInfo;
                        DirectoryInfo dpath = new DirectoryInfo(d.FullName.Replace((parent.LastIndexOf("\\") == parent.Length - 1) ? parent : parent + "\\", path));
                        dpath.Create();
                        foreach (FileSystemInfo fi in d.GetFileSystemInfos())
                        {
                            Folders.Enqueue(fi);
                        }
                    }
                    else
                    {
                        f.MoveTo(f.FullName.Replace(parent, path));
                    }
                }
                Directory.Delete(orignFolder, true);
            }
        }

        #endregion

        #region 检测指定目录是否存在
        /// <summary>
        /// 检测指定目录是否存在
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        /// <returns></returns>
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        #endregion

        #region 获取指定目录中所有子目录列表
        /// <summary>
        /// 获取指定目录中所有子目录列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>    
        /// <exception cref="IOException">I/O流发生错误</exception>
        /// <returns>返回所有子目录列表</returns>    
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                return Directory.GetDirectories(directoryPath);
            }
            catch (IOException ex)
            {
                throw;
            }
        }
        #endregion

        #region 检测指定目录中是否存在指定的文件

        /// <summary>
        /// 检测指定目录中是否存在指定的文件
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param> 
        /// <param name="isSearchChild">是否搜索子目录</param>
        /// <exception cref="Exception">未知错误</exception>
        /// <returns>返回true存在指定文件，false不存在指定文件</returns>
        public static bool Contains(string directoryPath, string searchPattern, bool isSearchChild = true)
        {
            try
            {
                //获取指定的文件列表
                string[] fileNames = GetFileNames(directoryPath, searchPattern, isSearchChild);

                //判断指定文件是否存在
                if (fileNames.Length == 0)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region 检测指定目录是否为空
        /// <summary>
        /// 检测指定目录是否为空
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>   
        /// <returns>返回true为空，false不为空</returns>

        public static bool IsEmptyDirectory(string directoryPath)
        {
            try
            {
                //判断是否存在文件
                string[] fileNames = GetFileNames(directoryPath);
                if (fileNames.Length > 0)
                    return false;

                //判断是否存在文件夹
                string[] directoryNames = GetDirectories(directoryPath);
                if (directoryNames.Length > 0)
                    return false;
                return true;
            }
            catch
            {
                return true;
            }
        }
        #endregion

        #region 在当前目录下创建目录
        /****************************************
          * 函数名称：FolderCreate
          * 功能说明：在当前目录下创建目录
          * 参     数：OrignFolder:当前目录,NewFloder:新目录
          * 调用示列：
          *            string orignFolder = Server.MapPath("test/");    
          *            string NewFloder = "new";
          *            EC.FileObj.FolderCreate(OrignFolder, NewFloder); 
         *****************************************/
        /// <summary>
        /// 在当前目录下创建目录
        /// </summary>
        /// <param name="orignFolder">当前目录</param>
        /// <param name="newFloder">新目录</param>
        /// <returns>成功返回true，异常返回false</returns>
        public static bool FolderCreate(string orignFolder, string newFloder)
        {
            try
            {
                Directory.SetCurrentDirectory(orignFolder);
                Directory.CreateDirectory(newFloder);
            }
            catch { return false; }
            return true;
        }
        #endregion

        #region 递归删除文件夹目录及文件
        /****************************************
          * 函数名称：DeleteFolder
          * 功能说明：递归删除文件夹目录及文件
          * 参     数：dir:文件夹路径
          * 调用示列：
          *            string dir = Server.MapPath("test/");  
          *            EC.FileObj.DeleteFolder(dir);       
         *****************************************/
        /// <summary>
        /// 递归删除文件夹目录及文件
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns>成功返回true，异常返回false</returns>
        public static bool DeleteFolder(string dirPath)
        {
            try
            {
                if (Directory.Exists(dirPath)) //如果存在这个文件夹删除之 
                {
                    foreach (string d in Directory.GetFileSystemEntries(dirPath))
                    {
                        if (File.Exists(d))
                            File.Delete(d); //直接删除其中的文件 
                        else
                            DeleteFolder(d); //递归删除子文件夹 
                    }
                    Directory.Delete(dirPath); //删除已空文件夹 
                }
            }
            catch { return false; }
            return true;

        }

        #endregion

        #region 移动文件夹中的所有文件夹与文件到另一个文件夹
        /// <summary>
        /// 移动文件夹中的所有文件夹与文件到另一个文件夹
        /// </summary>
        /// <param name="sourcePath">源文件夹</param>
        /// <param name="destPath">目标文件夹</param>
        /// <exception cref="DirectoryNotFoundException">目录不存在</exception>
        /// <exception cref="Exception">目录创建失败</exception>
        public static void MoveFolder(string sourcePath, string destPath)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destPath))
                {
                    //目标目录不存在则创建
                    try
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("创建目标目录失败：" + ex.Message);
                    }
                }
                //获得源文件下所有文件
                List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                files.ForEach(c =>
                {
                    string destFile = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    //覆盖模式
                    if (File.Exists(destFile))
                    {
                        File.Delete(destFile);
                    }
                    File.Move(c, destFile);
                });
                //获得源文件下所有目录文件
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));

                folders.ForEach(c =>
                {
                    string destDir = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    //Directory.Move必须要在同一个根目录下移动才有效，不能在不同卷中移动。
                    //Directory.Move(c, destDir);

                    //采用递归的方法实现
                    MoveFolder(c, destDir);
                });
            }
            else
            {
                throw new DirectoryNotFoundException("源目录不存在！");
            }
        }
        #endregion

        #region 复制文件夹中的所有文件夹与文件到另一个文件夹
        /// <summary>
        /// 复制文件夹中的所有文件夹与文件到另一个文件夹
        /// </summary>
        /// <param name="sourcePath">源文件夹</param>
        /// <param name="destPath">目标文件夹</param>
        /// <param name="isCover">是否覆盖同名文件</param>
        /// <exception cref="DirectoryNotFoundException">目录不存在</exception>
        /// <exception cref="Exception">目录创建失败</exception>
        public static void CopyFolder(string sourcePath, string destPath, bool isCover)
        {
            if (Directory.Exists(sourcePath))
            {
                if (!Directory.Exists(destPath))
                {
                    //目标目录不存在则创建
                    try
                    {
                        Directory.CreateDirectory(destPath);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("创建目标目录失败：" + ex.Message);
                    }
                }
                //获得源文件下所有文件
                List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                files.ForEach(c =>
                {
                    string destFile = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    File.Copy(c, destFile, isCover);//覆盖模式
                });
                //获得源文件下所有目录文件
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
                folders.ForEach(c =>
                {
                    string destDir = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    //采用递归的方法实现
                    CopyFolder(c, destDir, isCover);
                });
            }
            else
            {
                throw new DirectoryNotFoundException("源目录不存在！");
            }
        }
        #endregion

        #region 删除指定目录下所有内容：方法一--删除目录，再创建空目录
        /// <summary>
        /// 删除指定目录下所有内容
        /// </summary>
        /// <param name="deletePath">删除路径</param>
        /// <remarks>删除目录，再创建空目录</remarks>
        /// <returns>成功返回true，异常返回false</returns>
        public static bool DeleteDirectoryContentEx(string deletePath)
        {
            try
            {
                if (Directory.Exists(deletePath))
                {
                    Directory.Delete(deletePath);
                    Directory.CreateDirectory(deletePath);
                }
            }
            catch { return false; }
            return true;
        }
        #endregion

        #region 删除指定目录下所有内容：方法二--找到所有文件和子文件夹删除
        /// <summary>
        /// 删除指定目录下所有内容
        /// </summary>
        /// <param name="deletePath">删除路径</param>
        /// <remarks>找到所有文件和子文件夹删除</remarks>
        /// <returns>成功返回true，异常返回false</returns>
        public static bool DeleteDirectoryContent(string deletePath)
        {
            try
            {
                if (Directory.Exists(deletePath))
                {
                    foreach (string content in Directory.GetFileSystemEntries(deletePath))
                    {
                        if (Directory.Exists(content))
                        {
                            Directory.Delete(content, true);
                        }
                        else if (File.Exists(content))
                        {
                            File.Delete(content);
                        }
                    }
                }
            }
            catch { return false; }
            return true;
        }
        #endregion

        #region 将指定文件夹下面的所有内容copy到目标文件夹下面（如果目标文件夹为只读属性就会报错）
        /****************************************
          * 函数名称：CopyDir
          * 功能说明：将指定文件夹下面的所有内容copy到目标文件夹下面 如果目标文件夹为只读属性就会报错。
          * 参     数：srcPath:原始路径,aimPath:目标文件夹
          * 调用示列：
          *            string srcPath = Server.MapPath("test/");  
          *            string aimPath = Server.MapPath("test1/");
          *            EC.FileObj.CopyDir(srcPath,aimPath);   
         *****************************************/
        /// <summary>
        /// 指定文件夹下面的所有内容copy到目标文件夹下面
        /// </summary>
        /// <param name="srcPath">原始路径</param>
        /// <param name="aimPath">目标文件夹</param>
        /// <exception cref="Exception">未知错误，详见错误参数</exception>
        public static void CopyDir(string srcPath, string aimPath)
        {
            try
            {
                // 检查目标目录是否以目录分割字符结束如果不是则添加之
                if (aimPath[aimPath.Length - 1] != Path.DirectorySeparatorChar)
                    aimPath += Path.DirectorySeparatorChar;
                // 判断目标目录是否存在如果不存在则新建之
                if (!Directory.Exists(aimPath))
                    Directory.CreateDirectory(aimPath);
                // 得到源目录的文件列表，该里面是包含文件以及目录路径的一个数组
                //如果你指向copy目标文件下面的文件而不包含目录请使用下面的方法
                //string[] fileList = Directory.GetFiles(srcPath);
                string[] fileList = Directory.GetFileSystemEntries(srcPath);
                //遍历所有的文件和目录
                foreach (string file in fileList)
                {
                    //先当作目录处理如果存在这个目录就递归Copy该目录下面的文件

                    if (Directory.Exists(file))
                        CopyDir(file, aimPath + Path.GetFileName(file));
                    //否则直接Copy文件
                    else
                        File.Copy(file, aimPath + Path.GetFileName(file), true);
                }

            }
            catch (Exception ee)
            {
                throw new Exception(ee.ToString());
            }
        }

        #endregion

        #region 对一组文件夹按创建时间进行排序
        /// <summary>
        /// 对一组文件夹按创建时间进行排序
        /// </summary>
        /// <param name="infos">一组目录对象</param>
        /// <remarks>按创建先后顺序</remarks>
        public static void SortDirectoryPathByCreateTime(ref DirectoryInfo[] infos)
        {
            Array.Sort(infos, (s1, s2) => s1.CreationTime.CompareTo(s2.CreationTime));
        }
        #endregion
    }
}
