
using CommunityToolkit.Maui.Core;


namespace SampleMauiMvvmApp.ViewModels
{
    partial class MenuViewModel : BaseViewModel
    {
        public ObservableCollection<SampleMauiMvvmApp.Models.Menu> Menus { get; set; }

        public MenuViewModel()
        {
            Menus = new ObservableCollection<SampleMauiMvvmApp.Models.Menu>{
                //new Menu{
                //    Name = "Month",
                //    Image = "month_tabbed_icon.png",
                //    Label= "Five-time Pro Bowler (2018, 2019, 2020, 2021, 2022)",
                //    Url = "MonthPage",
                //    IsActive=false
                //},
                //new Menu{
                //    Name = "Uncaptured Readings",
                //    Image = "water_tap_thick.png",
                //    Label= "A vertical receiver who dominates opponents with size and great hands",
                //    Url = "MonthCustomerTabPage",
                //    IsActive=false
                //},
                new Menu{
                    Name = "Abnormal Consumption",
                    Image = "abnormal_use_icon.png",
                    Label= "",
                    Url = "ExceptionReadingListPage",
                    IsActive=false
                },
                new Menu{
                    Name = "My Notes",
                    Image = "notes_icon.png",
                    Label= "",
                    Url = "NotesListPage",
                    IsActive=false
                },
                new Menu{
                    Name = "Scan For New Customer(s)",
                    Image = "scan_db_icon.png",
                    Label= "",
                    Url = "SyncNewCustomersPage",
                    IsActive=true
                    //await Shell.Current.GoToAsync($"{nameof(CustomerDetailPage)}"
                },
                new Menu{
                    Name = "Recycle Readings",
                    Image = "export_sync.png",
                    Label= "",
                    Url = "ReflushPage",
                    IsActive=true
                    //await Shell.Current.GoToAsync($"{nameof(CustomerDetailPage)}"
                },
                new Menu{
                    Name = "Export to document",
                    Image = "export_document_icon.png",
                    Label= "",
                    //Url = "ReflushPage",
                    IsActive=true
                    //await Shell.Current.GoToAsync($"{nameof(CustomerDetailPage)}"
                },
            };
        }


        #region Prepare a toast/snackbar
        public Snackbar SnackBar()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.Red,
                TextColor = Colors.Green,
                ActionButtonTextColor = Colors.Yellow,
                CornerRadius = new CornerRadius(10),
                Font = Microsoft.Maui.Font.SystemFontOfSize(14),
                ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
                CharacterSpacing = 0.5
            };

            string text = "This is a Snackbar";
            string actionButtonText = "Click Here to Dismiss";
            Action action = async () => await Shell.Current.DisplayAlert("Reseting and re-seeding", "You are about to delete and restore", "OK", "Cancel");
            if (action.Equals("Cancel")) ;
            TimeSpan duration = TimeSpan.FromSeconds(5);

            var snackbar = Snackbar.Make(text, action, actionButtonText, duration, snackbarOptions);

            return (Snackbar)snackbar;
        }


        #endregion

        [RelayCommand]
        async Task GoToDetails(Menu menu)
        {
            if (menu == null)
                return;

            await Shell.Current.GoToAsync(menu.Url?.ToString());
        }

        [RelayCommand]
        async Task ConfirmLogout()
        {
            bool isConfirm = await Shell.Current.DisplayAlert($"Logout or switch users", $"You are about to logout of {Preferences.Default.Get("username","user")} profile", "OK", "Cancel");

            if (isConfirm.Equals(true))
            {
                IsBusy= true;
                await Task.Delay(TimeSpan.FromSeconds(3));
                SecureStorage.Remove("Token");
                Preferences.Default.Clear();
                IsBusy = false;
                await Shell.Current.GoToAsync(nameof(LoginPage));
            }
        }
    }
}
