﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RepReady.Models
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
        [MinLength(5, ErrorMessage = "Titlul trebuie sa aiba mai mult de 5 caractere")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Descrierea exercitiului este obligatorie")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Numarul de repetari este obligatoriu")]
        [Range(1, 32, ErrorMessage = "Numarul de repetari trebuie sa fie intre 1 si 32")]
        public int Reps { get; set; }

        [Required(ErrorMessage = "Numarul de seturi este obligatoriu")]
        [Range(1, 8, ErrorMessage = "Numarul de seturi trebuie sa fie intre 1 si 8")]
        public int Sets { get; set; }

        [Required(ErrorMessage = "Data de început este obligatorie")]
        public DateTime Start { get; set; }  // In case the user want to plan the workout for the future

        [Required(ErrorMessage = "Data de început este obligatorie")]
        [DateValidation]
        public DateTime Finish { get; set; }  // In case the user want to plan the workout for the future

        // path to media file
        public string? Image { get; set; }

        public string? CreatorId { get; set; }
        public int? WorkoutId { get; set; }

        public virtual Workout? Workout { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<ApplicationUser>? Users { get; set; }

        public virtual ICollection<ApplicationUserExercise>? UserExercises { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }

    }
}
