using System;
using System.Collections;

namespace SuperFramework
{
    public class PasswordHelper
    {
        /// <summary>
        /// 获取密码表
        /// </summary>
        /// <param name="minLength">最短位数</param>
        /// <param name="maxLength">最长位数</param>
        /// <param name="MyCode">生成的所有密码</param>
        /// <param name="codemap">密码元素表</param>
        /// <param name="addCodemap">向缺省密码元素表追加内容</param>
        public static void GetPasswordList(int minLength, int maxLength, ref ArrayList MyCode,string codemap= "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ",string addCodemap="")
        {
            //这个是密码表，还可以任意添加./+-*/之类的特殊符号，自由发挥，密码就是这些元素的组合
            //const string codemap = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            codemap += addCodemap;
            char[] charSet = codemap.ToCharArray();
            int length = codemap.Length;
            for (int i = minLength; i <= maxLength; i++)
            {
                long maxNum = (long)Math.Pow(length, i);
                for (long j = 0; j < maxNum; j++)
                {
                    //生成所有可能性的密码
                    MyCode.Add(ConvertToAny(j, i, length, charSet));
                }
            }
        }

        /// <summary>
        /// 将指定数字转换为N进制字符串 - 此方法爆破专用
        /// </summary>
        /// <param name="value">要转换的数字</param>
        /// <param name="length">长度</param>
        /// <param name="maplength">密码表长度</param>
        /// <param name="charSet">密码表的char[]形式</param>
        /// <returns>N进制表示格式</returns>
        public static string ConvertToAny(long value, int length, int maplength, char[] charSet)
        {
            string sixtyNum = string.Empty;
            if (value < maplength)
            {
                sixtyNum = charSet[value].ToString().PadLeft(length, '0');
            }
            else
            {
                long result = value;
                while (result > 0)
                {
                    long val = result % maplength;
                    sixtyNum = charSet[val] + sixtyNum;
                    result /= maplength;
                }
                sixtyNum = sixtyNum.PadLeft(length, '0');
            }
            return sixtyNum;
        }
    }
}
