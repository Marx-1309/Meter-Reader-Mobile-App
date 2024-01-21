﻿
namespace SampleMauiMvvmApp.Services
{
    public class BaseService : ObservableObject
    {
        protected readonly DbContext dbContext;
        protected readonly MonthService _monthService;
        protected readonly ReadingService _readingService;
        protected readonly CustomerService _customerService;
        protected readonly ReadingExportService _readingExportService;

        public string StatusMessage;

        public BaseService(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public BaseService(DbContext dbContext, MonthService monthService,
            ReadingService readingService, CustomerService customerService, ReadingExportService readingExportService)
        {
            this.dbContext = dbContext;
            _monthService = monthService;
            _ = Init(this.dbContext);
            _readingService = readingService;
            _customerService = customerService;
            _readingExportService = readingExportService;
        }



        public async Task Init(DbContext dbContext)
        {
            if (dbContext.Database is not null)
                return;

            dbContext.Database = new SQLiteAsyncConnection(DatabaseConstants.DatabasePath, DatabaseConstants.Flags);

            var migrationResult = await dbContext.Database.CreateTablesAsync(CreateFlags.None

                , typeof(SampleMauiMvvmApp.Models.Reading)
                , typeof(SampleMauiMvvmApp.Models.Month)
                , typeof(SampleMauiMvvmApp.Models.ReadingExport)
                , typeof(SampleMauiMvvmApp.Models.RM00303)
                , typeof(SampleMauiMvvmApp.Models.LoginHistory)
                , typeof(SampleMauiMvvmApp.Models.ReadingMedia)
                , typeof(SampleMauiMvvmApp.Models.Customer)
                , typeof(SampleMauiMvvmApp.Models.UnregReadings));



            if (migrationResult.Results != null && migrationResult.Results.Count > 0)
            {
                bool isNewDatabase = migrationResult.Results.Any(x => x.Value.ToString().ToUpper() == "CREATED");//this line checks if its the first run after migrations 
                if (isNewDatabase)
                {
                    await _customerService.GetListOfCustomerFromSql();
                    await SeedData(dbContext);
                    await _readingService.GetListOfPrevMonthReadingFromSql();
                }
                //Check if all the existing readings are synced!
                //await _readingExportService.ScanForNewExports();
            }
        }

        public async Task SeedData(DbContext dbContext)
        {
            await _readingExportService.CheckForNewExportInSql();
            //await _readingService.GetListOfReadingExportFromSql();

            #region Getting the latest export values(Id,Month & Year)
            var latestExportItem = await dbContext.Database.Table<ReadingExport>()
                       .OrderByDescending(r => r.WaterReadingExportID)
                       .FirstOrDefaultAsync();


            // If current month is January, adjust to December of previous year
            int currentExportId = latestExportItem.WaterReadingExportID;
            int currentMonthId = latestExportItem.MonthID;
            int currentYearId = latestExportItem.Year;
            if (currentMonthId == 0)
            {
                currentMonthId = 12;
                latestExportItem.Year -= 1;
            }
            #endregion



            List<Reading> GeneratedReadings = new();
            List<Customer> allCustomers = await dbContext.Database.Table<Customer>().ToListAsync();


            foreach (var customer in allCustomers)
            {
                var existingReading = await dbContext.Database.Table<Reading>()
                    .Where(r => r.CUSTOMER_NUMBER == customer.CUSTNMBR)
                    .FirstOrDefaultAsync();

                if (existingReading == null)
                {
                    var readingFaker = new ReadingFaker();
                    var reading = readingFaker.Generate(1).FirstOrDefault();

                    reading.CUSTOMER_NUMBER = customer.CUSTNMBR;
                    reading.CUSTOMER_NAME = customer.CUSTNAME;
                    reading.ERF_NUMBER = customer.ZIP;
                    //reading.AREA = customer.STATE;
                    reading.CUSTOMER_ZONING = customer.CUSTCLAS;
                    reading.CURRENT_READING = 0;
                    reading.Comment = string.Empty;
                    reading.MonthID = currentMonthId;
                    reading.Year = currentYearId;
                    //reading.Year = await _readingService.GetLatestExportItemYear() ?? reading.Year;
                    //reading.READING_DATE = DateTime.UtcNow.ToLongDateString();
                    reading.WaterReadingExportID = currentExportId;
                    reading.METER_READER = string.Empty;
                    reading.ReadingSync = false;
                    reading.ReadingNotTaken = false;

                    GeneratedReadings.Add(reading);
                }


            }
            await dbContext.Database.InsertAllAsync(GeneratedReadings);



        }
    }
}
