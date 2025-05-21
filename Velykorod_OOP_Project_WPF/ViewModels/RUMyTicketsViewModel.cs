using EventTickets;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Velykorod_OOP_Project_WPF.View;
using Velykorod_OOP_Project_WPF.ViewModel;
using System.Windows;
public class RUMyTicketsViewModel : INotifyPropertyChanged
{
    private readonly RegisteredUser _currentUser;
    public ObservableCollection<Ticket> PurchasedTickets { get; } = new ObservableCollection<Ticket>();

    public ICommand BackCommand { get; }
    public ICommand ClearListCommand { get; }
    public ICommand GeneratePdfCommand { get; }

    public RUMyTicketsViewModel(RegisteredUser currentUser)
    {
        _currentUser = currentUser;
        _currentUser.LoadPurchasedTickets();
        PurchasedTickets.Clear();
        // Завантаження куплених квитків
        foreach (var ticket in _currentUser.PurchasedTickets)
        {
            PurchasedTickets.Add(ticket);
        }

        BackCommand = new RelayCommand(Back);
        ClearListCommand = new RelayCommand(ClearList);
        GeneratePdfCommand = new RelayCommand(GeneratePdf, CanGeneratePdf);
    }

    private void Back(object parameter)
    {
        var mainWindow = new RUPanelNavi(_currentUser);
        mainWindow.Show();
        Application.Current.Windows.OfType<RUMyTickets>().FirstOrDefault()?.Close();
    }

    private void ClearList(object parameter)
    {
        if (PurchasedTickets.Count == 0)
        {
            MessageBox.Show("Список квитків порожній!");
            return;
        }

        _currentUser.PurchasedTickets.Clear();
        PurchasedTickets.Clear();
        MessageBox.Show("Список квитків очищено!");
    }

    private void GeneratePdf(object parameter)
    {
        if (parameter is Ticket selectedTicket)
        {
            try
            {
                _currentUser.DownloadTicketPDF(selectedTicket);
                MessageBox.Show("PDF-квиток згенеровано!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }
    }

    private bool CanGeneratePdf(object parameter) => parameter is Ticket;

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}