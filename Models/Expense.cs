using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace MVCBeginner.Models
{
    public class Expense
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You must enter a valid date!")]
        [Display(Name = "Expense Date:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ExpenseDate { get; set; }

        [Required(ErrorMessage = "You must enter a valid amount!")]
        [Display(Name = "Amount:")]
        [DataType(DataType.Currency)]
        [Range(0, 999999)]
        public double? Amount { get; set; }

        public int UserId { get; set; }


        public Expense()
        {
            
        }
        public Expense(DateTime expenseDate, double amount)
        {
            this.ExpenseDate = expenseDate; 
            this.Amount = amount;
        }

        
        public void SetExpenseDate(DateTime newExpenseDate)
        {
            this.ExpenseDate = newExpenseDate;
        }
        public DateTime? GetExpenseDate()
        {
            return this.ExpenseDate;
        }

        public string? GetFormattedExpenseDate() => this.ExpenseDate == null ?  null : this.ExpenseDate?.Date.ToShortDateString();


        public override String ToString()
        {
            return String.Format("[{0}] : ${1}", this.ExpenseDate is null ? "null" : this.ExpenseDate?.Date.ToShortDateString(), this.Amount);
        }
    }
}