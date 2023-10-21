using SQLite;
using SQLiteNetExtensions.Attributes;

namespace SampleMauiMvvmApp.Models
{
    [Table("Device")]
    public class Device
    {
        public Device()
        {
            this.Active = false;
            this.LastActive = DateTime.Now;
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } = DeviceInfo.Name.Length;
        public string SerialNumber { get; set; }

        [ForeignKey(typeof(User), Name = "Id")]
        public int UserId { get; set; }
        public DateTime LastActive { get; set; } = DateTime.Now;
        public bool Active { get; set; }

    }
}
