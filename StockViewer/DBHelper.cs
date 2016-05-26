using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace StockViewer
{
    public class DBHelper
    {
        static string lastError = "";

        private static SQLiteConnection OpenConn(string database)
        {
            string connStr = string.Format("Data Source=" + database);
            SQLiteConnection sqlConn = new SQLiteConnection();
            sqlConn.ConnectionString = connStr;
            if (sqlConn.State == ConnectionState.Open)
                sqlConn.Close();

            sqlConn.Open();
            return sqlConn;
        }

        private static bool RunSQL(string database, string sqlStr)
        {
            SQLiteConnection sqlConn = OpenConn(database);
            SQLiteCommand sqlCmd = new SQLiteCommand(sqlStr, sqlConn);
            try
            {
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                lastError = ex.Message;
                return false;
            }

            if (sqlConn.State == ConnectionState.Open)
                sqlConn.Close();

            return true;
        }

        private static SQLiteDataReader SelectSQL(string database, string sqlStr)
        {
            SQLiteDataReader reader = null;
            SQLiteConnection sqlConn = OpenConn(database);
            SQLiteCommand sqlCmd = new SQLiteCommand(sqlStr, sqlConn);
            try
            {
                reader = sqlCmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                lastError = ex.Message;
                return reader;
            }

            return reader;
        }

        private static bool CreateTable(string database)
        {
            string tablestr = "CREATE TABLE MyStock (" +
                               "StockId INTEGER NOT NULL," +
                                "PRIMARY KEY(StockId));";

            return RunSQL(database, tablestr);
        }

        public static bool OpenDB()
        {
            string database = "test123.db";

            if (!File.Exists(database))
                return CreateTable(database);

            return true;
        }

        public static bool InsertStockId(int id)
        {
            string database = "test123.db";

            if (!File.Exists(database))
            {
                lastError = "DB not exist";
                return false;
            }

            string insertStr = "INSERT INTO MyStock (StockId) " +
                            "values (" + id.ToString() + ")";

            return RunSQL(database, insertStr);
        }

        public static string GetLastError()
        {
            return lastError;
        }

        public static List<int> QueryMyStock()
        {
            List<int> stockList = new List<int>();
            string database = "test123.db";

            if (!File.Exists(database))
            {
                lastError = "DB not exist";
                return stockList;
            }

            SQLiteDataReader reader = SelectSQL(database, "SELECT * FROM MyStock");
            while (reader.Read())
            {
                string tmp = reader["StockId"].ToString();
                stockList.Add(int.Parse(tmp));
                //Console.WriteLine("Id: " + reader["Id"] + "\tName: " + reader["Name"] + "\tAge: " + reader["Age"] + "\tAddress: " + reader["Address"]);
            }

            return stockList;
        }
    }
}
