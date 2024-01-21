
namespace SampleMauiMvvmApp.ViewModels
{
    [QueryProperty(nameof(ReadingDetail), "ReadingDetail")]
    public partial class UpsertUnregReadingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private UnregReadings _readingDetail = new UnregReadings();

        private readonly UnregReadingService _unregReadingService;
        public UpsertUnregReadingsViewModel(UnregReadingService unregReadingService)
        {
           _unregReadingService = unregReadingService;
        }

        [RelayCommand]
        public async Task UpsertReading()
        {
            int response = -1;
            if (ReadingDetail.ReadingID > 0)
            {
                if (ReadingDetail.current_Reading == null)
                {
                    await Shell.Current.DisplayAlert("Empty Field", "Current Reading not provided.", "OK");
                    return;
                }

                if (decimal.TryParse(ReadingDetail.current_Reading, out decimal currentReadingValue))
                {
                    ReadingDetail.Reading = currentReadingValue;
                    response = await _unregReadingService.UpdateUnregReading(ReadingDetail);
                }
            }
            else
            {
                int OutputVal = 0;
                int.TryParse(ReadingDetail.MeterNo, out OutputVal);

                bool isReadingExist = await _unregReadingService.CheckExistingReadingListById(OutputVal);

                if (!isReadingExist)
                {
                    if(ReadingDetail.MeterNo ==null || ReadingDetail.ErfNo == null)
                    {
                        await Shell.Current.DisplayAlert("Empty Fields", "Meter or Erf No not provided.", "OK");
                        await Shell.Current.DisplayAlert("Heads up!", "Record NOT saved!.", "OK");
                        return;
                    }

                    if (ReadingDetail.current_Reading == null)
                    {
                        await Shell.Current.DisplayAlert("Empty Fields", "Current Reading not provided.", "OK");
                        return;
                    }

                    if (decimal.TryParse(ReadingDetail.current_Reading, out decimal currentReadingValue))
                    {
                        response = await _unregReadingService.AddUnregReading(new Models.UnregReadings
                        {
                            ErfNo = ReadingDetail.ErfNo,
                            MeterNo = ReadingDetail.MeterNo,
                            Reading = currentReadingValue,

                        });
                    }
                }
            }


            if (response > 0)
            {
                await Shell.Current.DisplayAlert("Reading Info Saved", "Record Saved", "OK");
                await ClearForm();
                await Task.Delay(1000);
                await GoBackAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Heads Up!", "Something went wrong while adding record", "OK");
                await ClearForm();
            }
        }

        [RelayCommand]
        public async Task GoBackAsync()
        {
            await Shell.Current.GoToAsync("..");
        }



        [RelayCommand]
        async Task ClearForm()
        {
            await Task.Yield();
            ReadingDetail.Reading =0;
            ReadingDetail.MeterNo =string.Empty;
            ReadingDetail.ErfNo =string.Empty;
        }

        //public async Task<List<UnregReadings>> CheckForExistingReadings(string meterNo)
        //{
        //    int ReadingValue = 0;

        //    if (!string.IsNullOrEmpty(meterNo))
        //    {
        //        if (int.TryParse(meterNo, out ReadingValue))
        //        {
        //            bool isExistingReading = await _unregReadingService.checkExistingReadingListById(ReadingValue);

        //            if (unregisteredReadingList.Count > 0)
        //            {
        //                await Shell.Current.DisplayAlert("Duplicate Records", "A reading with the same meter no was found!", "OK");
        //            }
        //            else
        //            {
        //                return unregisteredReadingList;
        //            }
        //        }
        //        else
        //        {
        //            // Handle the case where meterNo couldn't be parsed to an int
        //            await Shell.Current.DisplayAlert("Invalid Input", "Please enter a valid meter number.", "OK");
        //        }
        //    }

        //    return new List<UnregReadings>();
        //}

    }
}
