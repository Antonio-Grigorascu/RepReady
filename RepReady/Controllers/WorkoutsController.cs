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
            SetAccessRights();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View(workout);
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Show([FromForm] Exercise ex)
        {
            if (ModelState.IsValid)
            {
                db.Exercises.Add(ex);
                db.SaveChanges();
                return Redirect("/Workouts/Show/" + ex.WorkoutId);
            }
            else
            {
                Workout workout = db.Workouts.Include("Category")
                                             .Include("Users")
                                             .Include("Exercises")
                                             .Include("Exercises.Comments")
                                             .Include("Exercises.Comments.User")
                                             .Where(workout => workout.Id == ex.WorkoutId)
                                             .First();
                //return Redirect("/Workouts/Show/" + ex.WorkoutId);
                SetAccessRights();
                return View(workout);
            }
        }

        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult New()
        {
            Workout workout = new Workout();
            workout.Categ = GetAllCategories();
            return View(workout);
        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult New(Workout workout)
        {
            workout.Date = DateTime.Now;
            workout.OrganizerId = _userManager.GetUserId(User);
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

        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult Edit(int id)
        {
            Workout workout = db.Workouts.Include("Category")
                                         .Where(workout => workout.Id == id)
                                         .First();
            workout.Categ = GetAllCategories();
            if (workout.OrganizerId == _userManager.GetUserId(User) ||User.IsInRole("Admin"))
            {
                return View(workout);
            }
            else
            { 
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra acestui antrenament care nu va apworkoutine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public IActionResult Edit(int id, Workout requestWorkout)
        {
            Workout workout = db.Workouts.Find(id);
            if (ModelState.IsValid)
            {
                if (workout.OrganizerId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
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
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui antrenament care nu va apworkoutine";
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
        [Authorize(Roles = "Organizer,Admin")]
        public ActionResult Delete(int id)
        {
            Workout workout = db.Workouts.Include("Exercises")
                                         .Include("Exercises.Comments")  
                                         .Where(workout => workout.Id == id)
                                         .First();
            if (workout.OrganizerId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Workouts.Remove(workout);
                db.SaveChanges();
                TempData["message"] = "Antrenamentul a fost sters";
                TempData["messageType"] = "alert-success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un antrenament care nu va apworkoutine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }

        [NonAction]
        private void SetAccessRights()
        {
            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.AfisareButoane = false;
            if (User.IsInRole("Organizer"))
            {
                ViewBag.AfisareButoane = true;
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
