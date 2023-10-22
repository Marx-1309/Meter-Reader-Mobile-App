using AutoMapper;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SampleMauiMvvmApp.Mappings.Maps;
using SampleMauiMvvmApp.Services;
using SampleMauiMvvmApp.ViewModels;
using SampleMauiMvvmApp.Views;
using SampleMauiMvvmApp.Views.SecurityPages;
using SkiaSharp.Views.Maui.Controls.Hosting;
using SQLite;

namespace SampleMauiMvvmApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
#if DEBUG
        builder.Logging.AddDebug();
#endif
        builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
        builder.Services.AddSingleton<IGeolocation>(Geolocation.Default);
        builder.Services.AddSingleton<IMap>(Map.Default);

        builder.Services.AddTransient<BaseService>();

        builder.Services.AddSingleton<CustomerViewModel>();
        builder.Services.AddSingleton<CapturedReadingsPage>();

        builder.Services.AddTransient<CustomerDetailViewModel>();
        builder.Services.AddTransient<CustomerDetailPage>();

        builder.Services.AddSingleton<DbContext>();
        builder.Services.AddSingleton<CustomerService>();
        builder.Services.AddSingleton<ReadingService>();
        builder.Services.AddSingleton<ReadingExportService>();
        

        builder.Services.AddSingleton<MonthService>();
        builder.Services.AddSingleton<MonthViewModel>();
        builder.Services.AddSingleton<MonthPage>();

        builder.Services.AddSingleton<ListOfReadingByMonthPage>();

        builder.Services.AddSingleton<LoadingPage>();
        builder.Services.AddSingleton<SynchronizationPage>();
        builder.Services.AddSingleton<LoadingViewModel>();

        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<LoginViewModel>();

        builder.Services.AddSingleton<LogoutViewModel>();
        builder.Services.AddSingleton<LogoutPage>();

        builder.Services.AddSingleton<AuthenticationService>();


        builder.Services.AddAutoMapper(typeof(ClassDtoMapping));

        builder.Services.AddSingleton<MonthCustomerTabPage>();

        builder.Services.AddSingleton<UncapturedReadingsPage>();
        builder.Services.AddSingleton<ReadingViewModel>();




        return builder.Build();
	}
}
