using System.Runtime.InteropServices;

namespace SuperFramework.WindowsAPI
{
    public static class WinAPI
    {
        
        [StructLayout(LayoutKind.Sequential)]
        public struct CPU_INFO
        {
            public uint dwOemId;
            public uint dwPageSize;
            public uint lpMinimumApplicationAddress;
            public uint lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;
        }

        [DllImport("Iphlpapi.dll")]
        public static extern int SendARP(int dest, int host, ref long mac, ref int length);
        [DllImport("Ws2_32.dll")]
        public static extern int inet_addr(string ip);



    }
}
