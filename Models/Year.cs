namespace MVCBeginner.Models
{
    public class Year
    {
        public int YearId { get; set; }
        public List<Month>? MonthList { get; set; }
    }
}
