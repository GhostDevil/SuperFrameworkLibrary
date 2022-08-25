using System.Windows;

namespace SuperFramework.SuperWindows
{
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore() => new BindingProxy();

#if NET5_0_OR_GREATER
        public object? Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }
#else

        public object Data
        {
            get => GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

#endif
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}
