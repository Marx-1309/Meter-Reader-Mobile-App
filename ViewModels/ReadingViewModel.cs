
namespace SampleMauiMvvmApp.ViewModels
{
    [QueryProperty("Area", "Area")]
    public partial class ReadingViewModel : BaseViewModel
    {
        
        ReadingService readingService;
        ReadingExportService readingExportService;
        CustomerService customerService;
        MonthService monthService;
        DbContext dbContext;
        public ReadingViewModel(ReadingService _readingService,ReadingExportService _readingExportService, CustomerService _customerService, MonthService _monthService, DbContext _dbContext)
        {
            this.readingService = _readingService;
            this.readingExportService = _readingExportService;
            this.customerService = _customerService;
            this.monthService = _monthService;
            this.dbContext = _dbContext;
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
            var currentPage = Shell.Current.CurrentPage;
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
            await readingExportService.ScanForNewExports();
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
                Title = area?.AREANAME;
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


    

