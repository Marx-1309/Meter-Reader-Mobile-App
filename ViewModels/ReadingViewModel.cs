﻿
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System.Linq;

namespace SampleMauiMvvmApp.ViewModels
{
    [QueryProperty("Area", "Area")]
    [QueryProperty("Refresh", "Refresh")]
    public partial class ReadingViewModel : BaseViewModel
    {
        
        ReadingService readingService;
        ReadingExportService readingExportService;
        CustomerService customerService;
        MonthService monthService;
        DbContext dbContext;
        AppShell appShell;
        public ReadingViewModel(ReadingService _readingService,ReadingExportService _readingExportService, CustomerService _customerService, MonthService _monthService, DbContext _dbContext,AppShell _appShell)
        {
            this.readingService = _readingService;
            this.readingExportService = _readingExportService;
            this.customerService = _customerService;
            this.monthService = _monthService;
            this.dbContext = _dbContext;
            this.appShell = _appShell;
        }

        public ObservableCollection<Reading> AllReadings { get; set; } = new();
        public ObservableCollection<LocationReadings> AllLocation { get; set; } = new();
        [ObservableProperty]
        bool isRefreshing;
        [ObservableProperty]
        string area;
        [ObservableProperty]
        bool isAllLocationsCaptured;
        public static List<Reading> ReadingsListForSearch { get; private set; } = new List<Reading>();
        public static List<LocationReadings> LocationListForSearch { get; private set; } = new List<LocationReadings>();
        public ObservableCollection<Reading> Readings { get; set; } = new ObservableCollection<Reading>();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(GetCapturedReadingsCommand))]
        public string? refresh;


        [RelayCommand]
        async Task GetCapturedReadings()
        {
            if (IsBusy) return; // Return an empty list if already busy

            try
            {
                IsBusy = true;
                var listOfCapturedReadings = await readingService.GetAllCapturedReadings();
                if (listOfCapturedReadings != null && listOfCapturedReadings.Count > 0)
                {
                    AllReadings.Clear(); // Clear the list before adding readings
                    foreach (var reading in listOfCapturedReadings)
                    {
                        var IsFlagged = IsReadingFlagged((decimal)reading.PREVIOUS_READING,reading.CURRENT_READING);
                        if (IsFlagged)
                        {
                            reading.IsFlagged = true;
                        }
                        if (reading.CURRENT_READING >= 1)
                        {
                            reading.ReadingTaken = true;
                            reading.ReadingNotTaken = false;
                        }
                        else
                        {
                            reading.ReadingTaken = false;
                            reading.ReadingNotTaken = true;
                        }
                        AllReadings.Add(reading); // Add each reading to the list

                    }
                    //foreach (var reading in listOfCapturedReadings)
                    //{
                    //    Readings.Add(reading);
                    //}
                    ReadingsListForSearch.Clear();
                    ReadingsListForSearch.AddRange(listOfCapturedReadings);
                    // Set IsBusy to false after adding all readings
                    
                    
                }
                else
                {
                    await Shell.Current.DisplayAlert("Not Found", "No Captured readings found", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Unable to retrieve any readings", "Please try again", "OK");
            }
            finally
            {
                IsBusy = false; // Ensure IsBusy is set to false in case of any exception or no readings found
                IsRefreshing = false;
            }
        }



        [RelayCommand]
        async Task GetUncapturedReadings()
        {
            if (IsBusy) return; // Return an empty list if already busy

            try
            {
                IsBusy = true;
                var listOfCapturedReadings = await readingService.GetAllUncapturedReadings();
                if (listOfCapturedReadings != null && listOfCapturedReadings.Count > 0)
                {
                    AllReadings.Clear(); // Clear the list before adding readings
                    foreach (var reading in listOfCapturedReadings)
                    {
                        if (reading.CURRENT_READING >= 1)
                        {
                            reading.ReadingTaken = true;
                            reading.ReadingNotTaken = false;
                        }
                        else
                        {
                            reading.ReadingTaken = false;
                            reading.ReadingNotTaken = true;
                        }
                        Task.Delay(50);
                        AllReadings.Add(reading); // Add each reading to the list

                    }
                    foreach (var reading in listOfCapturedReadings)
                    {
                        Readings.Add(reading);
                    }
                    ReadingsListForSearch.Clear();
                    ReadingsListForSearch.AddRange(listOfCapturedReadings);
                    // Set IsBusy to false after adding all readings

                }
                else
                {
                    await Shell.Current.DisplayAlert("Not Found", "No Uncaptured readings found", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Unable to retrieve any readings", "Please try again", "OK");
            }
            finally
            {
                IsBusy = false; // Ensure IsBusy is set to false in case of any exception or no readings found
                IsRefreshing = false;
            }
        }


        [RelayCommand]
        public async Task GoToCustomerDetails(Reading reading)
        {
            if (reading.CUSTOMER_NUMBER == null) return;

            var customer = await customerService.GetCustomerDetails(reading.CUSTOMER_NUMBER);
            if (customer == null)
            {
                await Shell.Current.DisplayAlert("Error", "Failed getting customer details", "OK");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(CustomerDetailPage)}", true,
                new Dictionary<string, object>()
                {
                    { nameof(Customer), new CustomerWrapper(customer) }
                });
        }


        [RelayCommand]
        public async Task ScanForNewExport()
        {
            IsBusy = true;
            await Task.Delay(1000);
            await readingExportService.ScanForNewItems();
            IsBusy = false;
            await Shell.Current.GoToAsync("..");
        }
        private async Task DisplayAlert(string v1, string v2, string v3, string v4)
        {
            await Shell.Current.DisplayAlert(v1, v2, v3, v4);
        }

        [RelayCommand]
        public async Task ReflushData()
        {
            IsBusy = true;
            await Task.Delay(50);
            //CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            //var snackbarOptions = new SnackbarOptions
            //{
            //    BackgroundColor = Colors.Red,
            //    TextColor = Colors.Green,
            //    ActionButtonTextColor = Colors.Yellow,
            //    CornerRadius = new CornerRadius(10),
            //    Font = Microsoft.Maui.Font.SystemFontOfSize(14),
            //    ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14),
            //    CharacterSpacing = 0.5
            //};

            //string text = "This is a Snackbar";
            //string actionButtonText = "Click Here to Dismiss";
            //Action action = async () => await DisplayAlert("Reseting and re-seeding", "You are about to delete and restore", "OK", "Cancel");
            //if (action.Equals("Cancel")) return;
            //TimeSpan duration = TimeSpan.FromSeconds(5);

            //var snackbar = Snackbar.Make(text, action, actionButtonText, duration, snackbarOptions);

            //await snackbar.Show(cancellationTokenSource.Token);
            await readingExportService.FlushAndSeed();
            IsBusy = false;
            await Shell.Current.GoToAsync("..");
        }

        //Get Locations list

        [RelayCommand]
        async Task GetLocations()
        {
            if (IsBusy) return; // Return an empty list if already busy

            try
            {
                IsBusy = true;

                var listOfLocations = await readingService.GetListOfLocations();
                if (listOfLocations != null && listOfLocations.Count > 0)
                {

                    AllLocation.Clear(); // Clear the list before adding readings

                    foreach (var location in listOfLocations)
                    {
                        if ((bool)location.IsAllCaptured)
                        {
                            location.IsAllNotCaptured = false;
                        }
                        else
                        {
                            location.IsAllNotCaptured = true;
                        }

                        AllLocation.Add(location);
                    }
                    LocationListForSearch.Clear();
                    LocationListForSearch.AddRange(listOfLocations);

                    // Set IsBusy to false after adding all readings

                }
                else
                {
                    await Shell.Current.DisplayAlert("Not Found", "No Uncaptured readings found", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Unable to retrieve any readings", "Please try again", "OK");
            }
            finally
            {
                IsBusy = false; // Ensure IsBusy is set to false in case of any exception or no readings found
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        public async Task GoToListOfUncapturedReadingsByArea(LocationReadings area)
        {
            //if (IsBusy) return; // Return an empty list if already busy
            try
            {
                Title = area?.AREANAME.Trim();
                IsBusy = true;
                if (area == null)
                {
                    await Shell.Current.GoToAsync("..");
                    return;
                } ;
                var listReadings = new List<Reading>();
                //if (monthId.MonthID <= 0) return;
                var uncapturedReadings = await readingService.GetUncapturedReadingsByArea(area);

                if (uncapturedReadings != null)
                {
                    AllReadings.Clear();
                    foreach (var i in uncapturedReadings)
                    {
                        i.ReadingTaken = false;
                        i.ReadingNotTaken = true;
                        Task.Delay(50);
                        AllReadings.Add(i);
                    }

                   await Shell.Current.GoToAsync($"{nameof(UncapturedReadingsByAreaPage)}", true,
                   new Dictionary<string, object>()
                   {
                    { nameof(List<Reading>), new List<Reading>(AllReadings) }
                   });

                }

                if (uncapturedReadings.Count == 0)
                {
                    await Shell.Current.DisplayAlert("No Readings", $"No records found in {area}", "OK");
                    return;
                }
                IsBusy = false;
            }
            catch(Exception ex)
            {
                IsBusy = false;
            }
            
            
        }

        public bool IsReadingFlagged(decimal previous, decimal current)
        {
            decimal difference = Math.Abs(current - previous);

            if (difference >= 20)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsLocationCleared(int count)
        {
            if (count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


    }
}


    

