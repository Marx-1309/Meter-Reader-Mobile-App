
namespace SampleMauiMvvmApp.Services
{
    public interface IUnregReadingService
    {
        Task<int> AddUnregReading(UnregReadings readingModel);
        Task<bool> CheckExistingReadingListById(int Id);
        Task<int> DeleteUnregReading(UnregReadings readingModel);
        Task<List<UnregReadings>> GetUnregReadingList();
        Task<int> UpdateUnregReading(UnregReadings readingModel);
    }

    public class UnregReadingService : IUnregReadingService
    {
        protected readonly DbContext _dbConnection;
        public UnregReadingService(DbContext dbContext)
        {
            this._dbConnection = dbContext;
        }
        public async Task<int> AddUnregReading(UnregReadings readingModel)
        {

            return await _dbConnection.Database.InsertAsync(readingModel);
        }

        public async Task<int> DeleteUnregReading(UnregReadings readingModel)
        {
            return await _dbConnection.Database.DeleteAsync(readingModel);
        }

        public async Task<List<UnregReadings>> GetUnregReadingList()
        {

            List<UnregReadings> readingList = await _dbConnection.Database.Table<UnregReadings>().ToListAsync();
            return readingList;
        }

        public async Task<bool> CheckExistingReadingListById(int Id)
        {
            string i = Id.ToString();

            var readingListById = await _dbConnection.Database
                .Table<UnregReadings>()
                .Where(r => r.MeterNo == i)
                .ToListAsync();

            if (readingListById.Any())
            {
                return true;
            };
            return false;
        }


        public async Task<int> UpdateUnregReading(UnregReadings readingModel)
        {
            return await _dbConnection.Database.UpdateAsync(readingModel);
        }
    }
}
