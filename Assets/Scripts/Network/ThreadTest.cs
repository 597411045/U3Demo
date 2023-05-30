using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class ThreadTest
{
    private static bool flag;
    private static readonly object locker = new object();

    public delegate void ThreadStartDele();

    public ThreadStartDele z;

    void fun()
    {
        z += fun2;

        Thread a = new Thread(z.Invoke);
        a.Name = "a";
        a.Start();
        fun2();

        Thread.CurrentThread.Name = "MainThread";
        a.Join();
        CancellationTokenSource ca = new CancellationTokenSource();
        //ca.Token.WaitHandle;
        //ca.Token.Register();
        
        Thread x = new Thread(() => { fun3(4); });
        Thread x3 = new Thread(() => { fun5(ca.Token); });
        Thread x2 = new Thread(fun3);
        x2.Start(4);
        x2.IsBackground = true;
        bool a1 = x2.IsThreadPoolThread;
        var x2c = x2.ThreadState;
        //Process

        Task t2 = new Task(fun2);
        t2.Start();
        t2.RunSynchronously();
        t2.Wait();

        var c = t2.Status;
        Task.Factory.StartNew(fun2);
        Task<bool> t = Task.Factory.StartNew(fun4,ca.Token);
        bool b1 = t.Result;
        Task<bool> t3 = Task.Factory.StartNew((state) => { return fun4(); }, "state");

        var v = t3.AsyncState;

        Task.WaitAll();
        Task.WaitAny();
    }

    void fun2()
    {
        lock (locker)
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            if (flag)
            {
                //Thread.Sleep(0);
                //Thread.Yield();
            }

            try
            {
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e.InnerException.Message);
                throw;
            }
        }
    }

    void fun3(object a)
    {
    }

    bool fun4()
    {
        return true;
    }

    void fun5(CancellationToken c)
    {
        c.ThrowIfCancellationRequested();
        if (c.IsCancellationRequested)
        {
            
        }
    }
}