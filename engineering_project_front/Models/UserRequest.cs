using System;
using System.ComponentModel.DataAnnotations;

namespace engineering_project_front.Models
{
    public class UserRequest
    {
        public long ID { get; set; }
        [Required(ErrorMessage = "Imię jest wymagane.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Email musi mieć prawidłową strukturę.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Wybór zespołu jest wymagany.")]
        public long TeamID { get; set; }

        [Required(ErrorMessage = "Wybór roli jest wymagany.")]
        public int Role { get; set; }
    }
}
