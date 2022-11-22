using SuperFramework.SuperConvert;
using System.Text;

namespace System
{
    public static class CollectionsEx
    {
        public static T[] Add<T>(this T[] array, T item)
        {
            int _count = array.Length;
            Array.Resize(ref array, _count + 1);
            array[_count] = item;
            return array;
        }
        /// <summary>
        /// Array添加
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="sourceArray">Array</param>
        /// <param name="addArray">Array</param>
        /// <returns>返回新的Array</returns>
        public static T[] AddRange<T>(this T[] sourceArray, T[] addArray)
        {
            int _count = sourceArray.Length;
            int _addCount = addArray?.Length??0;
            if(_addCount ==0)
                return sourceArray;
            Array.Resize(ref sourceArray, _count + _addCount);
            addArray.CopyTo(sourceArray, _count);
            return sourceArray;
        }
        
        /// <summary>
        /// 将一组元素添加到原数组的指定位置处
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="arrayJoin">加入的数组</param>
        /// <param name="index">开始索引</param>
        /// <returns></returns>
        public static T[] InsertRange<T>(this T[] array, int index, T[] arrayJoin)
        {
            if (((array as Array)?.Length ?? 0) == 0)
                return arrayJoin ?? (T[])Convert.ChangeType(Array.CreateInstance(array.GetType().GetElementType(), 0), typeof(T[]));
            if (((arrayJoin as Array)?.Length ?? 0) == 0)
                return array;
            //新长度空数组
            var newArray = Array.CreateInstance(array.GetType().GetElementType(), array.Length + arrayJoin.Length);
            //截断从index开始
            var arrayleft = array.GetRange(0, index + 1);
            var arrayright = array.GetRange(index + 1, array.Length - 1 - index);

            Array.Copy(arrayleft, 0, newArray, 0, arrayleft.Length);
            if (arrayJoin.Length > 0)
                Array.Copy(arrayJoin, 0, newArray, arrayleft.Length, arrayJoin.Length);
            Array.Copy(arrayright, 0, newArray, arrayleft.Length + arrayJoin.Length, arrayright.Length);
            return (T[])Convert.ChangeType(newArray, typeof(T[]));
        }
        /// <summary>
        /// 获取指定数量的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array">集合</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static T[] GetRange<T>(this T[] array, int startIndex, int count)
        {

            var newArray = Array.CreateInstance(array.GetType().GetElementType(), count);
            Array.Copy(array, startIndex, newArray, 0, count);
            return (T[])Convert.ChangeType(newArray, typeof(T[]));
        }
        /// <summary>
        /// 移除指定位置开始的指定数量的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startIndex">开始处索引</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        public static T[] RemoveRange<T>(this T[] array, int startIndex, int count)
        {
            var newArray = Array.CreateInstance(array.GetType().GetElementType(), array.Length - count);
            if (startIndex == 0)
            {
                Array.Copy(array, startIndex + count, newArray, 0, newArray.Length);
            }
            else if (startIndex + count == array.Length)
            {
                Array.Copy(array, 0, newArray, 0, newArray.Length);
            }
            else
            {
                Array.Copy(array, 0, newArray, 0, startIndex);
                Array.Copy(array, startIndex + count, newArray, startIndex, newArray.Length - startIndex);
            }
            return (T[])Convert.ChangeType(newArray, typeof(T[]));
        }

        #region char[]
        /// <summary>
        /// char数组转换字符串
        /// </summary>
        /// <param name="chr">char数组</param>
        /// <returns>返回字符串</returns>        
        public static string ToString(this char[] chr)
        {
            string out_str;
            out_str = new string(chr);
            int i = out_str.IndexOf('\0', 0);
            if (i == -1)
                i = 16;
            return out_str.Substring(0, i);
        }
        #endregion

        #region string[]
        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <returns>返回清除重复后的字符串</returns>
        public static string[] DistinctArray(this string[] strArray) => strArray.DistinctArray(0);
        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <param name="maxElementLength">字符串数组中单个元素的最大长度</param>
        /// <returns>返回清除重复后的字符串</returns>
        public static string[] DistinctArray(this string[] strArray, int maxElementLength)
        {
            Collections.Hashtable h = new();

            foreach (string s in strArray)
            {
                string k = s;
                if (maxElementLength > 0 && k.Length > maxElementLength)
                {
                    k = k.Substring(0, maxElementLength);
                }
                h[k.Trim()] = s;
            }

            string[] result = new string[h.Count];

            h.Keys.CopyTo(result, 0);

            return result;
        }
        #endregion

        #region byte[]
        /// <summary>
        /// 将BCD字节数字转换为字符串
        /// </summary>
        /// <param name="bcdArray">BCD字节数组</param>
        /// <returns></returns>
        public static string BCDToString(this byte[] bcdArray)
        {
            Collections.Generic.List<byte> data = new();
            for (int i = 0; i < bcdArray.Length; i++, i++)
            {
                data.Add(ByteHelper.BCDToByte(bcdArray[i], bcdArray[i + 1]));
            }
            string str = Encoding.Default.GetString(data.ToArray());
            return str;

        }


        /// <summary>
        /// 字节数组转16进制字符串
        /// </summary>
        /// <param name="byteData"></param>
        /// <param name="splitStr">分割符</param>
        /// <returns></returns>
        public static string ToHexStr(this byte[] byteData, int length, string splitStr = " ")
        {
            StringBuilder builder = new();
            for (int i = 0; i < length; i++)
            {
                builder.Append($"{byteData[i]:X2}{splitStr}");
            }
            return builder.ToString().Trim();
        }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <param name="byteData">字节数组Byte[]</param>
        /// <param name="myEncoding">编码方式</param>
        /// <returns></returns>
        public static string ToString(this byte[] byteData, Encoding myEncoding)
        {
            int i, lngCount;
            StringBuilder aTemp = new(10000);
            lngCount = byteData.Length;
            for (i = 0; i < lngCount; i += 10000)
            {
                aTemp.Append(myEncoding.GetString(byteData, i, lngCount >= i + 10000 ? 10000 : lngCount - i));
            }
            if (i <= lngCount)
            {
                aTemp.Append(myEncoding.GetString(byteData, i, lngCount - i));
            }
            return aTemp.ToString();
        }

        /// <summary>
        /// 转换为int
        /// </summary>
        /// <param name="data">需要转换成整数的byte数组</param>
        public static int ToInt32(this byte[] data)
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
        /// <summary>
        /// CRC16校验
        /// </summary>
        /// <param name="data">校验的字节数组</param>         
        /// <param name="length">校验的数组长度</param>         
        /// <param name="isHighBefore">高位是否在前</param>
        /// <returns>校验值</returns>
        public static string GetCRC16(this byte[] data, int arrayLength, bool isHighBefore = true)
        {
            string returnVslue = string.Empty;
            byte[] temdata;
            int xda, xdapoly;
            int i, j, xdabit;
            xda = 0xFFFF;
            xdapoly = 0xA001;

            for (i = 0; i < arrayLength; i++)
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
            if (isHighBefore)
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
        /// CRC16校验设置
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void SetCRC16(this byte[] data, bool isHighBefore = false)
        {
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
            if (isHighBefore)
            {
                data[^1] = (byte)(xda & 0xFF);
                data[^2] = (byte)(xda >> 8);
            }
            else
            {
                data[^2] = (byte)(xda & 0xFF);
                data[^1] = (byte)(xda >> 8);
            }
        }

        /// <summary>
        /// CRC16校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckCRC16(this byte[] data, bool isHighBefore = false)
        {
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
            if (isHighBefore)
                return data[^1] == (byte)(xda & 0xFF) && data[^2] == (byte)(xda >> 8);
            else
                return data[^2] == (byte)(xda & 0xFF) && data[^1] == (byte)(xda >> 8);
        }
        public static byte[] SetCRC16_X25(this byte[] data, int arrayLength = -1, bool isHighBefore = false)
        {
            if (arrayLength == -1)
            {
                arrayLength = data.Length;
            }
            byte[] currentBytes = new byte[arrayLength + 2];
            Array.Copy(data, currentBytes, arrayLength);
            int xda, xdapoly;
            int i, j, xdabit;
            xda = 0xFFFF;
            xdapoly = 0x8408;
            for (i = 0; i < arrayLength; i++)
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
            if (isHighBefore)
            {
                currentBytes[^2] = (byte)(xda & 0xFF);
                currentBytes[^1] = (byte)(xda >> 8);
            }
            else
            {
                currentBytes[^1] = (byte)(xda & 0xFF);
                currentBytes[^2] = (byte)(xda >> 8);
            }
            return currentBytes;
        }

        /// <summary>
        /// CRC16_X25 校验
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool CheckCRC16_X25(this byte[] data)
        {
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

            return data[^2] == (byte)(xda & 0xFF) && data[^1] == (byte)(xda >> 8);
        }


        public static byte BBC(this byte[] data, int length)
        {
            byte checkCode = 0;
            for (int i = 0; i < length; i++)
            {
                checkCode ^= data[i];
            }
            return checkCode;
        }
        #endregion
    }
}
