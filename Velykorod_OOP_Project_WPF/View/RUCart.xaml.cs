using EventTickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Velykorod_OOP_Project_WPF.ViewModel;

namespace Velykorod_OOP_Project_WPF.View
{
   
    public partial class RUCart : Window
    {
        public RUCart(RegisteredUser? currentUser)
        {
            InitializeComponent();
            DataContext = new RUCartViewModel(currentUser);
        }

        
    }
}
