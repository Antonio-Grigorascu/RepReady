using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var exercises = from exercise in db.Exercises
                            select exercise;

            ViewBag.Exercises = exercises;

            return View();
        }

        public IActionResult Show(int? id)
        {
            Exercise? exercises = db.Exercises.Find(id);
            if (exercises == null)
            {
                ViewBag.ErrorMessage = "Exercise not found.";
                return View("Error");
            }
            ViewBag.Exercise = exercises;
            ViewBag.PageName = "Show";
            return View();
        }


        [HttpGet]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult New(Exercise exercise)
        {
            try
            {
                db.Exercises.Add(exercise);

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return View();
            }
        }

        public IActionResult Edit(int? id)
        {
            Exercise? exercise = db.Exercises.Find(id);

            if (exercise == null)
            {
                return Content("Articolul nu exista in baza de date!");
            }

            ViewBag.Exercise = exercise;
            return View();
        }

        [HttpPost]
        public ActionResult Edit(int id, Exercise requestExercise)
        {
            Exercise? exercise = db.Exercises.Find(id);

            try
            {
                exercise.Title = requestExercise.Title;
                exercise.Description = requestExercise.Description;
                exercise.Reps = requestExercise.Reps;
                exercise.Sets = requestExercise.Sets;
                exercise.Status = requestExercise.Status;
                exercise.Start = requestExercise.Start;
                exercise.Finish = requestExercise.Finish;

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Edit", exercise.Id);
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
                return RedirectToAction("Index");
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound);
            }
        }


    }
}
