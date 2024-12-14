using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using RepReady.Data;
using RepReady.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

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



        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Show(int id)
        {
            Exercise exercise = db.Exercises
                                .Include(e => e.Comments) // Include Comments
                                .ThenInclude(c => c.User) // Include User for each Comment
                                .FirstOrDefault(e => e.Id == id);

            return View(exercise);
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = System.DateTime.Now;
            comment.WasEdited = false;
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
            int w_id = TempData["workoutId"] == null ? 0 : (int)TempData["workoutId"];
            Workout workout = db.Workouts.Where(workout => workout.Id == w_id)
                                         .First();


            if (workout.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                ViewBag.Workout = workout;

                Exercise exercise = new Exercise();
                return View(exercise);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa adăugați un exercițiu într-un antrenament care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Workouts/Show/" + workout.Id);
            }
        }


        // se adauga in baza de date
        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult New(Exercise exercise)
        {
            
            if(ModelState.IsValid)
            {
                db.Exercises.Add(exercise);
                db.SaveChanges();
                TempData["message"] = "Exercițiul a fost adaugat";
                return Redirect("/Workouts/Show/" + exercise.WorkoutId);
            }
            else
            {
                // FOR DEBUG
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                }

                return View(exercise);
            }
        }

        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Edit(int id)
        {
            Exercise? exercise = db.Exercises.Include("Workout").FirstOrDefault(e => e.Id == id);

            if (exercise == null)
            {
                return Content("Exercitiul nu exista in baza de date!");
            }

            if (exercise.Workout.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                ViewBag.WorkoutId = exercise.WorkoutId;
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
        public ActionResult Edit(int id, Exercise requestExercise)
        {
            Exercise exercise = db.Exercises.Find(id);

            if(ModelState.IsValid)
            {
                exercise.Title = requestExercise.Title;
                exercise.Description = requestExercise.Description;
                exercise.Reps = requestExercise.Reps;
                exercise.Sets = requestExercise.Sets;
                exercise.Status = requestExercise.Status;
                exercise.Start = requestExercise.Start;
                exercise.Finish = requestExercise.Finish;

                db.SaveChanges();
                TempData["message"] = "Exercitiul a fost modificat";
                return Redirect("/Workouts/Show/" + exercise.WorkoutId);
            }
            else
            {
                return View(requestExercise);
            }
        }



        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public ActionResult Delete(int id)
        {
            Exercise? exercise = db.Exercises.Include("Workout").FirstOrDefault(e => e.Id == id);
            if (exercise == null)
            {
                return Content("Exercitiul nu exista in baza de date!");
            }
            if (exercise.Workout.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                
                int? w_id = exercise.WorkoutId;  // Cannot convert from int? to int
                db.Exercises.Remove(exercise);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost sters";
                return Redirect("/Workouts/Show/" + w_id);
                
                
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti un exercitiu care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Workouts");
            }
                
        }


    }
}
