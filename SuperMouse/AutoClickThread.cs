using System.Collections.Generic;
using System.Threading;

namespace SuperFramework.SuperMouseHelper
{
    /// <summary>
    /// 类 名:AutoClickThread
    /// 版 本:Release
    /// 日 期:2015-07-21
    /// 作 者:不良帥
    /// 描 述:自动点击封装
    /// </summary>
    static class AutoClickThread
    {
        public static List<string> ClickData;
        static CancellationTokenSource cts;

        //开始
        public static void Start(bool isRecycleRun)
        {
            if (ClickData.Count == 0) return;

            if (cts != null && !cts.IsCancellationRequested) return;

            cts = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(new WaitCallback(Run), isRecycleRun);
        }

        //停止
        public static void Stop()
        {
            cts?.Cancel();
        }

        //执行
        static void Run(object isRecycleRun)
        {
            string[] stringArray;
            int x, y, sleepTime;

            do
            {
                foreach (string item in ClickData)
                {
                    if (cts.Token.IsCancellationRequested) return;

                    stringArray = item.Split('|');
                    x = int.Parse(stringArray[0]);
                    y = int.Parse(stringArray[1]);
                    sleepTime = int.Parse(stringArray[3]);

                    switch (stringArray[2])
                    {
                        case "左单击": MouseHelper.LeftButtonClick(x, y); break;
                        case "左双击": MouseHelper.LeftButtonDoubleClick(x, y); break;
                        case "右单击": MouseHelper.RightButtonClick(x, y); break;
                        case "右双击": MouseHelper.RightButtonDoubleClick(x, y); break;
                    }

                    Thread.Sleep(sleepTime);
                }
            }
            while ((bool)isRecycleRun);
        }
    }
}
