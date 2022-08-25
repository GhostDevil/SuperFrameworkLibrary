using System;
using System.IO;
using static SuperFramework.WindowsAPI.APIStruct;
using static SuperFramework.WindowsAPI.Shell32API;
namespace SuperFramework.SuperFile
{
    /// <summary>
    /// 日 期:2014-09-22
    /// 作 者:不良帥
    /// 描 述:调用系统操作方式操作文件及文件夹，有系统对话框。
    /// </summary>
    public class FileSysOperate
    {

        #region 解析错误代码
        /// <summary>
        /// 解释错误代码
        /// </summary>
        /// <param name="n">代码号</param>
        /// <returns>返回关于错误代码的文字描述</returns>
        public static string GetErrorString(int n)
        {
            if (n == 0) return string.Empty;
            switch (n)
            {
                case 2:
                    return "系统找不到指定的文件。";
                case 7:
                    return "存储控制块被销毁。您是否选择的“取消”操作？";
                case 113:
                    return "文件已存在！";
                case 115:
                    return "重命名文件操作,原始文件和目标文件必须具有相同的路径名。不能使用相对路径。";
                case 117:
                    return "I/O控制错误";
                case 123:
                    return "指定了重复的文件名";
                case 116:
                    return "源是根目录，不能移动或重命名。";
                case 118:
                    return "安全设置拒绝访问源。";
                case 124:
                    return "源或目的地的路径或两者无效。";
                case 65536:
                    return "目的地发生未指定的错误。";
                case 1026:
                    return "在试图移动或拷贝一个不存在的文件.";
                case 1223:
                    return "操作被取消！";
                default:
                    return "未识别的错误代码：" + n;
            }
        }
        #endregion

        #region 删除单个文件
        /// <summary>
        /// 删除单个文件
        /// </summary>
        /// <param name="fileName">删除的文件名</param>
        /// <param name="toRecycle">指示是将文件放入回收站还是永久删除，true-放入回收站，false-永久删除</param>
        /// <param name="showDialog">指示是否显示确认对话框，true-显示确认删除对话框，false-不显示确认删除对话框</param>
        /// <param name="showProgress">指示是否显示进度对话框，true-显示，false-不显示。该参数当指定永久删除文件时有效</param>
        /// <param name="errorMsg">反馈错误消息的字符串</param>
        /// <returns>操作执行结果标识，删除文件成功返回0，否则，返回错误代码</returns>
        public static int DeleteFile(string fileName, ref string errorMsg, bool toRecycle=true, bool showDialog=false, bool showProgress=true)
        {
            try
            {
                string fName = FileHelper.GetFullName(fileName);
                return ToDelete(fName, ref errorMsg, toRecycle, showDialog, showProgress);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return -200;
            }
        }
        #endregion

        #region 删除一组文件
        /// <summary>
        /// 删除一组文件
        /// </summary>
        /// <param name="fileNames">字符串数组，表示一组文件名</param>
        /// <param name="toRecycle">指示是将文件放入回收站还是永久删除，true-放入回收站，false-永久删除</param>
        /// <param name="showDialog">指示是否显示确认对话框，true-显示确认删除对话框，false-不显示确认删除对话框</param>
        /// <param name="showProgress">指示是否显示进度对话框，true-显示，false-不显示。该参数当指定永久删除文件时有效</param>
        /// <param name="errorMsg">反馈错误消息的字符串</param>
        /// <returns>操作执行结果标识，删除文件成功返回0，否则，返回错误代码</returns>
        public static int DeleteFiles(string[] fileNames, ref string errorMsg, bool toRecycle = true, bool showDialog = false, bool showProgress = true)
        {
            try
            {
                string fName = "";
                foreach (string str in fileNames)
                {
                    fName += FileHelper.GetFullName(str) + "\0";     //组件文件组字符串
                }

                return ToDelete(fName, ref errorMsg, toRecycle, showDialog, showProgress);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return -200;
            }
        }
        #endregion 删除文件操作

        #region 删除单个或多个文件
        /// <summary>
        /// 删除单个或多个文件
        /// </summary>
        /// <param name="fileName">删除的文件名，如果是多个文件，文件名之间以字符串结尾符'\0'隔开</param>
        /// <param name="toRecycle">指示是将文件放入回收站还是永久删除，true-放入回收站，false-永久删除</param>
        /// <param name="showDialog">指示是否显示确认对话框，true-显示确认删除对话框，false-不显示确认删除对话框</param>
        /// <param name="showProgress">指示是否显示进度对话框，true-显示，false-不显示。该参数当指定永久删除文件时有效</param>
        /// <param name="errorMsg">反馈错误消息的字符串</param>
        /// <returns>操作执行结果标识，删除文件成功返回0，否则，返回错误代码</returns>
        private static int ToDelete(string fileName, ref string errorMsg, bool toRecycle = true, bool showDialog = false, bool showProgress = true)
        {
            SHFILEOPSTRUCT lpFileOp = new SHFILEOPSTRUCT() { wFunc = wFunc.FO_DELETE, pFrom = fileName + "\0" /*将文件名以结尾字符"\0"结束*/, fFlags = FILEOP_FLAGS.FOF_NOERRORUI };
            if (toRecycle)
                lpFileOp.fFlags |= FILEOP_FLAGS.FOF_ALLOWUNDO;  //设定删除到回收站
            if (!showDialog)
                lpFileOp.fFlags |= FILEOP_FLAGS.FOF_NOCONFIRMATION;     //设定不显示提示对话框
            if (!showProgress)
                lpFileOp.fFlags |= FILEOP_FLAGS.FOF_SILENT;     //设定不显示进度对话框

            lpFileOp.fAnyOperationsAborted = true;

            int n = SHFileOperation(ref lpFileOp);
            if (n == 0)
                return 0;

            string tmp = GetErrorString(n);

            //.av 文件正常删除了但也提示 402 错误，不知道为什么。屏蔽之。
            if ((fileName.ToLower().EndsWith(".av") && n.ToString("X") == "402"))
                return 0;

            errorMsg = string.Format("{0}({1})", tmp, fileName);

            return n;
        }
        #endregion

        #region 移动一个文件到指定路径下
        /// <summary>
        /// 移动一个文件到指定路径下
        /// </summary>
        /// <param name="sourceFileName">要移动的文件名</param>
        /// <param name="destinationPath">移动到的目的路径</param>
        /// <param name="errorMsg">反馈错误消息的字符串</param>
        /// <param name="autoRename">指示当文件名重复时，是否自动为新文件加上后缀名</param>
        /// <param name="showDialog">指示是否显示确认对话框，true-显示确认对话框，false-不显示确认对话框</param>
        /// <param name="showProgress">指示是否显示进度对话框</param>
        /// <returns>返回移动操作是否成功的标识，成功返回0，失败返回错误代码</returns>
        public static int MoveFile(string sourceFileName, string destinationPath,ref string errorMsg, bool autoRename=false, bool showDialog =false, bool showProgress=true)
        {
            try
            {
                string sfName = FileHelper.GetFullName(sourceFileName);
                string dfName = FileHelper.GetFullName(destinationPath);

                return ToMoveOrCopy(wFunc.FO_MOVE, sfName, dfName, ref errorMsg, showDialog, showProgress, autoRename);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return -200;
            }
        }
        #endregion

        #region 移动一组文件到指定的路径下
        /// <summary>
        /// 移动一组文件到指定的路径下
        /// </summary>
        /// <param name="sourceFileNames">要移动的文件名数组</param>
        /// <param name="destinationPath">移动到的目的路径</param>
        /// <param name="errorMsg">反馈错误消息的字符串</param>
        /// <param name="autoRename">指示当文件名重复时，是否自动为新文件加上后缀名</param>
        /// <param name="showDialog">指示是否显示确认对话框，true-显示确认对话框，false-不显示确认对话框</param>
        /// <param name="showProgress">指示是否显示进度对话框</param>
        /// <returns>返回移动操作是否成功的标识，成功返回0，失败返回错误代码,-200:表示其他异常</returns>
        public static int MoveFiles(string[] sourceFileNames, string destinationPath, ref string errorMsg, bool autoRename=false, bool showDialog=false, bool showProgress=true)
        {
            try
            {
                string sfName = "";
                foreach (string str in sourceFileNames)
                {
                    sfName += FileHelper.GetFullName(str) + "\0";   //组件文件组字符串
                }
                string dfName = FileHelper.GetFullName(destinationPath);

                return ToMoveOrCopy(wFunc.FO_MOVE, sfName, dfName, ref errorMsg, showDialog, showProgress, autoRename);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return -200;
            }
        }
        #endregion

        #region 复制一个文件到指定的文件名或路径
        /// <summary>
        /// 复制一个文件到指定的文件名或路径
        /// </summary>
        /// <param name="sourceFileName">要复制的文件名</param>
        /// <param name="destinationFileName">复制到的目的文件名或路径</param>
        /// <param name="showDialog">指示是否显示确认对话框，true-显示确认对话框，false-不显示确认对话框</param>
        /// <param name="errorMsg">反馈错误消息的字符串</param>
        /// <param name="showProgress">指示是否显示进度对话框</param>
        /// <param name="autoRename">指示当文件名重复时，是否自动为新文件加上后缀名</param>
        /// <returns>返回移动操作是否成功的标识，成功返回0，失败返回错误代码,-200:表示其他异常</returns>
        public static int CopyFile(string sourceFileName, string destinationFileName, ref string errorMsg, bool showDialog=false, bool showProgress=true, bool autoRename=false)
        {
            try
            {
                string sfName = FileHelper.GetFullName(sourceFileName);
                string dfName = FileHelper.GetFullName(destinationFileName);

                return ToMoveOrCopy(wFunc.FO_COPY, sfName, dfName, ref errorMsg, showDialog, showProgress, autoRename);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return -200;
            }
        }
        #endregion

        #region 复制一组文件到指定的路径
        /// <summary>
        /// 复制一组文件到指定的路径
        /// </summary>
        /// <param name="sourceFileNames">要复制的文件名数组</param>
        /// <param name="destinationPath">复制到的目的路径</param>
        /// <param name="errorMsg">反馈错误消息的字符串</param>
        /// <param name="showDialog">指示是否显示确认对话框，true-显示确认对话框，false-不显示确认对话框</param>
        /// <param name="showProgress">指示是否显示进度对话框</param>
        /// <param name="autoRename">指示当文件名重复时，是否自动为新文件加上后缀名</param>
        /// <returns>返回移动操作是否成功的标识，成功返回0，失败返回错误代码,-200:表示其他异常</returns>
        public static int CopyFiles(string[] sourceFileNames, string destinationPath, ref string errorMsg, bool showDialog=false, bool showProgress=true, bool autoRename= false)
        {
            try
            {
                string sfName = "";
                foreach (string str in sourceFileNames)
                {
                    sfName += FileHelper.GetFullName(str) + "\0";     //组件文件组字符串
                }
                string dfName = FileHelper.GetFullName(destinationPath);

                return ToMoveOrCopy(wFunc.FO_COPY, sfName, dfName, ref errorMsg ,showDialog, showProgress, autoRename);
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return -200;
            }
        }
        #endregion

        #region 移动或复制一个或多个文件到指定路径下
        /// <summary>
        /// 移动或复制一个或多个文件到指定路径下
        /// </summary>
        /// <param name="flag">操作类型，是移动操作还是复制操作</param>
        /// <param name="sourceFileName">要移动或复制的文件名，如果是多个文件，文件名之间以字符串结尾符'\0'隔开</param>
        /// <param name="destinationFileName">移动到的目的位置</param>
        /// <param name="errorMsg">反馈错误消息的字符串</param>
        /// <param name="showDialog">指示是否显示确认对话框，true-显示确认对话框，false-不显示确认对话框</param>
        /// <param name="showProgress">指示是否显示进度对话框</param>
        /// <param name="autoRename">指示当文件名重复时，是否自动为新文件加上后缀名</param>
        /// <returns>返回移动操作是否成功的标识，成功返回0，失败返回错误代码</returns>
        private static int ToMoveOrCopy(wFunc flag, string sourceFileName, string destinationFileName, ref string errorMsg, bool showDialog=false, bool showProgress=true, bool autoRename=false)
        {
            SHFILEOPSTRUCT lpFileOp = new SHFILEOPSTRUCT() { wFunc = flag, pFrom = sourceFileName + "\0" /*将文件名以结尾字符"\0\0"结束*/, pTo = destinationFileName + "\0\0", fFlags = FILEOP_FLAGS.FOF_NOERRORUI };
            lpFileOp.fFlags |= FILEOP_FLAGS.FOF_NOCONFIRMMKDIR; //指定在需要时可以直接创建路径
            if (!showDialog)
                lpFileOp.fFlags |= FILEOP_FLAGS.FOF_NOCONFIRMATION;     //设定不显示提示对话框
            if (!showProgress)
                lpFileOp.fFlags |= FILEOP_FLAGS.FOF_SILENT;     //设定不显示进度对话框
            if (autoRename)
                lpFileOp.fFlags |= FILEOP_FLAGS.FOF_RENAMEONCOLLISION;  //自动为重名文件添加名称后缀
            lpFileOp.fAnyOperationsAborted = true;
            int n = SHFileOperation(ref lpFileOp);
            if (n == 0)
                return 0;
            string tmp = GetErrorString(n);
            errorMsg = string.Format("{0}({1})", tmp, sourceFileName);
            return n;
        }
        #endregion

        #region 重命名一个文件为新名称
        /// <summary>
        /// 重命名一个文件为新名称，建议您使用更方便的Microsoft.VisualBasic.FileSystem.ReName();替换该方法
        /// </summary>
        /// <param name="sourceFileName">要复制的文件名</param>
        /// <param name="destinationFileName">复制到的目的文件名或路径</param>
        /// <param name="errorMsg">反馈错误消息的字符串</param>
        /// <param name="showDialog">指示是否显示确认对话框，true-显示确认对话框，false-不显示确认对话框</param>
        /// <returns>返回移动操作是否成功的标识，成功返回0，失败返回错误代码,-200:表示其他异常</returns>
        [Obsolete("建议使用 Microsoft.VisualBasic.FileSystem.ReName()方法")]
        public static int ReNameFile(string sourceFileName, string destinationFileName, ref string errorMsg, bool showDialog=false)
        {

            try
            {
                SHFILEOPSTRUCT lpFileOp = new SHFILEOPSTRUCT();
                lpFileOp.wFunc = wFunc.FO_RENAME;
                lpFileOp.pFrom = FileHelper.GetFullName(sourceFileName) + "\0\0";         //将文件名以结尾字符"\0\0"结束
                lpFileOp.pTo = FileHelper.GetFullName(destinationFileName) + "\0\0";

                lpFileOp.fFlags = FILEOP_FLAGS.FOF_NOERRORUI;
                if (!showDialog)
                    lpFileOp.fFlags |= FILEOP_FLAGS.FOF_NOCONFIRMATION;     //设定不显示提示对话框


                lpFileOp.fAnyOperationsAborted = true;

                int n = SHFileOperation(ref lpFileOp);
                if (n == 0)
                    return 0;

                string tmp = GetErrorString(n);

                errorMsg = string.Format("{0}({1})", tmp, sourceFileName);

                return n;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return -200;
            }
        }
        #endregion

        #region Microsoft.VisualBasic.FileSystem.ReName()重命名一个文件为新名称
        /// <summary>
        /// 利用Microsoft.VisualBasic.FileSystem.ReName()方法实现
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="newFileName">新文件名</param>
        /// <exception cref="Exception">未知错误，详见错误参数</exception>
        public static void ReNameFile(string filePath, string newFileName)
        {
            try
            {
                string extensName = Path.GetExtension(filePath);
                string newName = newFileName + extensName;
                Microsoft.VisualBasic.FileIO.FileSystem.RenameFile(filePath, newName);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        #endregion
    }
}
