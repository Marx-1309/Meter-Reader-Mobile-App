using Ardalis.GuardClauses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Maui.Graphics.Text;
using Plugin.Media;
using Plugin.Media.Abstractions;
//using Plugin.Media;
//using Plugin.Media.Abstractions;
using SampleMauiMvvmApp.Messages;
using SampleMauiMvvmApp.Models;
using SampleMauiMvvmApp.ModelWrappers;
using SampleMauiMvvmApp.Services;
using SampleMauiMvvmApp.Views;

namespace SampleMauiMvvmApp.ViewModels
{
    [QueryProperty("Customer", "Customer")]
    [QueryProperty("Reading", "Reading")]
    public partial class CustomerDetailViewModel : BaseViewModel
    {
        DbContext dbContext;
        HttpClient client;
        ReadingService readingService;
        MonthService monthService;
        CustomerService customerService;
        IConnectivity connectivity;

        [ObservableProperty]
        CustomerWrapper customer;

        [ObservableProperty]
        ReadingWrapper reading;

        [ObservableProperty]
        string erfNumber;
        [ObservableProperty]
        string custStateErf;
        [ObservableProperty]
        decimal custPrevReading;
        [ObservableProperty]
        decimal custCurrentReading;
        [ObservableProperty]
        string percentageChange;
        [ObservableProperty]
        string meterNumber;
        [ObservableProperty]
        string routeNumber;
        [ObservableProperty]
        ReadingWrapper vmReading;
        [ObservableProperty]
        int currentMonth;
        [ObservableProperty]
        public static bool isExist;
        private int selectedCompressionQuality;
        ReadingMedia capturedImage = new();

        public CustomerDetailViewModel(DbContext _dbContext, ReadingService readingService,
            CustomerService _customerService, MonthService _monthService, IConnectivity _connectivity)
        {
            Title = "Customer Detail Page";
            this.dbContext = _dbContext;
            this.readingService = readingService;
            this.connectivity = _connectivity;
            this.customerService = _customerService;
            this.monthService = _monthService;

            WeakReferenceMessenger.Default.Register<ReadingCreateMessage>(this, (obj, handler) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var newReading = new ReadingWrapper(handler.Value)
                    {
                        IsNew = true
                    };

                    if (Customer.Readings == null) Customer.Readings = new();
                    Customer.Readings.Insert(0, newReading);

                });
            });
        }


        [RelayCommand]
        public async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        async Task CustDisplayDetailsAsync()
        {
            //LastReadingByIdAsync
            int PrevMonthId = (int)await readingService.GetLatestExportItemMonthId();

            var reading = await readingService.GetLastReadingByIdAsync(Customer.Custnmbr);
            if (reading != null)
            {
                CustPrevReading = reading.PREVIOUS_READING;
                CustCurrentReading = reading.CURRENT_READING;
                MeterNumber = reading.METER_NUMBER;
                RouteNumber = reading.RouteNumber;
                ErfNumber = reading.ERF_NUMBER;
                CustStateErf = $"{reading.AREA.Trim()}\n({reading.ERF_NUMBER.Trim()})";
            }

            bool isExist = await readingService.IsReadingExistForMonthId(Customer.Custnmbr);
            IsExist = isExist;
            return;
        }


        [RelayCommand]
        public async Task CreateReadingAsync()
        {
            try
            {
                IsValid();
                var CurrentMonthReading = await readingService.GetCurrentMonthReadingByCustIdAsync(Customer.Custnmbr);
                var customerInfo = await customerService.GetCustomerDetails(Customer.Custnmbr);
                var loggedInUser = await dbContext.Database.Table<LoginHistory>()?.OrderByDescending(r => r.LoginId).FirstAsync();

                if (!VmReading.C_reading.IsNullOrEmpty())
                {
                    if (int.TryParse(VmReading.C_reading, out int intValue))
                    {
                        CurrentMonthReading.CURRENT_READING = intValue;
                    }
                    else
                    {
                        // Handle the case where the string cannot be parsed as an integer
                        // You can raise an exception, log an error, or handle it in another way
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert($"Null Or Empty",
                                                        $"Please enter a valid reading!", "OK");
                    return;
                }

                CurrentMonthReading.Comment = VmReading.Comment;
                CurrentMonthReading.Meter_Reader = loggedInUser.Username;
                CurrentMonthReading.ReadingTaken = true;
                CurrentMonthReading.ReadingNotTaken = false;
                CurrentMonthReading.ReadingSync = false;
                CurrentMonthReading.WaterReadingExportID = (int)await readingService.GetLatestExportItemId();



                if (CurrentMonthReading.WaterReadingExportID <= 0)
                {
                    await Shell.Current.DisplayAlert($"No Reading Export Found",
                                                         $"Confirm Database and try again!", "OK");
                    await ClearForm();
                    return;
                }

                if (CurrentMonthReading.CURRENT_READING < CustPrevReading && CurrentMonthReading.CURRENT_READING > 0)
                {

                    await Shell.Current.DisplayAlert($"Current Reading lesser than Previous of:{CustPrevReading}",
                                                          $"Please check current reading and try again!", "OK");

                    await Shell.Current.DisplayAlert($"Invalid Input!",
                                                          $"Please enter valid reading!", "OK");
                    await ClearForm();
                    return;
                }


                if (CurrentMonthReading.CURRENT_READING == 0)
                {
                    await Shell.Current.DisplayAlert($"Reset Success",
                                                          $"This reading has been uncaptured!", "OK");
                }


                if (CurrentMonthReading.CURRENT_READING != 0)
                {
                    CurrentMonthReading.ReadingTaken = true;

                }
                else
                {
                    CurrentMonthReading.ReadingTaken = false;
                }



                var newReading = await readingService.InsertReading(Models.Reading.GenerateNewFromWrapper(new ReadingWrapper(CurrentMonthReading)));
                IsExist = true;


                if (newReading != null)
                {
                    var latestMonthName = await monthService.GetMonthNameById();
                    await Shell.Current.DisplayAlert($"Success!", $"A reading for {latestMonthName} Created!", "OK");

                    // Propagate the new reading to the main reading page.
                    WeakReferenceMessenger.Default.Send(new ReadingCreateMessage(newReading));
                    await Task.Delay(1000);
                    await GoBackAsync();
                }

                else
                {
                    await Shell.Current.DisplayAlert($"Error!",
                                   $"Something Wrong,Try Again!", "OK");
                    await ClearForm();
                    await Task.Delay(500);
                    await GoBackAsync();
                }
            }
            catch
            {
                return;
            }
        }

        [RelayCommand]
        public async Task OnTakePhotoClicked()
        {
            var options = new StoreCameraMediaOptions { CompressionQuality = selectedCompressionQuality };
            var result = await CrossMedia.Current.TakePhotoAsync(options);
            if (result is null) return;



            var fileInfo = new FileInfo(result?.Path);
            var fileLength = fileInfo.Length;



            //Convert the image to Base64 string
            byte[] imageData = File.ReadAllBytes(result?.Path);
            string base64Image = Convert.ToBase64String(imageData);

            //Save the data to db
            var latestExportItem = await dbContext.Database.Table<ReadingExport>()
                       .OrderByDescending(r => r.WaterReadingExportID)
                       .FirstOrDefaultAsync();

            int currentExportId = latestExportItem.WaterReadingExportID;

            Reading reading = await dbContext.Database.Table<Reading>()
                .Where(r=>r.CUSTOMER_NUMBER == Customer.Custnmbr && r.WaterReadingExportID == currentExportId)
                .FirstOrDefaultAsync();

            List<ReadingMedia> existingImage = await dbContext.Database.Table<ReadingMedia>().Where(r=>r.WaterReadingExportId == currentExportId).ToListAsync();

            ReadingMedia capturedImage = new()
            {
                WaterReadingExportDataId = reading.WaterReadingExportDataID,
                WaterReadingExportId = reading.WaterReadingExportID,
                Title = result.OriginalFilename,
                Data = base64Image,
                DateTaken = DateTime.UtcNow.ToLongDateString(),
            };

            if (existingImage.Any())
            {
                foreach (var image in existingImage)
                {
                    await dbContext.Database.DeleteAsync(image);
                }
            }

            await dbContext.Database.InsertAsync(capturedImage);
            
        }

        public bool IsValid()
        {
            try
            {

                Guard.Against.OutOfRange<Decimal>((decimal)VmReading.Current_reading, nameof(VmReading.Current_reading), 0, Decimal.MaxValue);
                Guard.Against.OutOfRange<Decimal>((decimal)VmReading.Current_reading, nameof(VmReading.Current_reading), 100000, Decimal.MinValue);
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                return false;
            }

            return true;
        }

        [RelayCommand]
        async Task ClearForm()
        {
            await Task.Yield();
            VmReading.C_reading = string.Empty;
        }

    }
}