using System.Windows;
using Velykorod_OOP_Project_WPF.ViewModels;

namespace Velykorod_OOP_Project_WPF.View
{
    public partial class AEventManagement : Window
    {
        public AEventManagement()
        {
            InitializeComponent();
            DataContext = new EventManagementViewModel();
        }

       
    }
}
