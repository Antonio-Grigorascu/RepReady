using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RepReady.Models
{
    public class Workout
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(100, ErrorMessage = "Titlul nu poate avea mai mult de 100 de caractere")]
        [MinLength(3, ErrorMessage = "Titlul trebuie sa aiba mai mult de 3 caractere")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Descrierea antrenamentului este obligatorie")]
        public string Description { get; set; }

        public int Duration { get; set; }

        public DateTime Date { get; set; } // Data calendaristica

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int? CategoryId { get; set; }

        public string? OrganizerId { get; set; }

        public virtual Category? Category { get; set; }

        //public virtual ApplicationUser? Organizer { get; set; } // Organizatorul face CRUD

        public virtual ICollection<Exercise>? Exercises { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Categ { get; set; }


    }
}
