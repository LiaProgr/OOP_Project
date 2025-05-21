using EventTickets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Velykorod_OOP_Project_WPF.ViewModel;

namespace Velykorod_OOP_Project_WPF.View
{
    
    public partial class RUPanelNavi : Window
    {
        public RUPanelNavi(RegisteredUser? currentUser)
        {
            InitializeComponent();
            DataContext = new RUPanelNaviViewModel(currentUser);
        }

        
    }
}
