using EventTickets;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Velykorod_OOP_Project_WPF.View;
using Velykorod_OOP_Project_WPF.ViewModel;
using System.Windows;
using Application = System.Windows.Application;

public class RUPayViewModel : INotifyPropertyChanged
{
    private readonly RegisteredUser _currentUser;
    private readonly RUCartViewModel _cartViewModel;

    public PaymentDetails PaymentDetails { get; private set; }

    public ICommand PayCommand { get; }
    public ICommand BackCommand { get; }

    private string _cardNumber;
    public string CardNumber
    {
        get => _cardNumber;
        set { _cardNumber = value; OnPropertyChanged(); }
    }

    private string _cardHolderName;
    public string CardHolderName
    {
        get => _cardHolderName;
        set { _cardHolderName = value; OnPropertyChanged(); }
    }

    private string _expiryDate;
    public string ExpiryDate
    {
        get => _expiryDate;
        set { _expiryDate = value; OnPropertyChanged(); }
    }

    private string _cvv;
    public string CVV
    {
        get => _cvv;
        set { _cvv = value; OnPropertyChanged(); }
    }

    public RUPayViewModel(RegisteredUser currentUser, RUCartViewModel cartViewModel)
    {
        _currentUser = currentUser;
        _cartViewModel = cartViewModel;

        PayCommand = new RelayCommand(Pay);
        BackCommand = new RelayCommand(Back);
    }
        private void Pay(object parameter)
        {
            try
            {
                // 1. Перевірка ініціалізації кошика
                if (_cartViewModel == null)
                    throw new InvalidOperationException("Кошик не ініціалізовано.");

                if (_cartViewModel.CartItems == null || !_cartViewModel.CartItems.Any())
                    throw new InvalidOperationException("Кошик порожній.");

                // 2. Фільтрація невалідних квитків
                var validTickets = _cartViewModel.CartItems
                    .Where(t => t != null
                        && t.Event != null
                        && t.Event.TicketPrice > 0)
                    .ToList();

                if (!validTickets.Any())
                    throw new InvalidOperationException("У кошику немає валідних квитків.");

                // 3. Розрахунок суми з валідними квитками
                decimal totalAmount = validTickets
                    .Sum(t => t.Quantity * t.Event.TicketPrice);

                // 4. Валідація платіжних даних
                ValidatePaymentFields();

                // 5. Створення платіжного об'єкта
                var payment = new PaymentDetails(
                    CardNumber.Trim(),
                    CardHolderName.Trim(),
                    ExpiryDate.Trim(),
                    CVV.Trim()
                );

                // 6. Симуляція оплати
                if (!PaymentProcessor.ProcessPayment(payment, totalAmount))
                    throw new Exception("Операція відхилена платіжним шлюзом");

                // 7. Обробка успішної оплати
                ProcessSuccessfulPayment(validTickets);

                MessageBox.Show("Оплата успішна! Квитки додано до вашого профілю.",
                              "Успіх",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);

                // 8. Навігація назад
                Back(null);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Помилка введення",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка оплати: {ex.Message}", "Помилка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessSuccessfulPayment(List<Ticket> validTickets)
        {
            // Оновлення даних подій
            foreach (var ticket in validTickets)
            {
                ticket.Event.AvailableTickets -= ticket.Quantity;
                ticket.Event.ReservedTickets -= ticket.Quantity;
                EventRepository.UpdateEvent(ticket.Event);
            }

            // Очищення кошика
            _cartViewModel.CartItems.Clear();
            _currentUser.Cart.ClearCart();
            _currentUser.Cart.SaveCart();

            // Оновлення UI
            _cartViewModel.OnPropertyChanged(nameof(_cartViewModel.CartItems));
            EventRepository.SaveEvents();
         _currentUser.PurchasedTickets.AddRange(validTickets);
         _currentUser.SavePurchasedTickets(); // Зберегти зміни
    }

    private void ValidatePaymentFields()
    {
        if (string.IsNullOrWhiteSpace(CardNumber) ||
            string.IsNullOrWhiteSpace(CardHolderName) ||
            string.IsNullOrWhiteSpace(ExpiryDate) ||
            string.IsNullOrWhiteSpace(CVV))
        {
            throw new ArgumentException("Заповніть всі обов'язкові поля!");
        }
    }

    private void Back(object parameter)
    {
        var Window = new RUCart(_currentUser);
       Window.Show();
        Application.Current.Windows.OfType<RUPay>().FirstOrDefault()?.Close();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}