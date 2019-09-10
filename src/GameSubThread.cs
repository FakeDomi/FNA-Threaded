using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.Xna.Framework
{
    internal class GameSubThread
    {
        private readonly ConcurrentQueue<Tuple<Action, NullSignal>> scheduledTasks = new ConcurrentQueue<Tuple<Action, NullSignal>>();
        private readonly Thread thread;
        private readonly Game game;
        private bool shouldTick;

        internal static GameSubThread Instance { get; private set; }
        
        private GameSubThread(Game game)
        {
            this.game = game;
            thread = new Thread(RunLoop) {Name = "GameSubThread", IsBackground = true};
        }

        internal static void Setup(Game game)
        {
            Instance = new GameSubThread(game);
            Instance.thread.Start();
        }

        internal static void Abort()
        {
            Instance.thread.Abort();
        }

        internal void Schedule(Action task)
        {
            scheduledTasks.Enqueue(new Tuple<Action, NullSignal>(task, NullSignal.Instance));
        }

        internal void ScheduleWait(Action task)
        {
            var signal = new AutoResetSignal();
            scheduledTasks.Enqueue(new Tuple<Action, NullSignal>(task, signal));

            if (thread.IsAlive)
            {
                signal.Wait();
            }
        }

        internal void EnableTicking()
        {
            shouldTick = true;
        }

        private void RunLoop()
        {
            while (true)
            {
                if (!shouldTick && scheduledTasks.IsEmpty)
                {
                    Thread.Sleep(50);
                    continue;
                }

                while (!scheduledTasks.IsEmpty)
                {
                    scheduledTasks.TryDequeue(out var result);
                    result.Item1.Invoke();
                    result.Item2.Set();
                }

                if (shouldTick)
                {
                    game.Tick();
                }
            }
        }
    }

    internal class NullSignal
    {
        internal static NullSignal Instance { get; } = new NullSignal();

        internal virtual void Set()
        {
        }

        internal virtual void Wait()
        {
        }
    }

    internal class AutoResetSignal : NullSignal
    {
        private readonly AutoResetEvent signal = new AutoResetEvent(false);

        internal override void Set()
        {
            signal.Set();
        }

        internal override void Wait()
        {
            signal.WaitOne();
        }
    }
}
