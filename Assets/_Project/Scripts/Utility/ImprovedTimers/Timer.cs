using System;

namespace NJG.Utilities.ImprovedTimers
{
    public abstract class Timer
    {
        protected float _initialTime;
        public float Time { get; set; }
        public bool IsRunning { get; protected set; }
        public bool IsPaused { get; protected set; }

        public float Progress => Time / _initialTime;

        public Action OnTimerStart = delegate { };
        public Action OnTimerStop = delegate { };

        protected Timer(float value) 
        {
            _initialTime = value;
            IsRunning = false;
        }

        public void Start() 
        {
            Time = _initialTime;
            if (IsRunning) return;
            
            IsRunning = true;
            IsPaused = false;
            TimerManager.RegisterTimer(this);
            OnTimerStart.Invoke();
        }

        public void Stop()
        {
            if (!IsRunning) return;
            
            IsRunning = false;
            IsPaused = false;
            TimerManager.DeregisterTimer(this);
            OnTimerStop.Invoke();
        }

        public void Resume()
        {
            IsRunning = true;
            IsPaused = false;
        }

        public void Pause()
        {
            IsRunning = false;
            IsPaused = true;
        }

        public abstract void Tick(float deltaTime);
    }
}