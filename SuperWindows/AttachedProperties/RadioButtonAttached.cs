using System.Windows;
using System.Windows.Controls;

namespace SuperFramework.SuperWindows.AttachedProperties
{
    /// <summary>
    /// RadioButton 附加属性类
    /// </summary>
    public class RadioButtonAttached : DependencyObject
    {
        #region IsCanUncheck


        public static bool GetIsCanUncheck(FrameworkElement item)
        {
            return (bool)item.GetValue(IsCanUncheckProperty);
        }


        public static void SetIsCanUncheck(FrameworkElement item, bool value)
        {
            item.SetValue(IsCanUncheckProperty, value);
        }


        /// <summary>
        /// 是否能取消选中 (启用此功能会占用 Tag 属性)
        /// </summary>
        public static readonly DependencyProperty IsCanUncheckProperty =
            DependencyProperty.RegisterAttached(
                "IsCanUncheck",
                typeof(bool),
                typeof(RadioButtonAttached),
                new UIPropertyMetadata(false, OnIsCanUncheckChanged));


        static void OnIsCanUncheckChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement item = depObj as FrameworkElement;


            if (item == null)
                return;


            switch (depObj)
            {
                case RadioButton radioButton:
                    {
                        if ((bool)e.NewValue)
                        {
                            radioButton.PreviewMouseDown += RadioButton_PreviewMouseDown;
                            radioButton.Checked += RadioButton_Checked;
                            radioButton.Unchecked += RadioButton_Unchecked;
                        }
                        else
                        {
                            radioButton.PreviewMouseDown -= RadioButton_PreviewMouseDown;
                            radioButton.Checked -= RadioButton_Checked;
                            radioButton.Unchecked -= RadioButton_Unchecked;
                        }


                        break;
                    }
                default:
                    break;
            }
        }


        private static void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb == null)
            {
                return;
            }


            rb.Tag = false;
        }


        private static void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb == null)
            {
                return;
            }

            rb.Tag = true;
        }


        private static void RadioButton_PreviewMouseDown(object sender, RoutedEventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb == null)
            {
                return;
            }


            // 使用 RadioButton 的 Tag 来存储上次选中的状态，之后可以从中获取来进行判断；
            bool parseSuccess = bool.TryParse(rb.Tag + "", out bool lastChecked);
            if (!parseSuccess)
            {
                // 转换失败，说明是第一次点击，也就是本次本勾选了，所以应该把 true 存起来；
                rb.Tag = true;
            }
            else
            {
                rb.Tag = !lastChecked;
            }


            if (lastChecked)
            {
                rb.IsChecked = false;
                //lastChecked = false;
            }
            else
            {
                rb.IsChecked = true;
                //lastChecked = true;
            }


            e.Handled = true;
        }


        #endregion
    }
}
