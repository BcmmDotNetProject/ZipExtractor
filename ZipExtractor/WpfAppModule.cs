using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using ZipExtractor.Utils;
using ZipExtractor.ViewModels;
using ZipExtractor.Views;

namespace VegetableTooth
{
    [DependsOn(typeof(AbpAutofacModule))]
    public class WpfAppModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddSingleton<MainWindow>();
            context.Services.AddSingleton<MainWindowViewModel>();
            context.Services.AddSingleton<RenameManager>();
        }
    }
}
