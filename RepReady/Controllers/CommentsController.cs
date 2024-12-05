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
        //public IActionResult Index()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult New(Comment comm)
        //{
        //    comm.Date = DateTime.Now;
        //    comm.WasEdited = false;

        //    try
        //    {
        //        db.Comments.Add(comm);
        //        db.SaveChanges();
        //        return Redirect("/Exercises/Show/" + comm.ExerciseId);
        //    }

        //    catch (Exception)
        //    {
        //        return Redirect("/Exercises/Show/" + comm.ExerciseId);
        //    }

        //}


        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            db.Comments.Remove(comm);
            db.SaveChanges();
            return Redirect("/Exercises/Show/" + comm.ExerciseId);
        }

        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Find(id);
            
            return View(comm);
        }

        [HttpPost]
        public IActionResult Edit(int id, Comment requestComment)
        {
            Comment comm = db.Comments.Find(id);

            if (ModelState.IsValid)
            {

                comm.Content = requestComment.Content;
                comm.WasEdited = true;

                db.SaveChanges();

                return Redirect("/Exercises/Show/" + comm.ExerciseId);
            }
            else
            {
                return View(requestComment);
            }

        }


    }
}
