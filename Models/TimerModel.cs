using System;

namespace MultiTimer.Models
{
    public enum TimerState
    {
        Idle,
        Running,
        Paused,
        Finished
    }

    public class TimerModel
    {
        private TimerState _state = TimerState.Idle;

        public int TotalSeconds { get; private set; }
        public int RemainingSeconds { get; private set; }

        public TimerState State
        {
            get => _state;
            private set
            {
                if (_state != value)
                {
                    _state = value;
                    StateChanged?.Invoke(this, _state);
                }
            }
        }

        public event EventHandler<int> Tick;
        public event EventHandler<TimerState> StateChanged;
        public event EventHandler Finished;

        public void SetDuration(int seconds)
        {
            if (State == TimerState.Running)
                return;

            TotalSeconds = seconds;
            RemainingSeconds = seconds;
            State = TimerState.Idle;
            Tick?.Invoke(this, RemainingSeconds);
        }

        public void Start()
        {
            if (RemainingSeconds <= 0)
                return;

            if (State == TimerState.Idle || State == TimerState.Paused)
            {
                State = TimerState.Running;
            }
        }

        public void Pause()
        {
            if (State == TimerState.Running)
            {
                State = TimerState.Paused;
            }
        }

        public void Reset()
        {
            RemainingSeconds = TotalSeconds;
            State = TimerState.Idle;
            Tick?.Invoke(this, RemainingSeconds);
        }

        /// <summary>
        /// Called externally by the tick source (NUI Timer).
        /// Returns true if the timer is still running and should continue ticking.
        /// </summary>
        public bool OnTick()
        {
            if (State != TimerState.Running)
                return false;

            RemainingSeconds--;
            Tick?.Invoke(this, RemainingSeconds);

            if (RemainingSeconds <= 0)
            {
                State = TimerState.Finished;
                Finished?.Invoke(this, EventArgs.Empty);
                return false;
            }

            return true;
        }

        public string GetDisplayTime()
        {
            int minutes = RemainingSeconds / 60;
            int seconds = RemainingSeconds % 60;
            return $"{minutes:D2}:{seconds:D2}";
        }
    }
}
