using System.Windows;
using Velykorod_OOP_Project_WPF.ViewModels;

namespace Velykorod_OOP_Project_WPF.View
{
    /// <summary>
    /// Interaction logic for PanelAdm.xaml
    /// </summary>
    public partial class PanelAdm : Window
    {
        public PanelAdm()
        {
            InitializeComponent();
            DataContext = new AdminPanelViewModel();
        }

        
    }
}
