using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepReady.Models
{
    public class ApplicationUser : IdentityUser
    {

        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<Workout> Workouts { get; set; }

        //public virtual ICollection<Workout>? OrganizingWorkouts { get; set; }

        public virtual ICollection<Exercise> Exercises { get; set; }

        //public virtual ICollection<Exercise>? OrganizingExercises { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}
