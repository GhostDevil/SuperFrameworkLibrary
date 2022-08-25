/******************************************
 * 
 * 模块名称：内部类 LogEntity
 * 当前版本：1.0
 * 开发人员：不良帥
 * 开发时间：2011-06-28
 * 版本历史：
 * 
 * Copyright (C) 不良帥 2011-2015 
 * 
 ******************************************/

namespace SuperFramework.SuperNLogger.Asynchronous
{
    internal class LogEntity
    {
        private string _log_contents;
        private string _file_dir_name;
        private string _log_file_suffix;
        private string _log_extension;

        /// <summary>
        /// 日志内容
        /// </summary>
        public string LOG_CONTENTS
        {
            get { return _log_contents; }
            set { _log_contents = value; }
        }
        /// <summary>
        /// 日志文件路径+文件名（不含文件扩展名）
        /// </summary>
        public string FILE_DIR_NAME
        {
            get { return _file_dir_name; }
            set { _file_dir_name = value; }
        }
        /// <summary>
        /// 日志文件后缀（用以分开记录日志之用，空为不分开记录日志）
        /// </summary>
        public string LOG_FILE_SUFFIX
        {
            get { return _log_file_suffix; }
            set { _log_file_suffix = value; }
        }
        /// <summary>
        /// 日志文件扩展名
        /// </summary>
        public string LOG_EXTENSION
        {
            get { return _log_extension; }
            set { _log_extension = value; }
        }
    }
}
