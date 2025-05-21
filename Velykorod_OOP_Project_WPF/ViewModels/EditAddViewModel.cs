using EventTickets;
using System.ComponentModel;

using System.Runtime.CompilerServices;
using System.Windows;

using System.Windows.Input;
using Velykorod_OOP_Project_WPF.View;

namespace Velykorod_OOP_Project_WPF.ViewModel
{
    public class EditAddViewModel : INotifyPropertyChanged
    {
        private readonly Event _originalEvent;
        private readonly Event _backupEvent;
        private readonly bool _isEditMode;
       
        public string Name { get; set; }
        public Genre Genre { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly Time { get; set; }
        public City City { get; set; }
        public string Venue { get; set; }
        public string TicketPrice { get; set; }
        public string AvailableTickets { get; set; }
        public string Illustration { get; set; }

        public Array GenreValues => Enum.GetValues(typeof(Genre));
        public Array CityValues => Enum.GetValues(typeof(City));

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        private bool _shouldClose;
        public bool ShouldClose
        {
            get => _shouldClose;
            set
            {
                _shouldClose = value;
                OnPropertyChanged();
            }
        }
        //Замість DateOnly/TimeOnly використовуємо DateTime для прив'язки
        private DateTime _dateTime;

        public DateTime DateTimeForBinding
        {
            get => _dateTime;
            set
            {
                _dateTime = value;
                OnPropertyChanged();
            }
        }
        private string _timeString;

        public string TimeString
        {
            get => _timeString;
            set
            {
                if (TimeOnly.TryParse(value, out var time))  // Спробувати розпарсити час
                {
                    _timeString = value;
                    _dateTime = _dateTime.Date + time.ToTimeSpan();  // Оновити DateTime
                    OnPropertyChanged(nameof(TimeString));
                    OnPropertyChanged(nameof(DateTimeForBinding));  // Сповістити про зміну
                }
            }
        }
        public EditAddViewModel(Event eventToEdit = null)
        {   
            _isEditMode = eventToEdit != null;
            _originalEvent = eventToEdit;
            // Устанавливаем текущую дату и время по умолчанию
            _dateTime = DateTime.Now;
            if (_isEditMode)
            {
               
                TimeString = _originalEvent.Time.ToString("HH:mm");  // Ініціалізувати час
            }
            else
            {
                TimeString = DateTime.Now.ToString("HH:mm");  // Поточний час за замовчуванням
            }
            if (_isEditMode)
            {
                // Створюємо повну копію оригінальної події
                _backupEvent = new Event(
                    _originalEvent.Name,
                    _originalEvent.Genre,
                    _originalEvent.Date,
                    _originalEvent.Time,
                    _originalEvent.City,
                    _originalEvent.Venue,
                    _originalEvent.TicketPrice,
                    _originalEvent.AvailableTickets,
                    _originalEvent.Illustration);

                // Ініціалізуємо властивості
                Name = _originalEvent.Name;
                Genre = _originalEvent.Genre;
                Date = _originalEvent.Date;
                Time = _originalEvent.Time;
                City = _originalEvent.City;
                Venue = _originalEvent.Venue;
                TicketPrice = _originalEvent.TicketPrice.ToString();
                AvailableTickets = _originalEvent.AvailableTickets.ToString();
                Illustration = _originalEvent.Illustration;
                // Конвертуємо DateOnly+TimeOnly у DateTime для прив'язки
                _dateTime = _originalEvent.Date.ToDateTime(_originalEvent.Time);
            }

            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }
        
       

        private void Save(object parameter)
        {
            try
            { // Конвертируем DateTime обратно в DateOnly и TimeOnly
                DateOnly date = DateOnly.FromDateTime(_dateTime);
                TimeOnly time = TimeOnly.FromDateTime(_dateTime);

                var admin = new Admin();

                if (_isEditMode)
                {
                    admin.EditEvent(_originalEvent, Name, Genre, date, time, City, Venue,
                                   TicketPrice, AvailableTickets, Illustration);
                    EventRepository.UpdateEvent(_originalEvent);
                    MessageBox.Show("Подію успішно відредаговано!", "Успіх",
                          MessageBoxButton.OK, MessageBoxImage.Information);
           
                }
                else
                {
                    admin.AddEvent(Name, Genre, date, time, City, Venue,
                                  TicketPrice, AvailableTickets, Illustration);
                    MessageBox.Show("Подію успішно додано!", "Успіх",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    ShouldClose = true;
                }

                // Закриваємо вікно з результатом true
                Application.Current.Windows.OfType<AEditAdd>().FirstOrDefault()?.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object parameter)
        {
            if (_isEditMode && _originalEvent != null && _backupEvent != null)
            {
                // Відновлюємо всі властивості оригінальної події з backup
                _originalEvent.Name = _backupEvent.Name;
                _originalEvent.Genre = _backupEvent.Genre;
                _originalEvent.Date = _backupEvent.Date;
                _originalEvent.Time = _backupEvent.Time;
                _originalEvent.City = _backupEvent.City;
                _originalEvent.Venue = _backupEvent.Venue;
                _originalEvent.TicketPrice = _backupEvent.TicketPrice;
                _originalEvent.AvailableTickets = _backupEvent.AvailableTickets;
                _originalEvent.Illustration = _backupEvent.Illustration;
            }

            // Закриваємо вікно
            var window = parameter as Window;
            if (window != null)
            {
                window.DialogResult = false; // Вказуємо, що зміни скасовано
                window.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
       
        
    }
    
}