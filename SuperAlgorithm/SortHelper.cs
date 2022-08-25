using System;
using System.Data;
using System.Globalization;
using System.Threading;

namespace SuperFramework.SuperAlgorithm
{
    /// <summary>
    /// <para>日期:2015-07-11</para>
    /// <para>作者:不良帥</para>
    /// <para>描述:排序算法集合</para>
    /// </summary>
    public static class SortHelper
    {
        #region 对一个数组进行随机排序
        /// <summary>
        /// 对一个数组进行随机排序
        /// </summary>
        /// <typeparam name="T">数组的类型</typeparam>
        /// <param name="arr">需要随机排序的数组</param>
        public static void GetRandomArray<T>(T[] arr)
        {
            //为随机数对象赋值
            Random rdm = new Random();
            //对数组进行随机排序的算法:随机选择两个位置，将两个位置上的值交换
            //交换的次数,这里使用数组的长度作为交换次数
            int count = arr.Length;
            //开始交换
            for (int i = 0; i < count; i++)
            {
                //生成两个随机数位置
                int randomNum1 = rdm.Next(0, arr.Length);
                int randomNum2 = rdm.Next(0, arr.Length);
                //定义临时变量
                T temp;
                //交换两个随机数位置的值
                temp = arr[randomNum1];
                arr[randomNum1] = arr[randomNum2];
                arr[randomNum2] = temp;
            }
        }
        #endregion

        #region 基于指定列的指定类型的二维数组排序
        /// <summary>
        /// 基于指定列的指定类型的二维数组排序
        /// </summary>
        /// <param name="array">排序数组</param>
        /// <param name="sortCol">排序列的索引</param>
        /// <param name="order">排序规则：降序为 "DESC" 或 "DESCENDING" ，升序为 "ASC" 或者 "ASCENDING".</param>

        public static void Sort<T>(T[,] array, int sortCol, string order)
        {
            int colCount = array.GetLength(1), rowCount = array.GetLength(0);
            if (sortCol >= colCount || sortCol < 0)
                throw new ArgumentOutOfRangeException(nameof(sortCol), "列为必须包含在数组边界中的列。");
            DataTable dt = new DataTable();
            // 以“0”、“1”、“”等方式命名列
            for (int col = 0; col < colCount; col++)
            {
                DataColumn dc = new DataColumn(col.ToString(), typeof(T));
                dt.Columns.Add(dc);
            }
            // 将数据导入数据表
            for (int rowindex = 0; rowindex < rowCount; rowindex++)
            {
                DataRow rowData = dt.NewRow();
                for (int col = 0; col < colCount; col++)
                    rowData[col] = array[rowindex, col];
                dt.Rows.Add(rowData);
            }
            // 使用该列索引=名称+一个可选的顺序
            DataRow[] rows = dt.Select("", sortCol.ToString() + " " + order);
            for (int row = 0; row <= rows.GetUpperBound(0); row++)
            {
                DataRow dr = rows[row];
                for (int col = 0; col < colCount; col++)
                {
                    array[row, col] = (T)dr[col];
                }
            }
            dt.Dispose();
        }
        #endregion

        #region 泛型的排序方法, 此方法适应任何类型的排序。
        /// <summary>
        /// 泛型的排序方法, 此方法适应任何类型的排序。
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="order">数组</param>
        public static void Sort<T>(T[] order) where T : IComparable<T>
        {
            for (int i = 0; i < order.Length - 1; i++)
            {
                for (int j = 0; j < order.Length - 1 - i; j++)
                {
                    if (order[j].CompareTo(order[j + 1]) > 0)
                    {
                        (order[j + 1], order[j]) = (order[j], order[j + 1]);
                    }
                }
            }
        }
        #endregion

        #region 冒泡排序
        /// <summary>
        /// 冒泡排序
        /// </summary>
        /// <param name="ary">int数组</param>
        public static void BubbleSort(int[] ary)
        {
            //需要进行n-1轮
            for (int i = 0; i < ary.Length - 1; i++)
            {
                //跳出的标志
                bool exitFlag = true;
                //每轮进行n-i-1次比较
                for (int j = 0; j < ary.Length - i - 1; j++)
                {
                    //如果相邻的两个数据中的前面的比后面的大
                    //交换两个数据
                    if (ary[j] > ary[j + 1])
                    {
                        (ary[j + 1], ary[j]) = (ary[j], ary[j + 1]);
                        //交换次数加一
                        //如果有交换，那么标志为假，不跳出
                        //那就是说，有可能还有没排好序的元素
                        exitFlag = false;
                    }
                }
                //如果这一轮没有交换，说明后面的都已经排好序了
                //那么跳出
                if (exitFlag)
                    break;
            }
        }
        #endregion

        #region 选择排序
        /// <summary>
        /// 选择排序
        /// </summary>
        /// <param name="ary">int数组</param>
        public static void ChoiceSort(int[] ary)
        {
            //比较要进行序列长度减一次
            for (int i = 0; i < ary.Length - 1; i++)
            {
                //首先认为这一轮中的第一个是最小的
                //记录下最小哪个家伙的位置，最终这
                //个位置是由实力最弱的那个家伙占据的。
                int minIndex = i;
                //我们认为最小的哪个家伙要和其他人
                //都要进行一遍比较
                for (int j = i + 1; j < ary.Length; j++)
                {
                    //如果在比较的过程中有人比他还弱，
                    //那么最弱的头衔要转交
                    if (ary[j] < ary[minIndex])
                    {
                        minIndex = j;
                    }
                    //如果比了一轮，都比我们认为的哪个家伙强
                    //那么他就应该是最小的，
                }
                //一轮走完了以后，minIndex中肯定放的是
                //最弱哪个家伙当前的位置，i是我们为他
                //准备的座位，把他放到他应该在的位置。
                if (i != minIndex)
                {
                    int temp;
                    //交换最弱的哪个和现在在最弱的位置上的元素
                    temp = ary[i];
                    ary[i] = ary[minIndex];
                    ary[minIndex] = temp;
                }
            }
        }
        #endregion

        #region 插入排序
        public static void InsertSort(int[] ary)
        {
            for (int i = 1; i < ary.Length; i++)
            {
                //记录第i个元素的值
                int temp = ary[i];
                //第i个元素的索引
                int index = i;
                //为这个元素找位置
                //temp的位置应该在最后一个比它大的元素之前
                //前提是那些比它大的元素都已经排序好了
                //那么，只要找到最后一个比它大的元素，在大
                //元素所在的位置就是第i个元素应该在的位置
                //那么应该把这个大元素包括大元素以后的大元素
                //统统向后挪，给第i个元素腾出位置
                //从现在这个位置开始一直向前走
                while (index > 0 && ary[index - 1] > temp)//在这一句，认为前面的元素都是有序的
                {
                    //把前面的元素往后挪，可以这么做的原因是原始index中的数据已经在temp中了，不害怕覆盖
                    ary[index] = ary[index - 1];
                    //位置往前走一步
                    index--;
                }
                //比它大的元素都已经向后挪好了，
                //就把这个元素放在腾出的位置上吧
                if (index != i)
                    ary[index] = temp;
            }
        }
        #endregion

        #region 快速排序
        /// <summary>
        /// 递归算法快速排序
        /// </summary>
        /// <param name="ary">需要排序的序列</param>
        /// <param name="beginIndex">需要排序的段的起始索引</param>
        /// <param name="endIndex">需要排序的段的终止索引</param>
        public static void FastSort(int[] ary, int beginIndex, int endIndex)
        {
            //如果起始索引和终止索引一样
            //那么意味着需要排序的这个段
            //只有一个元素，一个元素那还
            //排个屁啊
            if (endIndex <= beginIndex)
                return;
            //以物理位置的中间元素作为中值
            int middle = ary[(beginIndex + endIndex) / 2];
            //设置从前向后遍历的起始值
            int low = beginIndex;
            //设置从后往前遍历的起始值
            int high = endIndex;
            //使劲循环
            while (true)
            {
                //找到第一个比中值大或等于中值的元素
                while (true)
                {
                    if (ary[low] >= middle)
                        break;
                    //要是没找到，那么索引加一
                    //继续看下一个元素是否满足
                    low++;
                }
                //找到第一个比中值小或等于中值的元素
                while (true)
                {
                    if (ary[high] <= middle)
                        break;
                    //要是没找到，那么索引减一
                    //继续看下一个元素是否满足
                    high--;
                }
                //要是从前向后找的指针大于等于从后向前找的指针
                //那就意味着这一轮，把中值左右该交换的交换完了
                //应该开始分成将序列分成两段了，再次进行排序了
                if (low >= high)
                    break;
                //将比中值大的和比中值小的两个元素交换
                //让小的在中值左边，比中值大的在中值右边
                Swap(ref ary[low], ref ary[high]);
                //到这里意味着一次交换已经完成
                //应该进行下一次的寻找了，所以
                //将从前向后的指针加一
                //将从后向前的指针减一
                low++;
                high--;
            }
            //到这里意味着，上一轮的交换已经完成
            //应该将序列分成两部分了
            //如果low=high，那么意味着我们选的中值很带劲
            //正好是序列中最应该在中间的那个
            //如果low>high，那么意味着从后往前找的小值，找到
            //中值前面去了，那么从前往后找的大值肯定是中值
            //那么low-1就是中值，high+1也是中值
            FastSort(ary, beginIndex, low - 1);
            FastSort(ary, high + 1, endIndex);
        }

        /// <summary>
        /// 非递归快速排序
        /// </summary>
        /// <param name="ary">要排序的数组</param>
        /**
         * /// 核心思想：将每次分治的两个序列的高位和低位入栈
        /// 每次都从栈中获取一对高位和低位，分别处理。
        /// 处理过程是：选取高位作为基准位置，从低位开始向
        /// 高位遍历，如果比基准元素小，那么和第i个交换，如
        /// 果有交换，那么i++，等一遍遍历完成后，如果i的位置
        /// 不等于基准位置，那么所选的基准位置的值不是最大的
        /// 而这时候i的位置之前的元素都比基准值小，那么i的位置
        /// 应该是基准值，将i所在位置的值和基准位置进行交换。
        /// 这时候，在i的左右就将序列分成两部分了，一部分比i所
        /// 在位置值小，一部分比i所在位置值大的，然后再次将前
        /// 面一部分和后面一部分的高位和低位分别入栈，再次选
        /// 择基准位置，直到所选择的区间大小小于2，就可以不用
        /// 入栈了。
         * */
        public static void NonrecursiveSort(int[] ary)
        {
            //如果数组中只有1一个元素或空数组，那就没必要排序了。
            if (ary.Length < 2)
                return;
            //数组栈：记录着高位和低位的值
            int[,] stack = new int[2, ary.Length];
            //栈顶部位置
            int top = 0;
            //低位，高位，循环变量，基准点，将数组的高位和低位位置入栈
            stack[1, top] = ary.Length - 1;
            stack[0, top] = 0;
            top++;
            //要是栈顶不空，那么继续
            while (top != 0)
            {
                //将高位和低位出栈，低位：排序开始的位置
                top--;
                int low = stack[0, top];
                //高位：排序结束的位置
                int high = stack[1, top];
                //将高位作为基准位置，基准位置
                int pivot = high;
                int i = low;
                for (int j = low; j < high; j++)
                {
                    //如果某个元素小于基准位置上的值
                    //那么将其和第i位交换，交换完成后
                    //将低位也就是i前进一位，也就是一
                    //轮循环下来以后，比基准位小的都
                    //到前面去了，如果这次选的基准位
                    //就是最大值，那么i最后应该和基准
                    //位重合，如果不重合，那么基准位
                    //应该就不是最大值，因为此时在i之
                    //前的数据都是比基准位的值还小的
                    //那么将基准位的值放到i所在的地方
                    if (ary[j] <= ary[pivot])
                    {
                        (ary[i], ary[j]) = (ary[j], ary[i]);
                        i++;
                    }
                }
                //如果i不是基准位，那么基准位选的就不是最大值
                //而i的前面放的都是比基准位小的值，那么基准位
                //的值应该放到i所在的位置上
                if (i != pivot)
                {
                    (ary[pivot], ary[i]) = (ary[i], ary[pivot]);
                }
                //下面这一段是保存现场的，一轮下来可能保存4个值，其实就是两个高位，两个低位
                //当i-low小于等于1的时候，就不往栈中放了，这就是外层while循环能结束的原因
                //如果从低位到i之间的元素个数多于一个，那么需要再次排序
                if (i - low > 1)
                {
                    //此时不排i的原因是i位置上的元素已经确定了，i前面的都是比i小的，i后面的都是比i大的
                    //所以此处i-1
                    //存高位
                    stack[1, top] = i - 1;
                    //存低位
                    stack[0, top] = low;
                    top++;
                }
                //当high-i小于等于1的时候，就不往栈中放了，这就是外层while循环能结束的原因
                //如果从i到高位之间的元素个数多于一个，那么需要再次排序
                if (high - i > 1)
                {
                    //此时不排i的原因是i位置上的元素已经确定了，i前面的都是比i小的，i后面的都是比i大的
                    //存高位
                    stack[1, top] = high;
                    //所以此处i+1
                    //存低位
                    stack[0, top] = i + 1;
                    top++;
                }
            }
        }
        #endregion

        #region 希尔排序
        /// <summary>
        /// 希尔排序
        /// </summary>
        /// <param name="ary">int数组</param>
        public static void HillSort(int[] ary)
        {
            for (int j = ary.Length / 2; j > 0; j /= 2)
            {
                int inc = j;
                //如果步长为1，那么起始值从索引1开始
                for (int i = inc + 1; i <= ary.Length; i++)
                {
                    int temp = ary[i - 1];
                    int index = i - 1;
                    while (index > inc - 1 && ary[index - inc] > temp)
                    {
                        ary[index] = ary[index - inc];
                        index -= inc;
                    }
                    ary[index] = temp;
                }
            }
        }
        #endregion

        #region 归并排序
        /// <summary>
        /// 归并排序（需要大量的外部空间，以供排序的中间过程使用，使用空间上不经济）
        /// </summary>
        /// <param name="ary">int数组</param>
        /// <returns>返回排序后的数组</returns>
        private static int[] MergeSort(int[] ary)
        {
            //如果待分割的数组中只有一个元素，
            //那么就没必要进行分割了
            if (ary.Length <= 1)
                return ary;
            //找到原始数组的中点
            int middleLength = ary.Length / 2;
            //设定分割后的左数组
            int[] left = new int[middleLength];
            //设定分割后的右数组
            int[] right = new int[ary.Length - middleLength];
            int j = 0;
            int k = 0;
            //填充左数组和右数组
            for (int i = 0; i < ary.Length; i++)
            {
                //原始序列的前一半给左数组
                if (i < middleLength)
                {
                    left[j] = ary[i];
                    j++;
                }
                else//原始序列的后一半给右数组
                {
                    right[k] = ary[i];
                    k++;
                }
            }
            //使用递归继续分割数组
            left = MergeSort(left);
            right = MergeSort(right);
            //知道两个数组的大小都为1的时候
            //再合并两个数组。
            int[] result = MergeSort(left, right);
            //把合并好的数组返回
            return result;
        }

        /// <summary>
        /// 归并排序（需要大量的外部空间，以供排序的中间过程使用，使用空间上不经济）
        /// </summary>
        /// <param name="ary1">int[]数组</param>
        /// <param name="ary2">int[]数组</param>
        /// <returns>返回合并排序后的数组</returns>
        public static int[] MergeSort(int[] ary1, int[] ary2)
        {

            //弄一个新序列，大小为两个序列长度之和
            int[] ary = new int[ary1.Length + ary2.Length];
            //序列1的顶端的指针
            int i = 0;
            //序列2的顶端的指针
            int j = 0;
            //合并后的序列的顶端的指针
            int k = 0;
            while (true)
            {
                //当任意一个序列的数据取完了
                //就停止这个循环
                if (i >= ary1.Length || j >= ary2.Length)
                    break;
                //如果序列1的顶端元素大于序列2的顶端元素
                if (ary1[i] > ary2[j])
                {
                    //那么把序列2顶端的值给合并后序列
                    ary[k] = ary2[j];
                    //序列2的指针下移一位
                    j++;
                }
                else
                {
                    //那么把序列1顶端的值给合并后序列
                    ary[k] = ary1[i];
                    //序列2的指针下移一位
                    i++;
                }
                //合并后序列指针下移一位
                k++;
            }
            while (true)
            {
                //检查序列1中的元素是否取完
                if (i >= ary1.Length)
                    break;
                //如果序列1还没取完
                //继续把序列1中的元素往合并后序列中填
                ary[k] = ary1[i];
                k++;
                i++;
            }
            while (true)
            {
                //检查序列2中的元素是否取完
                if (j >= ary2.Length)
                    break;
                //如果序列2还没取完
                //继续把序列2中的元素往合并后序列中填
                ary[k] = ary2[j];
                k++;
                j++;
            }
            //返回合并后的序列
            return ary;
        }
        #endregion

        #region 堆排序
        /// <summary>
        /// 堆排序
        /// </summary>
        /// <param name="ary">int数组</param>
        public static void HeapSort(int[] ary)
        {
            //创建堆
            CreateHeap(ary, 0, ary.Length - 1);
            for (int i = ary.Length - 1; i > 0; i--)
            {
                //交换
                Swap(ref ary[0], ref ary[i]);
                //再次创建堆
                GetMaxHeap(ary, 0, i - 1);
            }
        }
        /// <summary>
        /// 从某个节点开始构建一次堆
        /// </summary>
        /// <param name="ary">序列</param>
        /// <param name="startNodeIndex">起始索引</param>
        /// <param name="endNodeIndex">结束索引</param>
        private static void GetMaxHeap(int[] ary, int startNodeIndex, int endNodeIndex)
        {
            //设置父节点的左子节点索引
            int leftNodeIndex = 2 * startNodeIndex + 1;
            //先把父节点的值存储下来
            int temp = ary[startNodeIndex];
            //如果父节点的左子节点的索引大于高位，就停止循环
            while (leftNodeIndex <= endNodeIndex)
            {
                //右子节点
                int rightNodeIndex = leftNodeIndex + 1;
                //如果索引没有超范围，并且右子节点大于左子节点
                if ((leftNodeIndex < endNodeIndex) && (rightNodeIndex <= endNodeIndex) && (ary[leftNodeIndex] < ary[rightNodeIndex]))
                {
                    //让左子节点索引加一
                    //目的是为了后面用于比较父节点的值
                    leftNodeIndex++;
                }

                //注意，此处的leftNodeIndex可能是实际的右子节点的索引
                //主要取的是左右两个节点中较大的哪个，用来和父节点进行比较
                //如果左子节点大于父节点
                if (temp < ary[leftNodeIndex])
                {
                    //把左子节点的值赋给父节点
                    ary[startNodeIndex] = ary[leftNodeIndex];
                    //以左子节点的索引为父节点索引
                    startNodeIndex = leftNodeIndex;
                    //重新规划左子节点索引
                    leftNodeIndex = 2 * startNodeIndex + 1;
                }
                else
                {
                    //如果左右两个节点的值都没有父节点大
                    //那么这次查找就结束了。
                    break;
                }
            }
            //给最终的父节点赋值
            //此处就是某个元素最终的位置了
            ary[startNodeIndex] = temp;
        }
        /// <summary>
        /// 创建大根堆
        /// </summary>
        /// <param name="ary">序列</param>
        /// <param name="low">开始索引</param>
        /// <param name="high">结束索引</param>
        private static void CreateHeap(int[] ary, int low, int high)
        {
            //从中点开始回溯
            for (int i = high / 2; i >= low; --i)
            {
                GetMaxHeap(ary, i, ary.Length - 1);
            }
        }

        #endregion

        #region 交换两个元素
        /// <summary>
        /// 交换两个元素
        /// </summary>
        /// <param name="x">元素1</param>
        /// <param name="y">元素2</param>
        public static void Swap(ref int x, ref int y)
        {
            (y, x) = (x, y);
        }
        #endregion

        #region  发音排序 
        /// <summary>
        /// 按发音排序
        /// </summary>
        /// <param name="arr">数组</param>
        /// <param name="isENUS">是否英文</param>
        public static void PronunciationSort(string[] arr, bool isENUS)
        {
            //发音 LCID：0x00000804
            if (isENUS)
                //Thread.CurrentThread.CurrentCulture = new CultureInfo(2052);
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            else
                Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-cn");
            Array.Sort(arr);
        }
        #endregion

        #region  笔画排序 
        /// <summary>
        /// 笔画排序
        /// </summary>
        /// <param name="arr">数组</param>
        /// <param name="isZHTW">是否繁体中文</param>
        public static void StrokeSort(string[] arr, bool isZHTW)
        {
            //笔画数 LCID：0x00020804 
            if (isZHTW)
                Thread.CurrentThread.CurrentCulture = new CultureInfo("zh-tw");
            else
                Thread.CurrentThread.CurrentCulture = new CultureInfo(133124);
            Array.Sort(arr);
        }
        #endregion
    }
}
