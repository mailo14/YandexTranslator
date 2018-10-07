using Ninject;
using System.Windows;

namespace RStyleTranslator
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {      
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);          

            // внедрение зависимостей:
            var kernel = new StandardKernel();
            kernel.Bind<ITranslateService>().To<YandexTranslateService>()
                .WithConstructorArgument("key", "trnsl.1.1.20181004T101759Z.da55e9ef194a644a.5a7077fba3b31cd7108801c453b50b1a001781df");
            kernel.Bind<ILangDbQueries>().To<LangDbQueries>();
            kernel.Bind<TranslatorViewModel>().ToSelf();
         
            var mainWindow = new MainWindow();
            var vm = kernel.Get<TranslatorViewModel>();
            vm.FromLangId = "ru"; vm.ToLangId = "en";
            mainWindow.DataContext = vm;
            mainWindow.ShowDialog();
        }

    }
}