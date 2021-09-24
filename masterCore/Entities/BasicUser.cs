using System.ComponentModel.DataAnnotations;

namespace masterCore.Entities
{
    public class BasicUser
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public int IdGroup { get; set; }
    }
}
