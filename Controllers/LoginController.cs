using Microsoft.AspNetCore.Mvc;
using MVCBeginner.Models;
using Microsoft.AspNetCore.Http;
using MVCBeginner.Helpers;
using System.Data.SqlClient;
using Dapper;

namespace MVCBeginner.Controllers
{
    [Route("/login")]
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User postedUser)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
                
            if (String.IsNullOrEmpty(postedUser.Username) || String.IsNullOrEmpty(postedUser.Password))
                    return RedirectToAction("Error", "Home");

            string username = postedUser.Username;
            string password = postedUser.Password;

            using (var db = ConnectionManager.GetConnection())
            {



                List<User> foundUsersList = db.Query<User>("SELECT * FROM dbo.Users WHERE Username=@username", new { username = username } ).AsList();
                if(foundUsersList.Count == 0)
                {
                    ViewData["errorMessage"] = "Incorrect username or password";
                    return View();
                }
                User foundUser = foundUsersList.First();
                if(foundUser == null)
                {
                    ViewData["errorMessage"] = "Incorrect username or password";
                    return View();   
                }
                if(String.IsNullOrEmpty(foundUser.Password) || String.IsNullOrEmpty(foundUser.Salt))
                {
                    return RedirectToAction("Error", "Home");
                }


                if(!EncryptionManager.checkPassword(foundUser.Password, foundUser.Salt, password))
                {
                    ViewData["errorMessage"] = "Incorrect username or password";
                    return View();
                }



                HttpContext.Session.SetString("userid", foundUser.Id.ToString());
            }

            return RedirectToAction("Index", "Home");


        }

        [Route("/signup")]
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [Route("/signup")]
        [HttpPost]
        public IActionResult Signup(UserSignup newUserSignup)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            if(newUserSignup == null)
            {
                ViewData["errorMessage"] = "Please try again";
                return View();
            }

            String username = newUserSignup.Username;
            String password = newUserSignup.Password;

            if(String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                ViewData["errorMessage"] = "Please try again";
                return View();
            }

            using (var db = ConnectionManager.GetConnection())
            {
                List<User> userQueryResultsList = db.Query<User>("SELECT * FROM dbo.Users WHERE Username=@username", new { username= username} ).AsList();
                if(userQueryResultsList.Count != 0)
                {
                    ViewData["errorMessage"] = "Username already taken.";
                    return View();
                }


                (string hashedPassword, string salt) = EncryptionManager.encryptPassword(password);
                



                db.Execute("INSERT INTO dbo.Users (Username, Password, Salt) VALUES (@username, @password, @salt)", new { username = username, password = hashedPassword, salt=salt });
                userQueryResultsList = db.Query<User>("SELECT * FROM dbo.Users WHERE Username=@username", new { username = username }).AsList();
                if (userQueryResultsList.Count == 0)
                {
                    ViewData["errorMessage"] = "Please try again.";
                    return View();
                }


                User newlySignedUpUser = userQueryResultsList.First();

                HttpContext.Session.SetString("userid", newlySignedUpUser.Id.ToString());

            }


            return RedirectToAction("Index", "Home");
        }

        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userid");
            return RedirectToAction("Login");
        }


    }
}
