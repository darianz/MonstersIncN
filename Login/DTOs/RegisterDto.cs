using System.ComponentModel.DataAnnotations;

namespace Login.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string FirstName { get; set; }


        [MaxLength(50)]
        public string LastName { get; set; }


        [Phone]
        public string PhoneNumber { get; set; }


        public int TentaclesNumber { get; set; }
    }
}
