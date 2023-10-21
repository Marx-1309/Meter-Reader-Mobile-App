using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleMauiMvvmApp.Models
{
    [Table("LoginHistory")]
    public class LoginHistory
    {
        [PrimaryKey, AutoIncrement]
        public int? LoginId { get; set; }
        public string? Username { get; set; }
        public string? loginDate { get; set; }

    }
}