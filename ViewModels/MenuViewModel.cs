
namespace SampleMauiMvvmApp.ViewModels
{
    partial class MenuViewModel
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
                    Name = "Export Sync",
                    Image = "export_sync.png",
                    Label= "Pitt's all-time leader in passing yards (12,303), pass completions (1,045), total offense (13,112), touchdown responsibility (102) and passing touchdowns (81)",
                    Url = "SynchronizationPage",
                    IsActive=false
                },
                new Menu{
                    Name = "Add Reading",
                    Image = "add_readings_maually.jpg",
                    Label= "314 tackles (212 solo), 33 passes defensed, 13 interceptions, 5 tackles for loss, 4 fumble recoveries, four forced fumbles, 3 interception returns for TDs",
                    Url = "ReflushPage",
                    IsActive=true
                    //await Shell.Current.GoToAsync($"{nameof(CustomerDetailPage)}"
                },
            };
        }

        [RelayCommand]
        async Task GoToDetails(Menu menu)
        {
            if (menu == null)
                return;

            await Shell.Current.GoToAsync(menu.Url.ToString());
        }
    }
}
