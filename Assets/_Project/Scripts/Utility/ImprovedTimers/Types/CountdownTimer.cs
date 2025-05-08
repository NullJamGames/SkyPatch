namespace NJG.Utilities.ImprovedTimers
{
    /// <summary>
    ///     Basic Timer that counts down from a specified value.
    /// </summary>
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float value) : base(value) { }

        public bool IsFinished => Time <= 0;

        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0)
                Time -= deltaTime;

            if (IsRunning && Time <= 0)
                Stop();
        }

        public void Reset() => Time = _initialTime;

        public void Reset(float newTime)
        {
            _initialTime = newTime;
            Reset();
        }
    }
}