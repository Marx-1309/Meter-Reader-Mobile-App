using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMauiMvvmApp.Models
{
    public class ReadingMedia
    {
        [PrimaryKey]
        [Unique]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Data { get; set; }
        public int WaterReadingExportDataId { get; set; }
        public int WaterReadingExportId { get; set; }
        public string DateTaken { get;set; } = DateTime.UtcNow.ToLongDateString();
    }
}
