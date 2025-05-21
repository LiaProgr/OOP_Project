using EventTickets;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using Velykorod_OOP_Project_WPF.View;
using Velykorod_OOP_Project_WPF.ViewModel;

namespace Velykorod_OOP_Project_WPF.ViewModels
{
    public class EventManagementViewModel : INotifyPropertyChanged
    {
        private Event _selectedEvent;

        public ObservableCollection<Event> Events { get; } = new ObservableCollection<Event>();

        public Event SelectedEvent
        {
            get => _selectedEvent;
            set
            {
                _selectedEvent = value;
                OnPropertyChanged();
            }
        }

        public ICommand BackCommand { get; }
        public ICommand AddEventCommand { get; }
        public ICommand EditEventCommand { get; }
        public ICommand DeleteEventCommand { get; }
  

        public EventManagementViewModel()
        {
            LoadEvents();

            BackCommand = new RelayCommand(Back);
            AddEventCommand = new RelayCommand(AddEvent);
            EditEventCommand = new RelayCommand(EditEvent, CanEditOrDelete);
            DeleteEventCommand = new RelayCommand(DeleteEvent, CanEditOrDelete);
        }

        private void LoadEvents()
        {
            Events.Clear();
            var sortedEvents = EventRepository.Events.OrderBy(e => e).ToList(); // Викликає CompareTo
            foreach (var e in sortedEvents)
            {
                Events.Add(e);
            }
        }
 
        private bool CanEditOrDelete(object parameter) => SelectedEvent != null;

        private void Back(object parameter)
        {
            var adminWindow = new PanelAdm();
            adminWindow.Show();
            Application.Current.Windows.OfType<AEventManagement>().FirstOrDefault()?.Close();
        }

        private void AddEvent(object parameter)
        {
            var addEditWindow = new AEditAdd();
            if (addEditWindow.ShowDialog() == true)
            {
                LoadEvents(); // Оновлюємо список після додавання
            }
        }

        private void EditEvent(object parameter)
        {
            if (SelectedEvent != null)
            {
                var addEditWindow = new AEditAdd(SelectedEvent);
                if (addEditWindow.ShowDialog() == true)
                {
                    LoadEvents();
                }
            }
        }

        private void DeleteEvent(object parameter)
        {
            if (SelectedEvent != null &&
                MessageBox.Show("Ви впевнені, що хочете видалити подію?",
                              "Підтвердження",
                              MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    // Використовуємо ваш існуючий метод з класу Admin
                    new Admin().RemoveEvent(SelectedEvent);
                    Events.Remove(SelectedEvent);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
