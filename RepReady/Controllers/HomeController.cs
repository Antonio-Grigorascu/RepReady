using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RepReady.Data;
using RepReady.Models;
using System.Diagnostics;

namespace RepReady.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext db;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        public HomeController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<HomeController> logger

            )
        {
            db = context;

            _userManager = userManager;

            _roleManager = roleManager;

            _logger = logger;

        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewBag.TemplateModel = db.ExerciseTemplates;
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        public IActionResult MakeOrganizer(string email)
        {
            var userId = db.Users.Where(u => u.Email == email).FirstOrDefault().Id;
            db.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = db.Roles.Where(r => r.Name == "Organizer").FirstOrDefault().Id
            });

            db.SaveChanges();

            return RedirectToAction("Dashboard");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            var userCount = db.Users.Count();
            var workoutCount = db.Workouts.Count();
            var exerciseCount = db.Exercises.Count();
            var commentCount = db.Comments.Count();
            var templateCount = db.ExerciseTemplates.Count();
            var categoryCount = db.Categories.Count();

            ViewBag.UserCount = userCount;
            ViewBag.WorkoutCount = workoutCount;
            ViewBag.ExerciseCount = exerciseCount;
            ViewBag.CommentCount = commentCount;
            ViewBag.TemplateCount = templateCount;
            ViewBag.CategoryCount = categoryCount;

            return View();
        }

    }

}
