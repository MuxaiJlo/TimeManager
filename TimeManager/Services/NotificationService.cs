using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Security;
using System.Windows.Threading;
using TimeManager.Model;

namespace TimeManager.Services
{
    internal class NotificationService
{
    private ObservableCollection<TimeSlot> _timeSlots;
    private DispatcherTimer _timer;
    private HashSet<string> _sentNotifications = new HashSet<string>();

    public NotificationService(ObservableCollection<TimeSlot> timeSlots)
    {
        _timeSlots = timeSlots;
        
        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromSeconds(5); // Проверяем каждые 5 секунд
        _timer.Tick += Timer_Tick;
        _timer.Start();
        
        Debug.WriteLine($"✓ NotificationService запущен. Текущее время: {DateTime.Now:HH:mm:ss}");
        Debug.WriteLine($"✓ Таймер настроен с интервалом {_timer.Interval.TotalSeconds} сек");
        
        // Первая проверка сразу
        CheckAllSlots();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        CheckAllSlots();
    }

    private void CheckAllSlots()
    {
        var now = DateTime.Now;
        Debug.WriteLine($"");
        Debug.WriteLine($"========== ПРОВЕРКА В {now:HH:mm:ss} ==========");
        Debug.WriteLine($"Количество слотов: {_timeSlots.Count}");
        
        if (_timeSlots.Count == 0)
        {
            Debug.WriteLine("⚠ Нет слотов для проверки!");
            return;
        }
        
        foreach (var slot in _timeSlots)
        {
            Debug.WriteLine($"");
            Debug.WriteLine($"Слот: '{slot.ProcessName}'");
            Debug.WriteLine($"  Start: {slot.StartTime:HH:mm:ss}");
            Debug.WriteLine($"  End: {slot.EndTime:HH:mm:ss}");
            Debug.WriteLine($"  Duration: {slot.Duration.TotalMinutes:F1} мин");
            
            CheckStartTime(slot, now);
            CheckMidTime(slot, now);
            CheckEndTime(slot, now);
        }
        
        Debug.WriteLine($"========================================");
    }

    private void CheckStartTime(TimeSlot slot, DateTime now)
    {
        var startDateTime = DateTime.Today.Add(slot.StartTime.ToTimeSpan());
        var diff = (now - startDateTime).TotalSeconds;
        var notificationKey = $"start_{slot.ProcessName}_{startDateTime:yyyyMMddHHmm}";
        
        Debug.WriteLine($"  START проверка: разница {diff:F0} сек");
        
        if (!_sentNotifications.Contains(notificationKey) && Math.Abs(diff) <= 30)
        {
            _sentNotifications.Add(notificationKey);
            SendToast("🚀 Начало задачи", $"Началась задача: {slot.ProcessName}");
            Debug.WriteLine($"  ✓✓✓ START уведомление отправлено!");
        }
    }

    private void CheckMidTime(TimeSlot slot, DateTime now)
    {
        if (slot.Duration.TotalMinutes < 2)
        {
            Debug.WriteLine($"  MID пропущен (слишком короткая задача)");
            return;
        }

        var startDateTime = DateTime.Today.Add(slot.StartTime.ToTimeSpan());
        var midDateTime = startDateTime.Add(TimeSpan.FromMinutes(slot.Duration.TotalMinutes / 2));
        var diff = (now - midDateTime).TotalSeconds;
        var notificationKey = $"mid_{slot.ProcessName}_{midDateTime:yyyyMMddHHmm}";

        Debug.WriteLine($"  MID проверка: разница {diff:F0} сек");

        if (!_sentNotifications.Contains(notificationKey) && Math.Abs(diff) <= 30)
        {
            _sentNotifications.Add(notificationKey);
            SendToast("⏱ Середина задачи", $"Половина времени для: {slot.ProcessName}");
            Debug.WriteLine($"  ✓✓✓ MID уведомление отправлено!");
        }
    }

    private void CheckEndTime(TimeSlot slot, DateTime now)
    {
        var endDateTime = DateTime.Today.Add(slot.EndTime.ToTimeSpan());
        var diff = (now - endDateTime).TotalSeconds;
        var notificationKey = $"end_{slot.ProcessName}_{endDateTime:yyyyMMddHHmm}";

        Debug.WriteLine($"  END проверка: разница {diff:F0} сек");

        if (!_sentNotifications.Contains(notificationKey) && Math.Abs(diff) <= 30)
        {
            _sentNotifications.Add(notificationKey);
            SendToast("✅ Конец задачи", $"Закончилась задача: {slot.ProcessName}");
            Debug.WriteLine($"  ✓✓✓ END уведомление отправлено!");
        }
    }

    private void SendToast(string title, string message)
    {
        try
        {
            Debug.WriteLine($"");
            Debug.WriteLine($">>> Отправка Toast: {title} - {message}");
            
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

            var doc = new Windows.Data.Xml.Dom.XmlDocument();
            doc.LoadXml(toastXml);
            
            var toast = new Windows.UI.Notifications.ToastNotification(doc);
            Windows.UI.Notifications.ToastNotificationManager
                .CreateToastNotifier("TimeManager.App").Show(toast);
            
            Debug.WriteLine($">>> ✓ Toast отправлен успешно!");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($">>> ✗✗✗ ОШИБКА Toast: {ex.Message}");
            Debug.WriteLine($">>> StackTrace: {ex.StackTrace}");
        }
    }
}
}