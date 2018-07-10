using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ISD_1.Helper;
using ISD_1.Models;

namespace ISD_1.Controllers
{
    
    public class HomeController : Controller
    {
        private ISD_DemoEntities db = new ISD_DemoEntities();
        private static int saltLengthLimit = 32;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //Login
        public ActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult LogIn(string userName, string password)
        {
            try
            {
                using (var context = new ISD_DemoEntities2())
                {
                    var getUser = (from s in context.Users where s.UserName == userName || s.EmailId == userName select s).FirstOrDefault();
                    if (getUser != null)
                    {
                        var hashCode = getUser.VCode;
                        //Password Hasing Process Call Helper Class Method    
                        var encodingPasswordString = EncryptionHelper.EncodePassword(password, hashCode);
                        //Check Login Detail User Name Or Password    
                        var query = (from s in context.Users where (s.UserName == userName || s.EmailId == userName) && s.Password.Equals(encodingPasswordString) select s).FirstOrDefault();
                        if (query != null)
                        {
                            //RedirectToAction("Details/" + id.ToString(), "FullTimeEmployees");    
                            //return View("../Admin/Registration"); url not change in browser    
                            return RedirectToAction("Index", "Home");
                        }
                        ViewBag.ErrorMessage = "Invallid User Name or Password";
                        return View();
                    }
                    ViewBag.ErrorMessage = "Invallid User Name or Password";
                    return View();
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = " Error!!! contact ISD@gmail.com";
                return View();
            }
        }
        //Signup

        public ActionResult Signup()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Signup(User objNewUser)
        {
            try
            {
                using (var context = new ISD_DemoEntities2())
                {
                    var chkUser = (from s in context.Users where s.UserName == objNewUser.UserName || s.EmailId == objNewUser.EmailId select s).FirstOrDefault();
                    if (chkUser == null)
                    {
                        var keyNew = EncryptionHelper.GeneratePassword(10);
                        var password = EncryptionHelper.EncodePassword(objNewUser.Password, keyNew);
                        objNewUser.Password = password;
                        objNewUser.CreateDate = DateTime.Now;
                        objNewUser.ModifyDate = DateTime.Now;
                        objNewUser.VCode = keyNew;
                        context.Users.Add(objNewUser);
                        context.SaveChanges();
                        ModelState.Clear();
                        return RedirectToAction("LogIn", "Home");
                    }
                    ViewBag.ErrorMessage = "User Allredy Exixts!!!!!!!!!!";
                    return View();
                }
            }
            catch (Exception e)
            {
                ViewBag.ErrorMessage = "Some exception occured" + e;
                return View();
            }
        }

    }
}
