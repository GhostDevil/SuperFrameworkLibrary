using System.Collections.Generic;

namespace SuperFramework.SuperAlgorithm
{
    /// <summary>
    /// 算数算法
    /// </summary>
    public static class CountHelper
    {
        #region 最大公约数

        /// <summary>
        /// 递归版本的最大公约数
        /// </summary>
        /// <param name="x">第一个数</param>
        /// <param name="y">第二个数</param>
        /// <returns>两个数的最大公约数</returns>
        public static int GetConvention(int x, int y)
        {
            //当y是0的时候，那么两个数的最大公约数就是x
            if (y == 0)
                return x;
            return GetConvention(y, x % y);
        }
        /// <summary>
        /// 非递归版本的最大公约数
        /// </summary>
        /// <param name="x">第一个数</param>
        /// <param name="y">第二个数</param>
        /// <returns>两个数的最大公约数</returns>
        public static int GetCommonDivisor(int x, int y)
        {
            while (y != 0)
            {
                int temp = x % y;
                x = y;
                y = temp;
            }
            //当y是0的时候，那么两个数的最大公约数就是x
            return x;
        }
        #endregion

        #region 非递归版本的最小公倍数
        /// <summary>
        /// 非递归版本的最小公倍数
        /// </summary>
        /// <param name="x">第一个数</param>
        /// <param name="y">第二个数</param>
        /// <returns>最小公倍数</returns>
        static int Lcm1(int x, int y)
        {
            int z = x * y;
            while (y != 0)
            {
                int temp = x % y;
                x = y;
                y = temp;
            }
            //最小公倍数是两数之积除以最大公约数
            return z / x;
        }
        #endregion

        #region 借助于最大公约数获得最小公倍数
        /// <summary>
        /// 借助于最大公约数获得最小公倍数
        /// </summary>
        /// <param name="x">第一个数</param>
        /// <param name="y">第二个数</param>
        /// <returns>返回最小公倍数</returns>
        static int Lcm2(int x, int y)
        {
            //最小公倍数是两数之积除以最大公约数
            return x * y / GetConvention(x, y);
        }
        #endregion

        #region 将十进制的数转换为二进制
        /// <summary>
        /// 将十进制的数转换为二进制
        /// </summary>
        /// <param name="x">十进制的数</param>
        /// <returns>二进制数的数组</returns>
        static int[] Decimal2Binary(int x)
        {
            //数组长度
            int length = 0;
            int y = x;
            //这个循环用来获得结果数组的长度
            while (true)
            {
                if (y == 0)
                    break;
                y /= 2;
                length++;
            }
            //这是补丁，用来解决x=0的情况
            if (length == 0)
            {
                length = 1;
            }
            int[] ary = new int[length];
            for (int i = 0; i < length; i++)
            {
                //因为先得到的是末位，所以放在后面
                ary[length - i - 1] = x % 2;
                x /= 2;
            }
            return ary;
        }
        #endregion

        #region 获得指定小数位数的商
        /// <summary>
        /// 获得指定小数位数的商
        /// </summary>
        /// <param name="x">被除数</param>
        /// <param name="y">除数</param>
        /// <param name="count">要保留的小数位数</param>
        /// <returns>小数数组</returns>
        public static int[] GetPrecision(int x, int y, int count)
        {
            int[] ary = new int[count];
            //如果x大于y
            if (x > y)
            {
                //将x对y取余数
                x %= y;
            }
            for (int i = 0; i < count; i++)
            {
                //模拟在被除数后加一个零
                ary[i] = (x * 10) / y;
                //将余数再次作为被除数
                x = (x * 10) % y;
            }
            return ary;
        }
        #endregion

        #region 分解质因数
        /// <summary>
        /// 分解质因数
        /// </summary>
        /// <param name="x">待分解的数</param>
        /// <returns>返回因数集合</returns>
        public static List<int> DecompositionFactor(int x)
        {
            List<int> temp = new List<int>();
            int y = x;
            //这是补丁，用来解决1和2的问题
            if (y == 1 || y == 2)
                temp.Add(y);
            //从2开始直到这个数本身
            for (int i = 2; i <= y; i++)
            {
                //如果能被某个数整除
                if (x % i == 0)
                {
                    //那么这个数就是一个质因数
                    temp.Add(i);
                    //那么一定可以整除
                    x /= i;
                    //i--的意思是y有可能还有个i是质因数
                    i--;
                }
            }
            return temp;
        }
        #endregion

        #region 获得斐波那契数列
        /// <summary>
        /// 获得斐波那契数列，递归算法。
        /// 1，1，2，3，5，8，13，21，34，55......除了前两个数以外，其他的数都是它前面两个数的和，总结函数：除了第一位和第二位以外：f(n)=f(n-1)+f(n-2)，第一位和第二位：f(n)=1。
        /// </summary>
        /// <param name="n">引用参数，需要数列的位数</param>
        /// <returns>返回斐波那契数列</returns>
        public static int GetFibonacci(ref int n)
        {
            //显式跳出条件
            if (n == 1 || n == 2)
            {
                return 1;
            }
            //f(n)=f(n-1)+f(n-2)
            return GetFibonacci(n - 1) + GetFibonacci(n - 2);
        }

        //其实对于斐波那契数列来使用递归有点不太好，这里的递归次数呈指数增长，所以对于这个具体的题目来说，应该使用普通的循环来解决。
        /// <summary>
        /// 获得斐波那契数列，普通循环，效率高于递归算法。
        /// 1，1，2，3，5，8，13，21，34，55......除了前两个数以外，其他的数都是它前面两个数的和，总结函数：除了第一位和第二位以外：f(n)=f(n-1)+f(n-2)，第一位和第二位：f(n)=1。
        /// </summary>
        /// <param name="n">引用参数，需要数列的位数</param>
        /// <returns>返回斐波那契数列</returns>
        public static int GetFibonacci(int n)
        {
            int result, x, y;
            result = x = y = 1;
            if (n == 1 || n == 2)
            {
                return result;
            }
            for (int i = 3; i <= n; i++)
            {
                result = x + y;
                x = y;
                y = result;
            }
            return result;
        }
        #endregion

        #region 是否满足abcd=(ab+cd)*(ab+cd)
        /// <summary>
        /// 是否满足abcd=(ab+cd)*(ab+cd)
        /// </summary>
        /// <param name="x">四位数</param>
        /// <returns>满足要求返回true，不满足返回false</returns>
        public static bool IsResult(int x)
        {
            //前两位
            int before = x / 100;
            //后两位
            int front = x % 100;
            //前两位与后两位之和的平方
            int result = (before + front) * (before + front);
            if (x == result)
                return true;
            return false;
        }
        #endregion

        #region 判断某个数是否是素数
        /// <summary>
        /// 判断某个数是否是素数
        /// </summary>
        /// <param name="x"></param>
        /// <returns>满足要求返回true，不满足返回false</returns>
        public static bool IsPrime(int x)
        {
            for (int i = 2; i < x; i++)
                if (x % i == 0)
                    return false;
            return true;
        }
        #endregion

        #region 得到某个数的因式分解
        /// <summary>
        /// 得到某个数的因式分解
        /// </summary>
        /// <param name="x">int参数</param>
        /// <returns>返回因式字符串，如 1*2*4*6</returns>
        public static string GetFactor(int x)
        {
            //每个数都有1为因数
            string result = "1*";
            //从2开始
            for (int i = 2; i <= x;)
            {
                //如果可以被i整除
                if (x % i == 0)
                {
                    //那么结果中加上这个数
                    result += i.ToString() + "*";
                    //将x整除，将整除的结果给x，再次计算
                    x /= i;
                }
                else
                {
                    i++;
                }
            }
            //去掉最后一个*
            return result.Substring(0, result.Length - 1); ;
        }
        #endregion

        #region 将一个整数逆序
        /// <summary>
        /// 将一个整数逆序
        /// </summary>
        /// <param name="x">int整数</param>
        /// <returns>返回逆序后的整数</returns>
        public static int ReverseNum(int x)
        {
            int temp = 0;
            while (true)
            {
                if (x == 0)
                    break;
                //连升位带加下一位一块做了。
                //第一次temp中是4，第二次temp中就是43了，第三次temp中就是432了，第四次temp中就是4321了
                //相当于每次都对结果升位，再加上原来数的个位。
                temp = temp * 10 + x % 10;
                x /= 10;
            }
            return temp;
        }
        /// <summary>
        /// 将一个整数逆序
        /// </summary>
        /// <param name="x">long整数</param>
        /// <returns>返回逆序后的整数</returns>
        public static long ReverseNum(long x)
        {
            long temp = 0;
            while (true)
            {
                if (x == 0)
                    break;
                //连升位带加下一位一块做了。
                //第一次temp中是4，第二次temp中就是43了，第三次temp中就是432了，第四次temp中就是4321了
                //相当于每次都对结果升位，再加上原来数的个位。
                temp = temp * 10 + x % 10;
                x /= 10;
            }
            return temp;
        }
        #endregion

        #region 是否是自守数
        /// <summary>
        /// 是否是自守数 自守数：某个数的平方的尾数等于这个数自身。
        /// </summary>
        /// <param name="x">int整数</param>
        /// <returns></returns>
        public static bool IsAutomorphic(int x)
        {
            int y = x;
            //pow中存放x的位数，比如23，那么pow为10，比如123，那么pow为100
            int pow = 1;
            while (true)
            {
                if (y / 10 == 0)
                    break;
                pow *= 10;
                y /= 10;
            }
            //pow2中存放比x位数大一的最小整数，比如23，那么pow2为100，主要用来求余，获取一个整数的最后几位。
            int pow2 = pow * 10;
            int temp = 0;
            y = x;
            //这个循环主要用来将x中的每一位与x相乘，并把结果的最后几位取到。
            while (true)
            {
                if (pow == 0)
                    break;
                //temp*10是将某次乘积升位，y/pow是从第一位开始的x中的每一位，最后将结果取最后几位
                temp = ((temp * 10) % pow2 + (x * (y / pow)) % pow2) % pow2;
                y %= pow;
                pow /= 10;
            }
            return temp == x;
        }
        #endregion

        #region 获得两数的无限精度的商
        /// <summary>
        /// 获得两数的无限精度的商
        /// </summary>
        /// <param name="x">被除数</param>
        /// <param name="y">除数</param>
        /// <param name="length">小数位数</param>
        /// <returns>返回数组来保存小数点后面的值</returns>
        public static int[] GetPerc(int x, int y, int length)
        {
            int[] ary = new int[length];
            for (int i = 0; i < ary.Length; i++)
            {
                //因为x小于y，所以给x补零
                //整除的结果就是商，放入
                //数组中
                int temp = (x * 10) / y;
                ary[i] = temp;
                //将余数再次作为x继续除
                x = (x * 10) % y;
            }
            return ary;
        }
        #endregion

        #region 计算两个大数的乘积
        /// <summary>
        /// 计算两个大数的乘积
        /// </summary>
        /// <param name="ary1">第一个大数，数组表示大数的每一位</param>
        /// <param name="ary2">第二个大数，数组表示大数的每一位</param>
        /// <returns>返回相乘后结果，使用数组表示</returns>
        public static int[] GetMul(int[] ary1, int[] ary2)
        {
            //一个M位的数与一个N位的数相乘的积最少应该是M+N-1位，最多是M+N位
            //所以，简单期间将结果大小设置为M+N
            //这种算法有点怪，手算的时候是从低位开始向高位前进的
            //这种是从高位开始向低位前进，当然在最后的时候要做一
            //遍进位的工作
            int[] result = new int[ary1.Length + ary2.Length];
            //拿ary1中的一个和ary2中的每一个相乘，结果放入result中
            for (int i = 0; i < ary1.Length; i++)
            {
                //从第二个元素开始填
                int startIndex = i + 1;
                for (int j = 0; j < ary2.Length; j++)
                {
                    //每次填ary2.Length个元素
                    //最后要列竖式相加呢，咱提前加了
                    result[startIndex + j] += ary1[i] * ary2[j];
                }
            }
            //运行到此处，结果数组中只有
            //第一个元素没有赋值。
            //因为第一个元素和进位相关，
            //所以下面来处理进位的问题。
            for (int i = result.Length - 1; i > 0; i--)
            {
                //把低位的要进位的值加到高位上
                result[i - 1] += result[i] / 10;
                //低位的结果就剩余数了
                result[i] = result[i] % 10;
            }
            return result;
        }
        #endregion

        #region 拆分数字为数组，利用碾除法。
        /// <summary>
        /// 拆分数字为数组，利用碾除法。
        /// </summary>
        /// <param name="x">int整数</param>
        /// <returns>返回拆分后的数组</returns>
        public static int[] SplitNum(int x)
        {
            //因为是四位数，所以数组长度为四
            int[] ary = new int[4];
            int i = 0;
            while (true)
            {
                if (x == 0)
                {
                    break;
                }
                //每次取x的最后一位
                int temp = x % 10;
                //放入数组中
                ary[i] = temp;
                //将x除10，这样个位就消失了，现在的个位是x的原来的10位了
                x /= 10;
                i++;
            }
            return ary;
        }
        /// <summary>
        /// 拆分数字为数组，利用碾除法。
        /// </summary>
        /// <param name="x">long整数</param>
        /// <returns>返回拆分后的数组</returns>
        public static long[] SplitNum(long x)
        {
            //因为是四位数，所以数组长度为四
            long[] ary = new long[4];
            int i = 0;
            while (true)
            {
                if (x == 0)
                    break;
                //每次取x的最后一位
                long temp = x % 10;
                //放入数组中
                ary[i] = temp;
                //将x除10，这样个位就消失了，现在的个位是x的原来的10位了
                x /= 10;
                i++;
            }
            return ary;
        }
        #endregion

        #region 组最大数，小数乘小的幂次方，大数乘大的幂次方。
        /// <summary>
        /// 组最大数，小数乘小的幂次方，大数乘大的幂次方。
        /// </summary>
        /// <param name="ary"></param>
        /// <returns></returns>
        public static int GetMax(int[] ary)
        {
            int sum = 0;
            for (int i = 0; i < ary.Length; i++)
                sum += ary[i] * GetPowTen(i);
            return sum;
        }
        #endregion

        #region 组最小数，小数乘大的幂次方，大数乘小的幂次方。
        /// <summary>
        /// 组最小数，小数乘大的幂次方，大数乘小的幂次方。
        /// </summary>
        /// <param name="ary"></param>
        /// <returns></returns>
        public static int GetMin(int[] ary)
        {
            int sum = 0;
            for (int i = ary.Length - 1; i >= 0; i--)
                sum += ary[i] * GetPowTen(ary.Length - i - 1);
            return sum;
        }
        #endregion

        #region 得到10的x次幂
        /// <summary>
        /// 得到10的x次幂
        /// </summary>
        /// <param name="x">int整数</param>
        /// <returns>返回10的X次幂</returns>
        public static int GetPowTen(int x)
        {
            if (x == 0)
                return 1;
            int count = 1;
            for (int i = 0; i < x; i++)
                count *= 10;
            return count;
        }
        #endregion

        #region 获得大于原始数的最小的平方根，采用二分法来获取，此方法在数字较小的时候有效，数字较大将会溢出。
        /// <summary>
        /// 获得大于原始数的最小的平方根，采用二分法来获取，此方法在数字较小的时候有效，数字较大将会溢出。
        /// </summary>
        /// <param name="x">原始数</param>
        /// <returns>返回大于原始数的最小的平方根</returns>
        public static int GetSQRT(int x)
        {
            //设置低位数
            int low = 1;
            //设置高位数
            int high = x;
            //如果低位等于高位，或者高位减一等于低位
            //就直接结束，返回高位
            while (!(low == high || high - low == 1))
            {
                //如果中值的平方小于x
                if ((high + low) * (high + low) / 4 < x)
                {
                    //那么要找的哪个数在中位以上，高位以下。
                    low = (high + low) / 2;
                }
                else
                {
                    //否则要找的数在低位以上，中位以下。
                    high = (high + low) / 2;
                }
            }
            return high;
        }
        #endregion
    }
}
