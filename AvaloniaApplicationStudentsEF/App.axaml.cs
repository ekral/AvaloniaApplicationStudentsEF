using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AvaloniaApplicationStudentsEF.Services;
using AvaloniaApplicationStudentsEF.ViewModels;

namespace AvaloniaApplicationStudentsEF
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new Views.MainWindow(new StudentListViewModel(new DatabaseService()));
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
