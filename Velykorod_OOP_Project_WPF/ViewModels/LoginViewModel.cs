using System.Windows;
using System.ComponentModel;
using EventTickets;
using Velykorod_OOP_Project_WPF.View;

namespace Velykorod_OOP_Project_WPF.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _username;
        private string _password;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public RelayCommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(LoginExecute, CanLoginExecute);
        }

        private bool CanLoginExecute(object parameter) =>
            !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        private void LoginExecute(object parameter)
        {
            try
            {
                var user = UserRepository.GetUserByCredentials(Username, Password);

                if (user == null)
                {
                    MessageBox.Show("Невірний логін або пароль", "Помилка авторизації",
                                  MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                if (user is Admin)
                {
                    // Відкрити вікно адміністратора
                    var adminWindow = new PanelAdm();
                    adminWindow.Show();
                }
                else if (user is RegisteredUser)
                {
                    // Приклад (у вашому головному вікні або сервісі):
                    var currentUser = UserRepository.GetUserByCredentials (Username, Password) as RegisteredUser;
                    var viewModel = new RUPanelNaviViewModel(currentUser);
                    // Відкрити вікно зареєстрованого користувача
                    var userWindow = new RUPanelNavi(currentUser );
                    currentUser.LoadPurchasedTickets();
                    currentUser.Cart.LoadCart();
                    userWindow.Closed += (s, e) => { currentUser.Cart.SaveCart(); UserRepository.SaveCartBeforeLogout(); };

                    userWindow.Show();
                }

                // Закрити поточне вікно авторизації
                Application.Current.Windows.OfType<LoginWindow>().FirstOrDefault()?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
