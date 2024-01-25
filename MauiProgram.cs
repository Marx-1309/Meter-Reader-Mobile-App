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
            //.UseLocalNotification()
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
        builder.Services.AddSingleton<UnregReadingService>();
        builder.Services.AddSingleton<DbContext>();
        builder.Services.AddSingleton<CustomerService>();
        builder.Services.AddSingleton<ReadingService>();
        builder.Services.AddSingleton<ReadingExportService>();
        builder.Services.AddSingleton<MonthService>();
        builder.Services.AddSingleton<AuthenticationService>();
        builder.Services.AddTransient<AppShell>();


        builder.Services.AddSingleton<OnboardingPage>();
        builder.Services.AddSingleton<LoadingPage>();
        builder.Services.AddSingleton<SynchronizationPage>();
        builder.Services.AddSingleton<OnboardingViewModel>();
        builder.Services.AddTransient<ReflushPage>();
        builder.Services.AddTransient<UpsertUnregReadingPage>();
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<LogoutPage>();
        builder.Services.AddSingleton<ListOfReadingByMonthPage>();
        builder.Services.AddSingleton<MonthCustomerTabPage>();
        builder.Services.AddScoped<UncapturedReadingsPage>();
        builder.Services.AddSingleton<MonthPage>();
        builder.Services.AddTransient<CustomerDetailPage>();
        builder.Services.AddScoped<CapturedReadingsPage>();
        builder.Services.AddTransient<UncapturedReadingsByAreaPage>();
        builder.Services.AddTransient<LocationPage>();

        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<LoadingViewModel>();
        builder.Services.AddSingleton<LogoutViewModel>();
        builder.Services.AddSingleton<ReadingViewModel>();
        builder.Services.AddSingleton<UnregReadingsListViewModel>();
        builder.Services.AddTransient<UpsertUnregReadingsViewModel>();
        builder.Services.AddSingleton<MonthViewModel>();
        builder.Services.AddTransient<CustomerDetailViewModel>();
        builder.Services.AddSingleton<CustomerViewModel>();

        builder.Services.AddAutoMapper(typeof(ClassDtoMapping));

        return builder.Build();
	}
}
