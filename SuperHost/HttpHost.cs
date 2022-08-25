using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text.Json;
namespace SuperFramework.SuperHost
{
    public static class ContextType
    {
        /// <summary>
        /// 资源类型：普通文本
        /// </summary>
        public const string TEXT_PLAIN = "text/plain";

        /// <summary>
        /// 资源类型：JSON字符串
        /// </summary>
        public const string APPLICATION_JSON = "application/json";

        /// <summary>
        /// 资源类型：未知类型(数据流)
        /// </summary>
        public const string APPLICATION_OCTET_STREAM = "application/octet-stream";

        /// <summary>
        /// 资源类型：表单数据(键值对)
        /// </summary>
        public const string WWW_FORM_URLENCODED = "application/x-www-form-urlencoded";

        /// <summary>
        /// 资源类型：表单数据(键值对)。编码方式为 gb2312
        /// </summary>
        public const string WWW_FORM_URLENCODED_GB2312 = "application/x-www-form-urlencoded;charset=gb2312";

        /// <summary>
        /// 资源类型：表单数据(键值对)。编码方式为 utf-8
        /// </summary>
        public const string WWW_FORM_URLENCODED_UTF8 = "application/x-www-form-urlencoded;charset=utf-8";

        /// <summary>
        /// 资源类型：多分部数据
        /// </summary>
        public const string MULTIPART_FORM_DATA = "multipart/form-data";
    }
    public abstract class Controller
    {
    }
    public delegate void ControllerCreatedHandler(Controller controller);

    public class HttpHost : SafeObject
    {
        private HttpListener _listener = new HttpListener();
        private Dictionary<string, Type> _controllers = new Dictionary<string, Type>();

        public HttpHost()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsInterface && !type.IsAbstract && type.IsSubclassOf(typeof(Controller)))
                    {
                        var controller = Regex.Replace(type.Name, "Controller$", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
                        _controllers.Add(controller, type);
                    }
                }
            }
        }

        public event ControllerCreatedHandler OnCreateController;

        public bool Start(int port)
        {
            try
            {
                _listener.Prefixes.Clear();
                _listener.Prefixes.Add($"http://+:{port}/");
                _listener.Start();
                _listener.BeginGetContext(OnAcceptRequest, null);
                return true;
            }
            catch (Exception e)
            { }
            return false;
        }

        public void Stop()
        {
            _listener.Close();
        }

        private void OnAcceptRequest(IAsyncResult ar)
        {
            if (_listener.IsListening)
            {
                _listener.BeginGetContext(OnAcceptRequest, null);
                var context = _listener.EndGetContext(ar);
                using (var writer = new StreamWriter(context.Response.OutputStream))
                {
                    context.Response.ContentType = "application/json; charset=UTF-8";
                    try
                    {
                        var match = Regex.Match(context.Request.Url.AbsolutePath, "/([^/]+)/([^\\?]+)", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
                        if (match.Success && match.Groups.Count == 3)
                        {
                            var key = _controllers.Keys.FirstOrDefault(q => q.Equals(match.Groups[1].Value, StringComparison.CurrentCultureIgnoreCase));
                            MethodInfo method = null;
                            if (!string.IsNullOrWhiteSpace(key) &&
                                (method = _controllers[key].GetMethods(BindingFlags.Public | BindingFlags.Instance).
                                FirstOrDefault(q => q.Name.Equals(match.Groups[2].Value, StringComparison.CurrentCultureIgnoreCase))) != null)
                            {
                                var arguments = method.GetParameters();
                                var args = new object[arguments.Length];

                                // 解析Query参数
                                var query = context.Request.Url.Query.TrimStart(new char[] { '?' }).ToDictionary();
                                for (int i = 0; i < arguments.Length; i++)
                                {
                                    var qKey = query.Keys.FirstOrDefault(q => q.Equals(arguments[i].Name, StringComparison.CurrentCultureIgnoreCase));
                                    if (!string.IsNullOrWhiteSpace(qKey))
                                    {
                                        var value = query[qKey];
                                        if (!string.IsNullOrEmpty(value))
                                            args[i] = Convert.ChangeType(value, arguments[i].ParameterType);
                                    }
                                }

                                var text = string.Empty;
                                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                                {
                                    text = reader.ReadToEnd();
                                }

                                if (context.Request.ContentType == ContextType.APPLICATION_JSON) // json数据体
                                {
                                    Dictionary<string, object> dic = JsonSerializer.Deserialize<Dictionary<string, object>>(text);
                                    for (int i = 0; i < arguments.Length; i++)
                                    {
                                        var qKey = dic.Keys.FirstOrDefault(q => q.Equals(arguments[i].Name, StringComparison.CurrentCultureIgnoreCase));
                                        if (!string.IsNullOrWhiteSpace(qKey))
                                        {
                                            var value = dic[qKey].ToString();
                                            if (value != null)
                                                args[i] = Convert.ChangeType(value, arguments[i].ParameterType);
                                        }
                                    }
                                }
                                else if (args.Length > 0)
                                {
                                    args[args.Length - 1] = text;
                                }

                                // 解析数据体
                                //if (!context.Request.HttpMethod.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
                                //{

                                //    var dic = text.ToQuertString();
                                //    for (int i = 0; i < arguments.Length; i++)
                                //    {
                                //        var qKey = dic.Keys.FirstOrDefault(q => q.Equals(arguments[i].Name, StringComparison.CurrentCultureIgnoreCase));
                                //        if (!string.IsNullOrWhiteSpace(qKey))
                                //        {
                                //            var value = dic[qKey];
                                //            if (!string.IsNullOrEmpty(value))
                                //                args[i] = Convert.ChangeType(value, arguments[i].ParameterType);
                                //        }
                                //    }
                                //}
                                var controller = (Controller)Activator.CreateInstance(_controllers[key]);
                                OnCreateController?.Invoke(controller);
                                var result = method.Invoke(controller, args);
                                if (result != null)
                                {
                                    writer.Write(JsonSerializer.Serialize(result));
                                }
                            }
                            else
                            {
                                context.Response.StatusCode = 404;
                                writer.Write(JsonSerializer.Serialize("Service NotFound!"));
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                            writer.Write(JsonSerializer.Serialize("Service NotFound!"));
                        }
                    }
                    catch (Exception e)
                    {
                        context.Response.StatusCode = 500;
                        writer.Write(JsonSerializer.Serialize($"Service ERROR! ({e.Message})"));
                    }
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
                Stop();
        }
    }
}