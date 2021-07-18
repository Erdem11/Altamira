using System.ComponentModel.DataAnnotations;

namespace Altamira.Api.Models.Users.Login
{
    public class LoginRequest
    {
        [Required]
        [MinLength(3)]
        public string UserName { get; set; }
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}