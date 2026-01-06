using System;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace TimeManager.View
{
    /// <summary>
    /// Interaction logic for ClockControl.xaml
    /// </summary>
    public partial class ClockControl : UserControl
    {
        private Line _currentTimeLine;

        public ClockControl()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                var testSlots = new ObservableCollection<TimeSlot>
                {
                    new TimeSlot {
                        ProcessName = "Test",
                        StartTime = new TimeOnly(12, 0),
                        EndTime = new TimeOnly(15, 0),
                        ColorHex = "#FF0000"
                    },
                    new TimeSlot {
                        ProcessName = "Test",
                        StartTime = new TimeOnly(15, 0),
                        EndTime = new TimeOnly(18, 0),
                        ColorHex = "#AA0000"
                    },
                    new TimeSlot {
                        ProcessName = "Test",
                        StartTime = new TimeOnly(18, 0),
                        EndTime = new TimeOnly(20, 0),
                        ColorHex = "#BB1234"
                    }
                };
                DrawClockFace();
                StartClock();
                UpdateDashboard(testSlots);
            };
        }
        private double CalculateAngle(TimeOnly time)
        {
            double hours = time.Hour + (time.Minute / 60.0);
            double degrees = hours * 30;
            return degrees;
        }
        private Point GetPointOnClock(TimeOnly time, double radius, Point center)
        {
            double degrees = CalculateAngle(time);

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

                Point startPoint = GetPointOnClock(slot.StartTime, radius, center);
                Point endPoint = GetPointOnClock(slot.EndTime, radius, center);

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
                path.Fill = (Brush)new BrushConverter().ConvertFromString(slot.ColorHex); // Красим!
                path.Opacity = 0.7;

                SegmentsCanvas.Children.Add(path);
            }
        }
        public void DrawClockFace()
        {
            HoursCanvas.Children.Clear();

            // Берем радиус чуть меньше фактического, чтобы все влезло
            double baseRadius = Math.Min(HoursCanvas.ActualWidth, HoursCanvas.ActualHeight) / 2 - 20;
            Point center = new Point(HoursCanvas.ActualWidth / 2, HoursCanvas.ActualHeight / 2);

            for (int i = 0; i < 24; i++)
            {
                TimeOnly time = new TimeOnly(i, 0);

                // 1. Рисуем черточку (Tick)
                // Она идет от внешнего радиуса чуть-чуть внутрь
                Point startTick = GetPointOnClock(time, baseRadius, center);
                Point endTick = GetPointOnClock(time, baseRadius - 10, center);

                Line tick = new Line
                {
                    X1 = startTick.X,
                    Y1 = startTick.Y,
                    X2 = endTick.X,
                    Y2 = endTick.Y,
                    Stroke = Brushes.DimGray,
                    StrokeThickness = 2
                };
                HoursCanvas.Children.Add(tick);

                // 2. Рисуем текст (цифру) еще чуть дальше от центра
                Point textPos = GetPointOnClock(time, baseRadius + 15, center);
                TextBlock label = new TextBlock
                {
                    Text = i.ToString(),
                    Foreground = Brushes.Silver,
                    FontSize = 10
                };

                // Маленький хак для точного центрирования текста
                label.Loaded += (s, e) => {
                    Canvas.SetLeft(label, textPos.X - label.ActualWidth / 2);
                    Canvas.SetTop(label, textPos.Y - label.ActualHeight / 2);
                };

                HoursCanvas.Children.Add(label);
            }
        }
        private void StartClock()
        {
            _currentTimeLine = new Line { Stroke = Brushes.Red, StrokeThickness = 3, Opacity = 0.8 };
            HoursCanvas.Children.Add(_currentTimeLine);

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // Обновляем раз в секунду
            timer.Tick += (s, e) => UpdateCurrentTimeLine();
            timer.Start();
        }

        private void UpdateCurrentTimeLine()
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);
            double radius = Math.Min(HoursCanvas.ActualWidth, HoursCanvas.ActualHeight) / 2 - 20;
            Point center = new Point(HoursCanvas.ActualWidth / 2, HoursCanvas.ActualHeight / 2);

            Point endPoint = GetPointOnClock(now, radius, center);

            _currentTimeLine.X1 = center.X;
            _currentTimeLine.Y1 = center.Y;
            _currentTimeLine.X2 = endPoint.X;
            _currentTimeLine.Y2 = endPoint.Y;
        }
    }
}
