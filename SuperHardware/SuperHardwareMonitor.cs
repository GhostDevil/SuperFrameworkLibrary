

using LibreHardwareMonitor.Hardware;

namespace SuperFramework.SuperHardware
{
    /// <summary>
    /// 硬件监视器
    /// </summary>
    public class SuperHardwareMonitor : Computer 
    {
        public class UpdateVisitor : IVisitor
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="computer"></param>
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="hardware"></param>
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sensor"></param>
            public void VisitSensor(ISensor sensor) { }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="parameter"></param>
            public void VisitParameter(IParameter parameter) { }
        }
        
        //SuperHardwareMonitor updateVisitor = new SuperHardwareMonitor();
        ///// <summary>
        ///// 获取系统信息
        ///// </summary>
        //public static void GetSystemInfo()
        //{
        //    Computer computer = new Computer();
        //    computer.Open();
        //    computer.CPUEnabled = true;
        //    computer.GPUEnabled = true;
        //    computer.HDDEnabled = true;
        //    computer.MainboardEnabled = true;
        //    computer.RAMEnabled = true;
        //    computer.FanControllerEnabled = true;
        //    computer.Accept(updateVisitor);
        //    for (int i = 0; i < computer.Hardware.Length; i++)
        //    {
        //        try
        //        {
        //            //if (computer.Hardware[i].HardwareType == HardwareType.CPU)
        //            //{
        //            for (int j = 0; j < computer.Hardware[i].Sensors.Length; j++)
        //            {
        //                if (computer.Hardware[i].Sensors[j].SensorType == SensorType.Temperature)
        //                    Console.WriteLine(computer.Hardware[i].Sensors[j].Name + ":" + computer.Hardware[i].Sensors[j].Value.ToString() + "\r");
        //            }
        //            //}
        //        }
        //        catch { }
        //    }
        //    computer.Close();

        //}

    }
}
