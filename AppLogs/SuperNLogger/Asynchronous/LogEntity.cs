/******************************************
 * 
 * ģ�����ƣ��ڲ��� LogEntity
 * ��ǰ�汾��1.0
 * ������Ա��������
 * ����ʱ�䣺2011-06-28
 * �汾��ʷ��
 * 
 * Copyright (C) ������ 2011-2015 
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
        /// ��־����
        /// </summary>
        public string LOG_CONTENTS
        {
            get { return _log_contents; }
            set { _log_contents = value; }
        }
        /// <summary>
        /// ��־�ļ�·��+�ļ����������ļ���չ����
        /// </summary>
        public string FILE_DIR_NAME
        {
            get { return _file_dir_name; }
            set { _file_dir_name = value; }
        }
        /// <summary>
        /// ��־�ļ���׺�����Էֿ���¼��־֮�ã���Ϊ���ֿ���¼��־��
        /// </summary>
        public string LOG_FILE_SUFFIX
        {
            get { return _log_file_suffix; }
            set { _log_file_suffix = value; }
        }
        /// <summary>
        /// ��־�ļ���չ��
        /// </summary>
        public string LOG_EXTENSION
        {
            get { return _log_extension; }
            set { _log_extension = value; }
        }
    }
}
