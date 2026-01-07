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
using TimeManager.View;
using TimeManager.ViewModel;

namespace TimeManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent(); // Эта команда "оживляет" XAML и находит MyClock

            var vm = new MainViewModel();
            this.DataContext = vm;

            // Используем MyClock, который уже есть на форме (из XAML)
            ClockControl.SubscribeToData(vm.Slots);
        }
    }
}