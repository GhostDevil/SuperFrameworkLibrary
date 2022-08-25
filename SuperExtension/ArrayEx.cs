using System.Collections;

namespace System
{
    public static class ArrayEx
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
            int _addCount = addArray.Length;
            Array.Resize(ref sourceArray, _count + _addCount);
            addArray.CopyTo(sourceArray, _count);
            return sourceArray;
        }
        public static int IndexOfBlock<T>(this T array, T array1, int startIndex = -1) where T : IEnumerable
        {
            var arrayLen = (array as Array)?.Length ?? 0;
            var array1Len = (array1 as Array)?.Length ?? 0;
            if (arrayLen < array1Len)
                return -1;
            FIND_FRAME:
            var index = Array.IndexOf(array as Array, (array1 as Array).GetValue(0), startIndex + 1);
            if (index > -1 && index + array1Len <= arrayLen)
            {
                startIndex = index;
                for (int i = 0; i < array1Len; i++)
                {
                    if (!(array1 as Array).GetValue(i).Equals((array as Array).GetValue(startIndex + i)))
                        goto FIND_FRAME;
                }
            }
            return startIndex;
        }

        public static T Join<T>(this T array, T array1) where T : IEnumerable
        {
            if (((array as Array)?.Length ?? 0) == 0)
                return array1 != null ? array1 : (T)Convert.ChangeType(Array.CreateInstance(array.GetType().GetElementType(), 0), typeof(T));
            if (((array1 as Array)?.Length ?? 0) == 0)
                return array;
            var newArray = Array.CreateInstance(array.GetType().GetElementType(), (array as Array).Length + (array1 as Array).Length);
            if ((array as Array).Length > 0)
                Array.Copy((array as Array), 0, newArray, 0, (array as Array).Length);
            if ((array1 as Array).Length > 0)
                Array.Copy((array1 as Array), 0, newArray, (array as Array).Length > 0 ? (array as Array).Length : 0, (array1 as Array).Length);
            return (T)Convert.ChangeType(newArray, typeof(T));
        }

        public static T GetBlock<T>(this T array, int startIndex, int count) where T : IEnumerable
        {
            if (startIndex > (array as Array).Length)
                throw new IndexOutOfRangeException("起始位置必须在本数组长度之内!");
            if ((array as Array).Length - startIndex < count)
                throw new IndexOutOfRangeException("长度超出最大允许范围!");
            var newArray = Array.CreateInstance(array.GetType().GetElementType(), count);
            Array.Copy((array as Array), startIndex, newArray, 0, count);
            return (T)Convert.ChangeType(newArray, typeof(T));
        }

        public static T RemoveBlock<T>(this T array, int startIndex, int count) where T : IEnumerable
        {
            if (startIndex > (array as Array).Length)
                throw new IndexOutOfRangeException("起始位置必须在本数组长度之内!");
            if ((array as Array).Length - startIndex < count)
                throw new IndexOutOfRangeException("长度超出最大允许范围!");
            if (count == 0) return array;
            var newArray = Array.CreateInstance(array.GetType().GetElementType(), (array as Array).Length - count);
            if (startIndex == 0)
            {
                Array.Copy(array as Array, startIndex + count, newArray, 0, newArray.Length);
            }
            else if (startIndex + count == (array as Array).Length)
            {
                Array.Copy(array as Array, 0, newArray, 0, newArray.Length);
            }
            else
            {
                Array.Copy(array as Array, 0, newArray, 0, startIndex);
                Array.Copy(array as Array, startIndex + count, newArray, startIndex, newArray.Length - startIndex);
            }
            return (T)Convert.ChangeType(newArray, typeof(T));
        }
    }
}
