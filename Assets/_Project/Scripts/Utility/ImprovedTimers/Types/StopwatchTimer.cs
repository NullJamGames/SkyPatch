namespace NJG.Utilities.ImprovedTimers
{
    /// <summary>
    /// A timer counts up from zero to measure the duration of an event.
    /// </summary>
    public class StopwatchTimer : Timer
    {
        public StopwatchTimer() : base(0) {}

        public override void Tick(float deltaTime)
        {
            if (IsRunning)
            {
                Time += deltaTime;
            }
        }

        public void ResetStopwatch()
        {
            Time = 0;
        }
    }
}