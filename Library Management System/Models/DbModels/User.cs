
using Library_Management_System.Enum;

namespace Library_Management_System.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public String Role { get; set; }
    
    }
    
}
