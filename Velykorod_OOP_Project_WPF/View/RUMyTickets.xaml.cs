using EventTickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Velykorod_OOP_Project_WPF.ViewModel;

namespace Velykorod_OOP_Project_WPF.View
{
    /// <summary>
    /// Interaction logic for RUMyTickets.xaml
    /// </summary>
    public partial class RUMyTickets : Window
    {
        public RUMyTickets(RegisteredUser? currentUser)
        {
            InitializeComponent();
            DataContext = new RUMyTicketsViewModel(currentUser);
        }

        
    }
}
