﻿/********************************************************************
 * *
 * * 作者： 不良帥
 * *
 * * 创建时间：2014-08-13
 * *
 * * 说明：注册类
 * *
 * * 使用该类生成机器码（一台电脑机器码固定），并生成软件注册码（可扩张一机一软件）
 * *
********************************************************************/
using System;
using System.Management;

namespace SuperFramework.SuperHardware
{
    /// <summary>
    /// C#根据CPU+磁盘标号来注册软件
    /// </summary>
    internal static class SoftRegByHardware
    {
        #region 变量
        public static int[] IntCode = new int[127]; //存储密钥
        public static char[] CharCode = new char[25]; //存储ASCII码
        public static int[] IntNumber = new int[25]; //存储ASCII码值
        #endregion

        #region 方法
        /// <summary>
        /// 初始化存储密钥
        /// </summary>
        public static void SetIntCode()
        {
            for (int i = 1; i < IntCode.Length; i++)
            {
                IntCode[i] = i % 9;
            }
        }

        ///<summary>
        /// 获取硬盘卷标号
        ///</summary>
        public static string GetDiskVolumeSerialNumber()
        {
            //ManagementClass mc = new ManagementClass("win32_NetworkAdapterConfiguration");
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            disk.Get();
            return disk.GetPropertyValue("VolumeSerialNumber").ToString();
        }

        ///<summary>
        /// 获取CPU序列号
        ///</summary>
        public static string GetCpu()
        {
            string strCpu = null;
            ManagementClass myCpu = new ManagementClass("win32_Processor");
            ManagementObjectCollection myCpuCollection = myCpu.GetInstances();
            foreach (ManagementObject myObject in myCpuCollection)
            {
                strCpu = myObject.Properties["Processorid"].Value.ToString();
            }
            return strCpu;
        }

        ///<summary>
        /// 生成机器码(机器码由CPU序列号+硬盘卷标号合成)----可扩展
        ///</summary>
        public static string GetMNum()
        {
            string strNum = GetCpu() + GetDiskVolumeSerialNumber();
            string strMNum = strNum.Substring(0, 24); //截取前24位作为机器码
            return strMNum;
        }

        ///<summary>
        /// 生成注册码（根据本机机器码生成注册码）
        ///</summary>
        public static string GetRNum()
        {
            SetIntCode();
            string strMNum = GetMNum();
            for (int i = 1; i < CharCode.Length; i++) //存储机器码
            {
                CharCode[i] = Convert.ToChar(strMNum.Substring(i - 1, 1));
            }
            for (int j = 1; j < IntNumber.Length; j++) //改变ASCII码值
            {
                IntNumber[j] = Convert.ToInt32(CharCode[j]) + IntCode[Convert.ToInt32(CharCode[j])];
            }
            string strAsciiName = ""; //注册码
            for (int k = 1; k < IntNumber.Length; k++) //生成注册码
            {

                if ((IntNumber[k] >= 48 && IntNumber[k] <= 57) || (IntNumber[k] >= 65 && IntNumber[k]
                                                                   <= 90) || (IntNumber[k] >= 97 && IntNumber[k] <= 122))
                //判断如果在0-9、A-Z、a-z之间
                {
                    strAsciiName += Convert.ToChar(IntNumber[k]).ToString();
                }
                else if (IntNumber[k] > 122) //判断如果大于z
                {
                    strAsciiName += Convert.ToChar(IntNumber[k] - 10).ToString();
                }
                else
                {
                    strAsciiName += Convert.ToChar(IntNumber[k] - 9).ToString();
                }
            }
            return strAsciiName;
        }

        ///<summary>
        /// 生成注册码（根据传入的机器码生成注册码）
        ///</summary>
        ///<returns>机器码</returns>
        public static string GetRNum(string machineStr)
        {
            SetIntCode();
            string strMNum = machineStr;
            for (int i = 1; i < CharCode.Length; i++) //存储机器码
            {
                CharCode[i] = Convert.ToChar(strMNum.Substring(i - 1, 1));
            }
            for (int j = 1; j < IntNumber.Length; j++) //改变ASCII码值
            {
                IntNumber[j] = Convert.ToInt32(CharCode[j]) + IntCode[Convert.ToInt32(CharCode[j])];
            }
            string strAsciiName = ""; //注册码
            for (int k = 1; k < IntNumber.Length; k++) //生成注册码
            {

                if ((IntNumber[k] >= 48 && IntNumber[k] <= 57) || (IntNumber[k] >= 65 && IntNumber[k]
                                                                   <= 90) || (IntNumber[k] >= 97 && IntNumber[k] <= 122))
                //判断如果在0-9、A-Z、a-z之间
                {
                    strAsciiName += Convert.ToChar(IntNumber[k]).ToString();
                }
                else if (IntNumber[k] > 122) //判断如果大于z
                {
                    strAsciiName += Convert.ToChar(IntNumber[k] - 10).ToString();
                }
                else
                {
                    strAsciiName += Convert.ToChar(IntNumber[k] - 9).ToString();
                }
            }
            return strAsciiName;
        }
        #endregion
    }
}