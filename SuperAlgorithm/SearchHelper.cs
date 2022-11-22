using System.Collections.Generic;

namespace SuperFramework.SuperAlgorithm
{
    /// <summary>
    /// 查找算法集合
    /// </summary>
    public static class SearchHelper
    {
        #region 顺序查找
        /// <summary>
        /// 顺序查找
        /// </summary>
        /// <param name="ary">int数组</param>
        /// <param name="target">查找对象</param>
        /// <returns>返回查找对象所在数组下标</returns>
        public static int SequenceSearch(int[] ary, int target)
        {
            for (int i = 0; i < ary.Length; i++)
            {
                if (ary[i] == target)
                {
                    //找到了就返回找到的位置
                    return i;
                }
            }
            //没找到就返回-1，表示没找到
            return -1;
        }
        /// <summary>
        /// 顺序查找
        /// </summary>
        /// <param name="ary">int数组</param>
        /// <param name="target">查找对象</param>
        /// <returns>返回查找对象所在数组的一组下标</returns>
        public static List<int> SequenceFind(int[] ary, int target)
        {
            List<int> index = new();
            for (int i = 0; i < ary.Length; i++)
            {
                if (ary[i] == target)
                {
                    //找到了就返回找到的位置
                    index.Add(i);
                }
            }
            //没找到就返回-1，表示没找到
            return index;
        }
        #endregion

        #region 二分查找
        /// <summary>
        /// 二分查找，必须使用排序后的数组。
        /// </summary>
        /// <param name="ary">int数组</param>
        /// <param name="target">查找对象</param>
        /// <returns>返回查找对象所在数组下标</returns>
        public static int Find(int[] ary, int target)
        {
            int startIndex = 0;
            int endIndex = ary.Length - 1;
            int middleIndex;
            int middle;
            int targetIndex = -1;
            while (true)
            {
                //中点索引
                middleIndex = (endIndex + startIndex) / 2;
                //中点值
                middle = ary[middleIndex];
                //如果起始索引大于结束索引
                //那么查找结束。
                if (startIndex > endIndex)
                    break;
                //如果中点值碰巧就是目标
                //直接返回中点索引
                if (middle == target)
                {
                    targetIndex = middleIndex;
                    break;
                }
                //如果中点值大于目标，说明目标在前一半中
                //而且不会是中点索引，那么在范围中把中点
                //索引去掉
                else if (middle > target)
                {
                    //设定结束索引，抛弃中点索引
                    endIndex = middleIndex - 1;
                }
                else //如果中点值小于目标，说明目标在后一半
                     //而且肯定不会在中点索引，那么在范围中把
                     //中点索引抛弃
                {
                    //设定起始索引，抛弃中点索引
                    startIndex = middleIndex + 1;
                }
            }
            return targetIndex;
        }
        #endregion

        #region 获取第k大的元素
        /// <summary>
        /// 获取第k大的元素
        /// </summary>
        /// <param name="ary">要查找的序列</param>
        /// <param name="lowindex">查找范围的起始索引</param>
        /// <param name="highindex">查找范围的结束索引</param>
        /// <param name="k">第几大</param>
        /// <returns>第k大的元素</returns>
        public static int GetMaxkth(int[] ary, int lowindex, int highindex, int k)
        {
            //这一句是用来防止所要找的k大于序列的长度或小于零的情况
            //还有可能是，当序列中只有一个元素的情况
            if (lowindex == highindex)
                return ary[lowindex];
            //此处的代码是用来防止找到序列之外
            //当lowindex大于highindex的时候，就是要超范围了
            if (lowindex > highindex)
            {
                //如果结束索引小于零，那么到了最开头也没找到
                //那么lowindex应该是零，那么就返回起始索引处
                //的值
                if (highindex < 0)
                    return ary[lowindex];
                //如果highindex大于零，那么到了最末尾也没找到
                //那么highindex应该是最后一个元素的索引。
                return ary[highindex];
            }
            //基准元素的位置（在序列中的索引，不是第几个）
            int index = Partition(ary, lowindex, highindex);
            //基准元素在序列中的相对位置
            //这个相对位置，第一次是针对整个序列的
            //以后就是序列的一部分了，因为第二次的
            //时候，只需查找序列的一部分就可以了。
            //另外一种解释是：relativeindex是左边一部分序列的长度
            int relativeindex = index - lowindex + 1;
            //如果相对位置就是k，那意味着就找到了
            if (relativeindex == k)
                return ary[index];
            //如果相对位置大于k，那么意味着要找的元素在前一部分
            if (relativeindex > k)
            {
                //前一部分的范围是lowindex到index-1，减一的原因是
                //既然relativeindex不是第k个，那么干脆抛弃它，反正
                //已经用index将序列分成两部分了，这次只在前一部分中找
                //所以要找的位置还是k
                return GetMaxkth(ary, lowindex, index - 1, k);
            }
            else
            {
                //如果相对位置小于k，那么意味着要找的元素在后一部分
                //后一部分的范围是index+1到highindex，加一的原因和减一类似
                //既然在后一部分找，那么k应该是k-relativeindex了
                return GetMaxkth(ary, index + 1, highindex, k - relativeindex);
            }

        }
        #endregion

        #region 以序列中的第一个元素为基准，将序列划分成大于这个元素和小于这个元素的两部分。
        /// <summary>
        /// 以序列中的第一个元素为基准，
        /// 将序列划分成大于这个元素和
        /// 小于这个元素的两部分。
        /// </summary>
        /// <param name="ary">序列</param>
        /// <param name="lowindex">范围的起始索引</param>
        /// <param name="highindex">范围的结束索引</param>
        /// <returns>基准元素的最终所在的索引</returns>
        public static int Partition(int[] ary, int lowindex, int highindex)
        {
            //以起始索引的元素为基准
            int middle = ary[lowindex];
            int start = lowindex;
            int end = highindex;
            //当起始索引小于结束索引
            while (start < end)
            {
                //下面两个while意味着，每次等比例的交换大值和小值
                //从后向前找一个大于基准的元素
                while (start < end && ary[end] <= middle)//如果要第k小的，那么将小于等于改成大于等于
                    end--;
                //将找到的和start所在的元素进行调换
                swap(ref ary[start], ref ary[end]);

                //从前向后找一个小于基准的元素
                while (start < end && ary[start] >= middle)//如果要第k小的，那么将大于等于改成小于等于
                    start++;
                //将找到的和end随着的元素进行调换
                swap(ref ary[start], ref ary[end]);
            }
            return start;
        }
        #endregion

        /// <summary>
        /// 交换两个元素
        /// </summary>
        /// <param name="x">元素1</param>
        /// <param name="y">元素2</param>
        static void swap(ref int x, ref int y)
        {
            (y, x) = (x, y);
        }
    }
}
