using log4net.Layout.Pattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using System.IO;
using System.Reflection;

namespace SuperFramework.AppLogs
{
    public class MyPatternConverter:PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            if (null != Option)
                WriteObject(writer, loggingEvent.Repository, LookupProperty(Option, loggingEvent));
            else
                WriteDictionary(writer, loggingEvent.Repository, loggingEvent.GetProperties());
        }
        /// <summary>
        /// 通过反射获取传入的日志对象的某个属性
        /// </summary>
        /// <param name="property"></param>
        /// <param name="loggingEent"></param>
        /// <returns></returns>
        private object LookupProperty(string property, LoggingEvent loggingEent)
        {
            object propertyValue = string.Empty;
            PropertyInfo propertyInfo = loggingEent.MessageObject.GetType().GetProperty(property);
            if (null != propertyInfo)
                propertyValue = propertyInfo.GetValue(loggingEent.MessageObject, null);
            return propertyValue;
        }
    }
}
