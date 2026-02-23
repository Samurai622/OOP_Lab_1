using System;
using System.Threading;

namespace Lab1
{
    public delegate void TimerAction();
    public class Timer
    {
        private TimerAction _action;    
        private int _interval;
        private bool _isRunning;

        public Timer(TimerAction action, int interval)
        {
            _action = action;
            _interval = interval;
            _isRunning = false;
        }
        public void Start()
        {
            if (_isRunning)
                return;
            _isRunning = true;
            Thread timerThread = new Thread(() =>
            {
                while (_isRunning)
                {
                    Thread.Sleep(_interval * 1000);
                    if (_isRunning)
                        _action?.Invoke();
                }
            });    
            timerThread.IsBackground = true;
            timerThread.Start();        
        }
        public void Stop()
        {
            _isRunning = false;
        }
    }
}