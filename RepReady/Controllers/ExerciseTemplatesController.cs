using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RepReady.Data;
using RepReady.Models;

namespace RepReady.Controllers
{
    public class ExerciseTemplatesController : Controller
    {

        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        public ExerciseTemplatesController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment env
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _env = env;
        }
        public IActionResult Index(int workoutId)
        {
            InitializeExercises();
            var exercises = db.ExerciseTemplates.ToList();
            ViewBag.ExerciseTemplates = exercises;
            ViewBag.WorkoutId = workoutId;
            return View();
        }
        public void InitializeExercises()
        {
            ExerciseTemplate? template = db.ExerciseTemplates.FirstOrDefault();
            if (template != null)
            {
                return;
            }
            var exercises = new List<ExerciseTemplate>
            {
                new ExerciseTemplate { Title = "Push-Up", Description = "A bodyweight exercise to strengthen the chest and triceps.", Reps = 10, Sets = 3, Image = "/images/pushup.jpg" },
                new ExerciseTemplate { Title = "Pull-Up", Description = "An upper body exercise focusing on the back and biceps.", Reps = 8, Sets = 3, Image = "/images/pullup.jpg" },
                new ExerciseTemplate { Title = "Squat", Description = "A compound lower-body exercise targeting the legs.", Reps = 12, Sets = 3, Image = "/images/squat.jpg" },
                new ExerciseTemplate { Title = "Deadlift", Description = "A strength exercise for the posterior chain.", Reps = 5, Sets = 4, Image = "/images/deadlift.jpg" },
                new ExerciseTemplate { Title = "Bench Press", Description = "A chest exercise performed with a barbell.", Reps = 8, Sets = 4, Image = "/images/benchpress.jpg" },
                new ExerciseTemplate { Title = "Plank", Description = "A core stabilization exercise.", Reps = 1, Sets = 3, Image = "/images/plank.jpg" },
                new ExerciseTemplate { Title = "Lunges", Description = "A lower-body exercise for balance and strength.", Reps = 10, Sets = 3, Image = "/images/lunges.jpg" },
                new ExerciseTemplate { Title = "Bicep Curl", Description = "An isolation exercise for the biceps.", Reps = 12, Sets = 3, Image = "/images/bicepcurl.jpg" },
                new ExerciseTemplate { Title = "Tricep Dips", Description = "An arm exercise targeting the triceps.", Reps = 10, Sets = 3, Image = "/images/tricepdips.jpg" },
                new ExerciseTemplate { Title = "Shoulder Press", Description = "A pressing exercise for the shoulders.", Reps = 10, Sets = 4, Image = "/images/shoulderpress.jpg" },
                new ExerciseTemplate { Title = "Mountain Climbers", Description = "A cardio and core exercise.", Reps = 20, Sets = 3, Image = "/images/mountainclimbers.jpg" },
                new ExerciseTemplate { Title = "Burpees", Description = "A full-body exercise for cardio and strength.", Reps = 12, Sets = 3, Image = "/images/burpees.jpg" },
                new ExerciseTemplate { Title = "Leg Press", Description = "A machine-based lower-body exercise.", Reps = 10, Sets = 4, Image = "/images/legpress.jpg" },
                new ExerciseTemplate { Title = "Lat Pulldown", Description = "An upper-body exercise for the lats and biceps.", Reps = 10, Sets = 4, Image = "/images/latpulldown.jpg" },
                new ExerciseTemplate { Title = "Sit-Ups", Description = "A core exercise for abdominal strength.", Reps = 15, Sets = 3, Image = "/images/situps.jpg" }
            };

            foreach (var exercise in exercises)
            {
                db.ExerciseTemplates.Add(exercise);
            }
            db.SaveChanges();
        }
    }
}
