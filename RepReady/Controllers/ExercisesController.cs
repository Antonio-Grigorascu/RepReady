using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RepReady.Data;
using RepReady.Models;

namespace RepReady.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public ExercisesController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
    // TO DO: AntonioG
}
