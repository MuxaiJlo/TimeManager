using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimeManager.Services;
using TimeManager.View;
using TimeManager.ViewModel;

namespace TimeManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TrayService _trayService;

        public MainWindow()
        {
            InitializeComponent(); 
            var vm = new MainViewModel();
            this.DataContext = vm;
            _trayService = new TrayService(this);
            ClockControl.SubscribeToData(vm.Slots);
        }
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.Hide(); // Прячем окно
            }
            base.OnStateChanged(e);
        }

        // Правильное закрытие
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Если хотите, чтобы закрытие окна просто прятало его в трей:
            e.Cancel = true;
            this.Hide();

            // Если хотите полностью закрывать приложение:
            //_trayService?.Dispose();
            //Application.Current.Shutdown();

            base.OnClosing(e);
        }

        private void TestNotification_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string toastXml = @"
            <toast>
                <visual>
                    <binding template='ToastGeneric'>
                        <text>Тестовое уведомление</text>
                        <text>Если вы видите это, уведомления работают!</text>
                    </binding>
                </visual>
                <audio src='ms-winsoundevent:Notification.Default'/>
            </toast>";

                var doc = new Windows.Data.Xml.Dom.XmlDocument();
                doc.LoadXml(toastXml);
                var toast = new Windows.UI.Notifications.ToastNotification(doc);
                Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier("TimeManager.App").Show(toast);

                MessageBox.Show("Уведомление отправлено!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}