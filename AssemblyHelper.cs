using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SuperFramework
{
    /// <summary>
    /// 类名:AssemblyHelper
    /// 版本:Release
    /// 日期:2015-06-24
    /// 作者:不良帥
    /// 描述:程序集属性访问器
    /// </summary>
    public class AssemblyHelper
    {
        #region  获取此程序集的说明自定义属性 
        /// <summary>
        /// 获取此程序集的说明自定义属性
        /// </summary>
        public static string AssemblyTitle
        {
            get
            {
                // 获取此程序集上的所有 Title 属性
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                // 如果至少有一个 Title 属性
                if (attributes.Length > 0)
                {
                    // 请选择第一个属性
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    // 如果该属性为非空字符串，则将其返回
                    if (titleAttribute.Title != "")
                        return titleAttribute.Title;
                }
                // 如果没有 Title 属性，或者 Title 属性为一个空字符串，则返回 .exe 的名称
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            }
        }
        #endregion

        #region  获取此程序集的版本号 
        /// <summary>
        /// 获取此程序集的版本号
        /// </summary>
        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        #endregion

        #region  获取此程序集的所有文本书说明属性 
        /// <summary>
        /// 获取此程序集的所有文本说明属性
        /// </summary>
        public static string AssemblyDescription
        {
            get
            {
                // 获取此程序集的所有 Description 属性
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                // 如果 Description 属性不存在，则返回一个空字符串
                if (attributes.Length == 0)
                    return "";
                // 如果有 Description 属性，则返回该属性的值
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }
        #endregion

        #region  获取此程序集上的产品名称自定义属性 
        /// <summary>
        /// 获取此程序集上的所有产品名称自定义属性
        /// </summary>
        public static string AssemblyProduct
        {
            get
            {
                // 获取此程序集上的所有 Product 属性
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                // 如果 Product 属性不存在，则返回一个空字符串
                if (attributes.Length == 0)
                    return "";
                // 如果有 Product 属性，则返回该属性的值
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
        #endregion

        #region 获取此程序集上的版权自定义属性 
        /// <summary>
        /// 获取此程序集上的版权自定义属性
        /// </summary>
        public string AssemblyCopyright
        {
            get
            {
                // 获取此程序集上的所有 Copyright 属性
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                // 如果 Copyright 属性不存在，则返回一个空字符串
                if (attributes.Length == 0)
                    return "";
                // 如果有 Copyright 属性，则返回该属性的值
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        #endregion

        #region  获取此程序集上的公司名称自定义属性 
        /// <summary>
        /// 获取此程序集上的所有公司名称自定义属性
        /// </summary>
        public static string AssemblyCompany
        {
            get
            {
                // 获取此程序集上的所有 Company 属性
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                // 如果 Company 属性不存在，则返回一个空字符串
                if (attributes.Length == 0)
                    return "";
                // 如果有 Company 属性，则返回该属性的值
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        //readonly string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"/MyDLL/";

        /// <summary>  
        /// 获取程序集名称列表  
        /// </summary>  
        /// <param name="workPath">工作目录</param>
        public AssemblyResult GetAssemblyName(string workPath)
        {
            AssemblyResult result = new();
            string[] dicFileName = Directory.GetFileSystemEntries(workPath);
            if (dicFileName != null)
            {
                List<string> assemblyList = new();
                foreach (string name in dicFileName)
                {
                    assemblyList.Add(name.Substring(name.LastIndexOf('/') + 1));
                }
                result.AssemblyName = assemblyList;
            }
            return result;
        }

        /// <summary>  
        /// 获取程序集中的类名称  
        /// </summary>  
        /// <param name="assemblyName">程序集</param>  
        /// <param name="workPath">工作目录</param>
        public AssemblyResult GetClassName(string assemblyName, string workPath)
        {
            AssemblyResult result = new();
            if (!string.IsNullOrEmpty(assemblyName))
            {
                assemblyName =Path.Combine(workPath, assemblyName);
                Assembly assembly = Assembly.LoadFrom(assemblyName);
                Type[] ts = assembly.GetTypes();
                List<string> classList = new();
                foreach (Type t in ts)
                {
                    //classList.Add(t.Name);  
                    classList.Add(t.FullName);
                }
                result.ClassName = classList;
            }
            return result;
        }

        /// <summary>  
        /// 获取类的属性、方法  
        /// </summary>  
        /// <param name="assemblyName">程序集</param>  
        /// <param name="className">类名</param>  
        /// <param name="workPath">工作目录</param>
        public AssemblyResult GetClassInfo(string assemblyName, string className, string workPath)
        {
            AssemblyResult result = new();
            if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(className))
            {
                assemblyName = Path.Combine(workPath, assemblyName);
                Assembly assembly = Assembly.LoadFrom(assemblyName);
                Type type = assembly.GetType(className, true, true);
                if (type != null)
                {
                    //类的属性  
                    List<string> propertieList = new();
                    PropertyInfo[] propertyinfo = type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (PropertyInfo p in propertyinfo)
                    {
                        propertieList.Add(p.ToString());
                    }
                    result.Properties = propertieList;

                    //类的方法  
                    List<string> methods = new();
                    MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    foreach (MethodInfo mi in methodInfos)
                    {
                        methods.Add(mi.Name);
                        //方法的参数  
                        //foreach (ParameterInfo p in mi.GetParameters())  
                        //{  

                        //}  
                        //方法的返回值  
                        //string returnParameter = mi.ReturnParameter.ToString();  
                    }
                    result.Methods = methods;
                }
            }
            return result;
        }
        /// <summary>  
        /// 反射结果类  
        /// </summary>  
        public class AssemblyResult
        {
            /// <summary>  
            /// 程序集名称  
            /// </summary>  
            public List<string> AssemblyName { get; set; }

            /// <summary>  
            /// 类名  
            /// </summary>  
            public List<string> ClassName { get; set; }

            /// <summary>  
            /// 类的属性  
            /// </summary>  
            public List<string> Properties { get; set; }

            /// <summary>  
            /// 类的方法  
            /// </summary>  
            public List<string> Methods { get; set; }
        }
    }
}
