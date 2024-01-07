using System.ComponentModel.DataAnnotations;

namespace MVCBeginner.Models
{
    public class User
    {
        public User()
        {

        }

        public int Id { get; set; }
        [Required]
        public string? Username { get; set; }
        [DataType(DataType.Password)]
        [Required]
        public string? Password { get; set; }

        public string? Salt { get; set; }
    }
}
