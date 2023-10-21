using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui;
using SampleMauiMvvmApp.Models;
using SampleMauiMvvmApp.ModelWrappers;
using SampleMauiMvvmApp.Services;
using SampleMauiMvvmApp.Views;
using System.Collections.ObjectModel;

namespace SampleMauiMvvmApp.ViewModels
{
    public partial class ReadingViewModel : BaseViewModel
    {
        ReadingService readingService;
        CustomerService customerService;
        MonthService monthService;
        DbContext dbContext;
        public ReadingViewModel(ReadingService _readingService, CustomerService _customerService, MonthService _monthService, DbContext _dbContext)
        {
            this.readingService = _readingService;
            this.customerService = _customerService;
            this.monthService = _monthService;
            this.dbContext = _dbContext;
        }

        public ObservableCollection<Reading> AllReadings { get; set; } = new();
        [ObservableProperty]
        bool isRefreshing;
        public static List<Reading> ReadingsListForSearch { get; private set; } = new List<Reading>();
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



    }
}


    

