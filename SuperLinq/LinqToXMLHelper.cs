using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace SuperFramework.SuperLinq
{
    /// <summary>
    /// 用LinqToXml的方式操作XML
    /// 特别要说明的是 LinqToXml中每个xml节点都是一个元素（Element）
    /// 所有操作最后必须调用 SaveXmlFile方法才能更新到文件
    /// 否则只是修改内存中的数据
    /// </summary>
    public static class LinqToXMLHelper
    {
        #region  生成XML文件
        /// <summary>
        /// 生成XML文件
        /// </summary>
        /// <param name="XmlFile">XML保存的路径</param>
        public static void SaveXmlFile(string XmlFile, XElement element)
        {
            element.Save(GetXmlFullPath(XmlFile));
        }
        #endregion

        #region 加载xml文件到XElement
        /// <summary>
        /// 加载xml文件到XElement
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static XElement Load(string path)
        {
            return XElement.Load(GetXmlFullPath(path));
        }
        #endregion

        #region 添加元素
        /// <summary>
        /// 批量添加元素
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="childElements">new XElement("节点名称", new XAttribute("节点属性", 节点属性),  new XElement("子节点", new XAttribute("节点属性", 节点属性))，无限添加子节点   );</param>
        public static void AddElements(XElement parentElement, IEnumerable<XElement> childElements)
        {
            foreach (XElement xe in childElements)
            {
                parentElement.Add(xe);
            }
        }
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="childElement">new XElement("节点名称", new XAttribute("节点属性", 节点属性),  new XElement("子节点", new XAttribute("节点属性", 节点属性))，无限添加子节点   );</param>
        public static void AddElement(XElement parentElement, XElement childElement)
        {
            parentElement.Add(childElement);

        }
        #endregion

        #region 根据元素名称删除元素
        /// <summary>
        /// 根据元素名称删除元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="RemoveElementID"></param>
        public static void RemoveElement(XElement element, string RemoveElementID)
        {
            XElement xe = element.Element(RemoveElementID);
            xe.Remove();
        }

        #endregion

        #region 修改某元素的值
        /// <summary>
        /// 修改某元素的值
        /// </summary>
        /// <param name="element"></param>
        /// <param name="elementName"></param>
        /// <param name="setValue"></param>
        public static void ModifyElement(XElement element, string elementName, string setValue)
        {
            element.Element(elementName).SetValue(setValue);
        }
        #endregion

        #region 根据元素名称查询元素
        /// <summary>
        /// 根据元素名称查询元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="RemoveElementID"></param>
        public static XElement QueryElement(XElement element, string QueryElementID)
        {
            return element.Element(QueryElementID);

        }
        /// <summary>
        /// 根据元素名称批量查询元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="RemoveElementID"></param>
        public static IEnumerable<XElement> QueryElements(XElement element, string QueryElementID)
        {
            return element.Elements(QueryElementID);

        }
        /// <summary>
        /// 根据元素名和   属性名称批量查询元素
        /// </summary>
        /// <param name="element">源</param>
        /// <param name="QueryElementID"></param>
        /// <param name="AttributeName">属性名</param>
        /// <param name="AttributeValue">属性值</param>
        /// <returns></returns>
        public static IEnumerable<XElement> QueryElements(XElement element, string QueryElementID, string AttributeName, string AttributeValue)
        {
            return element.Elements(QueryElementID).Where(i => i.Attribute(AttributeName).Value == AttributeValue).ToList<XElement>();

        }
        #endregion

        #region 获取完整路径
        /// <summary>
        /// 获取完整路径 
        /// </summary>
        /// <param name="strPath">Xml的路径 </param>
        /// <returns>返回完整路径</returns>
        public static string GetXmlFullPath(string strPath)
        {
            //如果路径中含有:符号,则认定为传入的是完整路径 
            if (strPath.IndexOf(":") > 0)
            {
                return strPath;
            }
            else
            {
                //返回完整路径 
                return System.Web.HttpContext.Current.Server.MapPath(strPath);
            }
        }
        #endregion
    }
}
