using Microsoft.AspNetCore.Mvc.Rendering;
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

        public int Reps { get; set; }

        public int Sets { get; set; }

        public bool Status { get; set; }

        public DateTime Start { get; set; }

        public DateTime Finish { get; set; }

        //public file media

        [Required(ErrorMessage = "Antenamentul este obligatoriu")]
        public int? WorkoutId { get; set; }

        public virtual Workout? Workout { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<ApplicationUser>? Users { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? WorkoutList { get; set; }


    }
}
