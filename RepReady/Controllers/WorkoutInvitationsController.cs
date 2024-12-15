using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepReady.Data;
using RepReady.Data.Migrations;
using RepReady.Models;

namespace RepReady.Controllers
{
    public class WorkoutInvitationsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public WorkoutInvitationsController(
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
            // Used to get the invites for the current user
            var userId = _userManager.GetUserId(User); 

            var invitations = db.WorkoutInvitations
                                .Where(invitation => invitation.UserId == userId)
                                .ToList();


            // For displaying the invites in the view
            ViewBag.Invitations = invitations;

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public IActionResult New()
        {
            // Get the workout id for which we want to send an invitation
            int w_id = TempData["workoutId"] == null ? 0 : (int)TempData["workoutId"];

            // Get the workout for which we want to send an invitation
            Workout workout = db.Workouts.Where(workout => workout.Id == w_id)
                                         .First();


            if (workout.CreatorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                // For passing the WorkoutId (hidden)
                ViewBag.Workout = workout; 

                return View();
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa invitati useri într-un antrenament care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Workouts/Show/" + workout.Id);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public IActionResult New(string Email, int WorkoutId)
        {
            // Email and WorkoutId are passed from the form


            // The user we want to invite (by email)
            var user = db.Users.Where(u => u.Email == Email).FirstOrDefault();

            if (user == null) // If the user does not exist in the database
            {
                TempData["message"] = "Nu exista un user cu acest email";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Workouts/Show/" + WorkoutId);
            }

            // The current user that sends the invitation
            var currentUserId = _userManager.GetUserId(User);

            if (user.Id == currentUserId)
            {
                TempData["message"] = "Nu va puteti invita singur la un antrenament";
                TempData["messageType"] = "alert-danger";
                return Redirect("/Workouts/Show/" + WorkoutId);
            }

            // The name of the current user that sends the invitation 
            var userName = db.Users.Where(u => u.Id == currentUserId).First().UserName;

            // The name of the workout for which we send the invitation
            var WorkoutName = db.Workouts.Where(w => w.Id == WorkoutId).First().Name;


            // Create the invitation object and save it to the database
            WorkoutInvitation invitation = new WorkoutInvitation()
            {
                WorkoutId = WorkoutId,
                WorkoutName = WorkoutName,
                UserId = user.Id,
                InvitedBy = userName,
                Accepted = false
            };

            db.WorkoutInvitations.Add(invitation);
            db.SaveChanges();
            TempData["message"] = "Invitația a fost trimisă";
            return Redirect("/Workouts/Show/" + invitation.WorkoutId);
        }

        [HttpPost]
        public IActionResult Accept(int id) // Accept the invitation
        {
            // Find the invitation by id
            var invitation = db.WorkoutInvitations.Where(i => i.Id == id).First();

            // Set the invitation as accepted
            invitation.Accepted = true;


            var workoutId = invitation.WorkoutId; // Id of the workout for which the invitation was sent
            var userId = invitation.UserId; // Id of the user that accepted the invitation

            // Get the workout for which the invitation was sent
            Workout workout = db.Workouts.Include("Users")
                                         .Where(w => w.Id == workoutId).First();


            // Get the user that accepted the invitation
            ApplicationUser user = db.Users.Include("Workouts")
                                           .Where(u => u.Id == userId).First();

            workout.Users.Add(user); // Add the user to the workout
            user.Workouts.Add(workout); // Add the workout to the user


            db.SaveChanges();
            TempData["message"] = "Invitația a fost acceptată";
            TempData["messageType"] = "alert-success";
            return Redirect("/WorkoutInvitations/Index");
        }

        [HttpPost]
        public IActionResult Reject(int id) // Reject the invitation
        { 
            // Find the invitation by id
            var invitation = db.WorkoutInvitations.Where(i => i.Id == id).First();

            db.Remove(invitation);

            db.SaveChanges();
            TempData["message"] = "Invitația a fost respinsă";
            TempData["messageType"] = "alert-danger";
            return Redirect("/WorkoutInvitations/Index");
        }

    }
}
