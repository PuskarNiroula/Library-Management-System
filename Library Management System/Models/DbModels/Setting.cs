using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Models.DbModels
{
    [Index(nameof(Key), IsUnique = true)]
    public class Setting
    {
        [Key]
        public int SettingId { get; set; }

        [Required]
        [StringLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;
    }
}