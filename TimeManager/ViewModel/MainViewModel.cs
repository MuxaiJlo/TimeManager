using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TimeManager.Model;

namespace TimeManager.ViewModel
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TimeSlot> Slots { get; set; } = new();
        public ICommand AddProcessCommand { get; set; }

        public ObservableCollection<string> PresetColors { get; } = new()
        {
            "#FF5733", "#33FF57", "#3357FF", "#F333FF", "#FF33A8", "#33FFF3", "#FFC300", "#581845"
        };
        private bool _isFormVisible;
        public bool IsFormVisible
        {
            get => _isFormVisible;
            set { _isFormVisible = value; OnPropertyChanged(); }
        }
        public string NewTitle { get; set; }
        public string NewStart { get; set; }
        public string NewEnd { get; set; }
        public string NewColor { get; set; }

        public ICommand ShowFormCommand { get; }
        public ICommand AddTaskCommand { get; }
        public ICommand SelectColorCommand { get; }
        public ICommand DeleteTaskForm { get; }

        public MainViewModel()
        {
            ShowFormCommand = new RelayCommand(_ => IsFormVisible = !IsFormVisible);
            AddTaskCommand = new RelayCommand(_ => AddNewTask());
            SelectColorCommand = new RelayCommand(color =>
            {
                NewColor = color.ToString();
                OnPropertyChanged(nameof(NewColor));
            });
            DeleteTaskForm = new RelayCommand(taskObj =>
            {
                if (taskObj is TimeSlot task && Slots.Contains(task))
                {
                    Slots.Remove(task);
                    OnPropertyChanged(nameof(Slots));
                }
            });
        }

        
        private void AddNewTask()
        {
            if (TimeOnly.TryParse(NewStart, out TimeOnly startTime) && TimeOnly.TryParse(NewEnd, out TimeOnly endTime))
            {
                var newSlot = new TimeSlot
                {
                    ProcessName = NewTitle,
                    StartTime = startTime,
                    EndTime = endTime,
                    ColorHex = (NewColor?.StartsWith("#") == true) ? NewColor : "#808080"
                };
                Slots.Add(newSlot);
                var sortedSlots = Slots.OrderBy(s => s.StartTime).ToList();
                Slots.Clear();
                foreach (var slot in sortedSlots)
                    Slots.Add(slot);

                // Clear input fields
                NewTitle = string.Empty;
                NewStart = string.Empty;
                NewEnd = string.Empty;
                NewColor = string.Empty;
                OnPropertyChanged(nameof(NewTitle));
                OnPropertyChanged(nameof(NewStart));
                OnPropertyChanged(nameof(NewEnd));
                OnPropertyChanged(nameof(NewColor));
                IsFormVisible = false; // Hide form after adding
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите время в формате ЧЧ:ММ (например, 14:30)");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
