using System;
using System.IO;

namespace SuperFramework.SuperFile
{
    /// <summary>
    /// 日 期:2014-09-22
    /// 作 者:不良帥
    /// 描 述:文件操作监视辅助类
    /// </summary>
    public class FileListener
    {
        /// <summary>
        /// 文件改变事件委托
        /// </summary>
        /// <param name="change">文件改变信息结构</param>
        public delegate void FileChangeEvent(FileChange change);
        /// <summary>
        /// 文件改变事件
        /// </summary>
        public event FileChangeEvent ChangeEvent;
        private FileSystemWatcher watcher;
        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <exception cref="Exception">未知错误，详见错误参数</exception>
        public FileListener(string path)
        {

            try
            {

                watcher = new FileSystemWatcher() { Path = path, NotifyFilter = NotifyFilters.FileName | NotifyFilters.Size | NotifyFilters.DirectoryName, IncludeSubdirectories = true };
                watcher.Created += new FileSystemEventHandler(FileWatcher_Created);
                watcher.Changed += new FileSystemEventHandler(FileWatcher_Changed);
                watcher.Deleted += new FileSystemEventHandler(FileWatcher_Deleted);
                watcher.Renamed += new RenamedEventHandler(FileWatcher_Renamed);

            }

            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// 开始监视
        /// </summary>
        public void Start()
        {

            watcher.EnableRaisingEvents = true;

        }
        /// <summary>
        /// 停止监视
        /// </summary>
        public void Stop()
        {

            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            watcher = null;

        }
        /// <summary>
        /// 文件创建
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(string.Format("新增:{0};{1};{2}", e.ChangeType, e.FullPath, e.Name));
            ChangeEvent?.Invoke(new FileChange() { ChangeType = Enum.GetName(typeof(WatcherChangeTypes), e.ChangeType), FullPath = e.FullPath, Name = e.Name });
        }
        /// <summary>
        /// 文件改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(string.Format("变更:{0};{1};{2}", e.ChangeType, e.FullPath, e.Name));
            ChangeEvent?.Invoke(new FileChange() { ChangeType = Enum.GetName(typeof(WatcherChangeTypes), e.ChangeType), FullPath = e.FullPath, Name = e.Name });
        }
        /// <summary>
        /// 文件删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine(string.Format("删除:{0};{1};{2}", e.ChangeType, e.FullPath, e.Name));
            ChangeEvent?.Invoke(new FileChange() { ChangeType = Enum.GetName(typeof(WatcherChangeTypes), e.ChangeType), FullPath = e.FullPath, Name = e.Name });
        }
        /// <summary>
        /// 文件重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void FileWatcher_Renamed(object sender, RenamedEventArgs e)
        {

            Console.WriteLine("重命名: OldPath:{0} NewPath:{1} OldFileName{2} NewFileName:{3}", e.OldFullPath, e.FullPath, e.OldName, e.Name);
            ChangeEvent?.Invoke(new FileChange() { ChangeType = Enum.GetName(typeof(WatcherChangeTypes), e.ChangeType), FullPath = e.FullPath, Name = e.Name, OldFullPath = e.OldFullPath, OldName = e.OldName });
        }
        /// <summary>
        /// 文件改变信息结构
        /// </summary>
        public struct FileChange
        {
            /// <summary>
            /// 发生的目录事件类型
            /// </summary>
            public string ChangeType;
            /// <summary>
            /// 受影响的文件或目录的完全限定名
            /// </summary>
            public string FullPath;
            /// <summary>
            /// 受影响的文件或目录的名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 受影响前的文件或目录的完全限定名
            /// </summary>
            public string OldFullPath;
            /// <summary>
            /// 受影响前的文件或目录的名称
            /// </summary>
            public string OldName;
        }
    }
}
