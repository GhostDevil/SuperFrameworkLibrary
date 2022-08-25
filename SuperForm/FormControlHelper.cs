
using SuperFramework.SuperWinAPI;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static SuperFramework.SuperWinAPI.APIStruct;
using static SuperFramework.SuperWinAPI.User32API;
namespace SuperForm
{
    /// <summary>
    /// 日期:2014-09-15
    /// 作者:不良帥
    /// 描述:控件效果辅助方法类
    /// </summary>
    public static class FormControlHelper
    {
        #region  文本框只能输入数字 
        /// <summary>
        /// 文本框只能输入数字
        /// </summary>
        /// <param name="sender">object</param>
        /// <param name="e">KeyPressEventArgs</param>
        public static void TextEditIsNum(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        #endregion

        #region  控件加入水印文字 

        private const int EM_SETCUEBANNER = 0x1501;
        private const int EM_GETCUEBANNER = 0x1502;
        /// <summary>
        /// 控件加入水印文字
        /// </summary>
        /// <param name="control">控件id</param>
        /// <param name="text">水印内容</param>
        public static void SetCueText(Control control, string text)
        {
            if (control is ComboBox)
            {
                COMBOBOXINFO info = GetComboBoxInfo(control);
                SendMessage(info.hwndItem, EM_SETCUEBANNER, 0, text);
            }
            else
            {
                SendMessage(control.Handle, EM_SETCUEBANNER, 0, text);
            }
        }

        private static COMBOBOXINFO GetComboBoxInfo(Control control)
        {
            COMBOBOXINFO info = new COMBOBOXINFO();
            //a combobox is made up of three controls, a button, a list and textbox;
            //we want the textbox
            info.cbSize = Marshal.SizeOf(info);
            User32API.GetComboBoxInfo(control.Handle, ref info);
            return info;
        }
        /// <summary>
        /// 获取控件水印内容
        /// </summary>
        /// <param name="control">控件id</param>
        /// <returns>返回水印内容</returns>
        public static string GetCueText(Control control)
        {
            StringBuilder builder = new StringBuilder();
            if (control is ComboBox)
            {
                COMBOBOXINFO info = new COMBOBOXINFO();
                //a combobox is made up of two controls, a list and textbox;
                //we want the textbox
                info.cbSize = Marshal.SizeOf(info);
                User32API.GetComboBoxInfo(control.Handle, ref info);
                SendMessage(info.hwndItem, EM_GETCUEBANNER, 0, builder);
            }
            else
            {
                SendMessage(control.Handle, EM_GETCUEBANNER, 0, builder);
            }
            return builder.ToString();
        }
        #endregion

        #region  文本框只能输入时间 
        /// <summary>
        /// 文本框只能输入时间
        /// </summary>
        /// <param name="sender">文本框对象</param>
        /// <param name="e">KeyPressEventArgs</param>
        public static void TextEditIsTime(TextBox sender, KeyPressEventArgs e)
        {
            string oldtxt = sender.Text;
            if (oldtxt.Length > 5)
            {
                e.Handled = true;
            }
            int cursorPos = sender.SelectionStart;
            if (cursorPos == 2)
            {
                cursorPos = 3;
            }
            System.Text.RegularExpressions.Match match;
            switch (cursorPos)
            {
                case 0:
                    if (e.KeyChar.ToString() == "2")
                    {
                        if (int.Parse(oldtxt.Substring(1, 1)) > 3)
                        {
                            //小时最大是24小时，输入的小时单位上十位的数字如果是2，那么小时单位上个位的数字不能大于3，默认变成0
                            oldtxt = string.Format("{0}0{1}", oldtxt.Substring(0, 1), oldtxt.Substring(2, oldtxt.Length - 2));
                        }
                    }
                    match = System.Text.RegularExpressions.Regex.Match(e.KeyChar.ToString(), "[0-2]");
                    break;
                case 1:
                    if (oldtxt.Substring(0, 1) != "2")
                    {
                        //小时最大是24小时，如果小时单位上十位的数字不是2，那么小时单位上个位的数字可以为0~9
                        match = System.Text.RegularExpressions.Regex.Match(e.KeyChar.ToString(), "[0-9]");
                    }
                    else
                    {
                        //小时最大是24小时，如果小时单位上十位的数字是2，那么小时单位上个位的数字可以为0~3
                        match = System.Text.RegularExpressions.Regex.Match(e.KeyChar.ToString(), "[0-3]");
                    }

                    break;
                case 3:
                    //分钟最大是59分钟，所以分钟单位上十位的数字只能是0~5
                    match = System.Text.RegularExpressions.Regex.Match(e.KeyChar.ToString(), "[0-5]");
                    break;
                case 4:
                    //分钟最大是59分钟，所以分钟单位上个位的数字只能是0~9
                    match = System.Text.RegularExpressions.Regex.Match(e.KeyChar.ToString(), "[0-9]");
                    break;
                default:
                    match = System.Text.RegularExpressions.Regex.Match("x", "[0-2]");
                    break;
            }
            if (match.Success)
            {
                string s = oldtxt.Substring(0, cursorPos) + e.KeyChar.ToString() +
                    oldtxt.Substring(cursorPos + 1, oldtxt.Length - cursorPos - 1);
                sender.Text = s;
                //如果光标在小时的个位的位置上则直接跳到分钟十位的位置上
                sender.Select(cursorPos + (cursorPos != 1 ? 1 : 2), 1);
            }
            e.Handled = true;
        }
        #endregion

    }
}
    
