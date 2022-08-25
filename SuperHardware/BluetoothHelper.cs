using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
#if NET461_OR_GREATER
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
#endif
namespace SuperFramework.SuperHardware
{
    public class BluetoothHelper
    {
#if NET461_OR_GREATER

        /// <summary>
        /// 是否接收数据
        /// </summary>
        public bool IsReceiving { get; set; } = false;
        /// <summary>
        /// 搜索蓝牙
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, BluetoothAddress> GetClientBluetooth()
        {
            Dictionary<string, BluetoothAddress> dicBluetooth = new Dictionary<string, BluetoothAddress>();
            BluetoothClient client = new BluetoothClient();
            List<BluetoothDeviceInfo> devices = client.DiscoverDevices()?.ToList();//搜索蓝牙 10秒钟
            foreach (BluetoothDeviceInfo d in devices)
            {
                dicBluetooth[d.DeviceName] = d.DeviceAddress;
            }
            return dicBluetooth;
        }
        /// <summary>
        /// 设备列表
        /// </summary>
        public Dictionary<string, BluetoothAddress> ClientBluetooth { get { return GetClientBluetooth(); } }
        /// <summary>
        /// 连接蓝牙
        /// </summary>
        /// <param name="address">蓝牙地址</param>
        /// <returns></returns>
        public bool ConnBluetooth(BluetoothClient blueclient, BluetoothAddress address)
        {
            try
            {
                if (blueclient.Connected)
                {
                    //已连蓝牙
                    return true;
                }
                else
                {
                    blueclient.Connect(address, BluetoothService.SerialPort);// BluetoothService.Handsfree
                    //蓝牙配对   蓝牙4.0以上不用PIN
                    return true;
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                //配对失败
                return false;
            }
        }
        /// <summary>
        /// 断开连接蓝牙
        /// </summary>
        /// <param name="address">蓝牙地址</param>
        /// <returns></returns>
        public bool DisConnBluetooth(BluetoothClient blueclient)
        {
            try
            {
                if (blueclient.Connected)
                {
                    blueclient.Close();
                    //已连蓝牙
                    return true;
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="blueclient"></param>
        /// <param name="data"></param>
        public void SendData(BluetoothClient blueclient, byte[] data)
        {
            blueclient.GetStream().Write(data, 0, data.Length);
        }


        /// <summary>
        /// 接收数据----注意 接收数据  一般要校验
        /// </summary>
        /// <param name="blueclient"></param>
        /// <param name="outTime">超时时间 单位毫秒</param>
        /// <param name="sleep">蓝牙传输速度限制 mm</param>
        public async Task<List<byte>> ReceiveData(BluetoothClient blueclient, int outTime, int sleep = 200)
        {
            DateTime dt1 = DateTime.Now;
            IsReceiving = true;
            List<byte> temp = new List<byte>();
            while (IsReceiving)
            {
                Stream peerStream = blueclient.GetStream();

                byte[] recvBytes = new byte[1024];
                int bytes = 0;

                if (peerStream.CanRead)
                {
                    bytes = peerStream.Read(recvBytes, 0, recvBytes.Length);
                    if (bytes > 0)
                    {
                        if (bytes <= 1024)
                        {
                            temp.AddRange(recvBytes.Take(bytes));
                            break;
                        }
                        else
                        {
                            temp.AddRange(recvBytes.Take(bytes));
                        }
                    }
                }

                DateTime dt2 = DateTime.Now;
                TimeSpan ts = dt2 - dt1;
                if (ts.TotalMilliseconds > outTime) break;
                //蓝牙传输速度限制
                await Task.Delay(sleep).ConfigureAwait(false);
            }

            return temp;
        }
        /// <summary>
        /// 停止接受数据
        /// </summary>
        public void ReceiveStop()
        {
            IsReceiving = false;
        }
#endif
    }
}
