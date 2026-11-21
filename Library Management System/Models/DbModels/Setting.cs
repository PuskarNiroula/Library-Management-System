using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models.DbModels
{
    public class Setting
    {
        [Key]
        public int SettingId { get; set; }

        [Required]
        [StringLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        [Required]
        public string SettingValue { get; set; } = string.Empty;
    }
}