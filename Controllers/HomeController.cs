using Microsoft.AspNetCore.Mvc;
using MVCBeginner.Models;
using System.Diagnostics;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using MVCBeginner.Helpers;
using System.Data.SqlClient;
using Dapper;
using System.Data;

namespace MVCBeginner.Controllers
{

    [Route("/home")]
    public class HomeController : Controller
    {
        // TEMPORARY STATIC WHILE POSTS DATABASE IS UNDER CONSTRUCTION
        private static List<Expense> expenseList = new List<Expense>();

        private Random gen = new Random();

        [HttpGet]
        [Route("/")]
        [Route("/index")]
        public IActionResult Index()
        {

            string? userId = HttpContext.Session.GetString("userid");

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            using(var db = ConnectionManager.GetConnection() )
            {

                List<Expense> associatedExpenseList = db.Query<Expense>("SELECT * FROM dbo.Expenses WHERE UserId=@userId", new { userId = userId }).AsList();
                if (associatedExpenseList != null)
                {
                    expenseList = associatedExpenseList;
                    ViewBag.ExpenseList = associatedExpenseList;
                }

            }

            ViewData["userId"] = userId;
            return View();
        }

        [HttpPost]
        [Route("/")]
        [Route("/index")]
        public IActionResult Index(Expense userExpense)
        {
            ViewBag.ExpenseList = expenseList;
            if (ModelState.IsValid)
            {
                ViewBag.ExpenseList = expenseList;
                expenseList.Add(userExpense);

                string? userId = HttpContext.Session.GetString("userid");

                using ( var db = ConnectionManager.GetConnection())
                {
                    db.Execute("INSERT INTO dbo.Expenses (ExpenseDate, Amount, UserId) VALUES (@expenseDate, @amount, @userId)", new { expenseDate = userExpense.ExpenseDate, amount = userExpense.Amount, userId = userId });
                }

                return View();
            }
            return View();
        }

        [HttpPost]
        [Route("/removeexpense", Name = "RemoveExpense")]
        public IActionResult RemoveExpenseFromList(int index)
        {
            string? userId = HttpContext.Session.GetString("userid");

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if (index < 0 || index >= expenseList.Count)
                return RedirectToAction("Index");

            Expense expenseToRemove = expenseList.ElementAt(index);
            if(expenseToRemove == null)
            {
                return RedirectToAction("Index");
            }

            using(var db = ConnectionManager.GetConnection())
            {
                db.Execute("DELETE FROM dbo.Expenses WHERE Id=@expenseId", new { expenseId = expenseToRemove.Id });
                List<Expense> expenseQueryResultList = db.Query<Expense>("SELECT * FROM dbo.Expenses WHERE Id=@expenseId", new { expenseId = expenseToRemove.Id }).AsList();
                if(expenseQueryResultList.Count != 0)
                {
                    return RedirectToAction("Error");
                }
            }

            expenseList.RemoveAt(index);



            return RedirectToAction("Index");
        }

        [Route("/privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            if (statusCode != -1)
            {
                ViewData["statusCode"] = statusCode;
            }
            return View();
        }
        [Route("/error")]
        public IActionResult Error()
        {
            return View();
        }

        

    }
}