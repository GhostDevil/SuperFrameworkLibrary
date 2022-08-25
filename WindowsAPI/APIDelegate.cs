using System;

namespace SuperFramework.WindowsAPI
{
    
    /// <summary>
    /// 版 本:Release
    /// 日 期:2015-09-08
    /// 作 者:不良帥
    /// 描 述:API操作委托
    /// </summary>
    public static class APIDelegate
    {
        ///// <summary>
        ///// 键盘钩子委托
        ///// </summary>
        ///// <param name="nCode"></param>
        ///// <param name="wParam"></param>
        ///// <param name="lParam"></param>
        ///// <returns></returns>
        //public delegate int KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        /// <summary>
        /// 钩子委托
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    }
}
