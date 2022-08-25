using System;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Drawing;
using static SuperFramework.WindowsAPI.Shell32API;
using static SuperFramework.WindowsAPI.User32API;
using static SuperFramework.WindowsAPI.APIStruct;
namespace SuperFramework.SuperFile
{
    /// <summary>
    /// 日期:2015-06-24
    /// 作者:不良帥
    /// 描述:文件管理辅助类
    /// </summary>
    public class FileManager
    {
        #region 定义公共变量
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; //大图标
        public const uint SHGFI_SMALLICON = 0x1; //小图标
        public static string AllPath = "";//记录选择路径
        #endregion

        #region 获取指定路径下所有文件及其图标
        /// <summary>
        /// 获取指定路径下所有文件及其图标
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="imglist">ImageList对象</param>
        /// <param name="lv">ListView对象</param>
        public void GetListViewItem(string path, ImageList imglist, ListView lv)
        {
            lv.Items.Clear();
            SHFILEINFO shfi = new SHFILEINFO();
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                for (int i = 0; i < dirs.Length; i++)
                {
                    string[] info = new string[4];
                    DirectoryInfo dir = new DirectoryInfo(dirs[i]);
                    if (dir.Name == "RECYCLER" || dir.Name == "RECYCLED" || dir.Name == "Recycled" || dir.Name == "System Volume Information")
                    { }
                    else
                    {
                        //获得图标
                        SHGetFileInfo(dirs[i],
                                            (uint)0x80,
                                            ref shfi,
                                            (uint)Marshal.SizeOf(shfi),
                                            (uint)(0x100 | 0x400));//取得Icon和TypeName
                                                                   //添加图标
                        imglist.Images.Add(dir.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                        info[0] = dir.Name;
                        info[1] = "";
                        info[2] = "文件夹";
                        info[3] = dir.LastWriteTime.ToString();
                        ListViewItem item = new ListViewItem(info, dir.Name);
                        lv.Items.Add(item);
                        //销毁图标
                        DestroyIcon(shfi.hIcon);
                    }
                }
                for (int i = 0; i < files.Length; i++)
                {
                    string[] info = new string[5];
                    FileInfo fi = new FileInfo(files[i]);
                    string Filetype = fi.Name.Substring(fi.Name.LastIndexOf(".") + 1, fi.Name.Length - fi.Name.LastIndexOf(".") - 1);
                    string newtype = Filetype.ToLower();
                    if (newtype == "sys" || newtype == "ini" || newtype == "bin" || newtype == "log" || newtype == "com" || newtype == "bat" || newtype == "db")
                    { }
                    else
                    {
                        //获得图标
                        SHGetFileInfo(files[i],
                                            (uint)0x80,
                                            ref shfi,
                                            (uint)System.Runtime.InteropServices.Marshal.SizeOf(shfi),
                                            (uint)(0x100 | 0x400)); //取得Icon和TypeName
                                                                    //添加图标
                        imglist.Images.Add(fi.Name, (Icon)Icon.FromHandle(shfi.hIcon).Clone());
                        info[0] = fi.Name;
                        double dbLength = fi.Length / 1024;
                        if (dbLength < 1024)
                            info[1] = dbLength.ToString("0.00") + " KB";
                        else
                            info[1] = Convert.ToDouble(dbLength / 1024).ToString("0.00") + " MB";
                        info[2] = fi.Extension.ToString();
                        info[3] = fi.LastWriteTime.ToString();
                        info[4] = fi.IsReadOnly.ToString();
                        ListViewItem item = new ListViewItem(info, fi.Name);
                        lv.Items.Add(item);
                        //销毁图标
                        DestroyIcon(shfi.hIcon);
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region 将指定路径的下的文件及文件夹显示在ListView控件中
        /// <summary>
        /// 将指定路径的下的文件及文件夹显示在ListView控件中
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="imglist">ImageList控件对象</param>
        /// <param name="lv">ListView控件对象</param>
        /// <param name="intflag">标识要执行的操作</param>
        public void GetPath(string path, ImageList imglist, ListView lv, int intflag)
        {
            try
            {
                if (intflag == 0)
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
                        if (path.IndexOf("\\") == -1)//判断如果不是完整路径，先转换为完整路径，再打开
                        {
                            uu = AllPath + path;
                            System.Diagnostics.Process.Start(uu);
                        }
                        else//判断如果是完整路径，则直接打开
                            System.Diagnostics.Process.Start(path);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 返回上一级目录
        /// <summary>
        /// 返回上一级目录
        /// </summary>
        /// <param name="lv">显示文件及文件夹的ListView控件对象</param>
        /// <param name="imagelist">显示文件及文件夹图标的ImageList控件对象</param>
        public void BackPath(ListView lv, ImageList imagelist)
        {
            if (AllPath.Length != 3)//判断是否顶级目录
            {
                string NewPath = AllPath.Remove(AllPath.LastIndexOf("\\")).Remove(AllPath.Remove(AllPath.LastIndexOf("\\")).LastIndexOf("\\")) + "\\";
                lv.Items.Clear();
                GetListViewItem(NewPath, imagelist, lv);
                AllPath = NewPath;
            }
        }
        #endregion

        #region 获取ListView控件中的选择项
        /// <summary>
        /// 获取ListView控件中的选择项
        /// </summary>
        /// <param name="lv">ListView控件对象</param>
        /// <returns>ArrayList对象列表</returns>
        public ArrayList GetFiles(ListView lv)
        {
            ArrayList list = new ArrayList();
            foreach (object objFile in lv.SelectedItems)
            {
                string strFile = objFile.ToString();
                string strFileName = strFile.Substring(strFile.IndexOf("{") + 1, strFile.LastIndexOf("}") - strFile.IndexOf("{") - 1);
                list.Add(strFileName);
            }
            return list;
        }
        #endregion

        #region 新建文件或文件夹
        /// <summary>
        /// 新建文件或文件夹
        /// </summary>
        /// <param name="lv">显示文件及文件夹的ListView控件对象</param>
        /// <param name="imagelist">显示文件及文件夹图标的ImageList控件对象</param>
        /// <param name="strName">要新建的文件或文件夹名</param>
        /// <param name="intflag">标识是执行新建文件操作，还是执行新建文件夹操作</param>
        public void NewFile(ListView lv, ImageList imagelist, string strName, int intflag)
        {
            string strPath = AllPath + strName;
            if (intflag == 0)
            {
                File.Create(strPath);//新建文件
            }
            else if (intflag == 1)
            {
                Directory.CreateDirectory(strPath);//新建文件夹
            }
            GetListViewItem(AllPath, imagelist, lv);
        }
        #endregion

        #region 复制或剪切文件（包括批量复制、剪切）
        /// <summary>
        /// 复制或剪切文件（包括批量复制、剪切）
        /// </summary>
        /// <param name="lv">显示文件的ListView控件对象</param>
        /// <param name="imagelist">显示文件图标的ImageList控件对象</param>
        /// <param name="list">ListView控件中的选中项</param>
        /// <param name="strPath">文件的原始路径</param>
        /// <param name="strNewPath">文件的新路径</param>
        /// <param name="intflag">标识是执行复制操作，还是执行剪切操作</param>
        public void CopyFile(ListView lv, ImageList imagelist, ArrayList list, string strPath, string strNewPath, int intflag, ToolStripProgressBar TSPBar)
        {
            try
            {
                TSPBar.Maximum = list.Count;
                foreach (object objFile in list)
                {
                    string strFile = objFile.ToString();
                    string strOldFile = strPath + strFile;
                    string strNewFile = strNewPath + strFile;
                    if (File.Exists(strOldFile))
                    {
                        if (!Directory.Exists(strNewPath))
                            Directory.CreateDirectory(strNewPath);
                        if (intflag == 0)
                        {
                            File.Copy(strOldFile, strNewFile, true);
                        }
                        else if (intflag == 1)
                        {
                            File.Move(strOldFile, strNewFile);
                        }
                    }
                    TSPBar.Value += 1;
                }
                GetListViewItem(AllPath, imagelist, lv);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region  复制或剪切文件夹（包括批量复制、剪切）
        /// <summary>
        /// 复制或剪切文件夹（包括批量复制、剪切）
        /// </summary>
        /// <param name="Ddir">要复制文件夹的新路径</param>
        /// <param name="Sdir">要复制文件夹的原始路径</param>
        /// <param name="intflag">标识执行复制操作，还是执行剪切操作</param>
        public void CopyDir(string Ddir, string Sdir, int intflag)
        {
            DirectoryInfo dir = new DirectoryInfo(Sdir);
            try
            {
                if (!dir.Exists)//判断所指的文件或文件夹是否存在
                {
                    return;
                }
                DirectoryInfo dirD = dir as DirectoryInfo;//如果给定参数不是文件夹则退出
                string UpDir = UpAndDown_Dir(Ddir);
                if (dirD == null)//判断文件夹是否为空
                {
                    Directory.CreateDirectory(string.Format("{0}\\{1}", UpDir, dirD.Name));//如果为空，创建文件夹并退出
                    return;
                }
                else
                {
                    Directory.CreateDirectory(UpDir + "\\" + dirD.Name);
                }
                string SbuDir = UpDir + "\\" + dirD.Name + "\\";
                FileSystemInfo[] files = dirD.GetFileSystemInfos();//获取文件夹中所有文件和文件夹
                                                                   //对单个FileSystemInfo进行判断,如果是文件夹则进行递归操作
                foreach (FileSystemInfo FSys in files)
                {
                    FileInfo file = FSys as FileInfo;
                    if (file != null)//如果是文件的话，进行文件的复制操作
                    {
                        FileInfo SFInfo = new FileInfo(file.DirectoryName + "\\" + file.Name);//获取文件所在的原始路径
                        SFInfo.CopyTo(SbuDir + "\\" + file.Name, true);//将文件复制到指定的路径中
                    }
                    else
                    {
                        string pp = FSys.Name;//获取当前搜索到的文件夹名称
                        CopyDir(SbuDir + FSys.ToString(), Sdir + "\\" + FSys.ToString(), intflag);//如果是文件，则进行递归调用
                    }
                }
                if (intflag == 1)
                    Directory.Delete(Sdir, true);
            }
            catch
            {
                MessageBox.Show("文件夹复制失败。");
            }
        }
        /// <summary>
        /// 返回上一级目录
        /// </summary>
        /// <param dir="string">目录</param>
        /// <returns>返回String对象</returns>
        public string UpAndDown_Dir(string dir)
        {
            string Change_dir = Directory.GetParent(dir).FullName;
            return Change_dir;
        }
        #endregion

        #region 重命名文件及文件夹（包括批量重命名文件），该方法为重载方法
        /// <summary>
        /// 重命名文件及文件夹
        /// </summary>
        /// <param name="lv">显示文件及文件夹的ListView控件对象</param>
        /// <param name="imagelist">显示文件及文件夹图标的ImageList控件对象</param>
        /// <param name="strName">文件或文件夹的原名称</param>
        /// <param name="strNewName">文件或文件夹的新名称</param>
        public void RepeatFile(ListView lv, ImageList imagelist, string strName, string strNewName)
        {
            string strPath = AllPath + strName;
            string strNewPath = AllPath + strNewName;
            if (strNewName != null)
            {
                if (File.Exists(strPath))
                {
                    if (strPath != strNewPath)
                    {
                        File.Move(strPath, strNewPath);
                    }
                }
                if (Directory.Exists(strPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(strPath);
                    dir.MoveTo(strNewPath);
                }
                lv.Items.Clear();
                GetListViewItem(AllPath, imagelist, lv);
            }
        }
        /// <summary>
        /// 批量重命名文件（当文件为Word时，修改标题中的文字/，未用）
        /// </summary>
        /// <param name="lbox">显示要重命名文件的ListBox控件对象</param>
        /// <param name="intFlag">标识按编号重命名，还是按扩展名重命名 0-在前面加编号 1-在后面加编号 2-修改扩展名 3-替换文件名称中的文字</param>
        /// <param name="strExten">要重命名的扩展名</param>
        /// <param name="strOldName">要替换的旧名称</param>
        /// <param name="strNewName">要使用的新名称</param>
        /// <param name="PBar">进度条显示</param>
        public void RepeatFile(ListBox lbox, int intFlag, string strExten, string strOldName, string strNewName, ProgressBar PBar)
        {
            PBar.Maximum = lbox.Items.Count;
            for (int i = 0; i < lbox.Items.Count; i++)
            {
                string strFile = lbox.Items[i].ToString();
                if (File.Exists(strFile))
                {
                    FileInfo FInfo = new FileInfo(strFile);
                    string strPath = FInfo.DirectoryName;
                    string strFName = FInfo.Name;
                    string strFExtention = FInfo.Extension;
                    switch (intFlag)
                    {
                        case 0:
                            File.Move(strFile, strPath + "\\" + i.ToString().PadLeft(4, '0') + strFName);
                            break;
                        case 1:
                            File.Move(strFile, strPath + "\\" + strFName.Substring(0, strFName.LastIndexOf(".")) + i.ToString().PadLeft(4, '0') + strFExtention);
                            break;
                        case 2:
                            File.Move(strFile, strPath + "\\" + strFName.Substring(0, strFName.LastIndexOf(".")) + "." + strExten);
                            break;
                        case 3:
                            object strNewPath = strPath + "\\" + strFName.Replace(strOldName, strNewName);
                            File.Move(strFile, strNewPath.ToString());
                            //    //如果文件为Word文档格式，则替换标题中的文字
                            //    if (strFExtention.ToLower() == ".doc")
                            //    {
                            //        Word.Application newWord = new Word.Application();
                            //        object missing = System.Reflection.Missing.Value;
                            //        //打开一个Word文档
                            //        Word.Document newDocument = newWord.Documents.Open(ref strNewPath, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing);
                            //        object Replaceone = Word.WdReplace.wdReplaceOne;
                            //        //设置要查找的关键字
                            //        newWord.Selection.Find.ClearFormatting();
                            //        newWord.Selection.Find.Text = strOldName;
                            //        //设置要替换的关键字
                            //        newWord.Selection.Find.Replacement.ClearFormatting();
                            //        newWord.Selection.Find.Replacement.Text = strNewName;
                            //        //执行替换操作
                            //        newWord.Selection.Find.Execute(
                            //            ref missing, ref missing, ref missing, ref missing, ref missing,
                            //            ref missing, ref missing, ref missing, ref missing, ref missing,
                            //            ref Replaceone, ref missing, ref missing, ref missing, ref missing);
                            //        //保存Word文档
                            //        newWord.ActiveDocument.Save();
                            //        //关闭Word文档
                            //        object objSaveChanges = Word.WdSaveOptions.wdSaveChanges;
                            //        Word.DocumentClass doc = newWord.ActiveDocument as Word.DocumentClass;
                            //        doc.Close(ref objSaveChanges, ref missing, ref missing);
                            //    }
                            break;
                    }
                    PBar.Value = i + 1;
                }
            }
        }
        #endregion

        #region 重命名文件时，在树控件中显示文件名
        /// <summary>
        /// 重命名文件时，在树控件中显示文件名
        /// </summary>
        /// <param name="TNode">树节点对象</param>
        public void TVShow(TreeNode TNode)
        {
            try
            {
                if (TNode.Nodes.Count == 0)
                {
                    if (TNode.Parent == null)
                    {
                        foreach (string DirName in Directory.GetLogicalDrives())
                        {
                            TreeNode DirNode = new TreeNode(DirName);
                            DirNode.Tag = DirName;
                            TNode.Nodes.Add(DirNode);
                        }
                    }
                    else
                    {
                        foreach (string PathName in Directory.GetFileSystemEntries((string)TNode.Tag))
                        {
                            TreeNode PathNode = new TreeNode(PathName);
                            PathNode.Tag = PathName;
                            TNode.Nodes.Add(PathNode);
                        }
                    }
                }
            }
            catch { }
        }
        #endregion

        #region 删除文件/文件夹（包括批量删除）
        /// <summary>
        /// 删除文件/文件夹（包括批量删除）
        /// </summary>
        /// <param name="lv">显示文件及文件夹的ListView控件对象</param>
        /// <param name="imagelist">显示文件及文件夹图标的ImageList控件对象</param>
        public void DeleteFile(ListView lv, ImageList imagelist, ToolStripProgressBar TSPBar)
        {
            TSPBar.Maximum = lv.SelectedItems.Count;
            foreach (object objFile in lv.SelectedItems)
            {
                string strFile = objFile.ToString();
                string strFullFile = AllPath + strFile.Substring(strFile.IndexOf("{") + 1, strFile.LastIndexOf("}") - strFile.IndexOf("{") - 1);
                if (File.Exists(strFullFile))
                    File.Delete(strFullFile);
                else if (Directory.Exists(strFullFile))
                    Directory.Delete(strFullFile, true);
                TSPBar.Value += 1;
            }
            GetListViewItem(AllPath, imagelist, lv);
        }
        #endregion

        #region 搜索文件及文件夹
        /// <summary>
        /// 搜索文件及文件夹
        /// </summary>
        /// <param name="lv">显示搜索结果的ListView控件对象</param>
        /// <param name="strName">要搜索的文件或文件夹关键字</param>
        public void SearchFile(ListView lv, string strName)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(AllPath);
                FileSystemInfo[] files = dir.GetFileSystemInfos();
                //对单个FileSystemInfo进行判断,如果是文件夹则进行递归操作
                foreach (FileSystemInfo FSInfo in files)
                {
                    FileInfo file = FSInfo as FileInfo;
                    if (file != null)//判断是不是文件
                    {
                        if (file.Name.IndexOf(strName) != -1)
                        {
                            string[] info = new string[5];
                            info[0] = AllPath + file.Name;
                            double dbLength = file.Length / 1024;
                            if (dbLength < 1024)
                                info[1] = dbLength.ToString("0.00") + " KB";
                            else
                                info[1] = Convert.ToDouble(dbLength / 1024).ToString("0.00") + " MB";
                            info[2] = file.Extension.ToString();
                            info[3] = file.LastWriteTime.ToString();
                            info[4] = file.IsReadOnly.ToString();
                            ListViewItem item = new ListViewItem(info, Convert.ToString(AllPath + file.Name).Remove(0, 3));
                            lv.Items.Add(item);
                        }
                    }
                    else//如果是文件夹
                    {
                        if (FSInfo.Name.IndexOf(strName) != -1)
                        {
                            string[] info = new string[4];
                            info[0] = AllPath + FSInfo.Name;
                            info[1] = "";
                            info[2] = "文件夹";
                            info[3] = FSInfo.LastWriteTime.ToString();
                            ListViewItem item = new ListViewItem(info, AllPath + FSInfo.Name);
                            lv.Items.Add(item);
                        }
                        AllPath += FSInfo.Name + "\\";
                        while (!Directory.Exists(AllPath))
                        {
                            string strNewPath = AllPath.Substring(0, AllPath.LastIndexOf("\\"));
                            AllPath = string.Format("{0}\\{1}\\", Directory.GetParent(strNewPath.Substring(0, strNewPath.LastIndexOf("\\"))).FullName, FSInfo.Name);
                        }
                        SearchFile(lv, strName);
                    }
                }
            }
            catch { }
        }
        #endregion

        #region 分割文件
        /// <summary>
        /// 分割文件
        /// </summary>
        /// <param name="strFlag">分割单位</param>
        /// <param name="intFlag">分割大小</param>
        /// <param name="strPath">分割后的文件存放路径</param>
        /// <param name="strFile">要分割的文件</param>
        /// <param name="PBar">进度条显示</param>
        public void SplitFile(string strFlag, int intFlag, string strPath, string strFile, ProgressBar PBar)
        {
            int iFileSize = 0;
            //根据选择来设定分割的小文件的大小
            switch (strFlag)
            {
                case "Byte":
                    iFileSize = intFlag;
                    break;
                case "KB":
                    iFileSize = intFlag * 1024;
                    break;
                case "MB":
                    iFileSize = intFlag * 1024 * 1024;
                    break;
                case "GB":
                    iFileSize = intFlag * 1024 * 1024 * 1024;
                    break;
            }
            //如果计算机存在存放分割文件的目录，则全部删除此目录所有文件
            if (!Directory.Exists(strPath))
            {
                Directory.CreateDirectory(strPath);
            }
            //以文件的全路对应的字符串和文件打开模式来初始化FileStream文件流实例
            FileStream SplitFileStream = new FileStream(strFile, FileMode.Open);
            //以FileStream文件流来初始化BinaryReader文件阅读器
            BinaryReader SplitFileReader = new BinaryReader(SplitFileStream);
            //每次分割读取的最大数据
            byte[] TempBytes;
            //小文件总数
            int iFileCount = (int)(SplitFileStream.Length / iFileSize);
            PBar.Maximum = iFileCount;
            if (SplitFileStream.Length % iFileSize != 0) iFileCount++;
            string[] TempExtra = strFile.Split('.');
            //循环将大文件分割成多个小文件
            for (int i = 1; i <= iFileCount; i++)
            {
                //确定小文件的文件名称
                string sTempFileName = strPath + @"\" + i.ToString().PadLeft(4, '0') + "." + TempExtra[TempExtra.Length - 1]; //小文件名
                                                                                                                              //根据文件名称和文件打开模式来初始化FileStream文件流实例
                FileStream TempStream = new FileStream(sTempFileName, FileMode.OpenOrCreate);
                //以FileStream实例来创建、初始化BinaryWriter书写器实例
                BinaryWriter TempWriter = new BinaryWriter(TempStream);
                //从大文件中读取指定大小数据
                TempBytes = SplitFileReader.ReadBytes(iFileSize);
                //把此数据写入小文件
                TempWriter.Write(TempBytes);
                //关闭书写器，形成小文件
                TempWriter.Close();
                //关闭文件流
                TempStream.Close();
                PBar.Value = i - 1;
            }
            //关闭大文件阅读器
            SplitFileReader.Close();
            SplitFileStream.Close();
            MessageBox.Show("文件分割成功!");
        }
        #endregion

        #region 合并文件
        /// <summary>
        /// 合并文件
        /// </summary>
        /// <param name="strFile">要合并的文件集合</param>
        /// <param name="strPath">合并后的文件名称</param>
        /// <param name="PBar">进度条显示</param>
        public void CombinFile(string[] strFile, string strPath, ProgressBar PBar)
        {
            PBar.Maximum = strFile.Length;
            //以合并后的文件名称和打开方式来创建、初始化FileStream文件流
            FileStream AddStream = new FileStream(strPath, FileMode.Append);
            //以FileStream文件流来初始化BinaryWriter书写器，此用以合并分割的文件
            BinaryWriter AddWriter = new BinaryWriter(AddStream);
            BinaryReader TempReader;
            //循环合并小文件，并生成合并文件
            for (int i = 0; i < strFile.Length; i++)
            {
                //以小文件所对应的文件名称和打开模式来初始化FileStream文件流，起读取分割作用
                FileStream TempStream = new FileStream(strFile[i].ToString(), FileMode.Open);
                TempReader = new BinaryReader(TempStream);
                //读取分割文件中的数据，并生成合并后文件
                AddWriter.Write(TempReader.ReadBytes((int)TempStream.Length));
                //关闭BinaryReader文件阅读器
                TempReader.Close();
                //关闭FileStream文件流
                TempStream.Close();
                PBar.Value = i + 1;
            }
            //关闭BinaryWriter文件书写器
            AddWriter.Close();
            //关闭FileStream文件流
            AddStream.Close();
            MessageBox.Show("文件合并成功！");
        }
        #endregion
    }
}