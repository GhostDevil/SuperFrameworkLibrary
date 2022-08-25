using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
namespace SuperFramework.SuperObjectPersistence
{
    internal class ObjectPersistenceTools
    {
        /// <summary> 递归获取控件中的所有子控件
        /// </summary>
        /// <param name="parentControl">父控件</param>
        /// <returns></returns>
        public static List<Control> GetControlList(Control parentControl)
        {
            List<Control> controlList = new List<Control>();
            parentControl.Controls.Cast<Control>().ToList().ForEach(c =>
            {
                controlList.Add(c);
                controlList.AddRange(GetControlList(c));
            });
            return controlList;
        }

        public static void AddControlEvent(Control control, string eventName, Delegate d)
        {
            List<Control> controls = GetControlList(control);
            controls.ForEach(c =>
            {
                EventDescriptor ed = TypeDescriptor.GetEvents(c).Find(eventName, false);
                if (ed != null)
                {
                    ed.AddEventHandler(c, d);
                }
            });
        }

        public static void RemoveControlEvent(Control control, string eventName, Delegate d)
        {
            List<Control> controls = GetControlList(control);
            controls.ForEach(c =>
            {
                EventDescriptor ed = TypeDescriptor.GetEvents(c).Find(eventName, false);
                if (ed != null)
                {
                    ed.RemoveEventHandler(c, d);
                }
            });
        }
    }
}
