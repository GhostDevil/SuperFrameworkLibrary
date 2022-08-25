using System.Text;

namespace System
{
    public static class IntegerEx
    {

        /// <summary>
        /// 把1,2,3,...,35,36转换成A,B,C,...,Y,Z
        /// </summary>
        /// <param name="number">要转换成字母的数字（数字范围在闭区间[1,36]）</param>
        /// <returns>返回字母</returns>
        public static string NumberToChar(this int number)
        {
            if (1 <= number && 36 >= number)
            {
                int num = number + 64;
                ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                byte[] btNumber = new byte[] { (byte)num };
                return asciiEncoding.GetString(btNumber);
            }
            return "数字不在转换范围内";
        }
        /// <summary>
        /// 数字转字母
        /// </summary>
        /// <param name="number">要转换成字母的数字（数字范围在闭区间[65,90]）</param>
        /// <returns>返回字母</returns>
        public static string NumToChar(this int number)
        {
            if (65 <= number && 90 >= number)
            {
                ASCIIEncoding asciiEncoding = new ASCIIEncoding();
                byte[] btNumber = new byte[] { (byte)number };
                return asciiEncoding.GetString(btNumber);
            }
            return "数字不在转换范围内";
        }
        /// <summary>
        /// 获取来自int64的byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetByteFromInt64(this ulong value)
        {
            byte[] int64Byte = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                int64Byte[i] = (byte)(value % 0x100);
                value /= 0x100;
            }
            return int64Byte;
        }
        /// <summary>
        /// 获取来自int16的byte
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static public byte[] GetByteFromInt16(this ushort value)
        {
            byte[] int16Byte = new byte[2];
            for (int i = 0; i < 2; i++)
            {
                int16Byte[i] = (byte)(value % 0x100);
                value /= 0x100;
            }
            return int16Byte;
        }
    }
}
