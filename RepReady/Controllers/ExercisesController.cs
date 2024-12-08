using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        [NonAction]
        public Exercise[] GetExercises()
        {
            Exercise[] exercises = new Exercise[3];

            for(int i = 0; i < 3; i++)
            {
                Exercise exercise = new Exercise();
                exercise.Id = i;

                exercise.Title = "Exercise " + (i + 1).ToString();
                exercise.Description = "Description " + (i + 1).ToString();
                exercise.Reps = 10;
                exercise.Sets = 4;
                exercise.Status = false;
                exercise.Start = System.DateTime.Now;
                exercise.Finish = System.DateTime.Now;
                exercises[i] = exercise;

            }
            return exercises;
        }



        public IActionResult Index()
        {
            //var exercises = from exercise in db.Exercises
            //                select exercise;

            var exercise = db.Exercises;

            ViewBag.Exercises = exercise;

            return View();
        }

        public IActionResult Show(int? id)
        {
            //Exercise? exercises = db.Exercises.Find(id);



            //if (exercises == null)
            //{
            //    ViewBag.ErrorMessage = "Exercise not found.";
            //    return View("Error");
            //}
            //ViewBag.Exercise = exercises;
            //ViewBag.PageName = "Show";
            //return View();

            Exercise exercise = db.Exercises.Include("Comments").Where(e => e.Id == id).First();

            //ViewBag.Exercise = exercise;
            return View(exercise);
        }

        [HttpPost]
        public IActionResult Show([FromForm] Comment comment)
        {
            comment.Date = System.DateTime.Now;
            comment.WasEdited = false;
            comment.UserId = _userManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return Redirect("/Exercises/Show/" + comment.ExerciseId);
            }
            else
            {
                Console.WriteLine("EROARE");
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage); // Log or debug this
                    }
                }

                Exercise exercise = db.Exercises.Include("Comments")
                                                .Where(exercise => exercise.Id == comment.ExerciseId)
                                                .First();

                //return Redirect("/Exercises/Show/" + comment.ExerciseId);
                return View(exercise);
            }
        }


        [HttpGet]
        public IActionResult New()
        {
            var workouts = db.Workouts.Select(w => new
            {
                w.Id,
                w.Name
            }).ToList();

            ViewBag.Workouts = new SelectList(workouts, "Id", "Name");

            Exercise exercise = new Exercise();
            return View(exercise);
        }


        // se adauga in baza de date
        [HttpPost]
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
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine(error.ErrorMessage); // Log or debug this
                    }
                }

                var workouts = db.Workouts.Select(w => new
                {
                    w.Id,
                    w.Name
                }).ToList();
                ViewBag.Workouts = new SelectList(workouts, "Id", "Name");

                return View(exercise);
            }
        }

        public IActionResult Edit(int? id)
        {
            //Exercise? exercise = db.Exercises.Find(id);
            var exercise = db.Exercises.FirstOrDefault(e => e.Id == id);

            if (exercise == null)
            {
                return Content("Articolul nu exista in baza de date!");
            }

            
            return View(exercise);
        }

        // Se adauga comentariul modifcat in baza de date
        [HttpPost]
        public ActionResult Edit(int id, Exercise requestExercise)
        {
            Exercise exercise = db.Exercises.Find(id);

            if(ModelState.IsValid) // problema cu validarea
            {
                exercise.Title = requestExercise.Title;
                exercise.Description = requestExercise.Description;
                exercise.Reps = requestExercise.Reps;
                exercise.Sets = requestExercise.Sets;
                exercise.Status = requestExercise.Status;
                exercise.Start = requestExercise.Start;
                exercise.Finish = requestExercise.Finish;

                db.SaveChanges();
                TempData["message"] = "Articolul a fost modificat";
                return RedirectToAction("Index");
            }
            else
            {
                return View(requestExercise);
            }
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            Exercise? exercise = db.Exercises.Find(id);

            if (exercise != null)
            {
                db.Exercises.Remove(exercise);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost sters";
                return RedirectToAction("Index");
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }


    }
}
