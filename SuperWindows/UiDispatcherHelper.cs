using System;
using System.Windows.Threading;

namespace SuperFramework.SuperWindows
{
    public static class UiDispatcherHelper
    {
        public static Dispatcher UiDispatcher { get; private set; }

        /// <summary>
        /// 这个方法应该在UI线程上调用一次，以确保初始化属性。
        /// <para>在WPF中，在静态App()构造函数上调用此方法</para>
        /// </summary>
        public static void Initialize()
        {
            if (UiDispatcher != null && UiDispatcher.Thread.IsAlive)
                return;

            UiDispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        ///在UI线程上执行一个操作。该操作将在UI线程的dispatcher和异步执行.
        /// </summary>
        /// <param name="action">操作将在UI线程的调度程序中排队并异步执行。</param>
        public static void BeginInvokeOnUi(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            UiDispatcher.InvokeAsync(action, DispatcherPriority.Input);
        }

        /// <summary>
        /// 在UI线程上执行一个操作。操作将在UI线程的调度程序中排队并同步执行。
        /// </summary>
        /// <param name="action">
        /// 将在UI线程上同步执行的操作。
        /// </param>
        public static void InvokeOnUi(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            UiDispatcher.Invoke(action);
        }

        /// <summary>
        /// 在UI线程上执行一个操作。操作将在UI线程的调度程序中以指定的优先级排队并异步执行。
        /// </summary>
        /// <param name="action"> 将在UI线程上执行的操作。</param>
        /// <param name="priority"></param>

        public static DispatcherOperation InvokeOnUiAsync(Action action, DispatcherPriority priority)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            return UiDispatcher.InvokeAsync(action, priority);
        }
    }
}
