using Avalonia.Controls;
using AvaloniaApplicationStudentsEF.ViewModels;

namespace AvaloniaApplicationStudentsEF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(StudentListViewModel viewModel) : this()
        {
            DataContext= viewModel;
        }
    }
}
