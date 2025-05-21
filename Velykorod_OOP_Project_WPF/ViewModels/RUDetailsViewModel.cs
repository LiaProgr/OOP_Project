using EventTickets;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using Velykorod_OOP_Project_WPF.ViewModel;
using Velykorod_OOP_Project_WPF.View;

public class RUDetailsViewModel : INotifyPropertyChanged
{
    private readonly RegisteredUser _currentUser;
    private readonly Event _selectedEvent;

    public Event SelectedEvent
    {
        get => _selectedEvent;
        init
        {
            _selectedEvent = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FormattedDateTime));
            OnPropertyChanged(nameof(FormattedCityVenue));
            OnPropertyChanged(nameof(FormattedPrice));
        }
    }

    public string FormattedDateTime =>
        $"{SelectedEvent.Date:dd.MM.yyyy}, {SelectedEvent.Time:HH:mm}";

    public string FormattedCityVenue =>
        $"{SelectedEvent.City}, {SelectedEvent.Venue}";

    public string FormattedPrice =>
        $"{SelectedEvent.TicketPrice} грн";

    public int AvailableTickets => SelectedEvent.AvailableTickets /*- SelectedEvent.ReservedTickets*/;

    private int _ticketsToBuy;
    public int TicketsToBuy
    {
        get => _ticketsToBuy;
        set
        {
            _ticketsToBuy = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddToCartCommand { get; }
    public ICommand BackCommand { get; }

    public RUDetailsViewModel(Event selectedEvent, RegisteredUser currentUser)
    {
        SelectedEvent = selectedEvent ?? throw new ArgumentNullException(nameof(selectedEvent));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

        AddToCartCommand = new RelayCommand(AddToCart);
        BackCommand = new RelayCommand(Back);
    }
    private void AddToCart(object parameter)
    {
        try
        {
            _currentUser.AddToCart(SelectedEvent, TicketsToBuy);
            MessageBox.Show($"Додано {TicketsToBuy} квитків.");
           // OnPropertyChanged(nameof(AvailableTickets)); // Оновлення UI
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    private void Back(object parameter)
    {
       
        Application.Current.Windows.OfType<RUViewDetails>().FirstOrDefault()?.Close();
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}