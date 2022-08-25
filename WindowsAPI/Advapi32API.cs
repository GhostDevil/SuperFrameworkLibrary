using System;
using System.Runtime.InteropServices;
using static SuperFramework.WindowsAPI.APIStruct;
namespace SuperFramework.WindowsAPI
{
    /// <summary>
    /// 作者：不良帥
    /// 日期：2016-08-19
    /// 说明：一个高级API应用程序接口服务库的一部分，包含的函数与对象的安全性，注册表的操控以及事件日志有关。
    /// </summary>
    public static class Advapi32API
    {    
        /// <summary>
        /// OpenProcessToken 函数与进程关联的访问令牌
        /// </summary>
        /// <param name="ProcessHandle">打开访问令牌进程的句柄</param>
        /// <param name="DesiredAccess">一个访问符，指定需要的访问令牌的访问类型。
        /// 这些访问类型与访问令牌自定义的访问控制列表(DACL)比较后，决定哪些访问是允许的，哪些是拒绝的</param>
        /// <param name="TokenHandle">句柄指针:标志了刚刚打开的由函数返回的访问令牌</param>
        /// <returns>如果执行成功，返回非0,如果执行失败，返回0. 如果要获取更多的错误信息, 请调用Marshal.GetLastWin32Error.</returns>
        [DllImport("advapi32.dll", EntryPoint = "OpenProcessToken", CharSet = CharSet.Ansi)]
        public static extern int OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);
        /// <summary>
        /// LookupPrivilegeValue函数返回本地唯一标志LUID，用于在指定的系统上代表特定的权限名
        /// </summary>
        /// <param name="lpSystemName">以null结束的字符串指针，标志了在其上查找权限名的系统名称. 如果设置为null, 函数将试图查找指定系统的权限名.</param>
        /// <param name="lpName">以null结束的字符串指针，指定了在Winnt.h头文件中定义的权限名. 例如, 该参数可以是一个常量 SE_SECURITY_NAME, 或者对应的字符串 "SeSecurityPrivilege".</param>
        /// <param name="lpLuid">接收本地唯一标志LUID的变量指针，通过它可以知道由lpSystemName 参数指定的系统上的权限.</param>
        /// <returns>如果执行成功，返回非0,如果执行失败，返回0，如果要获取更多的错误信息，请调用Marshal.GetLastWin32Error.,</returns>
        [DllImport("advapi32.dll", EntryPoint = "LookupPrivilegeValue", CharSet = CharSet.Ansi)]
        public static extern int LookupPrivilegeValue(string lpSystemName, string lpName, ref LUID lpLuid);

        /// <summary>
        /// 函数查看系统权限的特权值，返回信息到一个LUID结构体里。
        /// </summary>
        /// <param name="host"></param>
        /// <param name="name"></param>
        /// <param name="pluid"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll ", SetLastError = true)]
        public static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);
        /// <summary>
        /// AdjustTokenPrivileges 函数可以启用或禁用指定访问令牌的权限. 在一个访问令牌中启用或禁用一个权限需要 TOKEN_ADJUST_PRIVILEGES 访问权限.
        /// </summary>
        /// <param name="TokenHandle">需要改变权限的访问令牌句柄. 句柄必须含有对令牌的 TOKEN_ADJUST_PRIVILEGES 访问权限. 
        /// 如果 PreviousState 参数非null, 句柄还需要有 TOKEN_QUERY 访问权限.</param>
        /// <param name="DisableAllPrivileges">执行函数是否禁用访问令牌的所有权限. 如果参数值为 TRUE, 
        /// 函数将禁用所有权限并忽略 NewState 参数. 如果其值为 FALSE, 函数将根据NewState参数指向的信息改变权限.</param>
        /// <param name="NewState">一个 TOKEN_PRIVILEGES 结构的指针，指定了一组权限以及它们的属性. 
        /// 如果 DisableAllPrivileges 参数为 FALSE, AdjustTokenPrivileges 函数将启用或禁用访问令牌的这些权限. 
        /// 如果你为一个权限设置了 SE_PRIVILEGE_ENABLED 属性, 本函数将启用该权限; 否则, 它将禁用该权限. 
        /// 如果 DisableAllPrivileges 参数为 TRUE, 本函数忽略此参数.</param>
        /// <param name="BufferLength">为PreviousState参数指向的缓冲区用字节设置大小. 如果PreviousState 参数为 NULL，此参数可以为0</param>
        /// <param name="PreviousState">一个缓冲区指针，被函数用来填充 TOKENT_PRIVILEGES结构，它包含了被函数改变的所有权限的先前状态. 此参数可以为 NULL.</param>
        /// <param name="ReturnLength">一个变量指针，指示了由PreviousState参数指向的缓冲区的大小.如果 PreviousState 参数为 NULL，此参数可以为NULL .</param>
        /// <returns>如果执行成功，返回非0. 如果要检测函数是否调整了指定的权限, 请调用 Marshal.GetLastWin32Error.</returns>
        [DllImport("advapi32.dll", EntryPoint = "AdjustTokenPrivileges", CharSet = CharSet.Ansi)]
        public static extern int AdjustTokenPrivileges(IntPtr tokenHandle, int disableAllPrivileges, ref TOKEN_PRIVILEGES newState, int bufferLength, ref TOKEN_PRIVILEGES previousState, ref int returnLength);
        /// <summary>
        /// 获取计算机用户名
        /// </summary>
        /// <param name="IpBuffer"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        [DllImport("advapi32", EntryPoint = "GetUserName", ExactSpelling = false, SetLastError = true)]//计算机用户名
        public static extern bool GetUserName([MarshalAs(UnmanagedType.LPArray)]byte[] IpBuffer, [MarshalAs(UnmanagedType.LPArray)] int[] nSize);
      
    }
}
