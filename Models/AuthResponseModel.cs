using Newtonsoft.Json;
using SampleMauiMvvmApp.Models;
using SQLite;
using System.Text.Json.Serialization;

namespace SampleMauiMvvmApp.Models
{
    [Table("AuthResponseModel")]
    public class AuthResponseModel
    {
        [JsonProperty]
        public string UserId { get; set; }
        [JsonProperty]
        public string Username { get; set; }
        [JsonProperty]
        public string Token { get; set; }
    }
}

[JsonSerializable(typeof(List<AuthResponseModel>))]

internal sealed partial class AuthResponseModelContext : JsonSerializerContext
{

}