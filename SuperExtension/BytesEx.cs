using SuperFramework.SuperConvert;
using System.Collections.Generic;
using System.Text;

namespace System.Collections
{
    public static class BytesEx
    {
        /// <summary>
        /// 将BCD字节数字转换为字符串
        /// </summary>
        /// <param name="bcdArray">BCD字节数组</param>
        /// <returns></returns>
        public static string BCDByteArrayToString(this byte[] bcdArray)
        {
            List<byte> data = new List<byte>();
            for (int i = 0; i < bcdArray.Length; i++, i++)
            {
                data.Add(ByteHelper.BCDToByte(bcdArray[i], bcdArray[i + 1]));
            }
            string str = Encoding.Default.GetString(data.ToArray());
            return str;

        }
        #region 字节数组转16进制字符串
        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="bytes">byte 数组</param>
        /// <param name="leng">需要转换的数组长度</param>
        /// <param name="isSplit">是否分割</param>
        /// <param name="splitStr">分割字符串</param>
        /// <returns>返回16进制字符串</returns>
        public static string ByteToHexStr(this byte[] bytes, int leng, bool isSplit = false, string splitStr = " ")
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < leng; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                    if (i < leng - 1)
                        returnStr += splitStr;
                }

            }
            //w = w.Substring(0, w.Length - splitStr.Length);
            return returnStr;
        }
        /// <summary>
        /// 字节数组转16进制数组
        /// </summary>
        /// <param name="bytes">byte 数组</param>
        /// <param name="leng">需要转换的数组长度</param>
        /// <returns>返回16进制数组</returns>
        public static string[] ByteToHexStrs(this byte[] bytes, int leng)
        {
            string[] strs = new string[leng];
            if (bytes != null)
            {
                for (int i = 0; i < leng; i++)
                {
                    strs[i] = bytes[i].ToString("X2");
                }
            }
            return strs;
        }
        #endregion

        /// <summary> 
        /// 字节数组转16进制字符串   
        /// </summary> 
        /// <param name="bytes"></param> 
        /// <returns></returns> 
        public static string ByteToHexStr(this byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }


        /// <summary>
        /// 字节数组转16进制字符串：空格分隔
        /// </summary>
        /// <param name="byteDatas"></param>
        /// <returns></returns>
        public static string ToHexStrFromByte(this byte[] byteDatas)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                builder.Append(string.Format("{0:X2} ", byteDatas[i]));
            }
            return builder.ToString().Trim();
        }
        /// <summary>
        /// 转换指定字节数组为字符串
        /// </summary>
        /// <param name="ByteGet">字节数组Byte[]</param>
        /// <param name="myEncoding">编码方式</param>
        /// <returns></returns>
        public static string StringFromByteArray(this byte[] ByteGet, Encoding myEncoding)
        {
            int i, lngCount;
            StringBuilder aTemp = new StringBuilder(10000);
            lngCount = ByteGet.Length;
            for (i = 0; i < lngCount; i += 10000)
            {
                aTemp.Append(myEncoding.GetString(ByteGet, i, (lngCount >= i + 10000 ? 10000 : lngCount - i)));
            }
            if (i <= lngCount)
            {
                aTemp.Append(myEncoding.GetString(ByteGet, i, (lngCount - i)));
            }
            return aTemp.ToString();
        }

        /// <summary>
        /// 将byte[]转换成int
        /// </summary>
        /// <param name="data">需要转换成整数的byte数组</param>
        public static int BytesToInt32(this byte[] data)
        {
            //如果传入的字节数组长度小于4,则返回0
            if (data.Length < 4)
                return 0;
            //定义要返回的整数
            int num = 0;

            //如果传入的字节数组长度大于4,需要进行处理
            if (data.Length >= 4)
            {
                //创建一个临时缓冲区
                byte[] tempBuffer = new byte[4];

                //将传入的字节数组的前4个字节复制到临时缓冲区
                Buffer.BlockCopy(data, 0, tempBuffer, 0, 4);

                //将临时缓冲区的值转换成整数，并赋给num
                num = BitConverter.ToInt32(tempBuffer, 0);
            }

            //返回整数
            return num;
        }
        #region CRC校验         
        /// <summary> 
        /// CRC高位校验码checkCRCHigh 
        /// </summary> 
        static readonly byte[] checkCRCHigh =
            {
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40, 0x00, 0xC1, 0x81, 0x40,
            0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0, 0x80, 0x41, 0x00, 0xC1,
            0x81, 0x40, 0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41,
            0x00, 0xC1, 0x81, 0x40, 0x01, 0xC0, 0x80, 0x41, 0x01, 0xC0,
            0x80, 0x41, 0x00, 0xC1, 0x81, 0x40
        };
        /// <summary> 
        /// CRC地位校验码checkCRCLow         
        /// </summary> 
        static readonly byte[] checkCRCLow =
            {
            0x00, 0xC0, 0xC1, 0x01, 0xC3, 0x03, 0x02, 0xC2, 0xC6, 0x06,
            0x07, 0xC7, 0x05, 0xC5, 0xC4, 0x04, 0xCC, 0x0C, 0x0D, 0xCD,
            0x0F, 0xCF, 0xCE, 0x0E, 0x0A, 0xCA, 0xCB, 0x0B, 0xC9, 0x09,
            0x08, 0xC8, 0xD8, 0x18, 0x19, 0xD9, 0x1B, 0xDB, 0xDA, 0x1A,
            0x1E, 0xDE, 0xDF, 0x1F, 0xDD, 0x1D, 0x1C, 0xDC, 0x14, 0xD4,
            0xD5, 0x15, 0xD7, 0x17, 0x16, 0xD6, 0xD2, 0x12, 0x13, 0xD3,
            0x11, 0xD1, 0xD0, 0x10, 0xF0, 0x30, 0x31, 0xF1, 0x33, 0xF3,
            0xF2, 0x32, 0x36, 0xF6, 0xF7, 0x37, 0xF5, 0x35, 0x34, 0xF4,
            0x3C, 0xFC, 0xFD, 0x3D, 0xFF, 0x3F, 0x3E, 0xFE, 0xFA, 0x3A,
            0x3B, 0xFB, 0x39, 0xF9, 0xF8, 0x38, 0x28, 0xE8, 0xE9, 0x29,
            0xEB, 0x2B, 0x2A, 0xEA, 0xEE, 0x2E, 0x2F, 0xEF, 0x2D, 0xED,
            0xEC, 0x2C, 0xE4, 0x24, 0x25, 0xE5, 0x27, 0xE7, 0xE6, 0x26,
            0x22, 0xE2, 0xE3, 0x23, 0xE1, 0x21, 0x20, 0xE0, 0xA0, 0x60,
            0x61, 0xA1, 0x63, 0xA3, 0xA2, 0x62, 0x66, 0xA6, 0xA7, 0x67,
            0xA5, 0x65, 0x64, 0xA4, 0x6C, 0xAC, 0xAD, 0x6D, 0xAF, 0x6F,
            0x6E, 0xAE, 0xAA, 0x6A, 0x6B, 0xAB, 0x69, 0xA9, 0xA8, 0x68,
            0x78, 0xB8, 0xB9, 0x79, 0xBB, 0x7B, 0x7A, 0xBA, 0xBE, 0x7E,
            0x7F, 0xBF, 0x7D, 0xBD, 0xBC, 0x7C, 0xB4, 0x74, 0x75, 0xB5,
            0x77, 0xB7, 0xB6, 0x76, 0x72, 0xB2, 0xB3, 0x73, 0xB1, 0x71,
            0x70, 0xB0, 0x50, 0x90, 0x91, 0x51, 0x93, 0x53, 0x52, 0x92,
            0x96, 0x56, 0x57, 0x97, 0x55, 0x95, 0x94, 0x54, 0x9C, 0x5C,
            0x5D, 0x9D, 0x5F, 0x9F, 0x9E, 0x5E, 0x5A, 0x9A, 0x9B, 0x5B,
            0x99, 0x59, 0x58, 0x98, 0x88, 0x48, 0x49, 0x89, 0x4B, 0x8B,
            0x8A, 0x4A, 0x4E, 0x8E, 0x8F, 0x4F, 0x8D, 0x4D, 0x4C, 0x8C,
            0x44, 0x84, 0x85, 0x45, 0x87, 0x47, 0x46, 0x86, 0x82, 0x42,
            0x43, 0x83, 0x41, 0x81, 0x80, 0x40
        };
        /// <summary>         
        /// CRC校验         
        /// </summary> 
        /// <param name="data">校验的字节数组</param>         
        /// <param name="length">校验的数组长度</param>         
        /// <returns>该字节数组的奇偶校验字节</returns> 
        public static byte[] CRC16(this byte[] data, int arrayLength)  //CRC校验        
        {
            byte CRCHigh = 0xFF; //重置CRC高位校验码            
            byte CRCLow = 0xFF; //重置CRC低位校验码            
            byte index; int i = 0;
            while (arrayLength-- > 0)
            {
                index = (byte)(CRCHigh ^ data[i++]);
                CRCHigh = (byte)(CRCLow ^ checkCRCHigh[index]);
                CRCLow = checkCRCLow[index];
            }
            byte[] ReturnData = { data[0], data[1], data[2], data[3], CRCHigh, CRCLow };
            return ReturnData;
        }
       
        /// <summary>
        /// CRC16校验
        /// </summary>
        /// <param name="data"></param>
        /// <param name="IsHighBefore">高位是否在前</param>
        /// <returns></returns>
        public static string CRC16(this byte[] data, bool IsHighBefore = true)
        {
            string returnVslue = string.Empty;
            if (data.Length == 0)
                throw new Exception("调用CRC16校验算法,（低字节在前，高字节在后）时发生异常");
            byte[] temdata = new byte[data.Length + 2];
            int xda, xdapoly;
            int i, j, xdabit;
            xda = 0xFFFF;
            xdapoly = 0xA001;

            for (i = 0; i < data.Length; i++)
            {
                xda ^= data[i];
                for (j = 0; j < 8; j++)
                {
                    xdabit = (byte)(xda & 0x01);
                    xda >>= 1;
                    if (xdabit == 1)
                        xda ^= xdapoly;
                }
            }
            if (IsHighBefore)
            {
                temdata = new byte[2] { (byte)(xda >> 8), (byte)(xda & 0xFF) };
            }
            else
            {
                temdata = new byte[2] { (byte)(xda & 0xFF), (byte)(xda >> 8) };
            }
            foreach (byte b in temdata)
            {
                returnVslue += string.Format("{0:X2}", b);
            }
            return returnVslue;
        }

        /// <summary>
        /// CRC16校验
        /// </summary>
        /// <param name="data"></param>
        /// <param name="IsHighBefore">高位是否在前</param>
        /// <returns></returns>
        public static void SetCRC16(this byte[] data)
        {
            string returnVslue = string.Empty;
            if (data.Length == 0)
                throw new Exception("调用CRC16校验算法,（低字节在前，高字节在后）时发生异常");
            byte[] temdata = new byte[data.Length + 2];
            int xda, xdapoly;
            int i, j, xdabit;
            xda = 0xFFFF;
            xdapoly = 0xA001;

            for (i = 0; i < data.Length - 2; i++)
            {
                xda ^= data[i];
                for (j = 0; j < 8; j++)
                {
                    xdabit = (byte)(xda & 0x01);
                    xda >>= 1;
                    if (xdabit == 1)
                        xda ^= xdapoly;
                }
            }

            data[data.Length - 2] = (byte)(xda & 0xFF);
            data[data.Length - 1] = (byte)(xda >> 8);
        }

        /// <summary>
        /// CRC16校验
        /// </summary>
        /// <param name="data"></param>
        /// <param name="IsHighBefore">高位是否在前</param>
        /// <returns></returns>
        public static bool CRC16(this byte[] data)
        {
            string returnVslue = string.Empty;
            if (data.Length == 0)
                throw new Exception("调用CRC16校验算法,（低字节在前，高字节在后）时发生异常");
            byte[] temdata = new byte[data.Length + 2];
            int xda, xdapoly;
            int i, j, xdabit;
            xda = 0xFFFF;
            xdapoly = 0xA001;

            for (i = 0; i < data.Length - 2; i++)
            {
                xda ^= data[i];
                for (j = 0; j < 8; j++)
                {
                    xdabit = (byte)(xda & 0x01);
                    xda >>= 1;
                    if (xdabit == 1)
                        xda ^= xdapoly;
                }
            }

            return data[data.Length - 2] == (byte)(xda & 0xFF) && data[data.Length - 1] == (byte)(xda >> 8);
        }
        public static byte[] SetCRC16_X25(this byte[] data, int len = -1)
        {
            if (data.Length == 0)
                throw new Exception("调用CRC16校验算法,（低字节在前，高字节在后）时发生异常");
            if (len == -1)
            {
                len = data.Length;
            }
            byte[] currentBytes = new byte[len + 2];
            Array.Copy(data, currentBytes, len);
            int xda, xdapoly;
            int i, j, xdabit;
            xda = 0xFFFF;
            xdapoly = 0x8408;
            for (i = 0; i < len; i++)
            {
                xda ^= data[i];
                for (j = 0; j < 8; j++)
                {
                    xdabit = (byte)(xda & 0x01);
                    xda >>= 1;
                    if (xdabit == 1)
                        xda ^= xdapoly;
                }
            }

            currentBytes[i++] = (byte)(xda & 0xFF);
            currentBytes[i] = (byte)(xda >> 8);
            return currentBytes;
        }

        /// <summary>
        /// CRC16_X25 校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CRC16_X25(this byte[] data)
        {
            if (data.Length == 0)
                throw new Exception("调用CRC16校验算法,（低字节在前，高字节在后）时发生异常");
            int xda, xdapoly;
            int i, j, xdabit;
            xda = 0xFFFF;
            xdapoly = 0x8408;

            for (i = 0; i < data.Length - 2; i++)
            {
                xda ^= data[i];
                for (j = 0; j < 8; j++)
                {
                    xdabit = (byte)(xda & 0x0001);
                    xda >>= 1;
                    if (xdabit == 1)
                        xda ^= xdapoly;
                }
            }

            return data[data.Length - 2] == (byte)(xda & 0xFF) && data[data.Length - 1] == (byte)(xda >> 8);
        }

        #endregion

        public static byte BBC(this byte[] data, int length)
        {
            byte checkCode = 0;
            for (int i = 0; i < length; i++)
            {
                checkCode ^= data[i];
            }
            return checkCode;
        }
    }
}
