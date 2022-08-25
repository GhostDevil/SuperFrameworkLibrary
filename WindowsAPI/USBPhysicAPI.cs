using System.Runtime.InteropServices;
using System.Text;

namespace SuperFramework.WindowsAPI
{
    public static class USBPhysicAPI
    {
        /// <summary>
        /// 用来注册USBPhysic.dll，
        /// </summary>
        /// <param name="sUser"></param>
        /// <param name="sRegCode"></param>
        /// <returns></returns>
        [DllImport("USBPhysic")]
        public static extern int Init(string sUser, string sRegCode);
        /// <summary>
        /// 获取U盘信息
        /// </summary>
        /// <param name="diskIndex"></param>
        /// <param name="InfoType"></param>
        /// <param name="pHddInfo"></param>
        /// <returns></returns>
        [DllImport("USBPhysic")]
        public static extern int GetUSBPhysicInfo(int diskIndex, int InfoType, StringBuilder pHddInfo);
    }
}
