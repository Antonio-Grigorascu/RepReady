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

            var userId = _userManager.GetUserId(User);

            // List of workouts
            List<Workout> workoutsPage;

            if (User.IsInRole("Admin"))
            {
                var workouts = db.Workouts.Include("Category").OrderBy(w => w.Date);

                // Pass the workouts to the view for display
                ViewBag.Workouts = workouts;
                workoutsPage = workouts.ToList();
            }
            else
            {

                // Select workouts created by the current user
                var workoutsCreated = db.Workouts
                    .Include(w => w.Category)
                    .Include(w => w.Users)
                    .Where(w => w.CreatorId == userId)
                    .ToList();

                // Select workouts in which the current user is participating
                var workoutsParticipating = db.Users
                    .Include(u => u.Workouts)
                    .ThenInclude(w => w.Category)
                    .Where(u => u.Id == userId)
                    .FirstOrDefault()?.Workouts
                    .ToList() ?? new List<Workout>(); // In case the user has no workouts we return an empty list

                // Set of all workouts order by date
                var workouts = new HashSet<Workout>(workoutsCreated)
                                .Union(new HashSet<Workout>(workoutsParticipating))
                                .OrderBy(w => w.Date)
                                .ToList();


                // Pass the workouts to the view for display
                ViewBag.Workouts = workouts;
                workoutsPage = workouts.ToList();
            }

            // Workouts per page
            int _perPage = 3;

            


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            ViewBag.InvitationsCount = db.WorkoutInvitations
                                        .Where(wi => wi.UserId == userId && wi.Accepted == false)
                                        .Count();

            // Total number of workouts
            int totalWorkouts = workoutsPage.Count;

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedWorkouts = workoutsPage.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalWorkouts / (float)_perPage);

            ViewBag.Workouts = paginatedWorkouts;

            ViewBag.PaginationBaseUrl = "/Workouts/Index?page";

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


            var users = workout.Users;
            var currentUser = db.Users.Find(_userManager.GetUserId(User));

            if (!users.Contains(currentUser))
            {
                TempData["message"] = "Nu aveti acces la acest antrenament";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Workouts");
            }

            // Get the creator of the workout
            ApplicationUser user = db.Users.Where(u => u.Id == workout.CreatorId).First();

            // Order the workout's exercises by start time
            workout.Exercises = workout.Exercises.OrderBy(exercise => exercise.Start).ToList();

            // Pass the creator's name to the view for display (in partial)
            ViewBag.CreatorName = user.UserName;

            SetAccessRights(); // For button visibility

            // Check if the current user is the creator of the workout, for button visibility
            ViewBag.EsteCreator = true;
            if (user.Id != _userManager.GetUserId(User))
            {
                ViewBag.EsteCreator = false;
            }

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            var exercisesPage = workout.Exercises;

            // Exercises per page
            int _perPage = 3;

            // Total number of exercises
            int totalExercises = exercisesPage.Count;

            var currentPage = Convert.ToInt32(HttpContext.Request.Query["page"]);

            var offset = 0;

            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }

            var paginatedExercises = exercisesPage.Skip(offset).Take(_perPage);

            ViewBag.lastPage = Math.Ceiling((float)totalExercises / (float)_perPage);

            ViewBag.Exercises = paginatedExercises;

            ViewBag.PaginationBaseUrl = "/Workouts/Show/" + workout.Id + "/?page";


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

            // The user that creates the workout is the current user
            workout.CreatorId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                workout.Users = new List<ApplicationUser>();
                // Add the creator to the list of users
                workout.Users.Add(db.Users.Where(w => w.Id == workout.CreatorId).First());
                
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
                                         .Include("Users")
                                         .Where(workout => workout.Id == id)
                                         .First();

            workout.Categ = GetAllCategories();

            var users = db.Workouts.Include("Users").Where(w => w.Id == workout.Id).First().Users;

            if (workout.CreatorId == _userManager.GetUserId(User) ||User.IsInRole("Admin"))
            {
                // For adding the users to the exercise (select checkboxes)
                ViewBag.Users = users;

                // For selecting the users (in the checkboxes) that are already part of the exercise
                ViewBag.SelectedUsers = workout.Users.Select(u => u.Id).ToList();

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
        public IActionResult Edit(int id, Workout requestWorkout, List<string> UsersIdList)
        {
            Workout workout = db.Workouts.Include("Users")
                                         .Where(workout => workout.Id == id)
                                         .First();

            // List of the users that were part of the workout before the edit
            List<string> previousUsers = workout.Users.Select(u => u.Id).ToList();

            // List of the users that are added to the workout in the edit
            List<string>? addUsers = UsersIdList.Except(previousUsers).ToList();

            // List of the users that are removed from the workout in the edit
            List<string>? removeUsers = previousUsers.Except(UsersIdList).ToList();

            if (ModelState.IsValid)
            {
                if (workout.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    workout.Name = requestWorkout.Name;
                    workout.Description = requestWorkout.Description;
                    workout.Duration = requestWorkout.Duration;
                    workout.Date = DateTime.Now;
                    workout.CategoryId = requestWorkout.CategoryId;

                    for (int i = 0; i < addUsers.Count; i++)
                    {
                        // Add the users to the workout
                        ApplicationUser user = db.Users.Find(addUsers[i]);
                        workout.Users.Add(user);
                    }

                    for (int i = 0; i < removeUsers.Count; i++)
                    {
                        // Remove the users from the workout
                        ApplicationUser user = db.Users.Find(removeUsers[i]);  
                        workout.Users.Remove(user);
                    }

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
            //Workout workout = db.Workouts.Include("Exercises") // Include exercises for delete in cascade
            //                             .Include("Exercises.Comments")  // Include comments for delete in cascade
            //                             .Where(workout => workout.Id == id)
            //                             .First();

            Workout? workout = db.Workouts.FirstOrDefault(w => w.Id == id);

            

            if (workout == null)
            {
                return Content("Exercitiul nu exista in baza de date!");
            }

            if (workout.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                var exercises = db.Exercises.Where(e => e.WorkoutId == id);
                foreach (var exercise in exercises)
                {
                    db.Exercises.Remove(exercise);
                }
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
            // For button visibility
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
            // For listing them in form (as dropdown list)
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

        [Authorize(Roles = "Admin")]
        public IActionResult TakeOver(int id)
        {
            Workout workout = db.Workouts.Include("Users").Where(w => w.Id == id).First();
            workout.CreatorId = _userManager.GetUserId(User);
            var currentUser = db.Users.Where(u => u.Id == workout.CreatorId).First();
            if (!workout.Users.Contains(currentUser))
            {
                workout.Users.Add(currentUser);
            }
            db.SaveChanges();
            return Redirect("/Workouts/Show/" + id);
        }


    }
}
