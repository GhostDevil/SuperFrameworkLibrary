using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Xml;
using System.Xml.Linq;

namespace SuperFramework.SuperConfig
{
    [Serializable]
    public abstract class SuperConfig : ConfigurationSection
    {
#if NET5_0_OR_GREATER
        public static Configuration Get() => Get(Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName + ".dll")));
#endif
#if NET20_OR_GREATER
        public static Configuration Get() => Get(Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile), Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile)));

#endif
        public static Configuration Get(Assembly assembly) => Get($"{assembly.Location}.config");
        public static Configuration Get(string filtPath)
        {
            return !string.IsNullOrWhiteSpace(filtPath) && File.Exists(filtPath) ?
                ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap
                {
                    ExeConfigFilename = filtPath
                }, ConfigurationUserLevel.None) :
                null;
        }

        public static bool TryGetSection(Assembly source, string sectionName, out Configuration config, out ConfigurationSection section)
        {
            if (source == null)
            {
                config = null;
                section = null;
                return false;
            }
            return TryGetSection($"{source.Location}.config", sectionName, out config, out section);
        }
        public static bool TryGetSection(string source, string sectionName, out Configuration config, out ConfigurationSection section)
        {
            try
            {
                var cfg = Get(source);
                if (cfg == null) throw new Exception("配置文件加载失败!");
                var cfgValue = (SuperConfig)cfg.GetSection(sectionName);
                if (cfgValue != null)
                {
                    config = cfg;
                    section = cfgValue;
                    return true;
                }
            }
            catch (Exception e)
            { }
            config = null;
            section = null;
            return false;
        }

        public static bool SaveSection(string source, string sectionName, ConfigurationSection section)
        {
            try
            {
                var cfg = Get(source);
                if (cfg == null) throw new Exception("配置文件加载失败!");
                var cfgValue = (SuperConfig)cfg.GetSection(sectionName);
                if (cfgValue != null)
                {
                    cfg.Sections.Remove(sectionName);
                }
                cfg.Sections.Add(sectionName, ((SuperConfig)section).Clone());
                cfg.Save(ConfigurationSaveMode.Minimal);

                return true;
            }
            catch (Exception ex)
            { }
            return false;
        }

        public SuperConfig Clone()
        {
            var t = Activator.CreateInstance(this.GetType());

            foreach (var p in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                p.SetValue(t, p.GetValue(this, null));
            }

            return (SuperConfig)t; //浅复制
        }

        protected sealed override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            var ignore = typeof(ConfigurationSection).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(q => q.Name);
            var element = XElement.Load(reader);
            foreach (var prop in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(q => !ignore.Contains(q.Name)))
            {
                if (element.Attributes().Any(q => q.Name.LocalName.Equals(prop.Name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    prop.SetValue(this, GetValue(element, prop.Name, prop.PropertyType));
                }
                else
                {
                    var ele = element.Elements().FirstOrDefault(q => q.Name.LocalName.Equals(prop.Name, StringComparison.CurrentCultureIgnoreCase));
                    if (ele != null)
                        prop.SetValue(this, GetValue(ele, prop.Name, prop.PropertyType));
                }
            }
        }

        protected sealed override string SerializeSection(ConfigurationElement parentElement, string name, ConfigurationSaveMode saveMode)
        {
            var doc = new XmlDocument();
            var element = doc.CreateElement(name);
            var ignore = typeof(ConfigurationSection).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(q => q.Name);
            foreach (var prop in this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(q => !ignore.Contains(q.Name)))
            {
                element.AppendChild(SetValue(doc, this, prop));
            }
            doc.AppendChild(element);
            return doc.InnerXml;
            //var sb = new StringBuilder();
            //using (var writer = XmlWriter.Create(sb, new XmlWriterSettings
            //{
            //    Indent = true,
            //    IndentChars = "\t",
            //    NewLineChars = "\r\n",
            //    Encoding = Encoding.UTF8,
            //    OmitXmlDeclaration = true
            //}))
            //{
            //    doc.Save(writer);
            //}

            //return sb.ToString();
        }

        private XmlElement SetValue(XmlDocument doc, ConfigurationElement cfg, PropertyInfo prop)
        {
            var type = prop.PropertyType;
            var element = doc.CreateElement(prop.Name);
            if (type.IsArray)
            {
                var array = (Array)prop.GetValue(cfg);
                for (int i = 0; i < array.Length; i++)
                {
                    var item = doc.CreateElement("Item");
                    var value = array.GetValue(i);
                    if (value.GetType().IsSubclassOf(typeof(ConfigurationElement)))
                    {
                        var ignore = typeof(ConfigurationSection).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(q => q.Name);
                        foreach (var itemProp in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(q => !ignore.Contains(q.Name)))
                        {
                            item.AppendChild(SetValue(doc, (ConfigurationElement)value, itemProp));
                        }
                    }
                    else
                    {
                        item.InnerText = value.ToString();
                    }
                    element.AppendChild(item);
                }
                return element;
            }
            else
            {
                var value = prop.GetValue(cfg);
                if (type.IsSubclassOf(typeof(ConfigurationElement)))
                {
                    var ignore = typeof(ConfigurationSection).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(q => q.Name);
                    foreach (var itemProp in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(q => !ignore.Contains(q.Name)))
                    {
                        element.AppendChild(SetValue(doc, (ConfigurationElement)value, itemProp));
                    }
                }
                else
                {
                    element.InnerText = value?.ToString();
                }
                return element;
            }
        }

        private object GetValue(XElement element, string name, Type type)
        {
            if (type.IsArray)
            {
                var list = element.Elements().Where(q => q.Name.LocalName.Equals("Add", StringComparison.CurrentCultureIgnoreCase) || q.Name.LocalName.Equals("Item", StringComparison.CurrentCultureIgnoreCase)).ToList();
                var eleType = type.GetElementType();
                var array = Array.CreateInstance(eleType, list.Count);
                for (int i = 0; i < list.Count; i++)
                    array.SetValue(GetValue(list[i], name, eleType), i);
                return array;
            }
            else
            {
                var attributes = element.Attributes();
                var subElements = element.Elements();
                if (type.IsSubclassOf(typeof(ConfigurationElement)))
                {
                    var value = Activator.CreateInstance(type);
                    var ignore = typeof(ConfigurationSection).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(q => q.Name);
                    foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(q => !ignore.Contains(q.Name)))
                    {
                        p.SetValue(value, GetValue(element, p.Name, p.PropertyType));
                    }
                    return value;
                }
                else
                {
                    var attr = attributes.Where(q => q.Name.LocalName.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                    if (attr == null)
                    {
                        var attrElement = element.Elements().FirstOrDefault(q => q.Name.LocalName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                        if (attrElement != null)
                            return Convert.ChangeType(attrElement.Value.Trim(), type);
                        if (element.Name.LocalName.Equals("Add", StringComparison.CurrentCultureIgnoreCase))
                            attr = element.Parent.Attributes().FirstOrDefault(q => q.Name.LocalName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                    }
                    return Convert.ChangeType(attr == null ? element.Value.Trim() : attr.Value.Trim(), type);
                }
            }
        }

        //protected sealed override ConfigurationElementProperty ElementProperty => null;
        protected sealed override object GetRuntimeObject() => null;
        protected sealed override string GetTransformedAssemblyString(string assemblyName) => assemblyName;
        protected sealed override string GetTransformedTypeString(string typeName) => typeName;
        protected sealed override bool IsModified() => true;
        public sealed override bool IsReadOnly() => false;
        protected sealed override bool OnDeserializeUnrecognizedAttribute(string name, string value) => true;
        protected sealed override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader) => true;
        protected sealed override object OnRequiredPropertyNotFound(string name) => null;
        protected sealed override bool ShouldSerializeElementInTargetVersion(ConfigurationElement element, string elementName, FrameworkName targetFramework) => true;
        protected sealed override bool ShouldSerializePropertyInTargetVersion(ConfigurationProperty property, string propertyName, FrameworkName targetFramework, ConfigurationElement parentConfigurationElement) => true;
        protected sealed override bool ShouldSerializeSectionInTargetVersion(FrameworkName targetFramework) => true;
        protected sealed override bool SerializeToXmlElement(XmlWriter writer, string elementName) => true;
        protected sealed override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey) => true;
        protected sealed override void Init() { }
        protected sealed override void SetReadOnly() { }
        protected sealed override void InitializeDefault() { }
        protected sealed override void Reset(ConfigurationElement parentElement) { }
        protected sealed override void ResetModified() { }
        protected sealed override void Unmerge(ConfigurationElement sourceElement, ConfigurationElement parentElement, ConfigurationSaveMode saveMode) { }
        protected sealed override void ListErrors(IList errorList) { }
        protected sealed override void PostDeserialize() { }
        protected sealed override void PreSerialize(XmlWriter writer) { }
    }
}