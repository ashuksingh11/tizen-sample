using System;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;
using Tizen.NUI.Components;

namespace MultiTimer.Views
{
    public class MainPage : View
    {
        private const int MaxTimers = 3;
        private int _timerCount = 0;

        private View _timerContainer;
        private Button _addButton;

        public MainPage()
        {
            WidthSpecification = LayoutParamPolicies.MatchParent;
            HeightSpecification = LayoutParamPolicies.MatchParent;
            BackgroundColor = Color.White;
            Layout = new LinearLayout
            {
                LinearOrientation = LinearLayout.Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Center,
                CellPadding = new Size2D(0, 12)
            };
            Padding = new Extents(16, 16, 16, 16);

            // Header
            var header = new TextLabel
            {
                Text = "Multi Timer",
                PointSize = 28,
                HorizontalAlignment = HorizontalAlignment.Center,
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Margin = new Extents(0, 0, 0, 8)
            };
            Add(header);

            // Scrollable container for timer slots
            var scrollable = new ScrollableBase
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.MatchParent,
                Weight = 1.0f,
                ScrollingDirection = ScrollableBase.Direction.Vertical,
                HideScrollbar = false
            };

            _timerContainer = new View
            {
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                Layout = new LinearLayout
                {
                    LinearOrientation = LinearLayout.Orientation.Vertical
                }
            };
            scrollable.Add(_timerContainer);
            Add(scrollable);

            // Add Timer button
            _addButton = new Button
            {
                Text = "+ Add Timer",
                WidthSpecification = LayoutParamPolicies.MatchParent,
                HeightSpecification = LayoutParamPolicies.WrapContent,
                PointSize = 18
            };
            _addButton.Clicked += OnAddTimerClicked;
            Add(_addButton);

            // Start with one timer
            AddTimerSlot();
        }

        private void OnAddTimerClicked(object sender, ClickedEventArgs e)
        {
            AddTimerSlot();
        }

        private void AddTimerSlot()
        {
            if (_timerCount >= MaxTimers)
                return;

            var slot = new TimerSlotView();
            _timerContainer.Add(slot);
            _timerCount++;

            if (_timerCount >= MaxTimers)
            {
                _addButton.Hide();
            }
        }
    }
}
