namespace NJG.Utilities.ImprovedTimers
{
    public class PulseTimer : Timer
    {
        private bool _isOnPhase = true;
        
        public float OnDuration { get; private set; }
        public float OffDuration { get; private set; }
        
        public PulseTimer(float onDuration, float offDuration) : base(onDuration)
        {
            OnDuration = onDuration;
            OffDuration = offDuration;
        }

        public override void Tick(float deltaTime)
        {
            if (IsRunning && Time > 0)
            {
                Time -= deltaTime;
            }

            if (!IsRunning || !(Time <= 0)) return;
            
            if (_isOnPhase)
            {
                Time = OffDuration;
                _isOnPhase = false;
            }
            else
            {
                Time = OnDuration;
                _isOnPhase = true;
            }

            OnTimerStop.Invoke();
            OnTimerStart.Invoke();
        }

        public bool IsOnPhase() => _isOnPhase;
    }
}