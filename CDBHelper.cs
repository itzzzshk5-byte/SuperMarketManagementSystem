using System;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace SuperMarket
{
    // Helper class that manages the SQLite database connection for the entire application
    public class CDBHelper
    {
        // Builds the full path to the database file located in the application's base directory
        private static string dbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Supermarket.db");

        // Connection string used to connect to the SQLite database using the constructed path
        private static string connectionString =
            "Data Source=" + dbPath + ";Version=3;";

        // Returns a new SQLite connection instance using the configured connection string
        // Called by all forms whenever a database operation is needed
        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }
    }
}