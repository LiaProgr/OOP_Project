using System.Windows;
using System.Windows.Input;
using EventTickets;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Velykorod_OOP_Project_WPF.View;
using System.Windows.Threading;

namespace Velykorod_OOP_Project_WPF.ViewModel
{
    public class RUCartViewModel : INotifyPropertyChanged
    {

        private readonly RegisteredUser _currentUser;

        public ObservableCollection<Ticket> CartItems { get; } = new ObservableCollection<Ticket>();

        public decimal TotalAmount => CartItems.Sum(t => t.TotalPrice);

        public ICommand BackCommand { get; }
        public ICommand ClearCartCommand { get; }
        public ICommand CheckoutCommand { get; }
        private DispatcherTimer _autoClearTimer;


        public RUCartViewModel(RegisteredUser? currentUser)
        {
            _currentUser = currentUser;
            // Очищаємо колекцію перед завантаженням
            CartItems.Clear();

            if (_currentUser != null)
            {
                // Підписка на зміни в кошику
                _currentUser.Cart.CartUpdated += OnCartUpdated;
                LoadCartItems();
            }

            BackCommand = new RelayCommand(Back);
            ClearCartCommand = new RelayCommand(ClearCart);
            CheckoutCommand = new RelayCommand(Checkout, CanCheckout);
            _autoClearTimer = new DispatcherTimer();
            _autoClearTimer.Interval = TimeSpan.FromMinutes(15);
            _autoClearTimer.Tick += AutoClearCart;
            _autoClearTimer.Start();
        }
        private void OnCartUpdated(object sender, EventArgs e)
        {
            LoadCartItems();
            OnPropertyChanged(nameof(TotalAmount));
        }

        private void LoadCartItems()
        {
            CartItems.Clear();
            foreach (var ticket in _currentUser.Cart.GetEventsInCart())
            {
                CartItems.Add(ticket);
            }
        }
        private void AutoClearCart(object sender, EventArgs e)
        {
            ClearCart(null);
            _autoClearTimer.Stop();
            foreach (var ticket in CartItems)
            {
                ticket.Event.Release(ticket.Quantity);
            }
            //MessageBox.Show("Кошик автоматично очищено через неактивність");
        }

        // Сбрасываем таймер при любом действии
        public void ResetAutoClearTimer()
        {
            _autoClearTimer.Stop();
            _autoClearTimer.Start();
        }

        private void Back(object parameter)
        {
            var mainWindow = new RUPanelNavi(_currentUser);
            mainWindow.Show();
            Application.Current.Windows.OfType<RUCart>().FirstOrDefault()?.Close();
        }


        public void ClearCart(object parameter)
        {
            if (CartItems.Count == 0)
            {
                MessageBox.Show("Кошик пустий!");
            }
            else
            {
                foreach (var ticket in CartItems)
                {
                    ticket.Event.Release(ticket.Quantity);
                }
                _currentUser.Cart.ClearCart();
                _currentUser.Cart.SaveCart();
                CartItems.Clear();

                OnPropertyChanged(nameof(TotalAmount));
 
                MessageBox.Show("Кошик очищено. Квитки повернено!");
            }
        }

        private void Checkout(object parameter)
        {
           
            var paymentWindow = new RUPay(_currentUser, this);
            paymentWindow.Show();
            Application.Current.Windows.OfType<RUCart>().FirstOrDefault()?.Close();
        }

        private bool CanCheckout(object parameter) => CartItems.Any();

        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}