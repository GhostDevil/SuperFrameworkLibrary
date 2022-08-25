using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;

namespace SuperFramework
{
    /// <summary>
    /// 动态编译类
    /// </summary>
    public static class CodeDriver
    {
        private static readonly string prefix = "using System;" +
                            "public static class Driver" +
                            "{" +
                            "public static void Run()" +
                            "{";

        private static readonly string postfix = "}" + "}";
        /// <summary>
        /// 编译并运行指定代码
        /// </summary>
        /// <param name="input">待编译的代码</param>
        /// <param name="hasError">是否有错误</param>
        /// <returns>如果无错误则返回正确结果，如果有错误则返回错误</returns>
        public static string CompileAndRun(string input, out bool hasError)
        {
            hasError = false;
            CompilerResults results = null;
            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                CompilerParameters options = new CompilerParameters() { GenerateInMemory = true };
                StringBuilder sb = new StringBuilder();
                sb.Append(prefix);
                sb.Append(input);
                sb.Append(postfix);
                results = provider.CompileAssemblyFromSource(options, sb.ToString());
            }
            string returnData;
            if (results.Errors.HasErrors)
            {
                hasError = true;
                StringBuilder errorMessage = new StringBuilder();
                foreach (CompilerError error in results.Errors)
                {
                    errorMessage.AppendFormat("{0} {1}/n", error.Line, error.ErrorText);
                }
                returnData = errorMessage.ToString();
            }
            else
            {
                TextWriter temp = Console.Out;
                StringWriter writer = new StringWriter();
                Console.SetOut(writer);
                Type driverType = results.CompiledAssembly.GetType("Driver");
                driverType.InvokeMember("Run", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, null, null, null);
                Console.SetOut(temp);
                returnData = writer.ToString();
            }
            return returnData;
        }
    }
}
