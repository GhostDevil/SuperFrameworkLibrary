using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace SuperFramework.SuperEncrypt
{
    /// <summary>
    /// 日 期:2014-09-15
    /// 作 者:不良帥
    /// 描 述:EXE加密解密
    /// </summary>
    public static class EXEEncrypt
    {
        #region 加密exe
        /// <summary>
        /// 加密exe
        /// </summary>
        /// <param name="exePath">exe路径</param>
        /// <param name="sprass">加密字符</param>
        /// <param name="type">加密类型</param>
        /// <returns>返回true标示成功，否则失败</returns>
        public static bool ExeEncoder(string exePath, string sprass, EncoderType type)
        {
            string Annex = "";//附加的高级条件
            if (exePath == "")
                return false;
            FileInfo SFInfo = new FileInfo(exePath);
            if (SFInfo.Exists == false)
                return false;
            if (SFInfo.Extension.ToUpper() != ".EXE")
                return false;
            string Sprass = CreatePass(type);

            if (Sprass.Trim() == "")
                return false;
            WriteEXE(exePath, Sprass.Trim() + Annex.Trim());
            CreateTXT(exePath,Sprass.Trim());
            return true;
        }
        /// <summary>
        /// 加密exe
        /// </summary>
        /// <param name="exePath">exe路径</param>
        /// <param name="sprass">加密字符</param>
        /// <param name="type">加密类型</param>
        /// <param name="shortDateStr">短日期字符串</param>
        /// <param name="month">月份</param>
        /// <param name="dayNum">天</param>
        /// <param name="runCount">运行次数</param>
        /// <returns>返回true标示成功，否则失败</returns>
        public static bool ExeEncoder(string exePath, string sprass, EncoderType type, string shortDateStr, int month, int dayNum, int runCount)
        {
            if (exePath == "")
                return false;
            FileInfo SFInfo = new FileInfo(exePath);
            if (SFInfo.Exists == false)
                return false;
            if (SFInfo.Extension.ToUpper() != ".EXE")
                return false;
            string Sprass = CreatePass(type);
            string Annex = string.Format(",{0}D", shortDateStr);
            Annex = string.Format(",{0}M", month);
            Annex = string.Format(",{0}A", dayNum);
            Annex = string.Format(",{0}C", runCount);
            if (Sprass.Trim() == "")
                return false;
            WriteEXE(exePath, Sprass.Trim() + Annex.Trim());
            CreateTXT(exePath,Sprass.Trim());
            return true;
        }



        private  static int ArrInt = 0;
        //private  static string PFileDir = "";
        private  static string PFileN = "";


        /// <summary>
        /// 获取计算机主机名
        /// </summary>
        /// <returns>计算机主机名</returns>
        private static string GetHostName()
        {
            return Dns.GetHostName();
        }

        #region  获取主机名
        /// <summary>
        /// 获取主机名
        /// </summary>
        private static string GetBIOSNumber()
        {
            // 显示主机名
            string hostname = Dns.GetHostName();
            // 显示每个IP地址
            IPHostEntry hostent = Dns.GetHostEntry(hostname); // 主机信息
            Array addrs = hostent.AddressList;            // IP地址数组
            IEnumerator it = addrs.GetEnumerator();       // 迭代器，添加名命空间using System.Collections;
            while (it.MoveNext())
            {   // 循环到下一个IP 地址
                IPAddress ip = (IPAddress)it.Current;      //获得IP地址，添加名命空间using System.Net;
                return ip.ToString();
            }
            return "";
        }
        #endregion

        /// <summary>
        /// 获取CPU序列号
        /// </summary>
        /// <returns>CPU序列号</returns>
        private static string GetCpuID()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();

                string strCpuID = null;
                foreach (ManagementObject mo in moc)
                {
                    strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                    mo.Dispose();
                    break;
                }
                moc.Dispose();
                mc.Dispose();
                return strCpuID;
            }
            catch
            {
                return "";
            }

        }

        /// <summary>
        /// 获取网卡硬件地址
        /// </summary>
        /// <returns>网卡硬件地址</returns>
        private static string GetNetworkCard()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc2 = mc.GetInstances();
                string StrNetworkCard = null;
                foreach (ManagementObject mo in moc2)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        StrNetworkCard = mo["MacAddress"].ToString();
                        mo.Dispose();
                        break;
                    }
                    mo.Dispose();
                }
                moc2.Dispose();
                mc.Dispose();
                return StrNetworkCard;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取本地计算机的硬盘盘符
        /// </summary>
        /// <remarks>返回本地硬盘盘符列表</remarks>
        private static List<string> GetHardDisk()
        {
            List<string> names = new List<string>();
            try
            {
                ManagementClass mcHD = new ManagementClass("win32_logicaldisk");
                ManagementObjectCollection mocHD = mcHD.GetInstances();
                foreach (ManagementObject mo in mocHD) //遍历硬盘信息
                {
                    names.Add(mo["DeviceID"].ToString());//添加硬盘的盘符名称
                    mo.Dispose();
                }
                mcHD.Dispose();
            }
            catch { }
            return names;
        }

        /// <summary>
        /// 获序硬盘序列号
        /// </summary>
        /// <param Disk="string">盘符</param>
        /// <returns>硬盘序列号</returns>
        private static string GetHardDiskID(string Disk)
        {
            try
            {
                string strHardDiskID = null;
                string DiskStr = Disk.Substring(0, 1) + ":";
                ManagementClass mcHD = new ManagementClass("win32_logicaldisk");
                ManagementObjectCollection mocHD = mcHD.GetInstances();
                foreach (ManagementObject mo in mocHD) //遍历硬盘信息
                {
                    if (mo["DeviceID"].ToString() == DiskStr)//如果硬盘等于指定的盘符
                    {
                        strHardDiskID = mo["VolumeSerialNumber"].ToString();//获取当前硬盘的序列号
                        mo.Dispose();
                        break;
                    }
                    mo.Dispose();
                }
                mcHD.Dispose();
                return strHardDiskID;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 对字符串进行加密
        /// </summary>
        /// <param former="string">加密字符串</param>
        /// <param spoon="string">密钥</param>
        /// <param n="int">密钥标识</param>
        /// <returns>加密后的字符串</returns>
        private static string Encrypt(string former, string spoon, int n)
        {
            byte[] FByteArray = Encoding.Default.GetBytes(former);//将字符串生成字节数组
            byte[] SByteArray = Encoding.Default.GetBytes(spoon);
            int Aleng;
            if (FByteArray.Length > SByteArray.Length)//获取字节数组的最大长度
                Aleng = FByteArray.Length;
            else
                Aleng = SByteArray.Length;
            char[] charData = new char[Aleng];//定义指定长度的字符数组
            for (int i = 0; i < FByteArray.Length; i++)//对字节数组中的单个字节进行异或运算
            {
                FByteArray[i] = Convert.ToByte(Convert.ToInt32(FByteArray[i]) ^ Convert.ToInt32(SByteArray[n]));
            }

            Decoder d = Encoding.UTF8.GetDecoder();//获取一个解码器
            d.GetChars(FByteArray, 0, FByteArray.Length, charData, 0);//将编码字节数组转换为字符数组
            d.Reset();//将解码器设为初始状态
            string Zpp = "";
            for (int i = 0; i < charData.Length; i++)//将字符数组组合成字符串
            {
                Zpp += charData[i].ToString();
            }
            n++;
            if (n < SByteArray.Length - 1)
                Encrypt(Zpp, spoon, n);//进行函数的递归调用
            return Zpp;
        }
        /// <summary>
        /// 加密方式
        /// </summary>
        public enum EncoderType
        {
            /// <summary>
            /// 主板编号
            /// </summary>
            BIOSNumber = 0,
            /// <summary>
            /// Cpu序列号
            /// </summary>
            CpuID = 1,
            /// <summary>
            /// 网卡硬件地址
            /// </summary>
            NetworkCard = 2,
            /// <summary>
            /// 硬盘序列号
            /// </summary>
            HardDiskID = 3

        }
        /// <summary>
        /// 根据条件生成加密字符串
        /// </summary>
        /// <param EncoderType="type">加密方式</param>
        /// <returns>加密后的字符串</returns>
        private static string CreatePass(EncoderType type)
        {
            ArrInt = 0;
            string PrassSum = null;
            ArrayList List = new ArrayList();

            switch (Convert.ToInt32(type))
            {
                case 0://主板序列号
                    {
                        PrassSum = GetBIOSNumber();
                        //if (PrassSum.Trim() == "")
                        // MessageBox.Show("无法获取主板序列号。");
                        break;
                    }
                case 1://CPU序列号
                    {
                        PrassSum = GetCpuID();
                        //if (PrassSum.Trim() == "")
                        // MessageBox.Show("无法获取CPU序列号。");
                        break;
                    }
                case 2://网卡硬件地址
                    {
                        PrassSum = GetNetworkCard();
                        //if (PrassSum.Trim() == "")
                        //MessageBox.Show("无法获取网卡硬件地址。");
                        break;
                    }
                case 3://硬盘序列号
                    {
                        PrassSum = GetHardDiskID("");
                        // if (PrassSum.Trim() == "")
                        //MessageBox.Show("无法获取" + Comb.Text + "盘序列号。");
                        break;
                    }
            }
            if (PrassSum.Trim() != "")
            {
                ArrInt++;
                List.Add(ArrInt);
                List[ArrInt - 1] = PrassSum;
            }


            if (List.Count == 0)
            {

                return "";
            }
            int Ci = 1;
            PrassSum = List[0].ToString();
            for (int i = Ci; i < List.Count; i++)
            {
                PrassSum = Encrypt(PrassSum, List[i].ToString(), 0);
            }

            MD5 md5 = MD5.Create();
            byte[] hdcode1 = System.Text.Encoding.UTF8.GetBytes(PrassSum + "new");
            byte[] hdcode2 = md5.ComputeHash(hdcode1);
            md5.Clear();
            char[] charData = new char[hdcode2.Length];//建立一个字符组
            Decoder d = Encoding.UTF8.GetDecoder();//实例化一个解码器
            d.GetChars(hdcode2, 0, hdcode2.Length, charData, 0);//将编码字节数组转换为字符数组
            PrassSum = "";
            for (int i = 0; i < charData.Length; i++)//将字符数组组合成字符串
            {
                PrassSum += charData[i].ToString();
            }
            return PrassSum;
        }

        /// <summary>
        /// 将密码写入EXE文件中
        /// </summary>
        /// <param StrDir="string">EXE文件的路径</param>
        /// <param Prass="string">加密数据</param>
        /// <returns>成功返回true，否则失败</returns>  
        private static bool WriteEXE(string StrDir, string Prass)
        {
            byte[] byData = new byte[100];//建立一个FileStream要用的字节组
            char[] charData = new char[100];//建立一个字符组

            try
            {
                Prass = Prass.Trim();
                FileStream aFile = new FileStream(StrDir, FileMode.Open);//实例化一个FileStream对象，用来操作data.txt文件，操作类型是
                charData = Prass.ToCharArray();//将字符串内的字符复制到字符组里
                aFile.Seek(0, SeekOrigin.End);//将指针移到文件尾
                Encoder el = Encoding.UTF8.GetEncoder();//解码器
                el.GetBytes(charData, 0, charData.Length, byData, 0, true);//将字符数组存入到字节数组中
                aFile.Write(byData, 0, byData.Length);//将字节写入到文件中
                aFile.Dispose();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 生成TXT文件
        /// </summary>
        /// <param Prass="string">加密数据</param>   
        /// <returns>成功返回true，否则失败</returns>         
        private static bool CreateTXT(string path,string Prass)
        {
            FileStream aFile;
            string TemDir = path.Substring(0, path.LastIndexOf("\\"));
            TemDir = string.Format("{0}\\{1}.TXT", TemDir, PFileN);
            byte[] byData = new byte[100];//建立一个FileStream要用的字节组
            char[] charData = new char[100];//建立一个字符组
            try
            {
                aFile = new FileStream(TemDir, FileMode.CreateNew);//实例化一个FileStream对象，用来操作data.txt文件，操作类型是
            }
            catch
            {
                aFile = new FileStream(TemDir, FileMode.Truncate);
            }
            try
            {
                Prass = "密码：" + Prass.Trim();
                charData = Prass.ToCharArray();//将字符串内的字符复制到字符组里
                aFile.Seek(0, SeekOrigin.Begin);//将指针移到文件首
                Encoder el = Encoding.UTF8.GetEncoder();//解码器
                el.GetBytes(charData, 0, charData.Length, byData, 0, true);
                aFile.Write(byData, 0, byData.Length);
                aFile.Dispose();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        #endregion

        #region 解密exe
        /// <summary>
        /// 密码
        /// </summary>
         static string StrPass = "";//密码
        /// <summary>
        /// 高等信息的值
        /// </summary>
         static string HighValue = "";//高等信息的值
        /// <summary>
        /// 高等信息的标识
        /// </summary>
         static string HighSgin = "";//高等信息的标识
        /// <summary>
        /// 记录限定时间
        /// </summary>
         static string enrolValse = "";//记录限定时间
        /// <summary>
        /// 记录日期，判断是否修改过日期
        /// </summary>
         static string NewDate = "";//记录日期，判断是否修改过日期
        /// <summary>
        /// 获取注册表中计算月数和天数的临时时间
        /// </summary>
         static string TemporarilyDate = "";//获取注册表中计算月数和天数的临时时间
        /// <summary>
        /// 是否显示解密窗体
        /// </summary>
         static string IfShow = "";//是否显示解密窗体
        /// <summary>
        /// 判断程序是不过期
        /// </summary>
         static bool Bypast = false;//判断程序是不过期

        /// <summary>
        /// 读取当前可执行文件的文件尾部信息
        /// </summary>
        /// <param Prass="path">exe路径 Application.ExecutablePath</param> 
        private static string ReadEXEFile(string path)
        {
            byte[] byData = new byte[100];//建立一个FileStream要用的字节组
            char[] charData = new char[100];//建立一个字符组

            try
            {
                FileStream aFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read);//实例化一个FileStream对象，用来操作data.txt文件，操作类型是
                aFile.Seek(-100, SeekOrigin.End);//把文件指针指向文件尾，从文件开始位置向前100位字节所指的字节
                aFile.Read(byData, 0, 100);//读取FileStream对象所指的文件到字节数组里
            }
            catch (Exception)
            {
                return "";
            }
            Decoder d = Encoding.UTF8.GetDecoder();//实例化一个解码器
            d.GetChars(byData, 0, byData.Length, charData, 0);//将编码字节数组转换为字符数组
            string Zpp = "";
            for (int i = 0; i < charData.Length; i++)//将字符组合成字符串
            {
                Zpp += charData[i].ToString();
            }
            Zpp = Zpp.Replace("\0", "");//将字符串后面的\0替换为空

            return Zpp.Trim();
        }

        /// <summary>
        /// 读取文件尾部的高等信息
        /// </summary>
        /// <param Field="string">文件尾部信息</param> 
        private static string ReadAltitude(string Field)
        {
            StrPass = Field;
            if (Field.LastIndexOf(",") > -1)
            {
                string Cauda = Field.Substring(Field.LastIndexOf(",") + 1, Field.Length - Field.LastIndexOf(",") - 1);
                switch (Cauda.Substring(Cauda.Length - 1, 1))
                {
                    case "D":
                        {
                            StrPass = Field.Substring(0, Field.LastIndexOf(","));//密码
                            HighValue = Cauda.Substring(0, Cauda.Length - 1);//高等信息的值
                            HighSgin = "D";//高等信息的标识
                            break;
                        }
                    case "M":
                        {
                            StrPass = Field.Substring(0, Field.LastIndexOf(","));//密码
                            HighValue = Cauda.Substring(0, Cauda.Length - 1);//高等信息的值
                            HighSgin = "M";//高等信息的标识
                            break;
                        }
                    case "A":
                        {
                            StrPass = Field.Substring(0, Field.LastIndexOf(","));//密码
                            HighValue = Cauda.Substring(0, Cauda.Length - 1);//高等信息的值
                            HighSgin = "A";//高等信息的标识
                            break;
                        }
                    case "C":
                        {
                            StrPass = Field.Substring(0, Field.LastIndexOf(","));//密码
                            HighValue = Cauda.Substring(0, Cauda.Length - 1);//高等信息的值
                            HighSgin = "C";//高等信息的标识
                            break;
                        }
                }
            }
            return StrPass;
        }

        /// <summary>
        /// 读取注册表中的信息
        /// </summary>
        /// <param Field="string">注册表中的信息</param> 
        private static bool ReadRegistered(string Field)
        {
            string Cauda = Field;
            IfShow = Cauda.Substring(Cauda.Length - 1, 1);
            if (Cauda.Length <= 1)
                return false;
            switch (Cauda.Substring(Cauda.Length - 2, 1))
            {
                case "D":
                    {
                        HighValue = Cauda.Substring(0, Cauda.Length - 2);//高等信息的值
                        HighSgin = "D";//高等信息的标识
                        break;
                    }
                case "M":
                    {
                        HighValue = Cauda.Substring(0, Cauda.Length - 2);//高等信息的值
                        HighSgin = "M";//高等信息的标识
                        break;
                    }
                case "A":
                    {
                        HighValue = Cauda.Substring(0, Cauda.Length - 2);//高等信息的值
                        HighSgin = "A";//高等信息的标识
                        break;
                    }
                case "C":
                    {
                        HighValue = Cauda.Substring(0, Cauda.Length - 2);//高等信息的值
                        HighSgin = "C";//高等信息的标识
                        break;
                    }

            }

            return true;
        }

        /// <summary>
        /// 比较前一个日期是否大于后一个日期
        /// </summary>
        /// <param Date_1="string">日期</param> 
        /// <param Date_2="string">日期</param>  
        private static bool DateCompare(string Date_1, string Date_2)
        {
            string[] D1;
            string[] D2;
            bool Comp = false;
            D1 = Date_1.Split(Convert.ToChar('-'));
            D2 = Date_2.Split(Convert.ToChar('-'));
            for (int i = 0; i < D1.Length; i++)
            {
                if (Convert.ToInt32(D1[i]) > Convert.ToInt32(D2[i]))
                {
                    Comp = true;
                    break;
                }
            }
            return Comp;
        }

        /// <summary>
        /// 比较两个日期的月份差
        /// </summary>
        /// <param Old_Date="DateTime">日期</param> 
        /// <param New_Date="DateTime">日期</param>  
        private static int MonthJob(DateTime Old_Date, DateTime New_Date)
        {
            int OY = Old_Date.Year;//年
            int OM = Old_Date.Month;//月
            int OD = Old_Date.Day;//日
            int NY = New_Date.Year;
            int NM = New_Date.Month;
            int ND = New_Date.Day;
            int fY = 0;//年进/减减
            int fM = 0;//月进/减减
            int d = ND - OD;
            if (NM > OM)
            {
                if (d > 0)
                {

                    fM = 1;
                }

                if (d < 0)
                    fM = -1;
            }
            int m = NM + fM - OM;
            if (m < 0)
            {
                fY = -1;
                m = 12 + m;
            }
            int y = NY + fY - OY;

            int Months = y * 12 + m;
            return Months;
        }

        /// <summary>
        /// 比较两个日期的天数差
        /// </summary>
        /// <param Old_Date="DateTime">老日期</param> 
        /// <param New_Date="DateTime">新日期</param>  
        private static int DayJob(DateTime Old_Date, DateTime New_Date)
        {
            TimeSpan TMs = New_Date - Old_Date;
            int ms = TMs.Days;
            return ms;
        }

        /// <summary>
        /// 修改注册表
        /// </summary>
        /// <param Field="string">当前可执行文件的名称</param> 
         static void AmendEnrol(string FDir)
        {
            int job = 0;
            RegistryKey retkey = Registry.CurrentUser.OpenSubKey("software", true).CreateSubKey("LB").CreateSubKey(FDir).CreateSubKey("Altitude");

            //判断应用程序的使用期限
            switch (HighSgin)
            {
                case "D"://日期
                    {
                        if (DateCompare(System.DateTime.Now.ToShortDateString().Trim(), HighValue.Trim()))
                            Bypast = true;
                        break;
                    }
                case "M"://月数
                    {
                        if (Convert.ToInt32(HighValue) <= 0)
                            Bypast = true;
                        else
                        {
                            job = MonthJob(Convert.ToDateTime(TemporarilyDate), Convert.ToDateTime(System.DateTime.Now.ToShortDateString()));
                            if (job > 0)
                                retkey.SetValue("DateMonth", System.DateTime.Now.ToString());
                        }
                        break;
                    }
                case "A"://天数
                    {

                        if (Convert.ToInt32(HighValue) <= 0)
                            Bypast = true;
                        else
                        {
                            job = DayJob(Convert.ToDateTime(TemporarilyDate), Convert.ToDateTime(System.DateTime.Now.ToShortDateString()));
                            if (job > 0)
                                retkey.SetValue("DateMonth", System.DateTime.Now.ToString());
                        }
                        break;
                    }
                case "C"://次数
                    {
                        if (Convert.ToInt32(HighValue) <= 0)
                            Bypast = true;
                        else
                            job = 1;
                        break;
                    }
            }
            if (HighSgin == "M" || HighSgin == "A" || HighSgin == "C")
            {
                job = Convert.ToInt32(HighValue) - job;
                retkey.SetValue("UserName", job.ToString() + HighSgin + IfShow);
            }
            retkey.SetValue("DateCounter", System.DateTime.Now.ToString());
        }




        /// <summary>
        /// 解密exe
        /// </summary>
        /// <param name="path">exe路径，包括文件名称</param>
        /// <param name="dispel">解码</param>
        /// <param name="isRegistry">是否写进注册表</param>
        public static void DecoderExe(string path, string dispel, bool isRegistry)
        {
            if (dispel == "")
            {
                return;
            }

            string PPrass = ReadEXEFile(path);
            PPrass = ReadAltitude(PPrass);//分离密码与高信息
            string TPrass = dispel;
            //MessageBox.Show("密码：  " + StrPass + "  高级： " + HighValue + "   标识：    " + HighSgin);
            dispel = TPrass;
            if (PPrass == dispel)
            {
                string Fshow;
                if (isRegistry == true)//将指定信息写入到注册表中
                {
                    Fshow = "T";
                }
                else
                {
                    Fshow = "F";
                }
                //添加的注册码路径：HKEY_CURRENT_USER-Software-LB
                string FDir = path;
                FDir = FDir.Substring(FDir.LastIndexOf("\\") + 1, FDir.Length - FDir.LastIndexOf("\\") - 1);
                RegistryKey retkey = Registry.CurrentUser.OpenSubKey("software", true).CreateSubKey("LB").CreateSubKey(FDir).CreateSubKey("Altitude");

                enrolValse = "";
                NewDate = "";
                TemporarilyDate = "";
                foreach (string sVName in retkey.GetValueNames())
                {
                    if (sVName == "UserName")
                    {
                        enrolValse = retkey.GetValue(sVName).ToString();
                    }
                    if (sVName == "DateCounter")
                    {
                        NewDate = retkey.GetValue(sVName).ToString();
                    }
                    if (sVName == "DateMonth")
                    {
                        TemporarilyDate = retkey.GetValue(sVName).ToString();
                    }
                }
                if (HighSgin == "C")
                    HighValue = Convert.ToString(Convert.ToInt32(HighValue) - 1);
                string Str_Altitude = HighValue + HighSgin + Fshow;
                if (enrolValse == "" || NewDate == "" || TemporarilyDate == "")
                {
                    retkey.SetValue("UserName", Str_Altitude.Trim());
                    retkey.SetValue("DateCounter", System.DateTime.Now.ToString());
                    retkey.SetValue("DateMonth", System.DateTime.Now.ToShortDateString());
                }
                else
                {
                    ReadRegistered(enrolValse);
                    AmendEnrol(FDir);
                }

            }
        }
        #endregion
    }
}
