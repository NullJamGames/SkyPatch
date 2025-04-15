namespace NJG.Utilities.ImprovedTimers
{
    /// <summary>
    /// A timer that automatically restarts each time it finishes.
    /// </summary>
    public class RepeatTimer : Timer
    {
        public RepeatTimer(float value) : base(value) {}

        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0)
            {
                Time -= deltaTime;
            }

            if (!IsRunning || !(Time <= 0)) return;
            
            Reset();
            OnTimerStop.Invoke();
            OnTimerStart.Invoke();
        }
        
        public void Reset() => Time = _initialTime;

        public void Reset(float newTime) 
        {
            _initialTime = newTime;
            Reset();
        }
    }
}