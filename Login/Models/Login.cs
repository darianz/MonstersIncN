using System.ComponentModel.DataAnnotations;

namespace Login.Models
{
    public class Loginm
    {

        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }
    }
}
