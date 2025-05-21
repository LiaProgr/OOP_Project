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
    /// Interaction logic for RUPay.xaml
    /// </summary>
    public partial class RUPay : Window
    {
        public RUPay(RegisteredUser currentUser, RUCartViewModel cartViewModel)
        {
            InitializeComponent();
            DataContext = new RUPayViewModel( currentUser, cartViewModel);
        }
    }
}
