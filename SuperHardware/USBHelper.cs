using System.Collections.Generic;
using System.Management;

namespace SuperFramework.SuperHardware
{
    class USBHelper
    {
        #region 内部对象
        static ManagementClass mc;
        static ManagementObjectCollection moc;
        #endregion

        #region 获得USB信息
        /// <summary>
        /// 获得USB信息
        /// </summary>
        public static List<HInfoStruct.USBInfo> GetUSBInfos()
        {
            mc = new ManagementClass("Win32_USBController");
            moc = mc.GetInstances();
            List<HInfoStruct.USBInfo> usbs = new List<HInfoStruct.USBInfo>();
            foreach (ManagementObject mo in moc)
            {
                HInfoStruct.USBInfo usb = new HInfoStruct.USBInfo();
                foreach (PropertyData pd in mo.Properties)
                {
                    if (pd.Name.Equals("Manufacturer"))
                        usb.Manufacturer = pd.Value.ToString();
                    else if (pd.Name.Equals("Name"))
                        usb.Name = pd.Value.ToString();
                    else if (pd.Name.Equals("Status"))
                        usb.Status = pd.Value.ToString();

                    else if (pd.Name.Equals("Size"))
                        usb.Size = pd.Value.ToString();
                    else if (pd.Name.Equals("Caption"))
                        usb.DeviceName = pd.Value.ToString();
                    else if (pd.Name.Equals("PNPDeviceID"))
                    {

                        usb.PNPDeviceID = pd.Value.ToString();

                        string[] info = usb.PNPDeviceID.Split('&');
                        string[] xx = info[3].Split('\\');

                        //序列号
                        usb.SerialId = xx[1];
                        xx = xx[0].Split('_');

                        //版本号
                        usb.VersionId = xx[1];

                        //制造商ID
                        xx = info[1].Split('_');
                        usb.ManufacturerId = xx[1];
                    }
                }
                usbs.Add(usb);
            }
            DisposeResource();
            return usbs;
        }

        #endregion

        #region 释放资源
        /// <summary>
        /// 释放资源
        /// </summary>
        private static void DisposeResource()
        {
            moc = null;
            mc = null;
        }
        #endregion

    }
}

