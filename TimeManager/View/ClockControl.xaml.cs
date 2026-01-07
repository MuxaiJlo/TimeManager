using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TimeManager.Model;

namespace TimeManager.View
{
    /// <summary>
    /// Interaction logic for ClockControl.xaml
    /// </summary>
    public partial class ClockControl : UserControl
    {
        private Line _currentTimeLine;
        private Line _minuteHandLine;
        private Line _secondHandLine;

        public ClockControl()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            { 
                DrawClockFace();
                StartClock();
            };
        }
        public void SubscribeToData(ObservableCollection<TimeSlot> slots)
        {
            slots.CollectionChanged += (s, e) => UpdateDashboard(slots);
            UpdateDashboard(slots); 
        }
        private double CalculateAngle(TimeOnly time)
        {
            double hours = time.Hour + (time.Minute / 60.0);
            double degrees = hours * 15;
            return degrees;
        }

        private double CalculateMinuteAngle(TimeOnly time)
        {
            double minutes = time.Minute + (time.Second / 60.0);
            double degrees = minutes * 6;
            return degrees;
        }

        private double CalculateSecondAngle(TimeOnly time)
        {
            double seconds = time.Second;
            double degrees = seconds * 6;
            return degrees;
        }

        private Point GetPointOnClock(double degrees, double radius, Point center)
        {
            double adjustedDegrees = degrees - 90;
            double radians = adjustedDegrees * (Math.PI / 180);

            double x = center.X + radius * Math.Cos(radians);
            double y = center.Y + radius * Math.Sin(radians);

            return new Point(x, y);
        }

        public void UpdateDashboard(ObservableCollection<TimeSlot> timeSlots)
        {
            SegmentsCanvas.Children.Clear();
            double radius = Math.Min(SegmentsCanvas.ActualWidth, SegmentsCanvas.ActualHeight) / 2;
            Point center = new Point(SegmentsCanvas.ActualWidth / 2, SegmentsCanvas.ActualHeight / 2);

            foreach (var slot in timeSlots)
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = center;
                figure.IsClosed = true;

                double startAngle = CalculateAngle(slot.StartTime);
                double endAngle = CalculateAngle(slot.EndTime);
                double sweepAngel = endAngle - startAngle;
                if (sweepAngel < 0) sweepAngel += 360;

                Point startPoint = GetPointOnClock(startAngle, radius, center);
                Point endPoint = GetPointOnClock(endAngle, radius, center);

                figure.Segments.Add(new LineSegment(startPoint, true));
                figure.Segments.Add(new ArcSegment(
                    endPoint, 
                    new Size(radius, radius),
                    0,
                    sweepAngel > 180,
                    SweepDirection.Clockwise,
                    true));
                Path path = new Path();
                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(figure);
                path.Data = geometry;
                try
                {
                    path.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(slot.ColorHex));
                }
                catch
                {
                    path.Fill = Brushes.Gray;
                }
                path.Opacity = 0.25;

                SegmentsCanvas.Children.Add(path);
            }
        }

        public void DrawClockFace()
        {
            HoursCanvas.Children.Clear();

            double baseRadius = Math.Min(HoursCanvas.ActualWidth, HoursCanvas.ActualHeight) / 2 - 20;
            Point center = new Point(HoursCanvas.ActualWidth / 2, HoursCanvas.ActualHeight / 2);

            for (int i = 0; i < 24; i++)
            {
                TimeOnly time = new TimeOnly(i, 0);

                Point startTick = GetPointOnClock(CalculateAngle(time), baseRadius, center);
                Point endTick = GetPointOnClock(CalculateAngle(time), baseRadius - 30, center);

                Line tick = new Line
                {
                    X1 = startTick.X,
                    Y1 = startTick.Y,
                    X2 = endTick.X,
                    Y2 = endTick.Y,
                    Stroke = Brushes.DimGray,
                    StrokeThickness = 4
                };
                HoursCanvas.Children.Add(tick);
            }
        }

        private void StartClock()
        {
            _currentTimeLine = new Line { Stroke = Brushes.Red, StrokeThickness = 3, Opacity = 0.8 };
            _minuteHandLine = new Line { Stroke = Brushes.Blue, StrokeThickness = 2, Opacity = 0.8 };
            _secondHandLine = new Line { Stroke = Brushes.Green, StrokeThickness = 1, Opacity = 0.8 };

            HoursCanvas.Children.Add(_currentTimeLine);
            HoursCanvas.Children.Add(_minuteHandLine);
            HoursCanvas.Children.Add(_secondHandLine);

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => UpdateCurrentTimeLine();
            timer.Start();
        }

        private void UpdateCurrentTimeLine()
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);
            double radius = Math.Min(HoursCanvas.ActualWidth, HoursCanvas.ActualHeight) / 2 - 20;
            Point center = new Point(HoursCanvas.ActualWidth / 2, HoursCanvas.ActualHeight / 2);

            // Hour hand
            Point hourEnd = GetPointOnClock(CalculateAngle(now), radius * 0.7, center);
            _currentTimeLine.X1 = center.X;
            _currentTimeLine.Y1 = center.Y;
            _currentTimeLine.X2 = hourEnd.X;
            _currentTimeLine.Y2 = hourEnd.Y;

            // Minute hand
            Point minuteEnd = GetPointOnClock(CalculateMinuteAngle(now), radius * 0.85, center);
            _minuteHandLine.X1 = center.X;
            _minuteHandLine.Y1 = center.Y;
            _minuteHandLine.X2 = minuteEnd.X;
            _minuteHandLine.Y2 = minuteEnd.Y;

            // Second hand
            Point secondEnd = GetPointOnClock(CalculateSecondAngle(now), radius * 0.95, center);
            _secondHandLine.X1 = center.X;
            _secondHandLine.Y1 = center.Y;
            _secondHandLine.X2 = secondEnd.X;
            _secondHandLine.Y2 = secondEnd.Y;
        }
    }
}