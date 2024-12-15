using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RepReady.Data;
using RepReady.Models;

namespace RepReady.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public CommentsController(
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
        public IActionResult Edit(int id)
        {
            // Find the comment we want to edit
            Comment comm = db.Comments.Find(id);
            
            if (comm == null) // For route protection
            {
                TempData["message"] = "Comentariul nu a fost găsit.";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Workouts");
            }

            // Get the id of the exercise for redirecting
            int exerciseId = comm.ExerciseId;

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comm);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Exercises/Show/" + exerciseId);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            // Find the comment we want to edit
            Comment comm = db.Comments.Find(id);

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (ModelState.IsValid)
                {
                    comm.Content = requestComment.Content;
                    comm.WasEdited = true; // Set the flag that the comment was edited

                    db.SaveChanges();

                    return Redirect("/Exercises/Show/" + comm.ExerciseId);
                }
                else
                {
                    return View(requestComment);
                }
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa editati comentariul";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index", "Workouts");
            }
        }


        [HttpPost]
        [Authorize(Roles = "User,Organizer,Admin")]
        public IActionResult Delete(int id)
        {
            // Find the comment we want to delete
            Comment comm = db.Comments.Find(id);

            // Get the id of the exercise for redirecting
            int exerciseId = comm.ExerciseId;

            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                db.Comments.Remove(comm);
                db.SaveChanges();
                return Redirect("/Exercises/Show/" + exerciseId);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti comentariul";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Exercises/Show/" + exerciseId);
            }

        }

    }
}
