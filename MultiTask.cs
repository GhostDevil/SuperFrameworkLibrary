using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace SuperFramework
{
    public class MultiTask
    {
         CancellationTokenSource tokenSource;
         CancellationToken token;
         ManualResetEvent resetEvent;
         ConcurrentQueue<CustomTask> taskQueue;
        bool isRun = false;
        Task task;
        void Init()
        {
            taskQueue = new ConcurrentQueue<CustomTask>();
            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            resetEvent = new ManualResetEvent(true);            
        }
        void UnInit()
        {
            taskQueue = null;
            tokenSource =null;
            resetEvent = null;
        }
        /// <summary>
        /// 开始队列执行
        /// </summary>
        public bool BeginQueue()
        {
            if (isRun)
                return false;
            Init();
            task = new Task(async () =>
            {
                isRun = true;
                while (true)
                {

                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    if (!taskQueue.IsEmpty)
                    {
                       await Task.Delay(500).ConfigureAwait(false);
                        continue;
                    }
                    if (taskQueue.IsEmpty)
                    {
                        resetEvent.Reset();
                    }
                    resetEvent.WaitOne();
                    if (taskQueue.TryDequeue(out CustomTask customTask))
                    {
                        Console.WriteLine($"Task {customTask.Id} starting···");
                        await Task.Run(customTask.action).ConfigureAwait(false);
                        Console.WriteLine($"Task {customTask.Id} finish !");
                    }
                }
            }, token);
            task.Start();
            return true;
        }
        /// <summary>
        /// 结束队列执行
        /// </summary>
        public void EndQueue()
        {
            tokenSource?.Cancel();
            isRun = false;
            while ( task.Status== TaskStatus.Running)
            {
                Task.Delay(100).Wait();
            }
            resetEvent.Reset();
            task.Dispose();
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            EndQueue();
            UnInit();
            task = null;
        }
        //public  async Task RunProgram(ConcurrentQueue<CustomTask> ts)
        //{

        //    var cts = new CancellationTokenSource();
        //    //生成任务添加至并发队列
        //    var taskSource = Task.Run(() => TaskProducer(ts));
        //    while (!taskSource.IsCompleted)
        //    {
        //        Task.Delay(100).Wait();
        //    } 
        //}
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="queue"></param>
        public void TaskProducer(ConcurrentQueue<CustomTask> queue)
        {
            
            while (!queue.IsEmpty)
            {
                if(queue.TryDequeue(out CustomTask customTask))
                    taskQueue.Enqueue(customTask);
            }

            resetEvent.Set();
        }
          /// <summary>
          /// 自定义任务
          /// </summary>
        public class CustomTask
        {
            /// <summary>
            /// 任务过程
            /// </summary>
            public Action action { get; set;}
            /// <summary>
            /// 任务id
            /// </summary>
            public int Id { get; set; }
        }
    }
}
