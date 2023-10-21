using Newtonsoft.Json;
using SampleMauiMvvmApp.Models;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Text.Json.Serialization;

namespace SampleMauiMvvmApp.Models
{
    [Table("Month")]
    public class Month
    {
        [JsonProperty]
        public int MonthID { get; set; }
        [JsonProperty]
        public string MonthName { get; set; }



        public string TitleProp => $"{MonthName} ";
    }


}
[JsonSerializable(typeof(List<Month>))]

internal sealed partial class MonthContext : JsonSerializerContext
{

}