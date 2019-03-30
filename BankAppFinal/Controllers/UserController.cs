using BankAppFinal.Models;
using BankingAppFinal.Common.Entities;
using BankingAppFinal.Services.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankAppFinal.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        ApplicationDbContext context;

        public UserController()
        {
            context = new ApplicationDbContext();
        }


        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;

                ViewBag.displayMenu = "No";

                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                }

                AccountService accountService = new AccountService();
                var accounts = accountService.GetAll();

                Account acn = new Account();
                acn.TypeOfAccount = "Test";
                acn.CreatedUser = user.Name;
                acn.UpdatedUser = user.Name;
                acn.CreatedTime = DateTime.Now;
                acn.UpdatedTime = DateTime.Now;
                accountService.Create(acn);

                var accounts1 = accountService.GetAll();

                accountService.Delete(4);

                var accounts2 = accountService.GetAll();

                return View();
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }
            return View();

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