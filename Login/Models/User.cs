
using System.ComponentModel.DataAnnotations;

namespace Login.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }


       
        [MaxLength(50)]
        public string FirstName { get; set; }

       
        [MaxLength(50)]
        public string LastName { get; set; }


        [Phone]
        public string PhoneNumber { get; set; }

       
        public int TentaclesNumber { get; set; }

        
        public DateTime ScaringStartDate { get; set; }

        //public int RoleId { get; set; }
        //public Role Role { get; set; }

        [Required]
        
        public byte[] PasswordHash { get; set; }
        [Required]
        
        public byte[] PasswordSalt { get; set; }
    }
}
