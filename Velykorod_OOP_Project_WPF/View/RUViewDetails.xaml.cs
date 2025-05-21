using EventTickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Velykorod_OOP_Project_WPF.View
{
  
    public partial class RUViewDetails : Window
    {
      

        public RUViewDetails(Event selectedEvent, RegisteredUser currentUser)
        {
            InitializeComponent();
            DataContext = new RUDetailsViewModel(selectedEvent, currentUser);
        }

       
    }
}
