using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace SuperFramework.SuperWindows.SuperControl
{
    internal class SearchComboBox : ComboBox
    {
        private bool t = true;//首次获取焦点标志位
        private ObservableCollection<object> bindingList = new();//数据源绑定List
        private string editText = "";//编辑文本内容

        /// <summary>
        /// 注册依赖事件
        /// </summary>
        public static readonly DependencyProperty SearchItemsSourceProperty = DependencyProperty.Register("SearchItemsSource", typeof(IEnumerable), typeof(SearchComboBox), new FrameworkPropertyMetadata(new PropertyChangedCallback(ValueChanged)));
        /// <summary>
        /// 数据源改变，添加数据源到绑定数据源
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SearchComboBox ecb = d as SearchComboBox;
            ecb.bindingList.Clear();
            //遍历循环操作
            foreach (var item in ecb.SearchItemsSource)
            {
                ecb.bindingList.Add(item);
            }
        }
        /// <summary>
        /// 设置或获取ComboBox的数据源
        /// </summary>
        public IEnumerable SearchItemsSource
        {
            get
            {
                return (IEnumerable)GetValue(SearchItemsSourceProperty);
            }

            set
            {
                if (value == null)
                    ClearValue(SearchItemsSourceProperty);
                else
                    SetValue(SearchItemsSourceProperty, value);
            }
        }
        /// <summary>
        /// 重写初始化
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            IsEditable = true;
            IsTextSearchEnabled = false;
            ItemsSource = bindingList;
        }
        /// <summary>
        /// 下拉框获取焦点，首次搜索文本编辑框
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (t)
                FindTextBox(this);
            else
                t = false;
        }
        /// <summary>
        /// 搜索编辑文本框，添加文本改变事件
        /// </summary>
        /// <param name="obj"></param>
        private void FindTextBox(DependencyObject obj)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is TextBox)
                {
                    //注册文本改变事件
                    (child as TextBox).TextChanged += EditComboBox_TextChanged;
                }
                else
                {
                    FindTextBox(child);
                }
            }
        }
        /// <summary>
        /// 文本改变，动态控制下拉条数据源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb.IsFocused)
            {
                IsDropDownOpen = true;
                if (editText == Text)
                    return;
                editText = Text;
                SetList(editText);
            }
        }
        /// <summary>
        /// 组合框关闭，数据源恢复
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);
            if (SearchItemsSource == null)
                return;
            foreach (var item in SearchItemsSource)
            {
                if (!bindingList.Contains(item))
                    bindingList.Add(item);
            }
        }
        /// <summary>
        /// 过滤符合条件的数据项，添加到数据源项中
        /// </summary>
        /// <param name="txt"></param>
        private void SetList(string txt)
        {
            try
            {
                string temp1 = "";
                string temp2 = "";
                if (SearchItemsSource == null)
                    return;
                foreach (var item in SearchItemsSource)
                {
                    temp1 = item.GetType().GetProperty(DisplayMemberPath).GetValue(item, null).ToString();
                    if (string.IsNullOrEmpty(SelectedValuePath))
                    {
                        temp2 = "";
                    }
                    else
                    {
                        temp2 = item.GetType().GetProperty(SelectedValuePath).GetValue(item, null).ToString();
                    }
                    if (temp1.Contains(txt) || temp2.StartsWith(txt))
                    {
                        if (!bindingList.Contains(item))
                            bindingList.Add(item);
                    }
                    else if (bindingList.Contains(item))
                    {
                        bindingList.Remove(item);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}