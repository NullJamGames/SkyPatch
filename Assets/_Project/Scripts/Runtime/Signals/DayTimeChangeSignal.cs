namespace NJG.Runtime.Signals
{
    public struct DayTimeChangeSignal
    {
        public bool IsDayTime { get; }

        public DayTimeChangeSignal(bool isDayTime) => IsDayTime = isDayTime;
    }
}