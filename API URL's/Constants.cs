
namespace SampleMauiMvvmApp.API_URL_s
{
    public static class Constants
    {
        public const string HOST = ListOfUrl.OkahaoTCStaff;
        //Month
        public const string GetMonth = HOST+"/api/Month";
        public const string PostMonth = HOST + "/api/Month";
        public const string GetMonthById = HOST + "/api/Month/{id}";
        public const string PutMonth = HOST + "/api/Month/{id}";
        public const string DeleteMonth = HOST + "/api/Month/{id}";
        //Reading Export
        public const string ReadingExport = HOST + "/api/ReadingExport";
        public const string ReadingExportById = HOST + "/api/ReadingExport/{id}";
        //Customer
        public const string GetCustomer = HOST+"/api/Customer";
        public const string PostCustomer = HOST+"/api/Customer";
        public const string GetCustomerById = HOST+"/api/Customer/{id}";
        public const string PutCustomer = HOST+"/api/Customer{id}";
        //Readings Data
        public const string GetReading = HOST+"/api/Reading";
        public const string PostReading = HOST+"/api/Reading";
        public const string SyncListOfReadingsToSql = HOST+ "/api/Reading/list";
        public const string SyncReadingByCustomerId = HOST + "api/reading/{id}";
        public const string GetReadingById = HOST+"/api/Reading/{id}";
        public const string GetWaterReadingExportDataID = HOST+ "/api/Reading/{WaterReadingExportDataID}";
        public const string PutReading = HOST+"/api/Reading/{id}";
        public const string DeleteReading = HOST+"/api/Reading/{id}";
        public const string SyncImages = HOST+"/api/Reading/Image/{id}";

        //Users
        public const string GetUser = HOST+"/api/Users";
        public const string GetUserById = HOST+"/api/Users/{id}";
        //Devices 
        public const string GetDevice = HOST+"/api/Device";
        public const string GetDeviceById = HOST+"/api/Device/{id}";
        //RM00303
        public const string GetRM00303 = HOST+"/api/RM00303";
        public const string GetRM00303ById = HOST+"/api/RM00303/{id}";
        //Login
        public const string PostLogin = HOST+"/api/login";
    }

    public static class ListOfUrl
    {
        //Kinetic
        public const string KineticWifi = "http://192.168.178.35:88";
        //Local
        public const string LocalIIS = "http://127.0.0.1";
        //TN Card
        public const string TnWifi = "http://192.168.8.118:86";
        //Home Wi-Fi
        public const string OkahaoHomeWifi = "http://192.168.178.78:88";
        //My Phone
        public const string SamsungA51 = "http://192.168.185.176:88";

        public const string RTCOFRuacanaTcWifi = "http://192.168.178.5:81";

        public const string RTCOFAPIWifi = "http://192.168.118.251:84";

        public const string OkahaoTCStaff = "http://192.168.1.4:8088";
        public const string RuacanaTcLocalPcDb = "http://192.168.178.72:88"; 
    }
}
