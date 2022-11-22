using System;
using System.Runtime.InteropServices;

namespace SuperFramework
{
    /**
    1.使用性能测试工具dotTrace 3.0，它能够计算出你程序中那些代码占用内存较多
    2.强制垃圾回收
    3.多dispose，close
    4.用timer，每几秒钟调用：SetProcessWorkingSetSize(Process.GetCurrentProcess().Handle,-1, -1);
    5.发布的时候选择Release
    6.注意代码编写时少产生垃圾，比如String + String就会产生大量的垃圾，可以用StringBuffer.Append
    7.this.Dispose();   this.Dispose(True);   this.Close();   GC.Collect();
    8.注意变量的作用域，具体说某个变量如果只是临时使用就不要定义成成员变量。GC是根据关系网去回收资源的。
    9.检测是否存在内存泄漏的情况，详情可参见：内存泄漏百度百科

    **/
    /// <summary>
    /// 版 本:Release
    /// 日 期:2014-12-10
    /// 作 者:不良帥
    /// </summary>
    class OptimizationHelper
    {
        #region 内存回收
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize")]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minSize, int maxSize);
        /// <summary>
        /// 释放内存
        /// </summary>
        public static void ClearMemory()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                _ = SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
            }
        }
        #endregion

    }
}
