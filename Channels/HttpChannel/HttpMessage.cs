using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SuperFramework.Channels.Channel
{
    public class HttpMessage : RequestBase
    {
        //public Dictionary<string, string> Headers = new Dictionary<string, string>();

        protected override void Init(string host)
        {
            if (Uri.IsWellFormedUriString(host, UriKind.Absolute))
                Url = new Uri(host);
        }
        protected T RequestExecute<T>(Dictionary<string, object> valuePairs, Dictionary<string, string> headers)
        {
            object data = valuePairs;
            return RequestExecute<T>(data, headers);

        }
        protected override T RequestExecute<T>(object[] @params, Dictionary<string, string> headers)
        {


            StackTrace stack = new();
            var method = stack.GetFrame(1).GetMethod();//想要获取关于方法的信息，可以自己断点调试这里

            object data = null;
            if (@params != null && @params.Length > 0)
            {
                if (@params.Length == 1)
                {
                    data = @params[0];
                }
                else
                {
                    Dictionary<string, object> keys = new();
                    var ps = method.GetParameters();

                    if (ps.Length != @params.Length)
                    {
                        throw new Exception($"方法{method.Name}的参数数量不匹配");
                    }
                    else
                    {
                        for (int i = 0; i < ps.Length; i++)
                        {
                            keys[ps[i].Name] = @params[i];
                        }
                    }
                    data = keys;
                }
            }
            return RequestExecute<T>(data, headers);
        }
        T RequestExecute<T>(object data, Dictionary<string, string> headers)
        {


            StackTrace stack = new();
            var method = stack.GetFrame(2).GetMethod();//想要获取关于方法的信息，可以自己断点调试这里
            var route = method.GetCustomAttribute<RouteAttribute>(true)?.Value ?? "";
            HttpMethod httpMethod = method.GetCustomAttribute<HttpMethodAttribute>(true)?.Method ?? HttpMethod.POST;

            var contentType = method.GetCustomAttribute<ContentTypeAttribute>(true)?.Value ?? ContentType.Json;

            var webRequest = (HttpWebRequest)WebRequest.Create(new Uri(Url, route));
            webRequest.Method = httpMethod.ToString();

            foreach (var kv in headers)
            { 
                webRequest.Headers.Add(kv.Key, kv.Value);
            }

            if (httpMethod != HttpMethod.GET)
            {
                using (var stream = webRequest.GetRequestStream())
                {
                    switch (contentType)
                    {
                        case ContentType.FormData:
                            using (var postStream = new MemoryStream())
                            {
                                string boundary = "----" + DateTime.Now.Ticks.ToString("x");//分隔符
                                webRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
                                List<FormMessage> messages = data as List<FormMessage>;

                                if (messages != null && messages.Count > 0)
                                {
                                    //文件数据模板
                                    string fileFormdataTemplate =
                                        "\r\n--" + boundary +
                                        "\r\nContent-Disp、osition: form-data; name=\"{0}\"; filename=\"{1}\"" +
                                        "\r\nContent-Type: application/octet-stream" +
                                        "\r\n\r\n";
                                    //文本数据模板
                                    string dataFormdataTemplate =
                                        "\r\n--" + boundary +
                                        "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                        "\r\n\r\n{1}";
                                    foreach (var item in messages)
                                    {
                                        string formdata = null;
                                        if (item.IsFile)
                                        {
                                            //上传文件
                                            formdata = string.Format(
                                                fileFormdataTemplate,
                                                item.Key, //表单键
                                                item.Value);
                                        }
                                        else
                                        {
                                            //上传文本
                                            formdata = string.Format(
                                                dataFormdataTemplate,
                                                item.Key,
                                                item.Value);
                                        }

                                        //统一处理
                                        byte[] formdataBytes = null;
                                        //第一行不需要换行
                                        if (postStream.Length == 0)
                                            formdataBytes = Encoding.UTF8.GetBytes(formdata.Substring(2, formdata.Length - 2));
                                        else
                                            formdataBytes = Encoding.UTF8.GetBytes(formdata);
                                        postStream.Write(formdataBytes, 0, formdataBytes.Length);

                                        //写入文件内容
                                        if (item.FileContent != null && item.FileContent.Length > 0)
                                        {
                                            using (var stream1 = item.FileContent)
                                            {
                                                byte[] buffer1 = new byte[1024];
                                                int bytesRead1 = 0;
                                                while ((bytesRead1 = stream1.Read(buffer1, 0, buffer1.Length)) != 0)
                                                {
                                                    postStream.Write(buffer1, 0, bytesRead1);
                                                }
                                            }
                                        }
                                    }
                                    //结尾
                                    var footer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
                                    postStream.Write(footer, 0, footer.Length);
                                }
                                else
                                {
                                    webRequest.ContentType = "application/x-www-form-urlencoded";
                                }

                                postStream.Position = 0;
                                byte[] buffer = new byte[1024];
                                int bytesRead = 0;
                                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                                {
                                    stream.Write(buffer, 0, bytesRead);
                                }

                                postStream.Close();
                            }
                            break;
                        case ContentType.Json:
                            webRequest.ContentType = "application/json; charset=utf-8";

                            if (data != null)
                            {
                                string json = JsonConvert.SerializeObject(data, Formatting.None);//, new JsonSerializerSettings { ContractResolver = ShouldSerializeContractResolver.Instance }
                                stream.Write(Encoding.UTF8.GetBytes(json));
                            }
                            break;
                        default:
                            throw new Exception("不合法的请求");
                    }
                }
            }
            T resultT = default;
            var response = webRequest.GetResponseAsync().GetAwaiter();

            while (!response.IsCompleted)
            {
                Task.Delay(50).Wait();
            }
            using (var stream = new StreamReader(response.GetResult().GetResponseStream()))
            {
                string result = stream.ReadToEnd();
                resultT = (T)JsonConvert.DeserializeObject(result, typeof(T));
            }

            return resultT;
        }
    }


}
