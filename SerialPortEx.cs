using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace SuperFramework
{
    /// <summary>
    ///串口操作辅助类，初始化必要调用SerialPortInstace(*,*,...)方法初始化。通过绑定自定义事件OnDataReceived获取串口数据
    /// </summary>

    public static class SerialPortEx
    {
        /// <summary>
        /// 接收数据统一事件绑定，注意：如果获取该数据需要对界面控件更新，绑定的方法中请使用委托，否则会报跨线程访问控件错误
        /// </summary>
        public static Action<ReceivedDataArgs> OnDataReceived;

        /// <summary>
        /// 串口操作类，可对该类设置需要的参数，如果需要重载接收数据，只需绑定事件OnDataReceived即可，如果需要协议，在重载事件中处理
        /// </summary>
        public static SerialPort SerialPort;

        /// <summary>
        /// 事件是否初始化
        /// </summary>
        static bool _bini = false;

        /// <summary>
        /// 打开串口，如果串口没有打开则会打开，串口已打开，则不处理
        /// </summary>
        public static void Open()
        {
            if (!SerialPort.IsOpen)
            {
                SerialPort.Open();
            }
        }

        /// <summary>
        /// 关闭串口，如果串口打开，则会关闭
        /// </summary>
        public static void Close()
        {
            if (_bini)
            {
                if (SerialPort.IsOpen)
                {
                    SerialPort.Close();
                }
            }
        }
        /// <summary>
        /// 设置串口号
        /// </summary>
        /// <param name="portname"></param>
        public static void SetPort(string portname)
        {
            if (_bini)
            {
                bool bopen = SerialPort.IsOpen;//设置串口号要保持串口号的状态一致
                if (bopen)
                {
                    SerialPort.Close();
                }
                SerialPort.PortName = portname;
                if (bopen)
                {
                    SerialPort.Open();
                }
            }
        }

        /// <summary>
        /// 返回当前电脑可用串口数
        /// </summary>
        /// <returns></returns>
        public static string[] Ports()
        {
            return SerialPort.GetPortNames();
        }

        #region 初始化串口参数
        /// <summary>
        /// 串口初始化（如果调用过该方法后，在再调用该方法或重载方法都无效，如一定要重新初始化，请先调用Clear()清除方法后再调用）
        /// </summary>
        public static void SerialPortInstace()
        {
            if (!_bini)
            {
                SerialPort = new SerialPort();
                SerialPort.DataReceived += SerialPort_DataReceived;
                _bini = true;
            }
        }
        /// <summary>
        /// 串口初始化（如果调用过该方法后，在再调用该方法或重载方法都无效，如一定要重新初始化，请先调用Clear()清除方法后再调用）
        /// </summary>
        /// <param name="portname">串口号</param>
        public static void SerialPortInstace(string portname)
        {
            if (!_bini)
            {
                SerialPort = new SerialPort(portname);
                SerialPort.DataReceived += SerialPort_DataReceived;
                _bini = true;
            }
        }

        /// <summary>
        /// 串口初始化（如果调用过该方法后，在再调用该方法或重载方法都无效，如一定要重新初始化，请先调用Clear()清除方法后再调用）
        /// </summary>
        /// <param name="portname">串口号</param>
        /// <param name="baudrate">波特率</param>
        public static void SerialPortInstace(string portname, int baudrate)
        {
            if (!_bini)
            {
                SerialPort = new SerialPort(portname, baudrate);
                SerialPort.DataReceived += SerialPort_DataReceived;
                _bini = true;
            }
        }

        /// <summary>
        /// 串口初始化（如果调用过该方法后，在再调用该方法或重载方法都无效，如一定要重新初始化，请先调用Clear()清除方法后再调用）
        /// </summary>
        /// <param name="portname">串口号</param>
        /// <param name="baudrate">波特率</param>
        /// <param name="parity">奇偶校验位</param>
        public static void SerialPortInstace(string portname, int baudrate, Parity parity)
        {
            if (!_bini)
            {
                SerialPort = new SerialPort(portname, baudrate, parity);
                SerialPort.DataReceived += SerialPort_DataReceived;
                _bini = true;
            }
        }

        /// <summary>
        /// 串口初始化（如果调用过该方法后，在再调用该方法或重载方法都无效，如一定要重新初始化，请先调用Clear()清除方法后再调用）
        /// </summary>
        /// <param name="portname"串口号></param>
        /// <param name="baudrate">波特率</param>
        /// <param name="parity">奇偶校验位</param>
        /// <param name="databits">数据位</param>
        public static void SerialPortInstace(string portname, int baudrate, Parity parity, int databits)
        {
            if (!_bini)
            {
                SerialPort = new SerialPort(portname, baudrate, parity, databits);
                SerialPort.DataReceived += SerialPort_DataReceived;
                _bini = true;
            }
        }
        /// <summary>
        /// 串口初始化（如果调用过该方法后，在再调用该方法或重载方法都无效，如一定要重新初始化，请先调用Clear()清除方法后再调用）
        /// </summary>
        /// <param name="portname">串口号</param>
        /// <param name="baudrate">波特率</param>
        /// <param name="parity">奇偶校验位</param>
        /// <param name="databits">数据位</param>
        /// <param name="stopbits">停止位</param>
        public static void SerialPortInstace(string portname, int baudrate, Parity parity, int databits, StopBits stopbits)
        {
            if (!_bini)
            {
                SerialPort = new SerialPort(portname, baudrate, parity, databits, stopbits);
                SerialPort.DataReceived += SerialPort_DataReceived;
                _bini = true;
            }
        }
        /// <summary>
        /// 清除串口配置参数同时并关闭串口
        /// </summary>
        public static void Clear()
        {
            if (_bini)
            {
                if (OnDataReceived != null)
                {
                    OnDataReceived(new ReceivedDataArgs(ReceivedDataArgs.StringToByte("##****$$")));//特殊指令，接到该指令，关闭串口，控制只能有一个地方使用串口
                }
                Close();
                SerialPort.DataReceived -= SerialPort_DataReceived;
                OnDataReceived = null;
                _bini = false;
            }
        }
        #endregion

        /// <summary>
        /// 时间缓存
        /// </summary>
        static int _timebuf = 100;
        public static int TimeReadBuf
        {
            get { return _timebuf; }
            set { _timebuf = value > 100 ? value : 100; }
        }
        // 串口数据接收
        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(TimeReadBuf);//不是在主线程，这里多少时间都没有关系，主要是缓存数据，读完缓冲区后会自动清除
            int iLen = SerialPort.BytesToRead;//获取缓存大小
            byte[] byRead = new byte[iLen];
            int iBarCodeLen = SerialPort.Read(byRead, 0, iLen);
            Close();//为了安全，关闭串口
            OnDataReceived?.Invoke(new ReceivedDataArgs(byRead));
            Open();//重新打开串口
        }

        ///<summary>
        /// 创建日期:   2015-08-12 
        /// 最后修改日期: 2015-08-21
        /// 当串口接收到数据时，会产生一个事件。
        /// SerialDataArgs就是该事件的参数，参数中的RecvData包含接收到的数据。
        /// 修改记录：
        /// 2015-08-21
        /// 新增字符串与字节之间的互相转化
        /// 使用方法：
        /// SerialPortEx.OnDataReceived+=***即可
        /// </summary>
      
        public class ReceivedDataArgs : EventArgs
        {

            static System.Text.UTF8Encoding _converter = new System.Text.UTF8Encoding();//支持中文

            /// <summary>
            /// 接收到的数据组成的字节数组
            /// </summary>
            private byte[] recvData;

            /// <summary>
            /// 构造函数,需要一个包含数据的byte[]作为初始化参数来实例化 SerialDataArgs
            /// </summary>
            /// <param name="_recvData">接收到的数据</param>
            public ReceivedDataArgs(byte[] _recvData)
            {
                if (_recvData == null)
                {
                    throw (new ArgumentNullException("recvData数据为null"));
                }
                recvData = _recvData;
            }

            /// <summary>
            /// 返回接收到的数据内容btye[]形式
            /// </summary>
            public byte[] RecvData
            {
                get
                {
                    return recvData;
                }
            }
            /// <summary>
            /// 返回操作系统当前ANSI代码页的编码形式接收数据
            /// </summary>
            public string EncodingDefaultData
            {
                get
                {
                    string strtemp = Encoding.Default.GetString(recvData);
                    if (strtemp.Length > 2)
                    {
                        if (strtemp.Substring(strtemp.Length - 2).IndexOf("\r\n") != -1)
                        {
                            strtemp = strtemp.Substring(0, strtemp.Length - 2);
                        }
                    }
                    return strtemp;
                }
            }

            /// <summary>
            /// 返回十六进制形式接收数据
            /// </summary>
            public string HexData
            {
                get { return ByteToHexString(recvData); }
            }
            /// <summary>
            /// 返回字符串接收数据，支持中文,该字符串如果末尾有\r\n换行符，已自动去除
            /// </summary>
            public string UTF8Data
            {
                get
                {
                    string strtemp = _converter.GetString(recvData);
                    if (strtemp.Length > 2)
                    {
                        if (strtemp.Substring(strtemp.Length - 2).IndexOf("\r\n") != -1)
                        {
                            strtemp = strtemp.Substring(0, strtemp.Length - 2);
                        }
                    }
                    return strtemp;
                }
            }

            #region 字节数组转化为十六进制
            /// <summary>
            /// 字节数组转化为十六进制
            /// </summary>
            /// <param name="InBytes">字节数组</param>
            /// <returns></returns>
            public static string ByteToHexString(byte[] InBytes)
            {
                StringBuilder sb = new StringBuilder();
                foreach (byte InByte in InBytes)
                {
                    sb.Append(String.Format("{0:X2} ", InByte));
                }
                return sb.ToString();
            }
            #endregion

            #region 字节数组转化为十六进制（指定长度）
            /// <summary>
            /// 字节数组转化为十六进制（指定长度）
            /// </summary>
            /// <param name="InBytes">字节数组</param>
            /// <param name="len"></param>
            /// <returns></returns>
            public static string ByteToHexString(byte[] InBytes, int len)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < len; i++)
                {
                    sb.Append(String.Format("{0:X2} ", InBytes));
                }
                return sb.ToString();
            }
            #endregion

            #region 十六进制字符串转换成字节型
            /// <summary>
            /// 十六进制字符串转换成字节型  
            /// </summary>
            /// <param name="InHexString">十六进制字符串</param>
            /// <returns></returns>
            public static byte[] HexStringToByte(string InHexString)
            {
                string[] ByteStrings = InHexString.Split(" ".ToCharArray());
                byte[] ByteOut = new byte[ByteStrings.Length - 1];
                for (int i = 0; i == ByteStrings.Length - 1; i++)
                {
                    ByteOut[i] = Convert.ToByte(("0x" + ByteStrings));
                }
                return ByteOut;
            }
            #endregion

            #region 字符串转换成字节型
            /// <summary>
            /// 字符串(支持中文)转换成字节型
            /// </summary>
            /// <param name="InString">字符串</param>
            /// <returns></returns>
            public static byte[] StringToByte(string InString)
            {
                return _converter.GetBytes(InString);
            }
            #endregion

            #region 字节数组转化为字符串
            /// <summary>
            /// 字节数组转化为字符串（支持中文）
            /// </summary>
            /// <param name="InBytes">字节数组</param>
            /// <returns></returns>
            public static string ByteToString(byte[] InBytes)
            {
                return _converter.GetString(InBytes);
            }
            #endregion
        }
    }
}

