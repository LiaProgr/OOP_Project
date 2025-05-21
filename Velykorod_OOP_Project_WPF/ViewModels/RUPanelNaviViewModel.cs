
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using EventTickets;
using Velykorod_OOP_Project_WPF.View;


namespace Velykorod_OOP_Project_WPF.ViewModel
{
    public class RUPanelNaviViewModel : INotifyPropertyChanged
    {
        private string _searchText;
        private City? _selectedCity;
        private Genre? _selectedGenre;
        private Event _selectedEvent;

        public ObservableCollection<Event> Events { get; } = new ObservableCollection<Event>();
        public ObservableCollection<Event> FilteredEvents { get; } = new ObservableCollection<Event>();

        public ICommand ExitCommand { get; }
        public ICommand OpenCartCommand { get; }
        public ICommand OpenPersonalCabinetCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ShowDetailsCommand { get; }
        public ICommand SaveToJsonCommand { get; }
        public IEnumerable<City> CityValues => Enum.GetValues(typeof(City)).Cast<City>();
        public IEnumerable<Genre> GenreValues => Enum.GetValues(typeof(Genre)).Cast<Genre>();
        private RegisteredUser _currentUser;

        public RegisteredUser CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                OnPropertyChanged();
            }
        }
     
        // Властивості для прив'язки
        public string SearchText
        {
            get => _searchText;
            set { _searchText = value; OnPropertyChanged(); }
        }

        public City? SelectedCity
        {
            get => _selectedCity;
            set { _selectedCity = value; OnPropertyChanged(); }
        }

        public Genre? SelectedGenre
        {
            get => _selectedGenre;
            set { _selectedGenre = value; OnPropertyChanged(); }
        }

        public Event SelectedEvent
        {
            get => _selectedEvent;
            set { _selectedEvent = value; OnPropertyChanged(); }
        }

        public RUPanelNaviViewModel(RegisteredUser? currentUser)
        {
            LoadEvents();
            _currentUser = currentUser;
            ExitCommand = new RelayCommand(Exit);
            OpenCartCommand = new RelayCommand(OpenCart);
            OpenPersonalCabinetCommand = new RelayCommand(OpenPersonalCabinet);
            ClearFiltersCommand = new RelayCommand(ClearFilters);
            SearchCommand = new RelayCommand(Search);
            ShowDetailsCommand = new RelayCommand(ShowDetails, CanShowDetails);
            SaveToJsonCommand = new RelayCommand(SaveToJson);
        }

      
        private void LoadEvents()
        {
            Events.Clear();
            foreach (var e in EventRepository.Events)
            {
                Events.Add(e);
            }
            ApplyFilters();
        }

        private void Exit(object parameter)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            _currentUser.Cart.SaveCart();
            UserRepository.SaveCartBeforeLogout();
            Application.Current.Windows.OfType<RUPanelNavi>().FirstOrDefault()?.Close();
        }

        private void OpenCart(object parameter)
        {
            var cartWindow = new RUCart(_currentUser);
            cartWindow.Show();
            Application.Current.Windows.OfType<RUPanelNavi>().FirstOrDefault()?.Close();
        }

        private void OpenPersonalCabinet(object parameter)
        {
            var personalCabinetWindow = new RUMyTickets(_currentUser);
            personalCabinetWindow.Show();
            Application.Current.Windows.OfType<RUPanelNavi>().FirstOrDefault()?.Close();
        }
        private void ClearFilters(object parameter)
        {
            SearchText = string.Empty;
            SelectedCity = null;
            SelectedGenre = null;
            ApplyFilters();
        }

        private void Search(object parameter)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            FilteredEvents.Clear();

            var filtered = EventRepository.Events.AsEnumerable();

            // Фільтрація за текстом (якщо є)
            if (!string.IsNullOrEmpty(SearchText))
            {
                filtered = filtered.Where(e =>
                    e.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    e.Venue.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Сувора фільтрація "І" для міста та жанру
            if (SelectedCity != null && SelectedGenre != null)
            {
                filtered = filtered.Where(e =>
                    e.City == SelectedCity &&
                    e.Genre == SelectedGenre);
            }
            else if (SelectedCity != null) // Тільки місто
            {
                filtered = filtered.Where(e => e.City == SelectedCity);
            }
            else if (SelectedGenre != null) // Тільки жанр
            {
                filtered = filtered.Where(e => e.Genre == SelectedGenre);
            }

            // Додаємо результати
            foreach (var e in filtered)
            {
                FilteredEvents.Add(e);
            }
          
        }

        private bool CanShowDetails(object parameter) => SelectedEvent != null;

        private void ShowDetails(object parameter)
        {
            if (SelectedEvent != null && CurrentUser != null)
            {
                var originalEvent = EventRepository.Events.FirstOrDefault(e => e.Name == SelectedEvent.Name);
                
                var detailsWindow = new RUViewDetails(SelectedEvent, CurrentUser); // Передаємо обидва параметри
                detailsWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Будь ласка, увійдіть в систему");
            }
        }

        private void SaveToJson(object parameter)
        {
            try
            {
                var registeredUser = new RegisteredUser();
                registeredUser.SaveEventsToJson(FilteredEvents.ToList(), "events.json");
                MessageBox.Show("Події збережено у файл events.json", "Успіх",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при збереженні: {ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
