using EventTickets;
using System.Windows;
using Velykorod_OOP_Project_WPF.ViewModel;

namespace Velykorod_OOP_Project_WPF.View
{
    public partial class AEditAdd : Window
    {
        public AEditAdd(Event eventToEdit = null)
        {
            InitializeComponent();
            DataContext = new EditAddViewModel(eventToEdit);
            // Додаємо обробник закриття вікна
            if (DataContext is EditAddViewModel vm)
            {
                vm.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(EditAddViewModel.ShouldClose))
                    {
                        DialogResult = true;
                        Close();
                    }
                };
            }
        }
    }
}