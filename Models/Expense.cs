using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace MVCBeginner.Models
{
    public class Expense
    {
        [DataType(DataType.Text)]
        public string? Title { get; set; } = "";
        public int Id { get; set; }
        [Required(ErrorMessage = "You must enter a valid date!")]
        [Display(Name = "Expense Date:")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpenseDate { get; set; }

        [Required(ErrorMessage = "You must enter a valid amount!")]
        [Display(Name = "Amount:")]
        [DataType(DataType.Currency)]
        [Range(0, 999999)]
        public double? Amount { get; set; }

        public int UserId { get; set; }

        public string? GetFormattedExpenseDate() => this.ExpenseDate.Date.ToShortDateString();

        public override String ToString()
        {
            return String.Format("[{0}] {1}: ${2}", GetFormattedExpenseDate(), this.Title, this.Amount);
        }
    }
}