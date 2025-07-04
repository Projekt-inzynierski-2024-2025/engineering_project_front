﻿using System.ComponentModel.DataAnnotations;

namespace engineering_project_front.Models.Request
{
    public class UserRequest
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane.")]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+(?:[' ]?[A-Za-zÀ-ÖØ-öø-ÿąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+)*$", ErrorMessage = "Imię może zawierać tylko litery.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [RegularExpression(@"^[A-Za-zÀ-ÖØ-öø-ÿąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+(?:[' ]?[A-Za-zÀ-ÖØ-öø-ÿąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+)*$", ErrorMessage = "Nazwisko może zawierać tylko litery.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email jest wymagany.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email musi mieć prawidłowy format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Wybór zespołu jest wymagany.")]
        [Range(1, long.MaxValue, ErrorMessage = "Wybierz zespół.")]
        public long TeamID { get; set; }

        [Required(ErrorMessage = "Wybór roli jest wymagany.")]
        [Range(0, 3, ErrorMessage = "Wybierz rolę.")]
        public int Role { get; set; }
    }
}
