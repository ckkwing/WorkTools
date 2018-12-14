using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTools.Infrastructure;
using WorkTools.Modules.Android.Views;

namespace WorkTools.Modules.Android
{
    [Module(ModuleName = "AndroidModule")]
    [ModuleDependency("NavigationModule")]
    public class AndroidModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(AndroidTranslationView));
            regionManager.RegisterViewWithRegion("TabRegion",
                                                    () => containerProvider.Resolve<TestView>());
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterForNavigation<TestView>("Eric");
        }
    }
}
