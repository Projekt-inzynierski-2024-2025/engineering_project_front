﻿using System.ComponentModel.DataAnnotations;

namespace engineering_project_front.Models.Request
{
    public class TeamRequest
    {
        public long ID { get; set; }

        [Required(ErrorMessage = "Podaj nazwę zespołu.")]
        [MaxLength(32, ErrorMessage = "Nazwa zespołu może mieć maksymalnie 32 znaki.")]
        [RegularExpression(@"^[a-zA-Z0-9\sąćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$", ErrorMessage = "Nazwa zespołu może zawierać tylko litery, cyfry, spacje oraz polskie znaki.")]
        [Display(Name = "Nazwa zespołu")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Przypisz kierownika.")]
        [Range(1, long.MaxValue, ErrorMessage = "Wybierz kierownika.")]
        [Display(Name = "Kierownik")]
        public long ManagerID { get; set; }
    }
}
