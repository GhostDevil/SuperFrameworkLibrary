namespace SuperFramework.SuperConvert
{
    /// <summary>
    /// <para>说明：byte字节帮助</para>
    /// <para>时间：2017-12-15</para>
    /// <para>作者：不良帥</para>
    /// </summary>
    public static class ByteHelper
    {

        #region 字节与BCD互转
        
        /// <summary>
        /// 将两字节BCD编码转换为单字节数据
        /// </summary>
        /// <param name="hByte">高位BCD编码</param>
        /// <param name="lbyte">低位BCD编码</param>
        /// <returns></returns>
        public static byte BCDToByte(byte hByte, byte lbyte)
        {
            byte[] data = new byte[] { hByte, lbyte };
            for (int i = 0; i < data.Length; i++)
            {
                if ((data[i] >= 48) && (data[i] <= 57))//数字
                {
                    data[i] = (byte)(data[i] - 48);
                }
                else
                {
                    if ((data[i] >= 97) && (data[i] <= 102))//小写字母
                    {
                        data[i] = (byte)(data[i] - 87);
                    }
                    else data[i] = (byte)(data[i] - 55);//大写字母
                }
            }
            return (byte)((data[0] << 4) + data[1]);
        }
        
        
        #endregion

        #region 翻转字节顺序
        /// <summary>
        /// 翻转字节顺序 (16-bit)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ushort ReverseBytes(ushort value)
        {
            return (ushort)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        /// <summary>
        /// 翻转字节顺序 (32-bit)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static uint ReverseBytes(uint value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }


        /// <summary>
        /// 翻转字节顺序 (64-bit)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong ReverseBytes(ulong value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }
        #endregion

       
    }
}
