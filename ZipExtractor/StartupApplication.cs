using System;
using System.Text;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using VegetableTooth;
using Volo.Abp;
using ZipExtractor.ViewModels;
using ZipExtractor.Views;

namespace ZipExtractor
{
    public class StartupApplication : Application
    {
        private IAbpApplicationWithInternalServiceProvider? _abpApplication;

        protected async override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
                    .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.txt"))
            .CreateLogger();

            try
            {
                Log.Information("Starting WPF host.");
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                _abpApplication = await AbpApplicationFactory.CreateAsync<WpfAppModule>(options =>
                {
                    options.UseAutofac();
                    options.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
                });

                await _abpApplication.InitializeAsync();

                var mainWindow = _abpApplication.Services.GetRequiredService<MainWindow>();

                mainWindow.DataContext = _abpApplication.Services.GetRequiredService<MainWindowViewModel>();
                mainWindow.Show();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
            }
        }
    }
}
