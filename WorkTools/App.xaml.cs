using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using Prism.Modularity;
using WorkTools.Modules.Navigation;
using WorkTools.Modules.Navigation.ViewModels;
using WorkTools.Modules.Android;
using Prism.Mvvm;
using CommonServiceLocator;
using WorkTools.Modules.Windows;

namespace WorkTools
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<NavigationModule>();
            moduleCatalog.AddModule<WindowsModule>();
            moduleCatalog.AddModule<AndroidModule>();



            // generic type
            //ViewModelLocationProvider.Register<MainWindow, CustomViewModel>();
            //ServiceLocator.Current.GetInstance<IRegionManager>();
        }
    }
}
