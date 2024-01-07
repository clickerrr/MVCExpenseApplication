using System;
using System.Data.SqlClient;


namespace MVCBeginner.Helpers
{
    public class ConnectionManager
    {
        public ConnectionManager()
        {

        }
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = ExpenseApp;");
        }

    }
}
