using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

namespace SuperFramework
{
    /// <summary>
    /// 日 期:2015-07-21
    /// 作 者:不良帥
    /// 描 述:串口开发辅助类(含静态方法)
    /// </summary>
    public class SerialPortHelper
    {
        /// <summary>
        /// 接收事件是否有效 false表示有效
        /// </summary>
        public bool ReceiveEventFlag = false;
        /// <summary>
        /// 结束符比特
        /// </summary>
        public byte EndByte = 0x23;//string End = "#";

        /// <summary>
        /// 完整协议的记录处理事件
        /// </summary>
        public event DataReceivedEventHandler DataReceived;
        /// <summary>
        /// 错误处理事件
        /// </summary>
        public event SerialErrorReceivedEventHandler Error;

        #region 变量属性
        private string _portName = "COM1";//串口号，默认COM1
        private SerialPortBaudRates _baudRate = SerialPortBaudRates.BaudRate_57600;//波特率
        private Parity _parity = Parity.None;//校验位
        private StopBits _stopBits = StopBits.One;//停止位
        private SerialPortDatabits _dataBits = SerialPortDatabits.EightBits;//数据位

        private SerialPort comPort = new SerialPort();

        /// <summary>
        /// 串口号
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }

        /// <summary>
        /// 波特率
        /// </summary>
        public SerialPortBaudRates BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        /// <summary>
        /// 奇偶校验位
        /// </summary>
        public Parity Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        /// <summary>
        /// 数据位
        /// </summary>
        public SerialPortDatabits DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 参数构造函数（使用枚举参数构造）
        /// </summary>
        /// <param name="baud">波特率</param>
        /// <param name="par">奇偶校验位</param>
        /// <param name="sBits">停止位</param>
        /// <param name="dBits">数据位</param>
        /// <param name="name">串口号</param>
        public SerialPortHelper(string name, SerialPortBaudRates baud, Parity par, SerialPortDatabits dBits, StopBits sBits)
        {
            _portName = name;
            _baudRate = baud;
            _parity = par;
            _dataBits = dBits;
            _stopBits = sBits;

            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
            comPort.ErrorReceived += new SerialErrorReceivedEventHandler(comPort_ErrorReceived);
        }

        /// <summary>
        /// 参数构造函数（使用字符串参数构造）
        /// </summary>
        /// <param name="baud">波特率</param>
        /// <param name="par">奇偶校验位</param>
        /// <param name="sBits">停止位</param>
        /// <param name="dBits">数据位</param>
        /// <param name="name">串口号</param>
        public SerialPortHelper(string name, string baud, string par, string dBits, string sBits)
        {
            _portName = name;
            _baudRate = (SerialPortBaudRates)Enum.Parse(typeof(SerialPortBaudRates), baud);
            _parity = (Parity)Enum.Parse(typeof(Parity), par);
            _dataBits = (SerialPortDatabits)Enum.Parse(typeof(SerialPortDatabits), dBits);
            _stopBits = (StopBits)Enum.Parse(typeof(StopBits), sBits);

            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
            comPort.ErrorReceived += new SerialErrorReceivedEventHandler(comPort_ErrorReceived);
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public SerialPortHelper()
        {
            _portName = "COM1";
            _baudRate = SerialPortBaudRates.BaudRate_9600;
            _parity = Parity.None;
            _dataBits = SerialPortDatabits.EightBits;
            _stopBits = StopBits.One;

            comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
            comPort.ErrorReceived += new SerialErrorReceivedEventHandler(comPort_ErrorReceived);
        }

        #endregion

        /// <summary>
        /// 端口是否已经打开
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return comPort.IsOpen;
            }
        }

        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        public void OpenPort()
        {
            if (comPort.IsOpen)
                comPort.Close();

            comPort.PortName = _portName;
            comPort.BaudRate = (int)_baudRate;
            comPort.Parity = _parity;
            comPort.DataBits = (int)_dataBits;
            comPort.StopBits = _stopBits;

            comPort.Open();
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        public void ClosePort()
        {
            if (comPort.IsOpen)
                comPort.Close();
        }

        /// <summary>
        /// 丢弃来自串行驱动程序的接收和发送缓冲区的数据
        /// </summary>
        public void DiscardBuffer()
        {
            comPort.DiscardInBuffer();
            comPort.DiscardOutBuffer();
        }

        /// <summary>
        /// 数据接收处理
        /// </summary>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //禁止接收事件时直接退出
            if (ReceiveEventFlag) return;

            #region 根据结束字节来判断是否全部获取完成
            List<byte> _byteData = new List<byte>();
            bool found = false;//是否检测到结束符号
            while (comPort.BytesToRead > 0 || !found)
            {
                byte[] readBuffer = new byte[comPort.ReadBufferSize + 1];
                int count = comPort.Read(readBuffer, 0, comPort.ReadBufferSize);
                for (int i = 0; i < count; i++)
                {
                    _byteData.Add(readBuffer[i]);

                    if (readBuffer[i] == EndByte)
                    {
                        found = true;
                    }
                }
            }
            #endregion

            //字符转换
            string readString = System.Text.Encoding.Default.GetString(_byteData.ToArray(), 0, _byteData.Count);

            //触发整条记录的处理
            DataReceived?.Invoke(new DataReceivedEventArgs(readString));
        }

        /// <summary>
        /// 错误处理函数
        /// </summary>
        void comPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Error?.Invoke(sender, e);
        }

        #region 数据写入操作

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="msg"></param>
        public void WriteData(string msg)
        {
            if (!(comPort.IsOpen)) comPort.Open();

            comPort.Write(msg);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="msg">写入端口的字节数组</param>
        public void WriteData(byte[] msg)
        {
            if (!(comPort.IsOpen)) comPort.Open();

            comPort.Write(msg, 0, msg.Length);
        }

        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="msg">包含要写入端口的字节数组</param>
        /// <param name="offset">参数从0字节开始的字节偏移量</param>
        /// <param name="count">要写入的字节数</param>
        public void WriteData(byte[] msg, int offset, int count)
        {
            if (!(comPort.IsOpen)) comPort.Open();

            comPort.Write(msg, offset, count);
        }

        /// <summary>
        /// 发送串口命令
        /// </summary>
        /// <param name="SendData">发送数据</param>
        /// <param name="ReceiveData">接收数据</param>
        /// <param name="Overtime">重复次数</param>
        /// <returns></returns>
        public int SendCommand(byte[] SendData, ref byte[] ReceiveData, int Overtime)
        {
            if (!(comPort.IsOpen)) comPort.Open();

            ReceiveEventFlag = true;        //关闭接收事件
            comPort.DiscardInBuffer();      //清空接收缓冲区                 
            comPort.Write(SendData, 0, SendData.Length);

            int num = 0, ret = 0;
            while (num++ < Overtime)
            {
                if (comPort.BytesToRead >= ReceiveData.Length) break;
                System.Threading.Thread.Sleep(1);
            }

            if (comPort.BytesToRead >= ReceiveData.Length)
            {
                ret = comPort.Read(ReceiveData, 0, ReceiveData.Length);
            }

            ReceiveEventFlag = false;       //打开事件
            return ret;
        }

        #endregion

        #region 常用的列表数据获取和绑定操作

        /// <summary>
        /// 封装获取串口号列表
        /// </summary>
        /// <returns></returns>
        public static string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        /// <summary>
        /// 设置串口号
        /// </summary>
        /// <param name="obj"></param>
        public static void SetPortNameValues(ComboBox obj)
        {
            obj.Items.Clear();
            foreach (string str in SerialPort.GetPortNames())
            {
                obj.Items.Add(str);
            }
        }

        /// <summary>
        /// 设置波特率
        /// </summary>
        public static void SetBauRateValues(ComboBox obj)
        {
            obj.Items.Clear();
            foreach (SerialPortBaudRates rate in Enum.GetValues(typeof(SerialPortBaudRates)))
            {
                obj.Items.Add(((int)rate).ToString());
            }
        }

        /// <summary>
        /// 设置数据位
        /// </summary>
        public static void SetDataBitsValues(ComboBox obj)
        {
            obj.Items.Clear();
            foreach (SerialPortDatabits databit in Enum.GetValues(typeof(SerialPortDatabits)))
            {
                obj.Items.Add(((int)databit).ToString());
            }
        }

        /// <summary>
        /// 设置校验位列表
        /// </summary>
        public static void SetParityValues(ComboBox obj)
        {
            obj.Items.Clear();
            foreach (string str in Enum.GetNames(typeof(Parity)))
            {
                obj.Items.Add(str);
            }
            //foreach (Parity party in Enum.GetValues(typeof(Parity)))
            //{
            //    obj.Items.Add(((int)party).ToString());
            //}
        }

        /// <summary>
        /// 设置停止位
        /// </summary>
        public static void SetStopBitValues(ComboBox obj)
        {
            obj.Items.Clear();
            foreach (string str in Enum.GetNames(typeof(StopBits)))
            {
                obj.Items.Add(str);
            }
            //foreach (StopBits stopbit in Enum.GetValues(typeof(StopBits)))
            //{
            //    obj.Items.Add(((int)stopbit).ToString());
            //}   
        }

        #endregion

        #region 格式转换
        /// <summary>
        /// 转换十六进制字符串到字节数组
        /// </summary>
        /// <param name="msg">待转换字符串</param>
        /// <returns>字节数组</returns>
        public static byte[] HexToByte(string msg)
        {
            msg = msg.Replace(" ", "");//移除空格

            //create a byte array the length of the
            //divided by 2 (Hex is 2 characters in length)
            byte[] comBuffer = new byte[msg.Length / 2];
            for (int i = 0; i < msg.Length; i += 2)
            {
                //convert each set of 2 characters to a byte and add to the array
                comBuffer[i / 2] = (byte)Convert.ToByte(msg.Substring(i, 2), 16);
            }

            return comBuffer;
        }

        /// <summary>
        /// 转换字节数组到十六进制字符串
        /// </summary>
        /// <param name="comByte">待转换字节数组</param>
        /// <returns>十六进制字符串</returns>
        public static string ByteToHex(byte[] comByte)
        {
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            foreach (byte data in comByte)
            {
                builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
            }

            return builder.ToString().ToUpper();
        }
        #endregion

        /// <summary>
        /// 检查端口名称是否存在
        /// </summary>
        /// <param name="port_name"></param>
        /// <returns></returns>
        public static bool Exists(string port_name)
        {
            foreach (string port in SerialPort.GetPortNames()) if (port == port_name) return true;
            return false;
        }

        /// <summary>
        /// 格式化端口相关属性
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static string Format(SerialPort port)
        {
            return string.Format("{0} ({1},{2},{3},{4},{5})",
                port.PortName, port.BaudRate, port.DataBits, port.StopBits, port.Parity, port.Handshake);
        }

        public class DataReceivedEventArgs : EventArgs
        {
            public string DataReceived;
            public DataReceivedEventArgs(string m_DataReceived)
            {
                DataReceived = m_DataReceived;
            }
        }

        public delegate void DataReceivedEventHandler(DataReceivedEventArgs e);


        /// <summary>
        /// 串口数据位列表（5,6,7,8）
        /// </summary>
        public enum SerialPortDatabits : int
        {
            /// <summary>
            /// 5 bit
            /// </summary>
            FiveBits = 5,
            /// <summary>
            /// 6 bit
            /// </summary>
            SixBits = 6,
            /// <summary>
            /// 7 bit
            /// </summary>
            SeventBits = 7,
            /// <summary>
            /// 8 bit
            /// </summary>
            EightBits = 8
        }

        /// <summary>
        /// 串口波特率列表。
        /// 75,110,150,300,600,1200,2400,4800,9600,14400,19200,28800,38400,56000,57600,
        /// 115200,128000,230400,256000
        /// </summary>
        public enum SerialPortBaudRates : int
        {
            BaudRate_75 = 75,
            BaudRate_110 = 110,
            BaudRate_150 = 150,
            BaudRate_300 = 300,
            BaudRate_600 = 600,
            BaudRate_1200 = 1200,
            BaudRate_2400 = 2400,
            BaudRate_4800 = 4800,
            BaudRate_9600 = 9600,
            BaudRate_14400 = 14400,
            BaudRate_19200 = 19200,
            BaudRate_28800 = 28800,
            BaudRate_38400 = 38400,
            BaudRate_56000 = 56000,
            BaudRate_57600 = 57600,
            BaudRate_115200 = 115200,
            BaudRate_128000 = 128000,
            BaudRate_230400 = 230400,
            BaudRate_256000 = 256000
        }

        #region 方法

        /// <summary>
        /// 十六进制字符串转为字节数组
        /// </summary>
        /// <param name="s">16进制字符串</param>
        /// <returns>返回16进制数组</returns>
        public static byte[] StringToHexByteArray(string s)
        {
            s = s.Replace(" ", "");
            if ((s.Length % 2) != 0)
                s += " ";
            byte[] returnBytes = new byte[s.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(s.Substring(i * 2, 2), 16);
            return returnBytes;

        }

        /// <summary>
        /// 十六进制字节数组转为字符串
        /// </summary>
        /// <param name="data">16进制数组</param>
        /// <returns>返回16进制字符串</returns>
        public static string HexByteArrayToString(byte[] data)
        {
            StringBuilder strBuilder = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                strBuilder.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            return strBuilder.ToString().ToUpper(); //将得到的字符全部以字母大写形式输出
        }
        #endregion

        #region 变量

        /// <summary>
        /// 申明对象
        /// </summary>
        public static SerialPort MySerialPort;
        /// <summary>
        /// 存储接收的缓存数据
        /// </summary>
        public static List<byte> BufferList = new List<byte>();

        #endregion

        /// <summary>
        /// 发送数据 "FF FE XX XX XX XX XX"
        /// </summary>
        /// <param name="strData">数据格式 "FF FE XX XX XX XX XX"</param>
        public static bool SendData(string strData)
        {
            try
            {
                //"FF FE XX XX XX XX XX";数据格式
                byte[] writeBuffer = StringToHexByteArray(strData);
                MySerialPort.Write(writeBuffer, 0, writeBuffer.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
                //MessageBox.Show(string.Format("发送指令数据错误，错误原因：{0}", ex.Message));
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="writeBuffer">数据</param>
        public static bool SendData(byte[] writeBuffer)
        {
            try
            {
                ////"FF FE XX XX XX XX XX";数据格式
                //byte[] writeBuffer = StringToHexByteArray(strData);
                MySerialPort.Write(writeBuffer, 0, writeBuffer.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
                //MessageBox.Show(string.Format("发送指令数据错误，错误原因：{0}", ex.Message));
            }
        }
        /// <summary>
        /// 接收数据委托
        /// </summary>
        /// <param name="str">16进制字符串</param>
        public delegate void ShowDataCallback(string str, byte[] data);
        /// <summary>
        /// 接收数据委托事件
        /// </summary>
        public static event ShowDataCallback DataCallBack;

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="baud">波特率</param>
        /// <param name="par">奇偶校验位</param>
        /// <param name="sBits">停止位</param>
        /// <param name="dBits">数据位</param>
        /// <param name="name">串口号</param>
        public static bool OpenSerialPort(string name, SerialPortBaudRates baud, Parity par, SerialPortDatabits dBits, StopBits sBits)
        {
            if (MySerialPort == null)
                MySerialPort = new SerialPort();
            try
            {
                //打开串口

                MySerialPort.PortName = name;
                MySerialPort.BaudRate = (int)baud; //波特率
                MySerialPort.Parity = par;
                MySerialPort.DataBits = (int)dBits;
                MySerialPort.StopBits = sBits;
                MySerialPort.DataReceived += new SerialDataReceivedEventHandler(SpDataReceived);
                MySerialPort.Open();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 串口接收数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        internal static void SpDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //根据不同协议更改协议长度和针头针尾数据
            //这里以FF FE XX XX XX XX XX AA为例
            //针头为：FF FE，针尾为：AA
            try
            {
                int n = MySerialPort.BytesToRead; //记录缓存数据的长度
                byte[] buf = new byte[n]; //声明一个临时数组存储当前来的串口数据  
                MySerialPort.Read(buf, 0, n); //读取缓冲数据  
                BufferList.AddRange(buf);//将本次接收到的缓存数据添加到BufferList列表数据中
                ////当BufferList缓存列表数据大于或等于协议数据长度时开始处理数据
                //while (BufferList.Count >= 8)
                //{
                //    //查找数据头（重要，防止数据丢失或不全）
                //    if (BufferList[0] != 0xFF || BufferList[1] != 0xFE || BufferList[7] != 0xAA)
                //    {
                //        //若针头针尾没有达到协议标准则移除BufferList第一位，继续下一轮判断
                //        BufferList.RemoveAt(0);
                //        continue;
                //    }
                //    //若前面数据验证成功则将数据取出
                //    StringBuilder myStringBuilder = new StringBuilder(MySerialPort.ReadBufferSize * 2);
                //    for (int i = 0; i < 8; i++)
                //    {
                //        myStringBuilder.Append(String.Format("{0:X2}", Convert.ToInt32(BufferList[i])) + " ");
                //    }
                //    //移除BufferList列表中已处理的数据
                //    BufferList.RemoveRange(0, 8); //从缓存中删除错误数据  
                //                                  // Invoke(new ShowDataCallback(ShowData), myStringBuilder.ToString());
                string vla = ByteToHex(buf);
                vla = SuperConvert.ByteHelper.StringFromByteArray(buf, Encoding.UTF8);
                DataCallBack?.BeginInvoke(vla, buf, null, null);
                BufferList.Clear();
                //}
            }
            catch (Exception)
            {
                //最好写日志文件，保存起来
                //MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 关闭串口按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static bool CloseSerialPort()
        {
            try
            {
                //Application.DoEvents();
                MySerialPort.Close();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}