using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SuperFramework.SuperWindows
{
    /// <summary>
    /// UI类操作
    /// </summary>
    public static class ElementHelper
    {
        /// <summary>
        /// 获取控件矩形信息
        /// </summary>
        /// <param name="element"></param>
        /// <param name="relativeToScreen"></param>
        /// <returns></returns>
        public static Rect GetAbsolutePlacement(FrameworkElement element, bool relativeToScreen = false)
        {
            var absolutePos = element.PointToScreen(new Point(0, 0));
            if (relativeToScreen)
            {
                return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
            }
            var posMW = Application.Current.MainWindow.PointToScreen(new Point(0, 0));
            absolutePos = new Point(absolutePos.X - posMW.X, absolutePos.Y - posMW.Y);
            return new Rect(absolutePos.X, absolutePos.Y, element.ActualWidth, element.ActualHeight);
        }
        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, 
        /// a null parent is being returned.</returns>
        public static T FindChild<T>(DependencyObject parent, string childName)
           where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static DependencyObject FindChild(DependencyObject o, Type childType)
        {
            DependencyObject foundChild = null;
            if (o != null)
            {
                int childrenCount = VisualTreeHelper.GetChildrenCount(o);
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(o, i);
                    if (child.GetType() != childType)
                    {
                        foundChild = FindChild(child, childType);
                    }
                    else
                    {
                        foundChild = child;
                        break;
                    }
                }
            }
            return foundChild;
        }

        public static T FindVisualParent<T>(UIElement element) where T : UIElement
        {
            UIElement parent = element; while (parent != null)
            {
                if (parent is T correctlyTyped)
                {
                    return correctlyTyped;
                }
                parent = VisualTreeHelper.GetParent(parent) as UIElement;
            }
            return null;
        }

        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            T childElement = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (!(child is T))
                {
                    childElement = FindChild<T>(child); if (childElement != null)
                    break;
                }
                else
                {
                    childElement = (T)child;
                    break;
                }
            }
            return childElement;
        }


        /// <summary>
        /// 根据控件名称，查找父控件
        /// elementName为空时，查找指定类型的父控件
        /// </summary>
        public static T FindParentByName<T>(this DependencyObject obj, string elementName) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if ((parent is T t) && (t.Name == elementName || string.IsNullOrEmpty(elementName)))
                    return t;
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
        }



        /// <summary>
        /// 根据控件名称，查找子控件
        /// elementName为空时，查找指定类型的子控件
        /// </summary>
        public static T FindChildByName<T>(this DependencyObject obj, string elementName) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t && (t.Name == elementName) || (string.IsNullOrEmpty(elementName)))
                    return (T)child;
                else
                {
                    T grandChild = FindChildByName<T>(child, elementName);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据控件名称，查找子控件集合
        /// elementName为空时，查找指定类型的所有子控件
        /// </summary>
        public static List<T> FindChildrenByName<T>(this DependencyObject obj, string elementName) where T : FrameworkElement
        {
            List<T> childList = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child is T t && (t.Name == elementName) || (string.IsNullOrEmpty(elementName)))
                    childList.Add((T)child);
                else
                {
                    List<T> grandChildList = FindChildrenByName<T>(child, elementName);
                    if (grandChildList != null)
                        childList.AddRange(grandChildList);
                }
            }
            return childList;
        }
        /// <summary>
        /// 获取某个容器中的一类控件
        /// </summary>
        /// <typeparam name="T">查找控件类型</typeparam>
        /// <param name="obj">所在容器</param>
        /// <returns></returns>
        public static List<T> FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            try
            {
                List<T> TList = new List<T> { };
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T)
                    {
                        TList.Add((T)child);
                        List<T> childOfChildren = FindVisualChild<T>(child);
                        if (childOfChildren != null)
                        {
                            TList.AddRange(childOfChildren);
                        }
                    }
                    else
                    {
                        List<T> childOfChildren = FindVisualChild<T>(child);
                        if (childOfChildren != null)
                        {
                            TList.AddRange(childOfChildren);
                        }
                    }
                }
                return TList;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
