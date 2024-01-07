using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel.DataAnnotations;

namespace MVCBeginner.Models
{
    public class UserSignup
    {
        [Required]
        public String Username { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [Compare("PasswordConfirm")]
        public String Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public String PasswordConfirm { get; set; }

    }
}
