using CommonServiceLocator;
using Prism.Commands;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTools.Infrastructure;

namespace WorkTools.Modules.Navigation.ViewModels
{
    public class NavigationViewModel : BindableBase
    {
        private static Uri TestUri = new Uri("AndroidTranslationView", UriKind.Relative);
        private readonly IRegionManager _regionManager;
        private readonly IContainerExtension _containerExtension;

        public DelegateCommand<string> NavigateCommand { get; private set; }

        public NavigationViewModel(IRegionManager regionManager, IContainerExtension containerExtension)
        {
            _regionManager = regionManager;
            _containerExtension = containerExtension;

            NavigateCommand = new DelegateCommand<string>(Navigate);
        }

        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
                _regionManager.RequestNavigate(RegionNames.MainRegion, navigatePath);
            
            //if (navigatePath != null)
            //{
            //    var backupAssembly = System.Reflection.Assembly.Load("WorkTools.Modules.Android");
            //    Type testType = backupAssembly.GetType("WorkTools.Modules.Android.Views.TestView");

            //    if (null == testType)
            //        return;
            //    AddView(RegionNames.MainRegion, "TestView", testType);
            //    _regionManager.RequestNavigate(RegionNames.MainRegion, "TestView");
            //}
        }

        //private void AddView(string regionName, string viewName, Type type)
        //{
        //    IRegion region = _regionManager.Regions[regionName];
        //    if (null == region)
        //        throw new ArgumentNullException("IRegion");
        //    try
        //    {
        //        var sourcesView = region.GetView(viewName);
        //        if (sourcesView == null)
        //        {
        //            sourcesView = _containerExtension.Resolve(type);
        //            //sourcesView = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance(type);
        //            region.Add(sourcesView, viewName);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //    }
        //}
    }
}
