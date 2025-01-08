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
                new ExerciseTemplate { Title = "Push-Up", Description = "Push-ups are a bodyweight exercise that target the chest, triceps, and shoulders. Start in a plank position with hands slightly wider than shoulder-width, body in a straight line from head to heels. Lower your chest to the floor by bending your elbows, then push back up to the starting position. Keep your core tight and avoid sagging your hips.", Reps = 20, Sets = 3, Image = "/images/pushup.jpg" },
                new ExerciseTemplate { Title = "Pull-Up", Description = "Pull-ups are a bodyweight exercise that target the back, biceps, and shoulders. Start by hanging from a bar with an overhand grip, hands slightly wider than shoulder-width. Engage your core and pull your chin above the bar, squeezing your shoulder blades together. Lower yourself back down with control. Keep your body straight and avoid swinging.", Reps = 10, Sets = 3, Image = "/images/pullup.jpg" },
                new ExerciseTemplate { Title = "Squat", Description = "The squat is a key lower-body exercise targeting the quadriceps, hamstrings, glutes, and core. Stand with feet shoulder-width apart, chest up, and core engaged. Lower by pushing your hips back and bending your knees until thighs are parallel to the ground, then press through your heels to stand. Keep your chest lifted and knees aligned with your toes.", Reps = 8, Sets = 5, Image = "/images/squat.webp" },

                new ExerciseTemplate { Title = "Lunges", Description = "Lunges are a lower-body exercise that target the quadriceps, hamstrings, glutes, and calves. Start by standing tall with feet hip-width apart. Step forward with one leg and lower your body until both knees are at 90-degree angles, keeping your chest up and back straight. Push through the front heel to return to the starting position. Alternate legs with each rep.", Reps = 16, Sets = 4, Image = "/images/lunges.jpg" },
                new ExerciseTemplate { Title = "Deadlift", Description = "The deadlift is a powerful exercise targeting the glutes, hamstrings, lower back, and core. Start with feet hip-width apart, barbell over mid-foot, and grip the bar just outside your knees. With a straight back and engaged core, hinge at the hips, bending your knees slightly. Lift the bar by driving through your heels, extending your hips and knees until standing upright. Lower the bar by hinging at the hips, keeping it close to your body.", Reps = 12, Sets = 4, Image = "/images/deadlift.webp" },
                new ExerciseTemplate { Title = "Leg raises", Description = "Hanging leg raises target the lower abdominals, hip flexors, and grip strength. Hang from a pull-up bar with your arms fully extended and legs straight. Engage your core and raise your legs in front of you, keeping them straight, until they reach about hip height. Slowly lower them back down with control. Avoid swinging your body. ", Reps = 10, Sets = 4, Image = "/images/legraises.jpg" },

                new ExerciseTemplate { Title = "Bench Press", Description = "The bench press is a core upper-body exercise targeting the chest, triceps, and shoulders. Lie on a bench with feet flat on the floor and grip the barbell slightly wider than shoulder-width. Lower the bar to your chest with control, then press it back up until your arms are fully extended. Keep your core engaged and avoid arching your back.", Reps = 6, Sets = 6, Image = "/images/benchpress.png" },
                new ExerciseTemplate { Title = "Shoulder Press", Description = "The shoulder press is an upper-body exercise targeting the deltoids, triceps, and upper chest. Start by standing or sitting with a barbell or dumbbells at shoulder height, palms facing forward. Engage your core and press the weight overhead until your arms are fully extended, then slowly lower it back to the starting position. Keep your back straight and avoid leaning.", Reps = 10, Sets = 3, Image = "/images/shoulderpress.webp" },
                new ExerciseTemplate { Title = "Tricep Dips", Description = "Triceps dips are an upper-body exercise that target the triceps, shoulders, and chest. Start by placing your hands on a bench or parallel bars, arms straight and feet flat on the floor. Lower your body by bending your elbows until they reach a 90-degree angle, then press back up to the starting position. Keep your chest up and avoid shrugging your shoulders.", Reps = 15, Sets = 3, Image = "/images/dips.jpg" }

            };


            foreach (var exercise in exercises)
            {
                db.ExerciseTemplates.Add(exercise);
            }
            db.SaveChanges();
        }
    }
}
