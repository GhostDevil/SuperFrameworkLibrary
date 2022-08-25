using System;
using System.Collections.Generic;
using System.Threading;

namespace SuperFramework.AppLogs
{
    /// <summary>
    /// 说明：消息信息
    /// 作者：不良帥
    /// 日期：2016-01-30
    /// </summary>
    public static class AppMsgHelper
    {
        /// <summary>
        /// 获取日志字符串
        /// </summary>
        /// <param name="msgTxt">日志内容</param>
        /// <param name="title">日志标题</param>
        /// <param name="isTitleNewline">是否标题换行</param>
        public static string GetLogStr(string msgTxt, string title = "" ,bool isTitleNewline = false)
        {
            return string.Format(Environment.NewLine+"【 时间：{0} {1} 】 {2}  {3}{4}", DateTime.Now, title, isTitleNewline ? Environment.NewLine: "", msgTxt, Environment.NewLine);
        }
        static bool isWrite = false;
        /// <summary>
        /// 获取日志字符串
        /// </summary>
        /// <param name="msgTxt">日志内容</param>
        /// <param name="box">Textbox 控件</param>
        /// <param name="title">日志标题</param>
        /// <param name="isTitleNewline">是否标题换行</param>
        public static void ShowLogStr(string msgTxt, System.Windows.Controls.TextBox box, string title = "", bool isTitleNewline = false)
        {
            while (isWrite)
            {
                Thread.Sleep(10);
            }
            isWrite = true;
            string str1 = string.Format(Environment.NewLine + "【 时间：{0} {1} 】 {2}  {3}", DateTime.Now, title, isTitleNewline ? Environment.NewLine : "" , Environment.NewLine);
            box.AppendText(str1);
            AppendBoxText(new List<object>() { msgTxt, box });
            //(new Thread(new ParameterizedThreadStart(AppendBoxText)) { IsBackground = true }).Start(new List<object>() { msgTxt, box });
        }

        private static void AppendBoxText(object obj)
        {
            List<object> objs = obj as List<object>;
            string msgTxt = objs[0].ToString();
            System.Windows.Controls.TextBox box = objs[1] as System.Windows.Controls.TextBox;
            //box.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            //{
                //string msg = msgTxt.Substring(1);
                foreach (var item in msgTxt)
                {
                    box.Text+=item.ToString();
                    Thread.Sleep(100);
                }
            box.Text += Environment.NewLine;
            isWrite = false;
            //}));
        }

        /// <summary>
        /// 保存日志文件
        /// </summary>
        /// <param name="savePath">保存完全路径</param>
        /// <param name="msgTxt">日志内容</param>
        /// <param name="title">日志标题</param>
        public static void SaveLogUseThread(string savePath,string msgTxt,string title="")
        {
            (new Thread(new ParameterizedThreadStart(SaveTxt)) { IsBackground = true }).Start(new List<object>() { savePath, msgTxt,title });
        }
        /// <summary>
        /// 保存日志文件
        /// </summary>
        /// <param name="savePath">保存完全路径</param>
        /// <param name="msgTxt">日志内容</param>
        /// <param name="title">日志标题</param>
        public static void SaveLogUse(string savePath, string msgTxt, string title = "")
        {
            SaveTxt(new List<object>() { savePath, msgTxt, title });
        }

        private static void SaveTxt(object obj)
        {
            try
            {
                List<object> ob = obj as List<object>;
                string path = ob[0].ToString();
                string txt = ob[1].ToString();
                string title = ob[2].ToString();
                txt = string.Format("【 时间：{0} {1} 】 {2}  {3}{4}", DateTime.Now, title, Environment.NewLine, txt, Environment.NewLine);
                SuperFile.FileRWHelper.AppendText(path, txt);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="box">文本框</param>
        /// <param name="msgTxt">日志内容</param>
        /// <param name="title">日志标题</param>
        public static void ShowLogUseThread(System.Windows.Forms.TextBox box, string msgTxt, string title = "")
        {
            (new Thread(new ParameterizedThreadStart(ShowTxt)) { IsBackground = true }).Start(new List<object>() { box, msgTxt, title });
        }
        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="box">文本框</param>
        /// <param name="msgTxt">日志内容</param>
        /// <param name="title">日志标题</param>
        public static void ShowLogUse(System.Windows.Forms.TextBox box, string msgTxt, string title = "")
        {
            ShowTxt(new List<object>() { box, msgTxt, title });
        }
        private static void ShowTxt(object obj)
        {
            List<object> ob = obj as List<object>;
            System.Windows.Forms.TextBox box = ob[0] as System.Windows.Forms.TextBox;
            string txt = ob[1].ToString();
            string title = ob[2].ToString();
            box.AppendText(string.Format("【 时间：{0} {1} 】 {2}  {3}{4}", DateTime.Now, title, Environment.NewLine, txt, Environment.NewLine));
        }
       
    }
}
