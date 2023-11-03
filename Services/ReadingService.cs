using AutoMapper;
using SampleMauiMvvmApp.API_URL_s;
using SampleMauiMvvmApp.Mappings.Dto_s;
using SampleMauiMvvmApp.Models;
using System.Net.Http.Json;

namespace SampleMauiMvvmApp.Services
{
    public class ReadingService : BaseService
    {
        HttpClient _httpClient;
        private readonly IMapper _mapper;
        AuthenticationService _authenticationService;
        MonthService _monthService;
        public ReadingService(DbContext dbContext, IMapper mapper,
            AuthenticationService authenticationService, MonthService monthService) : base(dbContext)
        {
            this._httpClient = new HttpClient();
            this._mapper = mapper;
            this._authenticationService = authenticationService;
            this._monthService = monthService;
            string StatusMessage = String.Empty;
        }


        public async Task<List<Reading>> GetReadingsByCustomerId(string customerId)
        {
            try
            {

                return await dbContext.Database.Table<Reading>().Where(x => x.CUSTOMER_NUMBER == customerId).ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. {ex.Message}";
            }

            return null;
        }

        public async Task<List<Reading>> GetReadingsByMonthId(int monthId)
        {
            try
            {
                await _authenticationService.SetAuthToken();
                var readingslist = await dbContext.Database.Table<Reading>().Where(x => x.MonthID == monthId && x.CURRENT_READING != 0 || x.CURRENT_READING! < 0).ToListAsync();
                return readingslist;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. {ex.Message}";
            }

            return null;
        }

        public async Task<int> CountReadingsByCustomerId(string customerId)
        {
            try
            {
                return await dbContext.Database
                    .Table<Reading>()
                    .Where(x => x.CUSTOMER_NUMBER == customerId)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. {ex.Message}";
            }

            return 0;
        }

        public async Task<Reading> InsertReading(Reading reading)
        {
            try
            {
                await dbContext.Database.UpdateAsync(reading);

                return reading;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to insert record. {ex.Message}";
            }

            return null;
        }

        public async Task<Reading> UpdateReading(Reading reading)
        {
            try
            {
                await dbContext.Database.UpdateAsync(reading);

                return reading;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to update record. {ex.Message}";
            }

            return null;
        }

        public async Task<Reading> DeleteReading(Reading reading)
        {
            try
            {
                await dbContext.Database.DeleteAsync(reading);

                return reading;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to delete record. {ex.Message}";
            }

            return null;
        }


        public async Task<List<Reading>> GetAllUncapturedByIdAsync(Customer customerId)
        {
            try
            {
                return await dbContext.Database.Table<Reading>()
                    .Where(x => x.CURRENT_READING <= 0 && x.CUSTOMER_NUMBER == customerId.CUSTNMBR)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. {ex.Message}";
            }

            return null;
        }

        public async Task<List<Reading>> GetListOfUncapturedReadingsByMonthId(int MonthId)
        {
            try
            {
                var response = await dbContext.Database.Table<Reading>()
                    .Where(r => r.CURRENT_READING <= 0 && r.MonthID == MonthId)
                    .ToListAsync();
                if (response.Count > 0)
                { return response; }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. {ex.Message}";
            }
            return null;
        }


        public async Task<List<Reading>> GetListOfUncapturedReadings()
        {
            try
            {
                var response = await dbContext.Database.Table<Reading>()
                    .Where(r => r.CURRENT_READING <= 0 && r.MonthID > 0)
                    .OrderBy(r => r.MonthID)
                    .ThenBy(r => r.CUSTOMER_NUMBER)
                    .ToListAsync();

                if (response.Count > 0)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. {ex.Message}";
            }

            return null;
        }


        public async Task<List<Reading>> GetListOfCapturedReadings()
        {
            try
            {
                var response = await dbContext.Database.Table<Reading>()
                    .Where(r => r.CURRENT_READING > 0 && r.MonthID > 0)
                    .OrderBy(r => r.MonthID)
                    .ThenBy(r => r.CUSTOMER_NUMBER)
                    .ToListAsync();

                if (response.Count > 0)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. {ex.Message}";
            }

            return null;
        }


        public async Task<Reading> GetLastReadingByIdAsync(string Id)
        {
            var lastExportItem = await dbContext.Database.Table<ReadingExport>()
                                      .OrderByDescending(r => r.WaterReadingExportID)
                                      .FirstOrDefaultAsync();

            //var currentMonthId = await dbContext.Database.Table<ReadingExport>()
            //.OrderByDescending(r => r.MonthID)
            //.FirstOrDefaultAsync();


            //var yearOfLastMonth = await dbContext.Database.Table<ReadingExport>()
            //.OrderByDescending(r => r.Year)
            //.FirstOrDefaultAsync();


            int prevMonthId = lastExportItem.MonthID;

            if (prevMonthId == 0) // If current month is January, adjust to December of previous year
            {
                prevMonthId = 12;
                //yearOfLastMonth.Year -= 1;
            }

            var PreviousReading = await dbContext.Database.Table<Reading>()
                .Where(r => r.CUSTOMER_NUMBER == Id && r.MonthID == prevMonthId)
                .FirstOrDefaultAsync();

            return PreviousReading;
        }

        public async Task<Reading> GetCurrentMonthReadingByCustIdAsync(string Id)
        {
            try
            {
                var currentMonth = await _monthService.GetLatestExportItemMonthId();

                if (currentMonth != null)
                {
                    var zeroMonthReading = await dbContext.Database.Table<Reading>()
                        .Where(r => r.CUSTOMER_NUMBER == Id && r.MonthID == 0)
                        .FirstOrDefaultAsync();

                    if (zeroMonthReading != null)
                    {
                        return zeroMonthReading;
                    }
                    else
                    {
                        var currentMonthReading = await dbContext.Database.Table<Reading>()
                            .Where(r => r.CUSTOMER_NUMBER == Id && r.MonthID == currentMonth)
                            .FirstOrDefaultAsync();

                        return currentMonthReading;
                    }
                }
                else
                {
                    // Handle the case when currentMonth is null (e.g., if it's not found)
                    // You might return null or throw an exception based on your requirements.
                    // For example:
                    throw new Exception("Could not retrieve current month ID.");
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the database operation
                StatusMessage = $"Error: {ex.Message}";
                return null;
            }
        }



        public async Task<List<Reading>> GetListOfReadingsNotSynced()
        {
            try
            {

                return await dbContext.Database.Table<Reading>().Where(r => r.ReadingSync == false).ToListAsync();

            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed to retrieve data. {ex.Message}";
            }
            return null;
        }

        public static int allReadingsItemsByCount=0;
        public async Task<int> SyncReadingsByMonthIdAsync(int Id)
        {
            try
            {
                if (Id != 0 || Id < 0)
                {
                    var r = await dbContext.Database.Table<Reading>()
                        .Where(r => r.MonthID == Id && r.ReadingSync == false && r.CURRENT_READING != 0 && r.WaterReadingExportDataID != 0)
                        .OrderBy(r => r.READING_DATE).ToListAsync();

                    //var loggedInUser = await dbContext.Database.Table<LoginHistory>().OrderByDescending(r => r.LoginId).FirstAsync();

                    var response = _mapper.Map<List<UpdateReadingDto>>(r);

                    if (response.Count>0)
                    {
                        // Initialize a count variable to keep track of the number of items processed.
                        int itemCount = 0;

                        foreach (var item in response)
                        {
                            //item.METER_READER = loggedInUser.Username;
                            item.Comment = item.Comment;
                            // Perform the update for each item in 'response'.
                            var IsSyncSuccess = await _httpClient.PutAsJsonAsync(Constants.PutReading, item);
                            StatusMessage = IsSyncSuccess.ReasonPhrase.ToString();

                            if (IsSyncSuccess.IsSuccessStatusCode)
                            {
                                // Update the item in the local database.
                                var updatedItem = await dbContext.Database.Table<Reading>()
                                    .Where(r => r.WaterReadingExportDataID == item.WaterReadingExportDataID)
                                    .FirstOrDefaultAsync();


                                if (updatedItem != null)
                                {
                                    updatedItem.ReadingTaken = true;
                                    updatedItem.ReadingSync = true;

                                    // Save the changes to the local database.
                                    await dbContext.Database.UpdateAsync(updatedItem);
                                }

                                // Increment the count for each successfully processed item.
                                itemCount++;
                            }
                            else
                            {
                                StatusMessage = IsSyncSuccess.IsSuccessStatusCode.ToString();
                                // Handle any failure cases here, if needed.
                            }
                        }

                        
                        await SyncImages();

                        // After processing all items, store the count in the static variable.
                        allReadingsItemsByCount = itemCount;
                        return allReadingsItemsByCount;

                        
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message.ToString();
            }

            // Return the default value of allItemsByCount if no items are processed.
            return allReadingsItemsByCount;
        }


        public static int allImageItemsByCount=0;
        public async Task<int> SyncImages()
        {
            int Count = 0;
            try
            {
                var lastExportItem = await dbContext.Database.Table<ReadingExport>()
                                     .OrderByDescending(r => r.WaterReadingExportID)
                                     .FirstOrDefaultAsync();

                var images = await dbContext.Database
                    .Table<ReadingMedia>()
                    .Where(r => r.WaterReadingExportId == lastExportItem.WaterReadingExportID && r.IsSynced != true)
                    .ToArrayAsync();

                var response = _mapper.Map<List<ImageSyncDto>>(images);

                if (response.Count>0)
                {
                    // Initialize a count variable to keep track of the number of items processed.
                    int itemCount = 0;

                    foreach (var item in response)
                    {
                        var IsSyncSuccess = await _httpClient.PutAsJsonAsync(Constants.SyncImages, item);
                        StatusMessage = IsSyncSuccess.ReasonPhrase.ToString();

                        if (IsSyncSuccess.IsSuccessStatusCode)
                        {
                            // Update the item in the local database.
                            var updatedItem = await dbContext.Database.Table<ReadingMedia>()
                                .Where(r => r.WaterReadingExportDataId == item.WaterReadingExportDataId)
                                .FirstOrDefaultAsync();


                            if (updatedItem != null)
                            {
                                updatedItem.IsSynced = true;

                                // Save the changes to the local database.
                                await dbContext.Database.UpdateAsync(updatedItem);
                            }

                            // Increment the count for each successfully processed item.
                            itemCount++;
                        }
                        else
                        {
                            StatusMessage = IsSyncSuccess.IsSuccessStatusCode.ToString();
                            // Handle any failure cases here, if needed.
                        }
                    }
                    var imagesToDelete = await dbContext.Database.Table<ReadingMedia>().Where(i => i.IsSynced == true).ToListAsync();

                    foreach (var i in imagesToDelete)
                    {
                        await dbContext.Database.DeleteAsync(i);
                    }

                    // After processing all items, store the count in the static variable.
                    allImageItemsByCount = itemCount;
                    return allImageItemsByCount;
                }
                return allImageItemsByCount;
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message.ToString();
            }
            return allImageItemsByCount;
        }


        public async Task<bool> IsReadingExistForMonthId(string customer)
        {
            var latestMonthName = await _monthService.GetLatestExportItemMonthId();

            var response = await dbContext.Database.Table<Reading>()
                                 .Where(r => r.MonthID == latestMonthName && r.CUSTOMER_NUMBER == customer)
                                 .ToListAsync();
            if (response.Count > 0)
            {
                return true;
            }
            return false;
        }

        public async Task<List<Reading>> GetAllUncapturedReadings()
        {
            var lastExportItem = await dbContext.Database.Table<ReadingExport>()
                    .OrderByDescending(r => r.WaterReadingExportID)
                    .FirstOrDefaultAsync();



            var ListOfAllReading = await dbContext.Database.Table<Reading>()
                .Where(r => r.WaterReadingExportID == lastExportItem.WaterReadingExportID
                && r.MonthID == lastExportItem.MonthID

                && r.CURRENT_READING == 0)
                .OrderBy(r => r.ERF_NUMBER)
                //.ThenBy(r=>r.CUSTOMER_NUMBER)
                .ToListAsync();
            if (ListOfAllReading.Count > 0)
            {
                return ListOfAllReading;
            }
            return null;
        }

        public async Task<List<Reading>> GetAllCapturedReadings()
        {
            var lastExportItem = await dbContext.Database.Table<ReadingExport>()
                    .OrderByDescending(r => r.WaterReadingExportID)
                    .FirstOrDefaultAsync();

            var ListOfAllReading = await dbContext.Database.Table<Reading>()
                .Where(r => r.WaterReadingExportID == lastExportItem.WaterReadingExportID
                && r.MonthID == lastExportItem.MonthID
                //&& r.Year == lastExportYearItem.Year
                && r.CURRENT_READING > 0)
                .OrderBy(r => r.ERF_NUMBER)
                //.ThenBy(r=>r.CUSTOMER_NUMBER)
                .ToListAsync();
            if (ListOfAllReading.Count > 0)
            {
                return ListOfAllReading;
            }
            return null;
        }

        #region GetListOfReadingFromSql

        List<Reading> readings;
        public async Task<List<ReadingDto>> GetListOfReadingFromSql()
        {
            if (readings == null)
            {
                try
                {
                    var readingsCount = await dbContext.Database.Table<Reading>().Where(c => c.WaterReadingExportDataID >= 1).ToListAsync();
                    if (readingsCount.Count > 0)
                    {
                        // Retrieve all the IDs of the existing Reading items in the SQLite database
                        var existingIds = readingsCount.Select(r => r.WaterReadingExportDataID).ToList();

                        var response = await _httpClient.GetAsync(SampleMauiMvvmApp.API_URL_s.Constants.GetReading);

                        if (response.IsSuccessStatusCode)
                        {
                            // Read and deserialize the response to a List<Reading>
                            var newReadings = await response.Content.ReadFromJsonAsync<List<Reading>>();

                            // Filter the new Reading items to get only the ones that do not exist in the SQLite database
                            var newItemsToInsert = newReadings.Where(r => !existingIds.Contains(r.WaterReadingExportDataID)).ToList();

                            if (newItemsToInsert.Any())
                            {
                                // Insert the new items into the SQLite database
                                var response2 = await dbContext.Database.InsertAllAsync(newItemsToInsert);

                                // Update the readings list to include both existing items and new items
                                foreach (var item in newItemsToInsert)
                                {
                                    readings?.Add(item);
                                }
                            }
                        }
                        else
                        {
                            // Handle unsuccessful response, maybe throw an exception or log an error
                            StatusMessage = $"Failed :." + response.StatusCode;
                        }
                    }
                    else
                    {
                        var response = await _httpClient.GetAsync(SampleMauiMvvmApp.API_URL_s.Constants.ReadingExport);

                        if (response.IsSuccessStatusCode)
                        {
                            // Read and deserialize the response to a List<Reading>
                            readings = await response.Content.ReadFromJsonAsync<List<Reading>>();

                            // Insert all items into the SQLite database since there are no existing items
                            var response2 = await dbContext.Database.InsertAllAsync(readings);
                        }
                        else
                        {
                            // Handle unsuccessful response, maybe throw an exception or log an error
                            StatusMessage = $"Failed :." + response.StatusCode;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any other exception that might occur during the API call
                    StatusMessage = $"Error." + ex.Message;
                }
            }

            // Use AutoMapper to map the Reading objects to ReadingDto objects
            var readingDtos = _mapper.Map<List<ReadingDto>>(readings);

            // Return the ReadingDto list, even if it's null (client code should handle this)
            return readingDtos;
        }
        #endregion



        #region Get ReadingExport

        List<ReadingExport> readingExports;

        public async Task<List<ReadingExport>> GetListOfReadingExportFromSql()
        {
            if (readingExports == null)
            {
                try
                {
                    var readingsCount = await dbContext.Database.Table<ReadingExport>().Where(c => c.WaterReadingExportID >= 1).ToListAsync();
                    if (readingsCount.Count > 0)
                    {
                        // Retrieve all the IDs of the existing ReadingExport items in the SQLite database
                        var existingIds = readingsCount.Select(r => r.WaterReadingExportID).ToList();

                        var response = await _httpClient.GetAsync(SampleMauiMvvmApp.API_URL_s.Constants.ReadingExport);

                        if (response.IsSuccessStatusCode)
                        {
                            // Read and deserialize the response to a List<ReadingExport>
                            var newReadingExports = await response.Content.ReadFromJsonAsync<List<ReadingExport>>();



                            // Filter the new ReadingExport items to get only the ones that do not exist in the SQLite database
                            var newItemsToInsert = newReadingExports.Where(r => !existingIds.Contains(r.WaterReadingExportID)).ToList();

                            if (newItemsToInsert.Any())
                            {
                                // Insert the new items into the SQLite database
                                var response2 = await dbContext.Database.InsertAllAsync(newItemsToInsert);


                                // Update the readingExports list to include both existing items and new items
                                foreach (var items in newItemsToInsert)
                                {
                                    readingExports?.Add(items);
                                }

                            }
                        }
                        else
                        {
                            // Handle unsuccessful response, maybe throw an exception or log an error
                            StatusMessage = $"Failed :." + response.StatusCode;
                        }
                    }
                    else
                    {
                        var response = await _httpClient.GetAsync(SampleMauiMvvmApp.API_URL_s.Constants.ReadingExport);

                        if (response.IsSuccessStatusCode)
                        {
                            // Read and deserialize the response to a List<ReadingExport>
                            readingExports = await response.Content.ReadFromJsonAsync<List<ReadingExport>>();

                            // Insert all items into the SQLite database since there are no existing items
                            var response2 = await dbContext.Database.InsertAllAsync(readingExports);
                        }
                        else
                        {
                            // Handle unsuccessful response, maybe throw an exception or log an error
                            StatusMessage = $"Failed :." + response.StatusCode;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any other exception that might occur during the API call
                    StatusMessage = $"Error." + ex.Message;
                }
            }

            // Return the ReadingExport list, even if it's null (client code should handle this)
            return null;
        }
        #endregion


        public async Task<int?> GetLatestExportItemId()
        {
            try
            {
                var lastItem = await dbContext.Database.Table<ReadingExport>()
                    .OrderByDescending(r => r.WaterReadingExportID)
                    .FirstOrDefaultAsync();

                // If the lastItem is not null, return its ID; otherwise, return null.
                return lastItem?.WaterReadingExportID;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the database operation
                StatusMessage = $"Error: {ex.Message}";
                return null;
            }
        }

        public async Task<int?> GetLatestExportItemMonthId()
        {
            try
            {
                var lastItem = await dbContext.Database.Table<ReadingExport>()
                    .OrderByDescending(r => r.MonthID)
                    .FirstOrDefaultAsync();

                // If the lastItem is not null, return its ID; otherwise, return null.
                return lastItem?.MonthID;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the database operation
                StatusMessage = $"Error: {ex.Message}";
                return null;
            }
        }


        public async Task<int?> GetLatestExportItemYear()
        {
            try
            {
                var lastItem = await dbContext.Database.Table<ReadingExport>()
                    .OrderByDescending(r => r.Year)
                    .FirstOrDefaultAsync();

                // If the lastItem is not null, return its Year; otherwise, return null.
                return lastItem?.Year;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the database operation
                StatusMessage = $"Error: {ex.Message}";
                return null;
            }
        }

        #region Check if there are existing PrevMonth readings in the Sqlite database 

        public async Task<bool> IsPrevMonthReadingsExist()
        {
            try
            {
                var lastExportItemTask = dbContext.Database.Table<ReadingExport>()
                    .OrderByDescending(r => r.WaterReadingExportID)
                    .FirstOrDefaultAsync();


                var lastExportItem = await lastExportItemTask;
                var lastExportMonth = await lastExportItemTask;
                var lastExportYear = await lastExportItemTask;

                var ItemID = lastExportItem.WaterReadingExportID;
                var monthID = lastExportMonth.MonthID;
                var yearId = lastExportYear.Year;
                // Use the 'monthID' variable as needed.

                int ii = monthID - 1;
                int xx = lastExportYear.Year;

                if (ii == 0) // If current month is January, adjust to December of the previous year
                {
                    ii = 12;
                    xx = xx - 1;
                }

                var results = await dbContext.Database.Table<Reading>()
                    .Where(x => x.MonthID == ii && x.Year == xx)
                    .ToListAsync();

                if (results == null || results.Count == 0)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
                return false;
            }
        }
        #endregion


        #region Download Prev Month Readings 
        List<Reading> prevMonthReadings;
        List<Reading> NonMatchingReadings = new();
        public async Task<List<ReadingDto>> GetListOfPrevMonthReadingFromSql()
        {
            bool result = (bool)await IsPrevMonthReadingsExist();
            if (!result)
            {
                try
                {
                    var lastExportItem = await dbContext.Database.Table<ReadingExport>()
                           .OrderByDescending(r => r.WaterReadingExportID)
                           .FirstOrDefaultAsync();


                    // If current month is January, adjust to December of previous year
                    int prevMonthId = lastExportItem.MonthID;
                    int prevYearId = lastExportItem.Year;
                    if (prevMonthId == 0)
                    {
                        prevMonthId = 12;
                        lastExportItem.Year -= 1;
                    }

                    var readingsCount = await dbContext.Database.Table<Reading>().Where(c => c.WaterReadingExportDataID >= 1
                    && c.MonthID == prevMonthId
                    && c.Year == prevYearId).ToListAsync();


                    if (readingsCount.Count < 0)
                    {

                        // Handle unsuccessful response, maybe throw an exception or log an error
                        StatusMessage = $"Previous Readings Exist!";
                        return null;
                    }
                    else
                    {
                        var response = await _httpClient.GetAsync(SampleMauiMvvmApp.API_URL_s.Constants.GetReading);

                        if (response.IsSuccessStatusCode)
                        {
                            // Read and deserialize the response to a List<Reading>
                            var readingsFromSqlServer = await response.Content.ReadFromJsonAsync<List<ReadingDto>>();
                            var lastExportItemx = await dbContext.Database.Table<ReadingExport>()
                          .OrderByDescending(r => r.WaterReadingExportID)
                          .FirstOrDefaultAsync();

                            // If current month is January, adjust to December of previous year
                            //int prevMonthIdx = lastExportMonthItem.MonthID - 1;
                            int currentMonth = lastExportItem.MonthID;
                            int prevYearIdx = lastExportItemx.Year;
                            if (currentMonth == 0)
                            {
                                currentMonth = 12;
                                lastExportItemx.Year -= 1;
                            }
                            var currentExportReadings = readingsFromSqlServer.Where(r => r.MonthID == currentMonth && r.PREVIOUS_READING > 0).ToList();

                            List<Reading> readingsToUpdateToSqlite = new();
                            List<Reading> readingsToDeleteFromSqlite = new();

                            foreach (var readingDto in currentExportReadings)
                            {

                                // Find matching records in SQLite
                                var matchingRecords = await dbContext.Database.Table<Reading>()
                                    .Where(r => r.CUSTOMER_NUMBER == readingDto.CUSTOMER_NUMBER && r.MonthID == currentMonth && r.CURRENT_READING == 0)
                                    .ToListAsync();
                                if (matchingRecords.Count != 0)
                                {
                                    // Update the matching records
                                    foreach (var record in matchingRecords)
                                    {
                                        record.WaterReadingExportDataID = readingDto.WaterReadingExportDataID;
                                        record.METER_NUMBER = readingDto.METER_NUMBER.Trim();
                                        record.PREVIOUS_READING = readingDto.PREVIOUS_READING;

                                        readingsToUpdateToSqlite.Add(record);
                                        // Update other properties as needed.
                                    }

                                }
                            }
                            var r = await dbContext.Database.UpdateAllAsync(readingsToUpdateToSqlite);
                            readingsToUpdateToSqlite.Clear();

                            List<Reading> nonMatchingRecords = await dbContext.Database.Table<Reading>().Where(r => r.WaterReadingExportDataID == 0).ToListAsync();

                            foreach (var item in nonMatchingRecords)
                            {
                                await dbContext.Database.DeleteAsync(item);
                            }

                            // Insert all items into the SQLite database since there are no existing items
                            //var response2 = await dbContext.Database.InsertAllAsync(readingsFromSqlServer);

                        }
                        else
                        {
                            // Handle unsuccessful response, maybe throw an exception or log an error
                            StatusMessage = $"Error :." + response.StatusCode;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any other exception that might occur during the API call
                    StatusMessage = $"Error." + ex.Message;
                }
            }
            return null;
        }
        #endregion

        public async Task<List<Reading>> GetAllCaptureAndUncapturedReadings()
        {
            var lastExportItem = await dbContext.Database.Table<ReadingExport>()
                    .OrderByDescending(r => r.WaterReadingExportID)
                    .FirstOrDefaultAsync();

            var ListOfAllReading = await dbContext.Database.Table<Reading>()
                .Where(r => r.WaterReadingExportID == lastExportItem.WaterReadingExportID
                && r.MonthID == lastExportItem.MonthID && r.WaterReadingExportDataID != 0
               )
                .OrderBy(r => r.READING_DATE)
                .ThenBy(r => r.CUSTOMER_NUMBER)
                .ToListAsync();
            if (ListOfAllReading.Count > 0)
            {
                return ListOfAllReading;
            }
            return null;
        }

    }
}