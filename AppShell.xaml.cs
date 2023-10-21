using SampleMauiMvvmApp.ViewModels;
using SampleMauiMvvmApp.Views;
using SampleMauiMvvmApp.Views.SecurityPages;

namespace SampleMauiMvvmApp;

public partial class AppShell : Shell
{
    public AppShell()
	{
        InitializeComponent();

        //Routes
        Routing.RegisterRoute(nameof(CustomerDetailPage), typeof(CustomerDetailPage));
        Routing.RegisterRoute(nameof(MonthPage), typeof(MonthPage));
        Routing.RegisterRoute(nameof(ListOfReadingByMonthPage), typeof(ListOfReadingByMonthPage));
        Routing.RegisterRoute(nameof(MonthCustomerTabPage), typeof(MonthCustomerTabPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(LogoutPage), typeof(LogoutPage));
        Routing.RegisterRoute(nameof(LoadingPage), typeof(LoadingPage));
        Routing.RegisterRoute(nameof(CapturedReadingsPage), typeof(CapturedReadingsPage));
        Routing.RegisterRoute(nameof(UncapturedReadingsPage), typeof(UncapturedReadingsPage));
        Routing.RegisterRoute(nameof(AppShell), typeof(AppShell));
       

        
    }  
}
