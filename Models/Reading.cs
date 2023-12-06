using SQLite;
using SQLiteNetExtensions.Attributes;
using SampleMauiMvvmApp.ModelWrappers;
using Bogus.DataSets;
using Microsoft.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Xml.Linq;
using System.ComponentModel.DataAnnotations;

namespace SampleMauiMvvmApp.Models
{

    public class Reading
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int WaterReadingExportDataID { get; set; }
        [ForeignKey(typeof(ReadingExport), Name = "WaterReadingExportID")]
        public int WaterReadingExportID { get; set; }

        [ForeignKey(typeof(Customer), Name = "CUSTNMBR")]
        public string CUSTOMER_NUMBER { get; set; }

        [ForeignKey(typeof(Customer), Name = "CUSTNAME")]
        public string CUSTOMER_NAME { get; set; }

        [ForeignKey(typeof(Customer), Name = "UPSZONE")]
        public string AREA { get; set; } 
        [ForeignKey(typeof(Customer), Name = "ZIP")]
        public string ERF_NUMBER { get; set; }
        public string? METER_NUMBER { get; set; }
        public decimal CURRENT_READING { get; set; } 
        public decimal? PREVIOUS_READING { get; set; }
        [ForeignKey(typeof(Month), Name = "Id")]
        public int MonthID { get; set; }
        public int Year { get; set; }
        [ForeignKey(typeof(Customer), Name = "UPSZONE")]
        public string? CUSTOMER_ZONING { get; set; }
        public string? RouteNumber { get; set; }
        public string Comment { get; set; }
        public int? WaterReadingTypeId { get; set; }

        [ForeignKey(typeof(User), Name = "Username")]
        public string? METER_READER { get; set; }
        public string? ReadingDate { get; set; }  /*= DateTime.UtcNow.ToLocalTime();*/
        public bool? ReadingTaken { get; set; }
        [Ignore]
        public bool ReadingNotTaken { get; set; }
        public bool? ReadingSync { get; set; }

        [Ignore]
        public string? ReadingInfo
        {
            get
            {
                return $"{CUSTOMER_NAME}{ERF_NUMBER}{AREA}{METER_NUMBER}";
            }
        }

        [Ignore]
        private int? PercentageChange { get; set; }
        

        public static Reading GenerateNewFromWrapper(ReadingWrapper wrapper)
        {
            return new Reading()
            {
                Id = wrapper.Id,
                WaterReadingExportDataID = wrapper.WaterReadingExportDataID,
                WaterReadingExportID = wrapper.WaterReadingExportId,
                CUSTOMER_NUMBER = wrapper.Customer_number,
                CUSTOMER_NAME = wrapper.Customer_name,
                AREA = wrapper.Area,
                ERF_NUMBER = wrapper.Erf_number,
                METER_NUMBER = wrapper.Meter_number,
                CURRENT_READING = (decimal)wrapper.Current_reading,
                PREVIOUS_READING = wrapper.Previous_reading,
                PercentageChange = wrapper.PercentageChange,
                MonthID = wrapper.MonthID,
                Year = wrapper.Year,
                CUSTOMER_ZONING = wrapper.Customer_zoning,
                RouteNumber = wrapper.RouteNumber,
                METER_READER = wrapper.MeterReader,
                //READING_DATE = wrapper.ReadingDate,
                ReadingTaken = wrapper.ReadingTaken,
                ReadingNotTaken = wrapper.ReadingNotTaken,
                ReadingSync = wrapper.ReadingSync,
                Comment = wrapper.Comment,
            };
        }
    }
}
