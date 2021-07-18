using System.ComponentModel.DataAnnotations;

namespace Altamira.Api.Models.Users
{
    public class UserModel
    {
        
        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [MinLength(3)]
        public string Username { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Website { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}