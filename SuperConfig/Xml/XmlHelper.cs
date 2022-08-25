using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;

namespace SuperFramework.SuperConfig.Xml
{
    /// <summary>
    /// 日 期:2022-08-22
    /// 作 者:不良帥
    /// 描 述:xml操作辅助类
    /// </summary>
    public static class XmlHelper
    {
        /// <summary>
        /// 格式化所提供的XML，使其缩进且易于阅读
        /// </summary>
        /// <param name="inputXml">要格式化的输入XML</param>
        /// <returns>标准格式XML字符串</returns>
        public static string FormatXml(string inputXml)
        {
            XmlDocument document = new XmlDocument();
            document.Load(new StringReader(inputXml));
            StringBuilder builder = new StringBuilder();
            using (XmlTextWriter writer = new XmlTextWriter(new StringWriter(builder)))
            {
                writer.Formatting = Formatting.Indented;
                document.Save(writer);
            }
            return builder.ToString();
        }


        #region  xml参数类型 
        /// <summary>
        /// xml参数类型
        /// </summary>
        public enum XmlType
        {
            /// <summary>
            /// xml文件
            /// </summary>
            File,
            /// <summary>
            /// xml字符串
            /// </summary>
            String
        }
        #endregion

        #region  读取xml文件为字符串 
        /// <summary>
        /// 读取xml文件为字符串
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>返回xml字符串</returns>
        public static string LoadXmlStr(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                StreamReader sr = new StreamReader(fs);
                return sr.ReadToEnd();
            }
        }
        #endregion

        #region  保存xml字符串为文件 
        /// <summary>
        /// 保存xml字符串为文件
        /// </summary>
        /// <param name="path">保存路径</param>
        /// <param name="xmlString">xml字符串</param>
        /// <returns>返回bool</returns>
        public static bool SaveXmlStr(string path, string xmlString)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    //开始写入
                    sw.Write(xmlString);
                    //清空缓冲区
                    sw.Flush();
                    //关闭流
                    //sw.Close();
                }
                return true;
            }
            catch { return false; }
        }
        #endregion

        #region  创建一个XML文档 
        /// <summary>
        /// 创建一个XML文档
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="rootNodeName">XML文档根节点名称(须指定一个根节点名称)</param>
        /// <param name="version">XML文档版本号(必须为:"1.0")</param>
        /// <param name="encoding">XML文档编码方式</param>
        /// <param name="standalone">该值必须是"yes"或"no",如果为null,Save方法不在XML声明上写出独立属性</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool CreateXmlDocument(string xmlFileName, string rootNodeName, Encoding encoding, string standalone = "yes", string version = "1.0")
        {
            bool isSuccess;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration(version, encoding.HeaderName, standalone);
                XmlNode root = xmlDoc.CreateElement(rootNodeName);
                xmlDoc.AppendChild(xmlDeclaration);
                xmlDoc.AppendChild(root);
                xmlDoc.Save(xmlFileName);
                isSuccess = true;
            }
            catch
            {
                isSuccess = false; //这里可以定义你自己的异常处理
            }
            return isSuccess;
        }
        #endregion

        #region 创建或更新
        /// <summary>
        /// 插入xml数据
        /// * 调用示例:
        /// * XmlMethod.InsertNode(path, "/Node", "Element", "", "Value")
        /// * XmlMethod.InsertNode(path, "/Node", "Element", "Attribute", "Value")
        /// * XmlMethod.InsertNode(path, "/Node", "", "Attribute", "Value")
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="element">元素名，非空时插入新元素，否则在该元素中插入属性</param>
        /// <param name="attribute">属性名，非空时插入该元素属性值，否则插入元素值</param>
        /// <param name="value">值</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool InsertNode(string path, string node, string element, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        XmlElement xe = (XmlElement)xn;
                        xe.SetAttribute(attribute, value);
                    }
                }
                else
                {
                    XmlElement xe = doc.CreateElement(element);
                    if (attribute.Equals(""))
                        xe.InnerText = value;
                    else
                        xe.SetAttribute(attribute, value);
                    xn.AppendChild(xe);
                }
                doc.Save(path);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 修改xml数据
        /// * 调用示例:
        /// * XmlMethod.Update(path, "/Node", "", "Value")
        /// * XmlMethod.Update(path, "/Node", "Attribute", "Value")
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时修改该节点属性值，否则修改节点值</param>
        /// <param name="value">值</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool Update(string path, string node, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xe.InnerText = value;
                else
                    xe.SetAttribute(attribute, value);
                doc.Save(path);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 更新XML文件中的指定节点内容
        /// * 调用示例:
        /// * XmlMethod.UpdateNode(filePath,"Node","nodeValue")
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="nodeValue">更新内容</param>
        /// <returns>更新成功返回true，失败返回false</returns>
        public static bool UpdateNode(string filePath, string nodeName, string nodeValue)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(filePath);
            XmlElement xe = xd.DocumentElement;
            XmlNode xn = xe.SelectSingleNode("//" + nodeName);
            bool flag;
            if (xn != null)
            {
                xn.InnerText = nodeValue;
                flag = true;
            }
            else
            {
                flag = false;
            }
            return flag;
        }
        /// <summary>
        /// 依据匹配XPath表达式的第一个节点来创建它的子节点(如果此节点已存在则追加一个新的同名节点)
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        /// <param name="xmlNodeName">要匹配xmlNodeName的节点名称</param>
        /// <param name="innerText">节点文本值</param>
        /// <param name="xmlAttributeName">要匹配xmlAttributeName的属性名称</param>
        /// <param name="value">属性值</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool CreateXmlNodeByXPath(string xmlFileName, string xpath, string xmlNodeName, string innerText, string xmlAttributeName, string value)
        {
            XmlDocument xmlDoc = new XmlDocument();
            bool isSuccess;
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                if (xmlNode != null)
                {
                    //存不存在此节点都创建
                    XmlElement subElement = xmlDoc.CreateElement(xmlNodeName);
                    subElement.InnerXml = innerText;

                    //如果属性和值参数都不为空则在此新节点上新增属性
                    if (!string.IsNullOrEmpty(xmlAttributeName) && !string.IsNullOrEmpty(value))
                    {
                        XmlAttribute xmlAttribute = xmlDoc.CreateAttribute(xmlAttributeName);
                        xmlAttribute.Value = value;
                        subElement.Attributes.Append(xmlAttribute);
                    }

                    xmlNode.AppendChild(subElement);
                }
                xmlDoc.Save(xmlFileName); //保存到XML文档
                isSuccess = true;
            }
            catch
            {
                isSuccess = false; //这里可以定义你自己的异常处理
            }
            return isSuccess;
        }

        /// <summary>
        /// 依据匹配XPath表达式的第一个节点来创建或更新它的子节点(如果节点存在则更新,不存在则创建)
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        /// <param name="xmlNodeName">要匹配xmlNodeName的节点名称</param>
        /// <param name="innerText">节点文本值</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool CreateOrUpdateXmlNodeByXPath(string xmlFileName, string xpath, string xmlNodeName, string innerText)
        {
            bool isExistsNode = false;//标识节点是否存在
            XmlDocument xmlDoc = new XmlDocument();
            bool isSuccess;
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                if (xmlNode != null)
                {
                    //遍历xpath节点下的所有子节点
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        if (node.Name.ToLower() == xmlNodeName.ToLower())
                        {
                            //存在此节点则更新
                            node.InnerXml = innerText;
                            isExistsNode = true;
                            break;
                        }
                    }
                    if (!isExistsNode)
                    {
                        //不存在此节点则创建
                        XmlElement subElement = xmlDoc.CreateElement(xmlNodeName);
                        subElement.InnerXml = innerText;
                        xmlNode.AppendChild(subElement);
                    }
                }
                xmlDoc.Save(xmlFileName); //保存到XML文档
                isSuccess = true;
            }
            catch
            {
                isSuccess = false; //这里可以定义你自己的异常处理
            }

            return isSuccess;
        }

        /// <summary>
        /// 依据匹配XPath表达式的第一个节点来创建或更新它的属性(如果属性存在则更新,不存在则创建)
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        /// <param name="xmlAttributeName">要匹配xmlAttributeName的属性名称</param>
        /// <param name="value">属性值</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool CreateOrUpdateXmlAttributeByXPath(string xmlFileName, string xpath, string xmlAttributeName, string value)
        {
            bool isExistsAttribute = false;//标识属性是否存在
            XmlDocument xmlDoc = new XmlDocument();
            bool isSuccess;
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                if (xmlNode != null)
                {
                    //遍历xpath节点中的所有属性
                    foreach (XmlAttribute attribute in xmlNode.Attributes)
                    {
                        if (attribute.Name.ToLower() == xmlAttributeName.ToLower())
                        {
                            //节点中存在此属性则更新
                            attribute.Value = value;
                            isExistsAttribute = true;
                            break;
                        }
                    }
                    if (!isExistsAttribute)
                    {
                        //节点中不存在此属性则创建
                        XmlAttribute xmlAttribute = xmlDoc.CreateAttribute(xmlAttributeName);
                        xmlAttribute.Value = value;
                        xmlNode.Attributes.Append(xmlAttribute);
                    }
                }
                xmlDoc.Save(xmlFileName); //保存到XML文档
                isSuccess = true;
            }
            catch
            {
                isSuccess = false; //这里可以定义你自己的异常处理
            }
            return isSuccess;
        }

        #endregion

        #region 删除
        /// <summary>
        /// 删除xml数据
        /// * 调用示例:
        /// * XmlMethod.Delete(path, "/Node", "")
        /// * XmlMethod.Delete(path, "/Node", "Attribute")
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时删除该节点属性值，否则删除节点值</param>
        /// <param name="value">值</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool Delete(string path, string node, string attribute)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xn.ParentNode.RemoveChild(xn);
                else
                    xe.RemoveAttribute(attribute);
                doc.Save(path);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// 删除匹配XPath表达式的第一个节点(节点中的子元素同时会被删除)
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool DeleteXmlNodeByXPath(string xmlFileName, string xpath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            bool isSuccess;
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                if (xmlNode != null)
                {
                    //删除节点
                    xmlNode.ParentNode.RemoveChild(xmlNode);
                }
                xmlDoc.Save(xmlFileName); //保存到XML文档
                isSuccess = true;
            }
            catch
            {
                isSuccess = false; //这里可以定义你自己的异常处理
            }
            return isSuccess;
        }

        /// <summary>
        /// 删除匹配XPath表达式的第一个节点中的匹配参数xmlAttributeName的属性
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        /// <param name="xmlAttributeName">要删除的xmlAttributeName的属性名称</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool DeleteXmlAttributeByXPath(string xmlFileName, string xpath, string xmlAttributeName)
        {
            bool isExistsAttribute = false;
            XmlDocument xmlDoc = new XmlDocument();
            bool isSuccess;
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                XmlAttribute xmlAttribute = null;
                if (xmlNode != null)
                {
                    //遍历xpath节点中的所有属性
                    foreach (XmlAttribute attribute in xmlNode.Attributes)
                    {
                        if (attribute.Name.ToLower() == xmlAttributeName.ToLower())
                        {
                            //节点中存在此属性
                            xmlAttribute = attribute;
                            isExistsAttribute = true;
                            break;
                        }
                    }
                    if (isExistsAttribute)
                    {
                        //删除节点中的属性
                        xmlNode.Attributes.Remove(xmlAttribute);
                    }
                }
                xmlDoc.Save(xmlFileName); //保存到XML文档
                isSuccess = true;
            }
            catch
            {
                isSuccess = false; //这里可以定义你自己的异常处理
            }
            return isSuccess;
        }

        /// <summary>
        /// 删除匹配XPath表达式的第一个节点中的所有属性
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        /// <returns>成功返回true,失败返回false</returns>
        public static bool DeleteXmlAttributeAllByXPath(string xmlFileName, string xpath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            bool isSuccess;
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                if (xmlNode != null)
                {
                    //遍历xpath节点中的所有属性
                    xmlNode.Attributes.RemoveAll();
                }
                xmlDoc.Save(xmlFileName); //保存到XML文档
                isSuccess = true;
            }
            catch
            {
                throw; //这里可以定义你自己的异常处理
            }
            return isSuccess;
        }
        #endregion

        #region 读取

        /// <summary>
        /// 读取xml数据
        /// * 调用示例:
        /// * XmlMethod.Read(path, "/Node", "")
        /// * XmlMethod.Read(path, "/Node/Element[@Attribute='Name']", "Attribute")
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="node">节点</param>
        /// <param name="attribute">属性名，非空时返回该属性值，否则返回串联值</param>
        /// <returns>返回string</returns>
        public static string Read(string path, string node, string attribute)
        {
            string value = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
            }
            catch (Exception) { }
            return value;
        }

        /// <summary>
        /// 获得XML文档XmlDocument对象
        /// * .net中调用方法：写入文件中,则：
        /// * document = XmlMethod.GetXmlDocument("sex", "sexy")
        /// * document.Save("c:/bookstore.xml")  
        /// </summary>
        /// <param name="name">根节点名称</param>
        /// <param name="type">根节点的一个属性值</param>
        /// <returns>返回XmlDocument对象</returns>        
        public static XmlDocument GetXmlDocument(string name, string type)
        {
            XmlDocument doc;
            try
            {
                doc = new XmlDocument();
                doc.LoadXml("<" + name + "/>");
                XmlElement rootEle = doc.DocumentElement;
                rootEle.SetAttribute("type", type);
            }
            catch
            {
                throw;
            }
            return doc;
        }

        /// <summary>
        /// 获得xml文件中指定节点的节点数据
        /// * 调用示例:
        /// * XmlMethod.GetNodeInfoByNodeName(path, "Node")
        /// </summary>
        /// <param name="path">文件地址</param>
        /// <param name="nodeName">节点名称</param>
        /// <returns>返回string</returns>
        public static string GetNodeInfoByNodeName(string path, string nodeName)
        {
            string XmlString = "";
            XmlDocument xml = new XmlDocument();
            xml.Load(path);
            XmlElement root = xml.DocumentElement;
            XmlNode node = root.SelectSingleNode("//" + nodeName);
            if (node != null)
            {
                XmlString = node.InnerText;
            }
            return XmlString;
        }

        /// <summary>
        /// 读取XML资源中的指定节点内容
        /// * 调用示例:
        /// * XmlMethod.GetNodeValue(path, XmlType.File,"Node")
        /// * XmlMethod.GetNodeValue(xmlstring,XmlType.Sreing, "Node")
        /// </summary>
        /// <param name="source">XML资源</param>
        /// <param name="xmlType">XML资源类型：文件，字符串</param>
        /// <param name="nodeName">节点名称</param>
        /// <returns>返回节点内容string</returns>
        public static string GetNodeValue(string source, XmlType xmlType, string nodeName)
        {
            XmlDocument xd = new XmlDocument();
            if (xmlType == XmlType.File)
                xd.Load(source);
            else
                xd.LoadXml(source);
            XmlElement xe = xd.DocumentElement;
            XmlNode xn = xe.SelectSingleNode("//" + nodeName);
            if (xn != null)
                return xn.InnerText;
            else
                return null;
        }

        /// <summary>
        /// 读取XML资源中的指定节点内容
        /// * 调用示例:
        /// * XmlMethod.GetNodeValue(xmlstring,"Node")
        /// </summary>
        /// <param name="source">XML资源字符串</param>
        /// <param name="nodeName">节点名称</param>
        /// <returns>返回节点内容string</returns>
        public static string GetNodeValue(string source, string nodeName)
        {
            if (source == null || nodeName == null || source == "" || nodeName == "" || source.Length < nodeName.Length * 2)
                return null;
            else
            {
                int start = source.IndexOf("<" + nodeName + ">") + nodeName.Length + 2;
                int end = source.IndexOf("</" + nodeName + ">");
                if (start == -1 || end == -1)
                    return null;
                else if (start >= end)
                    return null;
                else
                    return source.Substring(start, end - start);
            }
        }

        /// <summary>
        /// 选择匹配XPath表达式的第一个节点XmlNode
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名")</param>
        /// <returns>返回XmlNode</returns>
        public static XmlNode GetXmlNodeByXpath(string xmlFileName, string xpath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                return xmlNode;
            }
            catch
            {
                return null;
                //throw ex; //这里可以定义你自己的异常处理
            }
        }

        /// <summary>
        /// 选择匹配XPath表达式的节点列表XmlNodeList
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名")</param>
        /// <returns>返回XmlNodeList</returns>
        public static XmlNodeList GetXmlNodeListByXpath(string xmlFileName, string xpath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNodeList xmlNodeList = xmlDoc.SelectNodes(xpath);
                return xmlNodeList;
            }
            catch
            {
                return null;
                //throw ex; //这里可以定义你自己的异常处理
            }
        }

        /// <summary>
        /// 选择匹配XPath表达式的第一个节点的匹配xmlAttributeName的属性XmlAttribute
        /// </summary>
        /// <param name="xmlFileName">XML文档完全文件名(包含物理路径)</param>
        /// <param name="xpath">要匹配的XPath表达式(例如:"//节点名//子节点名</param>
        /// <param name="xmlAttributeName">要匹配xmlAttributeName的属性名称</param>
        /// <returns>返回xmlAttributeName</returns>
        public static XmlAttribute GetXmlAttribute(string xmlFileName, string xpath, string xmlAttributeName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlAttribute xmlAttribute = null;
            try
            {
                xmlDoc.Load(xmlFileName); //加载XML文档
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath);
                if (xmlNode != null)
                {
                    if (xmlNode.Attributes.Count > 0)
                    {
                        xmlAttribute = xmlNode.Attributes[xmlAttributeName];
                    }
                }
            }
            catch
            {
                throw; //这里可以定义你自己的异常处理
            }
            return xmlAttribute;
        }
        #endregion

        #region datatable xml
        /// <summary>
        /// 将DataTable以指定的根结点名称写入文件
        /// </summary>
        /// <param name="dt">含有数据的DataTable</param>
        /// <param name="rootName">根结点名称</param>
        /// <param name="filePath">文件路径</param>
        public static void SaveTableToFile(DataTable dt, string rootName, string filePath)
        {
            DataSet ds = new DataSet(rootName);
            ds.Tables.Add(dt.Copy());
            ds.WriteXml(filePath);
        }

        /// <summary>
        /// 将DataTable写入XML文件中
        /// </summary>
        /// <param name="dt">含有数据的DataTable</param>
        /// <param name="filePath">文件路径</param>
        public static void SaveTableToFile(DataTable dt, string filePath)
        {
            DataSet ds = new DataSet("Config");
            ds.Tables.Add(dt.Copy());
            ds.WriteXml(filePath);
        }


        /// <summary>
        /// 读取XML资源中指定的DataTable的指定行指定列的值
        /// </summary>
        /// <param name="source">XML资源</param>
        /// <param name="xmlType">XML资源类型：文件，字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="rowIndex">行号</param>
        /// <param name="colIndex">列号</param>
        /// <returns>返回值，不存在时返回Null</returns>
        public static object GetTableCell(string source, XmlType xmlType, string tableName, int rowIndex, int colIndex)
        {
            DataSet ds = new DataSet();
            if (xmlType == XmlType.File)
            {
                ds.ReadXml(source);
            }
            else
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(source);
                XmlNodeReader xnr = new XmlNodeReader(xd);
                ds.ReadXml(xnr);
            }
            return ds.Tables[tableName].Rows[rowIndex][colIndex];
        }

        /// <summary>
        /// 读取XML资源中指定的DataTable的指定行指定列的值
        /// </summary>
        /// <param name="source">XML资源</param>
        /// <param name="xmlType">XML资源类型：文件，字符串</param>
        /// <param name="tableName">表名</param>
        /// <param name="rowIndex">行号</param>
        /// <param name="colName">列名</param>
        /// <returns>返回值，不存在时返回Null</returns>
        public static object GetTableCell(string source, XmlType xmlType, string tableName, int rowIndex, string colName)
        {
            DataSet ds = new DataSet();
            if (xmlType == XmlType.File)
            {
                ds.ReadXml(source);
            }
            else
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(source);
                XmlNodeReader xnr = new XmlNodeReader(xd);
                ds.ReadXml(xnr);
            }
            return ds.Tables[tableName].Rows[rowIndex][colName];
        }

        /// <summary>
        /// 读取XML资源到DataTable中
        /// </summary>
        /// <param name="source">XML资源，文件为路径，否则为XML字符串</param>
        /// <param name="xmlType">XML资源类型：文件，字符串</param>
        /// <param name="tableName">表名称</param>
        /// <returns>返回DataTable</returns>
        public static DataTable GetTable(string source, XmlType xmlType, string tableName)
        {
            DataSet ds = new DataSet();
            if (xmlType == XmlType.File)
            {
                ds.ReadXml(source);
            }
            else
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(source);
                XmlNodeReader xnr = new XmlNodeReader(xd);
                ds.ReadXml(xnr);
            }
            return ds.Tables[tableName];
        }

        /// <summary>
        /// 获取一个字符串xml文档中的DataSet
        /// </summary>
        /// <param name="xmlString">含有xml信息的字符串</param>
        /// <param name="ds">输出参数</param>
        /// <returns>返回DataSet</returns>
        public static DataSet GetXmlValueDataSet(string xmlString, DataSet ds)
        {
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xmlString);
            XmlNodeReader xnr = new XmlNodeReader(xd);
            ds.ReadXml(xnr);
            xnr.Close();
            return ds;
        }

        /// <summary>
        /// 读取XML资源到DataSet中
        /// </summary>
        /// <param name="source">XML资源，文件为路径，否则为XML字符串</param>
        /// <param name="xmlType">XML资源类型</param>
        /// <returns>返回DataSet</returns>
        public static DataSet GetDataSet(string source, XmlType xmlType)
        {
            DataSet ds = new DataSet();
            if (xmlType == XmlType.File)
            {
                ds.ReadXml(source);
            }
            else
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(source);
                XmlNodeReader xnr = new XmlNodeReader(xd);
                ds.ReadXml(xnr);
            }
            return ds;
        }

        #endregion

    }
}
