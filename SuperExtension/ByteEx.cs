namespace System
{
    public static class ByteEx
    {
        /// <summary>
        /// 获取字节中指定bit位的值：1 或 0
        /// </summary>
        /// <param name="pStatus">判断的参数</param>
        /// <param name="pPos">第几位</param>
        /// <returns>返回字节中指定bit位的值：1 或 0</returns>
        public static byte GetPositionBit(this byte _,uint pStatus, int pPos)
        {
            //转换高低字节
            uint a = (uint)(1 << (pPos + 1));
            uint b = (uint)(1 << pPos);
            if (pStatus % a >= b)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// 设置byte某一位的值 ：1 或 0
        /// </summary>
        /// <param name="data">byte对象</param>
        /// <param name="index">要设置的位， 值从低到高为 1-8</param>
        /// <param name="flag">要设置的值 true / false</param>
        /// <returns>返回新的byte</returns>
        public static byte SetByteBitValue(this byte data, int index, bool flag)
        {
            if (index > 8 || index < 1)
                throw new ArgumentOutOfRangeException();
            int v = index < 2 ? index : (2 << (index - 2));
            return flag ? (byte)(data | v) : (byte)(data & ~v);
        }
        /// <summary>
        /// 将字节数据转换为BCD编码
        /// </summary>
        /// <param name="rawByte">字节数据</param>
        /// <param name="isHight">是否转换为高位字节，默认为false</param>
        /// <returns></returns>
        public static byte ByteToBCD(this byte rawByte, bool isHight = false)
        {
            byte data = isHight ? (byte)(rawByte >> 4) : (byte)(rawByte & 0x0f);
            data = (data >= 0 && data <= 9) ? (byte)(48 + data) : (byte)(55 + data);
            return data;
        }

        /// <summary>
        /// 将byte转换为一个长度为8的byte数组，数组每个值代表bit 
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static byte[] GetBooleanArray(this byte b)
        {
            byte[] array = new byte[8];
            for (int i = 7; i >= 0; i--)
            {
                array[i] = (byte)(b & 1);
                b = (byte)(b >> 1);
            }
            return array;
        }
        /// <summary>
        /// 把byte转为bit字符串
        /// </summary>
        /// <param name="b">byte 值</param>
        /// <returns>返回bit字符串</returns>
        public static string ByteToBit(this byte b)
        {
            return ""
            + (byte)((b >> 7) & 0x1) + (byte)((b >> 6) & 0x1)
            + (byte)((b >> 5) & 0x1) + (byte)((b >> 4) & 0x1)
            + (byte)((b >> 3) & 0x1) + (byte)((b >> 2) & 0x1)
            + (byte)((b >> 1) & 0x1) + (byte)((b >> 0) & 0x1);
        }
        #region 对byte逐位异或进行偶校验并返回校验结果
        /// <summary>
        /// 对byte逐位异或进行偶校验并返回校验结果
        /// </summary>
        /// <param name="AByte">要取bit值的byte，一个byte有8个bit</param>
        /// <returns>如果byte里有偶数个1则返回true，如果有奇数个1则返回false</returns>
        public static bool EvenParityCheck(this byte AByte)
        {
            return !GetBit(AByte, 0) ^ GetBit(AByte, 1) ^ GetBit(AByte, 2) ^ GetBit(AByte, 3)
            ^ GetBit(AByte, 4) ^ GetBit(AByte, 5) ^ GetBit(AByte, 6) ^ GetBit(AByte, 7);
        }
        #endregion

        #region 取一个byte中的第几个bit的值
        /// <summary>
        /// 取一个byte中的第几个bit的值
        /// </summary>
        /// <param name="AByte">要取bit值的byte，一个byte有8个bit</param>
        /// <param name="iIndex">在byte中要取bit的位置，一个byte从左到右的位置分别是0,1,2,3,4,5,6,7</param>
        /// <returns>返回bit的值</returns>
        public static bool GetBit(this byte AByte, int iIndex)
        {
            //将要取的bit右移到第一位，再与1与运算将其它位置0
            return (AByte >> (7 - iIndex) & 1) != 0 ? true : false;
        }
        #endregion


    }
}
