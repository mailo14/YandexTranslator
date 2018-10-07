using System.Windows;
using Ninject;

namespace RStyleTranslator
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
           /* var vm = App.Kernel.Get<TranslatorViewModel>();
            vm.FromLangId = "ru"; vm.ToLangId = "en";
            DataContext = vm;*/
        }

    }
}