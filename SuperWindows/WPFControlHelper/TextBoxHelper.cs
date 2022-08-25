using System.Windows;
using System.Windows.Controls;

namespace SuperFramework.SuperWindows.WPFControlHelper
{
    /// <summary>
    /// 文本框
    /// </summary>
    public static class TextBoxHelper
    {
        /// <summary>
        /// 文本框的水印附加属性
        /// </summary>
        public class TextBoxMonitor : DependencyObject
        {
            public static bool GetIsMonitoring(DependencyObject obj)
            {
                return (bool)obj.GetValue(IsMonitoringProperty);
            }

            public static void SetIsMonitoring(DependencyObject obj, bool value)
            {
                obj.SetValue(IsMonitoringProperty, value);
            }

            public static readonly DependencyProperty IsMonitoringProperty =
                DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(TextBoxHelper), new UIPropertyMetadata(false, OnIsMonitoringChanged));



            public static int GetTextBoxLength(DependencyObject obj)
            {
                return (int)obj.GetValue(TextBoxLengthProperty);
            }

            public static void SetTextBoxLength(DependencyObject obj, int value)
            {
                obj.SetValue(TextBoxLengthProperty, value);
            }

            public static readonly DependencyProperty TextBoxLengthProperty =
                DependencyProperty.RegisterAttached("TextBoxLength", typeof(int), typeof(TextBoxHelper), new UIPropertyMetadata(0));

            private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var tx = d as TextBox;
                if (tx == null)
                {
                    return;
                }
                if ((bool)e.NewValue)
                {
                    tx.TextChanged += new TextChangedEventHandler(tx_TextChanged);
                }
                else
                {
                    tx.TextChanged -= new TextChangedEventHandler(tx_TextChanged);
                }
            }

            static void tx_TextChanged(object sender, TextChangedEventArgs e)
            {
                var tx = sender as TextBox;
                if (tx == null)
                {
                    return;
                }
                SetTextBoxLength(tx, tx.Text.Length);
            }


        }
        /// <summary>
        /// 密码框的水印附加属性
        /// </summary>
        public class PasswordBoxMonitor : DependencyObject
        {
            public static bool GetIsMonitoring(DependencyObject obj)
            {
                return (bool)obj.GetValue(IsMonitoringProperty);
            }

            public static void SetIsMonitoring(DependencyObject obj, bool value)
            {
                obj.SetValue(IsMonitoringProperty, value);
            }

            public static readonly DependencyProperty IsMonitoringProperty =
                DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(PasswordBoxMonitor), new UIPropertyMetadata(false, OnIsMonitoringChanged));



            public static int GetPasswordLength(DependencyObject obj)
            {
                return (int)obj.GetValue(PasswordLengthProperty);
            }

            public static void SetPasswordLength(DependencyObject obj, int value)
            {
                obj.SetValue(PasswordLengthProperty, value);
            }

            public static readonly DependencyProperty PasswordLengthProperty =
                DependencyProperty.RegisterAttached("PasswordLength", typeof(int), typeof(PasswordBoxMonitor), new UIPropertyMetadata(0));

            private static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var pb = d as PasswordBox;
                if (pb == null)
                {
                    return;
                }
                if ((bool)e.NewValue)
                {
                    pb.PasswordChanged += PasswordChanged;
                }
                else
                {
                    pb.PasswordChanged -= PasswordChanged;
                }
            }

            static void PasswordChanged(object sender, RoutedEventArgs e)
            {
                var pb = sender as PasswordBox;
                if (pb == null)
                {
                    return;
                }
                SetPasswordLength(pb, pb.Password.Length);
            }
        }
    }
}
