using EventTickets;
using QuestPDF.Infrastructure;
using System.Windows;

namespace Velykorod_OOP_Project_WPF
{
  
    public partial class App : Application
    {
     
        protected override void OnExit(ExitEventArgs e)
        {
          
            UserRepository.SaveAllUsersData(); // Зберігаємо всі дані

            EventRepository.SaveEvents();
            base.OnExit(e);

        }
        protected override void OnStartup(StartupEventArgs e)
        {
            QuestPDF.Settings.License = LicenseType.Community; // Безкоштовна ліцензія
            EventRepository.LoadEvents();
            base.OnStartup(e);
           
        }
    }

}
