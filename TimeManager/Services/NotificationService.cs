using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using TimeManager.Model;

namespace TimeManager.Services
{
    internal class NotificationService
    {
        private ObservableCollection<TimeSlot> _timeSlots;
        private DispatcherTimer _timer;
        private const string APP_ID = "TimeManager.App";

        public NotificationService(ObservableCollection<TimeSlot> timeSlots)
        {
            _timeSlots = timeSlots;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) => CheckAllSlots();
            _timer.Start();

            Debug.WriteLine($"NotificationService запущен. Текущее время: {DateTime.Now:HH:mm:ss}");
        }

        private void CheckAllSlots()
        {
            var now = TimeOnly.FromDateTime(DateTime.Now);

            foreach (var slot in _timeSlots)
            {
                CheckStartTime(slot, now);
                CheckMidTime(slot, now);
                CheckEndTime(slot, now);
            }
        }

        private void CheckStartTime(TimeSlot slot, TimeOnly now)
        {
            // Проверяем что время совпадает с точностью до минуты
            if (!slot.StartNotified &&
                now.Hour == slot.StartTime.Hour &&
                now.Minute == slot.StartTime.Minute)
            {
                slot.StartNotified = true;
                SendToast("Начало задачи", $"Началась задача: {slot.ProcessName}");
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] Start notification sent for {slot.ProcessName}");
            }
        }

        private void CheckMidTime(TimeSlot slot, TimeOnly now)
        {
            if (slot.Duration.TotalMinutes >= 2 && !slot.MidNotified)
            {
                var midTime = slot.StartTime.Add(TimeSpan.FromMinutes(slot.Duration.TotalMinutes / 2));

                // Проверяем совпадение часа и минуты (без секунд!)
                if (now.Hour == midTime.Hour && now.Minute == midTime.Minute)
                {
                    slot.MidNotified = true;
                    SendToast("Середина задачи", $"Половина времени для: {slot.ProcessName}");
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] Mid notification sent for {slot.ProcessName}");
                }
            }
        }

        private void CheckEndTime(TimeSlot slot, TimeOnly now)
        {
            // Проверяем что время совпадает с точностью до минуты
            if (!slot.EndNotified &&
                now.Hour == slot.EndTime.Hour &&
                now.Minute == slot.EndTime.Minute)
            {
                slot.EndNotified = true;
                SendToast("Конец задачи", $"Закончилась задача: {slot.ProcessName}");
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] End notification sent for {slot.ProcessName}");
            }
        }

        private void SendToast(string title, string message)
        {
            try
            {
                Debug.WriteLine($"Попытка отправить уведомление: {title} - {message}");

                string toastXml = $@"
                    <toast>
                        <visual>
                            <binding template='ToastGeneric'>
                                <text>{System.Security.SecurityElement.Escape(title)}</text>
                                <text>{System.Security.SecurityElement.Escape(message)}</text>
                            </binding>
                        </visual>
                        <audio src='ms-winsoundevent:Notification.Default'/>
                    </toast>";

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(toastXml);

                ToastNotification toast = new ToastNotification(doc);
                ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);

                Debug.WriteLine("Уведомление успешно отправлено!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ОШИБКА отправки уведомления: {ex.Message}");
                Debug.WriteLine($"StackTrace: {ex.StackTrace}");
            }
        }
    }
}