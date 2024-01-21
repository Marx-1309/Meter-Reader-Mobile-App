namespace SampleMauiMvvmApp;

public partial class AppShell : Shell
{
    public AppShell()
	{
        InitializeComponent();
        _ = DisplayLoggedInUserInfo();
        //Routes
        Routing.RegisterRoute(nameof(CustomerDetailPage), typeof(CustomerDetailPage));
        Routing.RegisterRoute(nameof(MonthPage), typeof(MonthPage));
        Routing.RegisterRoute(nameof(ListOfReadingByMonthPage), typeof(ListOfReadingByMonthPage));
        Routing.RegisterRoute(nameof(MonthCustomerTabPage), typeof(MonthCustomerTabPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(LogoutPage), typeof(LogoutPage));
        Routing.RegisterRoute(nameof(OnboardingPage), typeof(OnboardingPage));
        Routing.RegisterRoute(nameof(CapturedReadingsPage), typeof(CapturedReadingsPage));
        Routing.RegisterRoute(nameof(UncapturedReadingsPage), typeof(UncapturedReadingsPage));
        Routing.RegisterRoute(nameof(UpsertUnregReadingPage), typeof(UpsertUnregReadingPage));
        Routing.RegisterRoute(nameof(UnregReadingListPage), typeof(UnregReadingListPage));
        Routing.RegisterRoute(nameof(SynchronizationPage), typeof(SynchronizationPage));
        Routing.RegisterRoute(nameof(OnboardingPage), typeof(OnboardingPage));
        Routing.RegisterRoute(nameof(LocationPage), typeof(LocationPage));
        Routing.RegisterRoute(nameof(UncapturedReadingsByAreaPage), typeof(UncapturedReadingsByAreaPage));
        Routing.RegisterRoute(nameof(MenuPage), typeof(MenuPage));
        Routing.RegisterRoute(nameof(AppShell), typeof(AppShell));
    }

    private async Task DisplayLoggedInUserInfo()
    {
        await Task.Delay(1000);
        IsBusy = true;
        //Retrieve Token from internal Secure Storage
                
                lblUsername.Text = Preferences.Default.Get("username", "Not logged in");
                lblRole.Text = "Meter Reader" ?? "Meter Reader";

                IsBusy = false;
                await GoToMainPage();
    }

    private async Task GoToLoginPage()
    {
        await Shell.Current.GoToAsync($"{nameof(LoginPage)}");
    }

    private async Task GoToMainPage()
    {
        await Shell.Current.GoToAsync($"//{nameof(MonthCustomerTabPage)}"); ;
    }
}
