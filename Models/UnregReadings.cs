using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMauiMvvmApp.Models
{
    public class UnregReadings
    {
        [PrimaryKey, AutoIncrement]
        public int ReadingID { get; set; }
        public string ErfNo { get; set; }
        public string MeterNo { get; set; }
        public Decimal Reading { get; set; }
        [NotMapped]
        public string current_Reading { get; set; }
        [Ignore]
        public string FullName => $"{ErfNo} {MeterNo}";
    }
}
