using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;
using MultiTimer.Models;
using MultiTimer.Services;

namespace MultiTimer.Views
{
    public class TimerSlotView : View
    {
        private readonly TimerModel _model = new TimerModel();
        private Tizen.NUI.Timer _tickTimer;

        private TextLabel _timeDisplay;
        private Button _startPauseButton;
        private Button _resetButton;
        private View _presetRow;

        private static readonly (string Label, int Seconds)[] Presets = new[]
        {
            ("1m", 60),
            ("5m", 300),
            ("10m", 600),
            ("15m", 900),
            ("30m", 1800),
            ("1h", 3600)
        };

        public TimerSlotView()
        {
            BuildLayout();
            BindModelEvents();
            UpdateControlStates();
        }

        private void BuildLayout()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent;
            HeightSpecification = LayoutParamPolicies.WrapContent;
            BackgroundColor = new Color(0.94f, 0.94f, 0.94f, 1.0f);
            Padding = new Extents(20, 20, 16, 16);
            Margin = new Extents(0, 0, 0, 12);
            Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                CellPadding = new Size2D(0, 12)
            };

            // Time display
            _timeDisplay = new TextLabel
            {
                Text = "00:00",
                PointSize = 48,
                FontFamily = "monospace",
                HorizontalAlignment = HorizontalAlignment.Center,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent
            };
            Add(_timeDisplay);

            // Preset row
            _presetRow = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    CellPadding = new Size2D(8, 0)
                }
            };

            foreach (var (label, seconds) in Presets)
            {
                var btn = new Button
                {
                    Text = label,
                    WidthSpecification = LayoutParamPolicies.WrapContent,
                    HeightSpecification = LayoutParamPolicies.WrapContent,
                    PointSize = 14
                };
                int dur = seconds;
                btn.Clicked += (s, e) => OnPresetClicked(dur);
                _presetRow.Add(btn);
            }
            Add(_presetRow);

            // Control row
            var controlRow = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    CellPadding = new Size2D(16, 0)
                }
            };

            _startPauseButton = new Button
            {
                Text = "Start",
                WidthSpecification = 160,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                PointSize = 16
            };
            _startPauseButton.Clicked += OnStartPauseClicked;
            controlRow.Add(_startPauseButton);

            _resetButton = new Button
            {
                Text = "Reset",
                WidthSpecification = 160,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                PointSize = 16
            };
            _resetButton.Clicked += OnResetClicked;
            controlRow.Add(_resetButton);

            Add(controlRow);
        }

        private void BindModelEvents()
        {
            _model.Tick += (s, remaining) =>
            {
                _timeDisplay.Text = _model.GetDisplayTime();
            };

            _model.StateChanged += (s, state) =>
            {
                UpdateControlStates();
            };

            _model.Finished += (s, e) =>
            {
                StopTickTimer();
                AlertService.PlayAlert();
            };
        }

        private void OnPresetClicked(int seconds)
        {
            AlertService.StopAlert();
            _model.SetDuration(seconds);
        }

        private void OnStartPauseClicked(object sender, ClickedEventArgs e)
        {
            switch (_model.State)
            {
                case TimerState.Idle:
                case TimerState.Paused:
                    _model.Start();
                    StartTickTimer();
                    break;
                case TimerState.Running:
                    _model.Pause();
                    StopTickTimer();
                    break;
                case TimerState.Finished:
                    AlertService.StopAlert();
                    _model.Reset();
                    _model.Start();
                    StartTickTimer();
                    break;
            }
        }

        private void OnResetClicked(object sender, ClickedEventArgs e)
        {
            AlertService.StopAlert();
            StopTickTimer();
            _model.Reset();
        }

        private void StartTickTimer()
        {
            if (_tickTimer != null)
                return;

            _tickTimer = new Tizen.NUI.Timer(1000);
            _tickTimer.Tick += OnTimerTick;
            _tickTimer.Start();
        }

        private void StopTickTimer()
        {
            if (_tickTimer == null)
                return;

            _tickTimer.Stop();
            _tickTimer.Tick -= OnTimerTick;
            _tickTimer.Dispose();
            _tickTimer = null;
        }

        private bool OnTimerTick(object sender, Tizen.NUI.Timer.TickEventArgs e)
        {
            bool keepRunning = _model.OnTick();
            if (!keepRunning)
            {
                _tickTimer = null;
                return false;
            }
            return true;
        }

        private void UpdateControlStates()
        {
            bool isRunning = _model.State == TimerState.Running;
            bool hasTime = _model.TotalSeconds > 0;

            // Update Start/Pause button text
            switch (_model.State)
            {
                case TimerState.Idle:
                    _startPauseButton.Text = "Start";
                    _startPauseButton.IsEnabled = hasTime;
                    break;
                case TimerState.Running:
                    _startPauseButton.Text = "Pause";
                    _startPauseButton.IsEnabled = true;
                    break;
                case TimerState.Paused:
                    _startPauseButton.Text = "Resume";
                    _startPauseButton.IsEnabled = true;
                    break;
                case TimerState.Finished:
                    _startPauseButton.Text = "Start";
                    _startPauseButton.IsEnabled = hasTime;
                    break;
            }

            // Reset enabled when not idle with no time set
            _resetButton.IsEnabled = _model.State != TimerState.Idle || _model.RemainingSeconds != _model.TotalSeconds;

            // Disable presets while running
            foreach (var child in _presetRow.Children)
            {
                if (child is Button btn)
                {
                    btn.IsEnabled = !isRunning;
                }
            }
        }
    }
}
