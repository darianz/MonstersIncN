using System.ComponentModel.DataAnnotations;

namespace Management.DTOs
{
    public class UserDetailsDto
    {

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }


        public int TentaclesNumber { get; set; }


        public DateTime ScaringStartDate { get; set; }
    }
}
