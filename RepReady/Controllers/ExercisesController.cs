using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using RepReady.Data;
using RepReady.Models;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading.Tasks;

namespace RepReady.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        public ExercisesController(
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



        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Show(int id)
        {

            Exercise exercise = db.Exercises.Include("Users")
                                .Include("Comments")
                                .Include("Comments.User")  // All includes are for display
                                .FirstOrDefault(e => e.Id == id);

            var users = exercise.Users;
            var currentUser = db.Users.Find(_userManager.GetUserId(User));

            if (!users.Contains(currentUser))
            {
                TempData["message"] = "Nu aveti acces la acest exercitiu";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Workouts");
            }


            // For displaying the edit and delete buttons
            ViewBag.EsteAdmin = User.IsInRole("Admin");

            // For displaying the edit and delete buttons
            ViewBag.CurrentUser = _userManager.GetUserId(User);

            // For displaying the status of the exercise (complete/incomplete)
            ApplicationUserExercise? exercisesForUser = db.UserExercises
                    .Where(ue => ue.ExerciseId == id && ue.UserId == _userManager.GetUserId(User))
                    .FirstOrDefault();


            if (exercisesForUser == null)
            {
                ViewBag.Status = null;
            }
            else
            {
                ViewBag.Status = exercisesForUser.Status;
            }


            // For displaying the creator of the exercise
            ViewBag.CreatorName = db.Users.Find(exercise.CreatorId).UserName;

            // For displaying buttons
            ViewBag.EsteCreator = (exercise.CreatorId == _userManager.GetUserId(User));





            return View(exercise);
        }


        // Action for adding a comment to an exercise
        // Show because we are displaying the exercise and the comments and we want to preserve the current Exercise
        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            

            comment.Date = System.DateTime.Now;
            comment.WasEdited = false;

            // The user that left the comment is the current user
            comment.UserId = _userManager.GetUserId(User);

            // Check if the user is logged in and the UserId is not null
            if (string.IsNullOrEmpty(comment.UserId))
            {
                TempData["message"] = "Utilizatorul nu este autentificat. Încercați să vă autentificați din nou.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Workouts");
            }

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Exercises/Show/" + comment.ExerciseId);
            }
            else
            {
                // FOR DEBUG
                Console.WriteLine("EROARE");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                Exercise exercise = db.Exercises
                                    .Include("Users")
                                    .Include(e => e.Comments)        // Include comments
                                    .ThenInclude(c => c.User)        // Include the User for each comment
                                    .FirstOrDefault(e => e.Id == comment.ExerciseId);

                TempData["message"] = "Eroare de validare! Verificați câmpurile introduse.";
                TempData["messageType"] = "alert-danger";

                return View(exercise);
            }
        }


        [HttpGet]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult New()
        {
            // The id of the workout we add the exercise to
            int w_id = TempData["workoutId"] == null ? 0 : (int)TempData["workoutId"];

            // The workout we add the exercise to
            Workout workout = db.Workouts.Include("Users").Where(workout => workout.Id == w_id)
                                         .First();

            // The users that are part of the workout
            var users = db.Workouts.Include("Users").Where(w => w.Id == w_id).First().Users;

            if (workout.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Organizer"))
            {
                ViewBag.Workout = workout;  // For adding the workout id to the exercise (hidden)
                ViewBag.Users = users;      // For adding the users to the exercise (select checkboxes)

                Exercise exercise = new Exercise();

                ViewBag.ErrorMessage = TempData["errorMessage"];
                return View(exercise);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa adăugați un exercițiu într-un antrenament care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Workouts/Show/" + workout.Id);
            }
        }


        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public async Task<IActionResult> New(Exercise exercise, List<string> UsersIdList)
        {

            // UsersIdList is a list of user ids that would be added to the exercise - sent from the form (checkboxes)

            // Adding the image
            IFormFile Image = exercise.File;
            if (Image != null && Image.Length > 0)
            {
                // Check the extension
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".mp4", ".mov" };
                var fileExtension = Path.GetExtension(Image.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ExerciseImage", "Fișierul trebuie să fie o imagine(jpg, jpeg, png, gif) sau un video(mp4, mov).");
                    return View(exercise);
                }

                // Storage path
                var storagePath = Path.Combine(_env.WebRootPath, "images", Image.FileName);
                var databaseFileName = "/images/" + Image.FileName;

                // File saving
                using (var fileStream = new FileStream(storagePath, FileMode.Create))
                {
                    await Image.CopyToAsync(fileStream);
                }
                ModelState.Remove(nameof(exercise.Image));
                exercise.Image = databaseFileName;

            }


            if (TryValidateModel(exercise))
            {
                exercise.CreatorId = _userManager.GetUserId(User);

                exercise.Users = new List<ApplicationUser>(); // Initialize the list of users for the exercise


                // Add the exercise to the database and save it to generate the ID
                db.Exercises.Add(exercise);
                await db.SaveChangesAsync(); // Now `exercise.Id` is available

                // Add users to the exercise
                foreach (var userId in UsersIdList)
                {
                    ApplicationUser user = db.Users.Find(userId);
                    exercise.Users.Add(user); // Associate users directly to the exercise

                    // Add the user-exercise relation to the database
                    var userExercise = new ApplicationUserExercise
                    {
                        UserId = user.Id,
                        ExerciseId = exercise.Id, // Now `exercise.Id` is available
                        Status = -1
                    };
                    db.UserExercises.Add(userExercise);
                }

                await db.SaveChangesAsync();
                TempData["message"] = "Exercițiul a fost adaugat";
                return Redirect("/Workouts/Show/" + exercise.WorkoutId);
            }
            else
            {
                // FOR DEBUG
                Console.WriteLine("EROARE -------------------------------------");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        TempData["errorMessage"] =error.ErrorMessage;
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                TempData["workoutId"] = exercise.WorkoutId;
                
                return RedirectToAction("New");
            }
        }

        [HttpGet]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult NewTemplate(int templateId, int workoutId)
        {
            // The workout we add the exercise to
            Workout workout = db.Workouts.Include("Users").Where(workout => workout.Id == workoutId)
                                         .First();

            // The users that are part of the workout
            var users = db.Workouts.Include("Users").Where(w => w.Id == workoutId).First().Users;

            if (workout.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin") || User.IsInRole("Organizer"))
            {
                ViewBag.Workout = workout;  // For adding the workout id to the exercise (hidden)
                ViewBag.Users = users;      // For adding the users to the exercise (select checkboxes)

                Exercise exercise = new Exercise();

                var template = db.ExerciseTemplates.Find(templateId);

                exercise.Title = template.Title;
                exercise.Description = template.Description;
                exercise.Reps = template.Reps;
                exercise.Sets = template.Sets;
                exercise.Image = template.Image;
                exercise.WorkoutId = workoutId;


                ViewBag.ErrorMessage = TempData["errorMessage"];
                return View(exercise);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa adăugați un exercițiu într-un antrenament care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Workouts/Show/" + workout.Id);
            }
        }


        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Edit(int id)
        {
            // Get the exercise we want to edit
            Exercise? exercise = db.Exercises
                                    .Include("Workout")
                                    .Include("Users")
                                    .FirstOrDefault(e => e.Id == id);

            if (exercise == null)
            {
                return Content("Exercitiul nu exista in baza de date!");
            }

            // Get the users that are part of the workout
            var users = db.Workouts.Include("Users").Where(w => w.Id == exercise.WorkoutId).First().Users;

            if (exercise.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                // For adding the workout id to the exercise (hidden)
                ViewBag.WorkoutId = exercise.WorkoutId;

                // For adding the users to the exercise (select checkboxes)
                ViewBag.Users = users;

                // For selecting the users (in the checkboxes) that are already part of the exercise
                ViewBag.SelectedUsers = exercise.Users.Select(u => u.Id).ToList();

                // For displaying the error messages from the POST method
                ViewBag.ErrorMessage = TempData["errorMessage"];

                return View(exercise);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra acestui exercitiu care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Workouts/Show/" + exercise.WorkoutId);
            }
               
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public ActionResult Edit(int id, Exercise requestExercise, List<string> UsersIdList)
        {
            // UsersIdList is a list of user ids that would be added to the exercise - sent from the form (checkboxes)

            // Get the exercise we want to edit
            Exercise exercise = db.Exercises.Include("Users").Where(e => e.Id == id).First();

            // List of the users that were part of the exercise before the edit
            List<string> previousUsers = exercise.Users.Select(u => u.Id).ToList();

            // List of the users that are added to the exercise in the edit
            List<string>? addUsers = UsersIdList.Except(previousUsers).ToList();

            // List of the users that are removed from the exercise in the edit
            List<string>? removeUsers = previousUsers.Except(UsersIdList).ToList();

            if (ModelState.IsValid)
            {
                exercise.Title = requestExercise.Title;
                exercise.Description = requestExercise.Description;
                exercise.Reps = requestExercise.Reps;
                exercise.Sets = requestExercise.Sets;
                exercise.Start = requestExercise.Start;
                exercise.Finish = requestExercise.Finish;

                for (int i = 0; i < addUsers.Count; i++)
                {
                    // Add the users to the exercise
                    ApplicationUser user = db.Users.Find(addUsers[i]);
                    exercise.Users.Add(user);

                    // Add the user-exercise relation to the database
                    var userExercise = new ApplicationUserExercise
                    {
                        UserId = user.Id,
                        ExerciseId = exercise.Id, // Now `exercise.Id` is available
                        Status = -1
                    };
                    db.UserExercises.Add(userExercise);
                }

                for (int i = 0; i < removeUsers.Count; i++)
                {
                    // Remove the users from the exercise
                    ApplicationUser user = db.Users.Find(removeUsers[i]);

                    var userExercise = db.UserExercises
                        .Where(ue => ue.UserId == user.Id && ue.ExerciseId == exercise.Id)
                        .First();

                    db.UserExercises.Remove(userExercise);
                    exercise.Users.Remove(user);


                    
                }

                db.SaveChanges();
                TempData["message"] = "Exercitiul a fost modificat";
                TempData["messageType"] = "alert-success";
                return Redirect("/Workouts/Show/" + exercise.WorkoutId);
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        TempData["errorMessage"] = error.ErrorMessage;
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                //return View(requestExercise);
                TempData["workoutId"] = exercise.WorkoutId;

                return RedirectToAction("Edit");
            }
        }



        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public ActionResult Delete(int id)
        {
            // Get the exercise we want to delete
            Exercise? exercise = db.Exercises.Include("Workout").FirstOrDefault(e => e.Id == id);

            if (exercise == null)
            {
                return Content("Exercitiul nu exista in baza de date!");
            }

            if (exercise.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {

                // Get the workout id for redirecting after the exercise is deleted
                int? w_id = exercise.WorkoutId;  // Cannot convert from int? to int
                db.Exercises.Remove(exercise);
                db.SaveChanges();
                TempData["message"] = "Exercitiul a fost sters";
                TempData["messageType"] = "alert-success";
                return Redirect("/Workouts/Show/" + w_id);
                
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un exercitiu care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Workouts");
            }
                
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public ActionResult ChangeStatus(int ExerciseId, string UserId) {
            // Change status of the exercise (complete/incomplete)
            ApplicationUserExercise userExercise = db.UserExercises.Where(ue => ue.ExerciseId == ExerciseId && ue.UserId == UserId).First();
            
            if(userExercise.Status == -1)
            {
                // status = -1 means that the user has not started the exercise
                userExercise.Status = 0; // status = 0 means that the user has started the exercise
            }
            else if (userExercise.Status == 0)
            {
                // status = 0 means that the user has started the exercise
                userExercise.Status = 1; // status = 1 means that the user has completed the exercise

            }
            else
            {
                // status = 1 means that the user has completed the exercise
                userExercise.Status = -1; // status = -1 means that the user has not started the exercise

            }

            //userExercise.Status = !userExercise.Status;
            db.SaveChanges();
            return Redirect("/Exercises/Show/" + ExerciseId);
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public ActionResult AddUsers(int id) { 

            // Get the exercise we want to edit
            Exercise? exercise = db.Exercises
                                    .Include("Workout")
                                    .Include("Users")
                                    .FirstOrDefault(e => e.Id == id);

            if (exercise == null)
            {
                return Content("Exercitiul nu exista in baza de date!");
            }

            // Get the users that are part of the workout
            var users = db.Workouts.Include("Users").Where(w => w.Id == exercise.WorkoutId).First().Users;

            if (exercise.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                // For adding the workout id to the exercise (hidden)
                ViewBag.WorkoutId = exercise.WorkoutId;

                // For adding the users to the exercise (select checkboxes)
                ViewBag.Users = users;

                // For selecting the users (in the checkboxes) that are already part of the exercise
                ViewBag.SelectedUsers = exercise.Users.Select(u => u.Id).ToList();

                // For displaying the error messages from the POST method
                ViewBag.ErrorMessage = TempData["errorMessage"];

                return View(exercise);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra acestui exercitiu care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Workouts/Show/" + exercise.WorkoutId);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public ActionResult AddUsers(int id, List<string> UsersIdList)
        {
            // UsersIdList is a list of user ids that would be added to the exercise - sent from the form (checkboxes)

            // Get the exercise we want to edit
            Exercise exercise = db.Exercises.Include("Users").Where(e => e.Id == id).First();

            // List of the users that were part of the exercise before the edit
            List<string> previousUsers = exercise.Users.Select(u => u.Id).ToList();

            // List of the users that are added to the exercise in the edit
            List<string>? addUsers = UsersIdList.Except(previousUsers).ToList();

            // List of the users that are removed from the exercise in the edit
            List<string>? removeUsers = previousUsers.Except(UsersIdList).ToList();

            if (ModelState.IsValid)
            {

                for (int i = 0; i < addUsers.Count; i++)
                {
                    // Add the users to the exercise
                    ApplicationUser user = db.Users.Find(addUsers[i]);
                    exercise.Users.Add(user);

                    // Add the user-exercise relation to the database
                    var userExercise = new ApplicationUserExercise
                    {
                        UserId = user.Id,
                        ExerciseId = exercise.Id, // Now `exercise.Id` is available
                        Status = -1
                    };
                    db.UserExercises.Add(userExercise);
                }

                for (int i = 0; i < removeUsers.Count; i++)
                {
                    // Remove the users from the exercise
                    ApplicationUser user = db.Users.Find(removeUsers[i]);

                    var userExercise = db.UserExercises
                        .Where(ue => ue.UserId == user.Id && ue.ExerciseId == exercise.Id)
                        .First();

                    db.UserExercises.Remove(userExercise);
                    exercise.Users.Remove(user);

                }

                db.SaveChanges();
                TempData["message"] = "Exercitiul a fost modificat";
                TempData["messageType"] = "alert-success";
                return Redirect("/Workouts/Show/" + exercise.WorkoutId);
            }
            else
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        TempData["errorMessage"] = error.ErrorMessage;
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                //return View(requestExercise);
                TempData["workoutId"] = exercise.WorkoutId;

                return RedirectToAction("Edit");
            }
        }



    }
}
