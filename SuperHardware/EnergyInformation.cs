using System;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace SuperFramework.SuperHardware
{

    /// <summary>
    /// 日 期:2015-08-18
    /// 作 者:不良帥
    /// 描 述:计算机能源信息类
    /// </summary>
    public sealed class EnergyInformation
    {
        /// <summary>
        /// 获取AC电源的状态
        /// </summary>
        /// <param name="_ps">能源信息结构体</param>
        /// <returns>true:成功 false:失败</returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool GetSystemPowerStatus(SPowerStatus _ps);
        //[DllImport("powrprof.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private extern static uint PowerGetActiveScheme(IntPtr _usrrtpwrkey, ref IntPtr _guidpointer);
        //[DllImport("powrprof.dll", CharSet = CharSet.Auto)]
        //private extern static uint PowerSetActiveScheme(IntPtr _usrrtperkey, ref IntPtr _sguid);
        //[DllImport("powrprof.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
        //private extern static uint PowerReadACValueIndex(IntPtr _rtperkey, byte[] _sguid, byte[] _subgroup, byte[] _settingguid, ref IntPtr _acvalidx);
        //[DllImport("powrprof.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto, SetLastError = true)]
        //private extern static uint PowerWriteACValueIndex(IntPtr _rtperkey, byte[] _sguid, byte[] _subgroup, byte[] _settingguid, int _acvalidx);
        /// <summary>
        /// 阻止系统休眠，直到线程结束恢复休眠策略
        /// </summary>
        /// <param name="_flags"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        private extern static uint SetThreadExecutionState(EExecutionFlag _flags);
        //[DllImport("user32.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall)]
        //private extern static IntPtr RegisterPowerSettingNotification(IntPtr _recipient, ref Guid _settingguid, uint _flags);
        /// <summary>
        /// 获取AC电源的状态
        /// </summary>
        /// <returns>获取当前计算机所连接的AC电源适配器的状态。</returns>
        public static EAcPowerStatus GetAcPowerStatus()
        {
            SPowerStatus ps = new();
            bool b = GetSystemPowerStatus(ps);
            EAcPowerStatus stat = EAcPowerStatus.Unknown;
            switch (ps.AcLineStatus)
            {
                case 0:
                    stat = EAcPowerStatus.TurnOff;
                    break;
                case 1:
                    stat = EAcPowerStatus.TurnOn;
                    break;
                case 255:
                    stat = EAcPowerStatus.Unknown;
                    break;
            }
            return stat;
        }
        /// <summary>
        /// 获取电池状态
        /// </summary>
        /// <returns>获取当前计算机所配备的电池的状态信息。</returns>
        public static EBatteryStatus GetBatteryStatus()
        {
            SPowerStatus ps = new();
            bool b = GetSystemPowerStatus(ps);
            EBatteryStatus stat = EBatteryStatus.Charging;
            if (Convert.ToBoolean(ps.BatteryFlag) & Convert.ToBoolean(1)) stat = EBatteryStatus.High;
            if (Convert.ToBoolean(ps.BatteryFlag) & Convert.ToBoolean(2)) stat = EBatteryStatus.Low;
            if (Convert.ToBoolean(ps.BatteryFlag) & Convert.ToBoolean(4)) stat = EBatteryStatus.Critical;
            if (Convert.ToBoolean(ps.BatteryFlag) & Convert.ToBoolean(8)) stat = EBatteryStatus.Charging;
            if (Convert.ToBoolean(ps.BatteryFlag) & Convert.ToBoolean(128)) stat = EBatteryStatus.NoSystemBattery;
            return stat;
        }
        /// <summary>
        /// 获取电池电量
        /// </summary>
        /// <returns>获取当前计算机所配备的电池的可用电量，该操作执行后会返回一个整型数据，100表示电量已满，0表示电量归零，255则表示电池电量未知。</returns>
        public static int GetQuantityOfBattery()
        {
            SPowerStatus ps = new();
            bool b = GetSystemPowerStatus(ps);
            if (ps.BatteryLifePercent == 255) throw new UnknownQuantityOfBatteryException("电池电量未知！");
            return ps.BatteryLifePercent;
        }
        /// <summary>
        /// 获取未充电时的电池可用时间
        /// </summary>
        /// <returns>获取当前计算机在未连接或者未充电的情况下的电池的电量的可用时间，单位为second。</returns>
        public static int GetBatteryLifeTime()
        {
            SPowerStatus ps = new();
            bool b = GetSystemPowerStatus(ps);
            if (ps.BatteryLifeTime == -1) throw new UnknownBatteryUseTimeSpanException("电池剩余电量的使用时间暂时无法测定！");
            if (GetBatteryStatus() == EBatteryStatus.Charging) throw new UnknownBatteryUseTimeSpanException("无法查看充电状态下的电池电量使用时间！");
            return ps.BatteryLifeTime;
        }
        /// <summary>
        /// 阻止系统休眠，直到线程结束恢复休眠策略
        /// </summary>
        /// <param name="_includeDisplay">是否阻止关闭显示器</param>
        public static void PreventSleep(bool _includeDisplay)
        {
            if (_includeDisplay)
            {
                SetThreadExecutionState(EExecutionFlag.System | EExecutionFlag.Display | EExecutionFlag.Continus);
            }
            else
            {
                SetThreadExecutionState(EExecutionFlag.System | EExecutionFlag.Continus);
            }
        }
        /// <summary>
        /// 恢复系统休眠策略
        /// </summary>
        public static void ResotreSleep()
        {
            SetThreadExecutionState(EExecutionFlag.Continus);
        }
        /// <summary>
        /// 重置系统休眠计时器
        /// </summary>
        /// <param name="_includeDisplay">是否阻止关闭显示器</param>
        public static void ResetSleepTimer(bool _includeDisplay = false)
        {
            if (_includeDisplay)
            {
                SetThreadExecutionState(EExecutionFlag.System | EExecutionFlag.Display);
            }
            else
            {
                SetThreadExecutionState(EExecutionFlag.System);
            }
        }
        /// <summary>
        /// 能源信息结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SPowerStatus
        {
            public byte AcLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved;
            public int BatteryLifeTime;
            public uint BatteryFullLifeTime;
        }
        /// <summary>
        /// 休眠机制的执行标志。只使用Continus参数时，则是恢复系统休眠策略。不使用Continus参数时，实现阻止系统休眠或显示器关闭一次。组合使用Continus参数时，实现阻止系统休眠或显示器关闭至线程终止
        /// </summary>
        [Flags()]
        public enum EExecutionFlag : uint
        {
            System = 0x1,
            Display = 0x2,
            Continus = 0x80000000u
        }
        /// <summary>
        /// AC电源的状态
        /// </summary>
        public enum EAcPowerStatus : byte
        {
            TurnOff = 0,
            TurnOn = 1,
            Unknown = 255
        }
        /// <summary>
        /// 电池状态的枚举
        /// </summary>
        public enum EBatteryStatus : byte
        {
            /// <summary>
            /// 电量充足
            /// </summary>
            High = 1,
            /// <summary>
            /// 电量较低
            /// </summary>
            Low = 2,
            /// <summary>
            /// 电量严重不足
            /// </summary>
            Critical = 4,
            /// <summary>
            /// 正在充电
            /// </summary>
            Charging = 8,
            /// <summary>
            /// 无电源
            /// </summary>
            NoSystemBattery = 128
        }
        /// <summary>
        /// 电池使用时间未知时而抛出的异常
        /// </summary>
        [Serializable]
        public class UnknownBatteryUseTimeSpanException : Exception
        {
            public UnknownBatteryUseTimeSpanException() { }
            public UnknownBatteryUseTimeSpanException(string message) : base(message) { }
            public UnknownBatteryUseTimeSpanException(string message, Exception inner) : base(message, inner) { }
            protected UnknownBatteryUseTimeSpanException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
        /// <summary>
        /// 不清楚电池电量时而抛出的异常
        /// </summary>
        [Serializable]
        public class UnknownQuantityOfBatteryException : Exception
        {
            public UnknownQuantityOfBatteryException() { }
            public UnknownQuantityOfBatteryException(string message) : base(message) { }
            public UnknownQuantityOfBatteryException(string message, Exception inner) : base(message, inner) { }
            protected UnknownQuantityOfBatteryException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        }
    }
}
