using BankAppFinal.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankAppFinal.Controllers
{
    public class RoleController : Controller
    {
        ApplicationDbContext context;

        public RoleController()
        {
            context = new ApplicationDbContext();
        }

        // GET: Role
        public ActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {

                if (!isAdminUser())
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

            var Roles = context.Roles.ToList();
            return View(Roles);

        }

        public ActionResult Create()
        {
            NewRole newRole = new NewRole();
            return View(newRole);
        }

        [HttpPost]
        public ActionResult Create(NewRole newRole)
        {
            string Output = "";
            ApplicationDbContext db = new ApplicationDbContext();
            RoleManager<IdentityRole> RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            if (!RoleManager.RoleExists("Admins"))
            {
                IdentityResult Result = RoleManager.Create(new IdentityRole(newRole.Name));
                if (Result.Succeeded)
                {
                    Output = "the role created";
                }
                else
                {
                    int ErrorCount = Result.Errors.Count();
                    Output = "Errors is: " + Result.Errors.ToList()[0];
                }
            }
            else
            {
                Output = "the roles exist";
            }

            if (Output == "")
            {
                ModelState.AddModelError(string.Empty, "tesssst");
                return View(newRole);
            }
            else
            {
                var Roles = context.Roles.ToList();
                return View("Index", Roles);
            }

        }

        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}