using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkTools.Infrastructure;
using WorkTools.Modules.Windows.Views;

namespace WorkTools.Modules.Windows
{
    [Module(ModuleName = "WindowsModule")]
    [ModuleDependency("NavigationModule")]
    public class WindowsModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(WindowsToolsMainView));
  
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
