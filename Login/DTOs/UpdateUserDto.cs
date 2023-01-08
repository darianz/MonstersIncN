using System.ComponentModel.DataAnnotations;

namespace Login.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public int TentaclesNumber { get; set; }
    }
}
