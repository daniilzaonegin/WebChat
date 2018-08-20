using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebChat_Model.Entities
{
    [Table("users")]
    public class User
    {
        
        public string Email { get; set; }
        [Key]
        [Required(ErrorMessage ="Please enter your name")]
        public string Name { get; set; }
        public string Surname { get; set; }
        [Required(ErrorMessage = "Password couldn't be empty")]
        public string Password { get; set; }       
    }
}