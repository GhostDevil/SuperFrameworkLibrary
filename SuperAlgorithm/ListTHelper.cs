using System.Collections.Generic;
using System.Collections;

namespace SuperFramework.SuperAlgorithm
{
    /// <summary>
    /// 排列组合算法
    /// </summary>
    /// <typeparam name="T">T对象</typeparam>
    public static class ListTHelper<T>
    {
        /// <summary>
        /// 递归算法求数组的组合(私有成员)，核心算法
        /// </summary>
        /// <param name="list">返回的范型</param>
        /// <param name="t">所求数组</param>
        /// <param name="n">辅助变量</param>
        /// <param name="m">辅助变量</param>
        /// <param name="b">辅助数组</param>
        /// <param name="M">辅助变量M</param>
        private static void GetCombination(ref List<T[]> list, T[] t, int n, int m, int[] b, int M)
        {
            for (int i = n; i >= m; i--)
            {
                b[m - 1] = i - 1;
                if (m > 1)
                {
                    GetCombination(ref list, t, i - 1, m - 1, b, M);
                }
                else
                {
                    if (list == null)
                    {
                        list = new List<T[]>();
                    }
                    T[] temp = new T[M];
                    for (int j = 0; j < b.Length; j++)
                    {
                        temp[j] = t[b[j]];
                    }
                    list.Add(temp);
                }
            }
        }

        /// <summary>
        /// 递归算法求排列(私有成员)，核心算
        /// </summary>
        /// <param name="list">返回的列表</param>
        /// <param name="t">所求数组</param>
        /// <param name="startIndex">起始标号</param>
        /// <param name="endIndex">结束标号</param>
        private static void GetPermutation(ref List<T[]> list, T[] t, int startIndex, int endIndex)
        {
            if (startIndex == endIndex)//只有一种方法
            {
                if (list == null)
                {
                    list = new List<T[]>();
                }
                T[] temp = new T[t.Length];
                t.CopyTo(temp, 0);
                list.Add(temp);
            }
            else
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    Swap(ref t[startIndex], ref t[i]);
                    GetPermutation(ref list, t, startIndex + 1, endIndex);
                    Swap(ref t[startIndex], ref t[i]);
                }
            }
        }


        /// <summary>
        /// 交换两个变量
        /// </summary>
        /// <param name="a">变量1</param>
        /// <param name="b">变量2</param>
        public static void Swap(ref T a, ref T b)
        {
            (b, a) = (a, b);
        }       

        /// <summary>
        /// 求从起始标号到结束标号的排列，其余元素不变
        /// </summary>
        /// <param name="t">所求数组</param>
        /// <param name="startIndex">起始标号</param>
        /// <param name="endIndex">结束标号</param>
        /// <returns>从起始标号到结束标号排列的范型</returns>
        public static List<T[]> GetPermutation(T[] t, int startIndex, int endIndex)
        {
            if (startIndex < 0 || endIndex > t.Length - 1)
            {
                return null;
            }
            List<T[]> list = new();
            GetPermutation(ref list, t, startIndex, endIndex);
            return list;
        }

        /// <summary>
        /// 返回数组所有元素的全排列
        /// </summary>
        /// <param name="t">所求数组</param>
        /// <returns>全排列的范型</returns>
        public static List<T[]> GetPermutation(T[] t)
        {
            return GetPermutation(t, 0, t.Length - 1);
        }

        /// <summary>
        /// 求数组中n个元素的排列
        /// </summary>
        /// <param name="t">所求数组</param>
        /// <param name="n">元素个数</param>
        /// <returns>数组中n个元素的排列</returns>
        public static List<T[]> GetPermutation(T[] t, int n)
        {
            if (n > t.Length)
            {
                return null;
            }
            List<T[]> list = new();
            List<T[]> c = GetCombination(t, n);
            for (int i = 0; i < c.Count; i++)
            {
                List<T[]> l = new();
                GetPermutation(ref l, c[i], 0, n - 1);
                list.AddRange(l);
            }
            return list;
        }


        /// <summary>
        /// 求数组中n个元素的组合
        /// </summary>
        /// <param name="t">所求数组</param>
        /// <param name="n">元素个数</param>
        /// <returns>数组中n个元素的组合的范型</returns>
        public static List<T[]> GetCombination(T[] t, int n)
        {
            if (t.Length < n)
            {
                return null;
            }
            int[] temp = new int[n];
            List<T[]> list = new();
            GetCombination(ref list, t, t.Length, n, temp, n);
            return list;
        }
        /// <summary>
        /// 判断数组中是否有重复元素，有重复元素则将重复的以-1代替
        /// </summary>
        /// <param name="CheckArrary">待对比数组</param>
        /// <param name="NewArrary">新数组</param>
        /// <returns>如果有发现重复，则返回true </returns>
        public static bool GetCopyArray(ArrayList CheckArrary, ref ArrayList NewArrary)
        {
            bool GetCopy = false;
            ArrayList arrCopy = new();//定义一个arr的副本
            arrCopy = CheckArrary;
            for (int i = 0; i < arrCopy.Count; i++)//从头开始，选择一个arr[i]与后边的进行比较
            {
                for (int j = i + 1; j < arrCopy.Count; j++)//向后遍历
                {
                    if (arrCopy[i] == CheckArrary[j] && arrCopy[i].ToString() != "-1")
                    {
                        GetCopy = true;
                        CheckArrary[j] = "-1";//如果有重复的，在副本中做标记
                    }
                    if (j == arrCopy.Count - 1)
                    {
                        NewArrary.Add(CheckArrary[i]);//遍历到最后一个
                    }
                }
            }
            return GetCopy;
        }
    }
}
