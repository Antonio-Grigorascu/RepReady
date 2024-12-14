using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepReady.Data;
using RepReady.Models;

namespace RepReady.Controllers
{
    public class WorkoutsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public WorkoutsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Index()
        {

            var workouts = db.Workouts.Include("Category")
                                      .Include("Users")        
                                      .OrderByDescending(a => a.Date);
            ViewBag.Workouts = workouts;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View();
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Show(int id)
        {
            Workout workout = db.Workouts.Include("Category")
                                         .Include("Exercises")
                                         .Include("Users")
                                         .Where(workout => workout.Id == id)
                                         .First();

            ApplicationUser user = db.Users.Where(u => u.Id == workout.CreatorId).First();
            ViewBag.CreatorName = user.UserName;

            SetAccessRights();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View(workout);
        }


        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult New()
        {
            Workout workout = new Workout();
            workout.Categ = GetAllCategories();
            return View(workout);
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult New(Workout workout)
        {
            workout.Date = DateTime.Now;
            workout.CreatorId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                db.Workouts.Add(workout);
                db.SaveChanges();
                TempData["message"] = "Antrenamentul a fost adaugat";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                workout.Categ = GetAllCategories();
                return View(workout);
            }
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id)
        {
            Workout workout = db.Workouts.Include("Category")
                                         .Where(workout => workout.Id == id)
                                         .First();
            workout.Categ = GetAllCategories();
            if (workout.CreatorId == _userManager.GetUserId(User) ||User.IsInRole("Admin"))
            {
                return View(workout);
            }
            else
            { 
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra acestui antrenament care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Edit(int id, Workout requestWorkout)
        {
            Workout workout = db.Workouts.Find(id);
            if (ModelState.IsValid)
            {
                if (workout.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    workout.Name = requestWorkout.Name;
                    workout.Description = requestWorkout.Description;
                    workout.Duration = requestWorkout.Duration;
                    workout.Date = DateTime.Now;
                    workout.CategoryId = requestWorkout.CategoryId;
                    TempData["message"] = "Antrenamentul a fost modificat";
                    TempData["messageType"] = "alert-success";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui antrenament care nu va apartine";
                    TempData["messageType"] = "alert-danger";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                requestWorkout.Categ = GetAllCategories();
                return View(requestWorkout);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult Delete(int id)
        {
            Workout workout = db.Workouts.Include("Exercises")
                                         .Include("Exercises.Comments")  
                                         .Where(workout => workout.Id == id)
                                         .First();
            if (workout.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Workouts.Remove(workout);
                db.SaveChanges();
                TempData["message"] = "Antrenamentul a fost sters";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un antrenament care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [NonAction]
        private void SetAccessRights()
        {
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.EsteOrganizer = false;
            if (User.IsInRole("Organizer"))
            {
                ViewBag.EsteOrganizer = true;
            }
        }

        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var selectList = new List<SelectListItem>();
            var categories = from cat in db.Categories
                             select cat;
            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                {
                    Value = category.Id.ToString(),
                    Text = category.Name
                });
            }
            return selectList;
        }

    }
}
