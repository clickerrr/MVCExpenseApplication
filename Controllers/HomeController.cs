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
        [Route("/dashboard")]
        public IActionResult Dashboard()
        {

            string? userId = HttpContext.Session.GetString("userid");

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            if(expenseList.Count == 0)
            {
                using (var db = ConnectionManager.GetConnection())
                {
                    List<DateTime> validDates = db.Query<DateTime>("SELECT ExpenseDate from dbo.Expenses WHERE UserId=@userId ORDER BY ExpenseDate ASC", new { userId = userId }).AsList();

                    validDates.ForEach((date) => { System.Diagnostics.Debug.WriteLine(date.ToString()); });

                    List<Expense> associatedExpenseList = db.Query<Expense>("SELECT * FROM dbo.Expenses WHERE UserId=@userId ORDER BY ExpenseDate ASC", new { userId = userId }).AsList();
                    if (associatedExpenseList != null)
                    {
                        expenseList = associatedExpenseList;
                    }

                }
            }

            ViewBag.ExpenseList = expenseList;
            // sortExpenseList(expenseList);


            



            ViewData["userId"] = userId;
            return View();
        }

        [HttpPost]
        [Route("/")]
        [Route("/dashboard")]
        public IActionResult Dashboard(Expense userExpense)
        {
            ViewBag.ExpenseList = expenseList;
            if (ModelState.IsValid)
            {
                ViewBag.ExpenseList = expenseList;
                expenseList.Add(userExpense);

                sortExpenseList(expenseList);

                string? userId = HttpContext.Session.GetString("userid");

                using ( var db = ConnectionManager.GetConnection())
                {
                    db.Execute("INSERT INTO dbo.Expenses (ExpenseDate, Title, Amount, UserId) VALUES (@expenseDate, @title, @amount, @userId)", new { expenseDate = userExpense.ExpenseDate, title = userExpense.Title,amount = userExpense.Amount, userId = userId });
                }

                return RedirectToAction("Dashboard");
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
                return RedirectToAction("Dashboard");

            Expense expenseToRemove = expenseList.ElementAt(index);
            if(expenseToRemove == null)
            {
                return RedirectToAction("Dashboard");
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



            return RedirectToAction("Dashboard");
        }

        [Route("/updateexpense", Name = "UpdateExpense")]
        public IActionResult UpdateExpense(int updateIndex)
        {
            string? userId = HttpContext.Session.GetString("userid");

            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            Expense expenseToUpdate = expenseList.ElementAt(updateIndex);
            DateTime newDate = DateTime.Parse(HttpContext.Request.Form["ExpenseDate"]);
            String newTitle = HttpContext.Request.Form["Title"];
            double newAmount = Double.Parse(HttpContext.Request.Form["Amount"]);

            if(!expenseToUpdate.ExpenseDate.Equals(newDate))
            {
                expenseToUpdate.ExpenseDate = newDate;
            }
            if(expenseToUpdate.Title != null)
            {
                if (!expenseToUpdate.Title.Equals(newTitle))
                {
                    expenseToUpdate.Title = newTitle;
                }
            }
            if(expenseToUpdate.Amount != newAmount )
            {
                expenseToUpdate.Amount = newAmount;
            }

            using (var db = ConnectionManager.GetConnection())
            {
                db.Execute("UPDATE dbo.Expenses SET Title=@title, ExpenseDate=@expenseDate, Amount=@amount WHERE Id=@postId", new { title = expenseToUpdate.Title, expenseDate = expenseToUpdate.ExpenseDate, amount = expenseToUpdate.Amount, postId = expenseToUpdate.Id });
                sortExpenseList(expenseList);
            }

            return RedirectToAction("Dashboard");
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

        private void sortExpenseList(List<Expense> inputList)
        {
            // ?? is the if null set x = ...
            // provides a convenient way to set possibly null DateTime variables to a default value, since in ExpenseDate it can be nullable (it shouldnt be though wait)
            inputList.Sort((x, y) => DateTime.Compare(x.ExpenseDate, y.ExpenseDate));
        }

    }
}