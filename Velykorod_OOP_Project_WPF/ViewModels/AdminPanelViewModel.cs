using System.Windows.Input;
using Velykorod_OOP_Project_WPF.View;
using Velykorod_OOP_Project_WPF.ViewModel;

namespace Velykorod_OOP_Project_WPF.ViewModels
{
    public class AdminPanelViewModel
    {
        public ICommand ManageEventsCommand { get; }
        public ICommand ViewSalesCommand { get; }
        public ICommand LogoutCommand { get; }

        public AdminPanelViewModel()
        {
            ManageEventsCommand = new RelayCommand(OpenEventsManagement);
            LogoutCommand = new RelayCommand(Logout);
        }

        private void OpenEventsManagement(object parameter)
        {
            // Відкриваємо вікно керування подіями
            var eventsWindow = new AEventManagement();
            eventsWindow.Show();

            // Закриваємо поточне вікно
            System.Windows.Application.Current.Windows.OfType<PanelAdm>().FirstOrDefault()?.Close();
        }

        private void Logout(object parameter)
        {
            // Повертаємось на вікно авторизації
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            System.Windows.Application.Current.Windows.OfType<PanelAdm>().FirstOrDefault()?.Close();
        }
    }
}
