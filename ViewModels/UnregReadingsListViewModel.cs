
namespace SampleMauiMvvmApp.ViewModels
{
    public partial class UnregReadingsListViewModel : ObservableObject
    {
        public static List<UnregReadings> UnregReadingsListForSearch { get; private set; } = new List<UnregReadings>();
        public ObservableCollection<UnregReadings> Readings { get; set; } = new ObservableCollection<UnregReadings>();

        private readonly UnregReadingService _unregReadingService;

        public UnregReadingsListViewModel(UnregReadingService unregReadingService)
        {
            _unregReadingService = unregReadingService;
        }

        [RelayCommand]
        public async Task GetReadingsList()
        {
            Readings.Clear();
            var unregisteredReadingList = await _unregReadingService.GetUnregReadingList();
            if (unregisteredReadingList?.Count > 0)
            {
                unregisteredReadingList = unregisteredReadingList.OrderBy(f => f.FullName).ToList();
                foreach (var reading in unregisteredReadingList)
                {
                    Readings.Add(reading);
                }
                UnregReadingsListForSearch.Clear();
                UnregReadingsListForSearch.AddRange(unregisteredReadingList);
            }
        }


        [RelayCommand]
        public async Task AddUpdateUnregReading()
        {
            await AppShell.Current.GoToAsync(nameof(UpsertUnregReadingPage));
        }

        [RelayCommand]
        public async Task EditUnregReading(UnregReadings readingModel)
        {
            var navParam = new Dictionary<string, object>();
            navParam.Add("ReadingDetail", readingModel);
            await AppShell.Current.GoToAsync(nameof(UpsertUnregReadingPage), navParam);
        }

        [RelayCommand]
        public async Task DeleteUnregReading(UnregReadings readingModel)
        {
            var delResponse = await _unregReadingService.DeleteUnregReading(readingModel);
            if (delResponse > 0)
            {
                await GetReadingsList();
            }
        }


        [RelayCommand]
        public async Task DisplayAction(UnregReadings readingModel)
        {
            var response = await AppShell.Current.DisplayActionSheet("Select Option", "OK", null, "Edit", "Delete");
            if (response == "Edit")
            {
                var navParam = new Dictionary<string, object>();
                navParam.Add("ReadingDetail", readingModel);
                await AppShell.Current.GoToAsync(nameof(UpsertUnregReadingPage), navParam);
            }
            else if (response == "Delete")
            {
                var delResponse = await _unregReadingService.DeleteUnregReading(readingModel);
                if (delResponse > 0)
                {
                    await GetReadingsList();
                }
            }
        }
    }
}
