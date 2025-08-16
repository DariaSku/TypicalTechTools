using System.ComponentModel.DataAnnotations;

namespace TypicalTechTools.Models
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; }
    }
}
