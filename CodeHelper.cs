using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SuperFramework
{
    /// <summary>
    /// 代码信息
    /// </summary>
    public static class CodeHelper
    {
        /// <summary>
        /// 取得当前源码的哪一行
        /// </summary>        
        /// <param name="skipFrame">从调用方的框架跳过指定的帧数，方法自身为0</param>
        /// <param name="frameIndex">需要读取的堆栈帧索引</param>
        /// <returns>返回执行代码行号</returns>
        public static string GetLineNum(int skipFrame=1,int frameIndex=0)
        {
            StackTrace st = new StackTrace(skipFrame, true);
            return st.GetFrame(frameIndex).GetFileLineNumber().ToString();
        }
        /// <summary>
        /// 取得当前源码的哪一列
        /// </summary>
        /// <param name="skipFrame">从调用方的框架跳过指定的帧数，方法自身为0</param>
        /// <param name="frameIndex">需要读取的堆栈帧索引</param>
        /// <returns>返回执行代码行号</returns>
        public static string GetColumnNumber(int skipFrame = 1, int frameIndex = 0)
        {
            StackTrace st = new StackTrace(skipFrame, true);
            return st.GetFrame(frameIndex).GetFileColumnNumber().ToString();
        }
        /// <summary>
        /// 取当前源码的源文件名
        /// </summary>
        /// <returns>返回文件路径以及名称</returns>
        public static string GetCurSourceFileName()
        {
            StackTrace st = new StackTrace(1, true);
            return st.GetFrame(0).GetFileName();
        }
        /// <summary>
        /// 取当前函数名
        /// </summary>
        /// <returns>返回当前函数名</returns>
        public static string GetMethodName()
        {

            //StackTrace ss = new StackTrace(true);
            //Type t = ss.GetFrame(1).GetMethod().DeclaringType;
            //return t.FullName;
            var method = new StackFrame(1).GetMethod(); // 这里忽略1层堆栈，也就忽略了当前方法GetMethodName，这样拿到的就正好是外部调用GetMethodName的方法信息
            var property = (
                      from p in method.DeclaringType.GetProperties(
                               BindingFlags.Instance |
                               BindingFlags.Static |
                               BindingFlags.Public |
                               BindingFlags.NonPublic)
                      where p.GetGetMethod(true) == method || p.GetSetMethod(true) == method
                      select p).FirstOrDefault();
            return property == null ? method.DeclaringType.FullName : property.Name;
        }
        /// <summary>
        /// 获得方法名
        /// </summary>
        /// <param name="depth">
        /// 表示调用此方法的回溯深度
        /// <para>比如，A方法调用B方法，B方法调用GetCurrentMethodFullName(2)，那么得到的结果是A方法的全名（namespace+class名+method名），若要获得当前方法，depth应为0</para>
        /// </param>
        /// <returns>返回namespace+class名+method名</returns>
        public static string GetCurrentMethodFullName(int depth=1)
        {
            try
            {
                StackTrace st = new StackTrace();
                string methodName = st.GetFrame(depth).GetMethod().ToString();
                string className = st.GetFrame(depth).GetMethod().DeclaringType.ToString();
                return className + "：" + methodName;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 取当前类名
        /// </summary>
        /// <returns>返回当前类名</returns>
        public static string GetClassName()
        {
            StackTrace trace = new StackTrace();
            StackFrame frame = trace.GetFrame(1);//1代表上级，2代表上上级，以此类推  
            MethodBase method = frame.GetMethod();
            return method.DeclaringType.ToString();
        }
    }
}
