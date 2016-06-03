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
    public class SqliteHelper
    {
        SQLiteConnection sqlConn;
        string currentDatabase;
        static string lastError = "";

        public SqliteHelper(string database)
        {
            sqlConn = OpenConn(database);
            currentDatabase = database;
        }

        ~SqliteHelper()
        {
            //CloseConn();
        }

        private SQLiteConnection OpenConn(string database)
        {
            string connStr = string.Format("Data Source=" + database);
            SQLiteConnection sqlConn = new SQLiteConnection();
            sqlConn.ConnectionString = connStr;
            if (sqlConn.State == ConnectionState.Open)
                sqlConn.Close();

            sqlConn.Open();
            return sqlConn;
        }

        private void CloseConn()
        {
            if (sqlConn != null && sqlConn.State == ConnectionState.Open)
                sqlConn.Close();
        }

        public void ExecuteNonQuery(string database, string sqlStr)
        {
            if (sqlConn == null || database != currentDatabase)
                sqlConn = OpenConn(database);

            SQLiteCommand sqlCmd = new SQLiteCommand(sqlStr, sqlConn);
            try
            {
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public SQLiteDataReader SelectSQL(string database, string sqlStr)
        {
            if (sqlConn == null || database != currentDatabase)
                sqlConn = OpenConn(database);

            SQLiteDataReader reader = null;
            SQLiteCommand sqlCmd = new SQLiteCommand(sqlStr, sqlConn);
            try
            {
                reader = sqlCmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return reader;
            }

            return reader;
        }

        public DataTable GetDataTable(string database, string sqlStr)
        {
            if (sqlConn == null || database != currentDatabase)
                sqlConn = OpenConn(database);

            DataTable dataTable = new DataTable();
            SQLiteDataAdapter da = new SQLiteDataAdapter(sqlStr, sqlConn);
            DataSet ds = new DataSet();
            ds.Clear();
            da.Fill(ds);
            dataTable = ds.Tables[0];
            return dataTable;
        }

        private void CreateTable(string database)
        {
            string tablestr = "CREATE TABLE MyStock (" +
                               "StockId INTEGER NOT NULL," +
                                "PRIMARY KEY(StockId));";

            ExecuteNonQuery(database, tablestr);
        }

        //public static bool OpenDB()
        //{
        //    string database = "test123.db";

        //    if (!File.Exists(database))
        //        return CreateTable(database);

        //    return true;
        //}

        public bool InsertStockId(int id)
        {
            string database = "test123.db";

            if (!File.Exists(database))
            {
                lastError = "DB not exist";
                return false;
            }

            string insertStr = "INSERT INTO MyStock (StockId) " +
                            "values (" + id.ToString() + ")";

            ExecuteNonQuery(database, insertStr);

            return true;
        }

        public static string GetLastError()
        {
            return lastError;
        }

        public List<int> QueryMyStock()
        {
            List<int> stockList = new List<int>();

            if (!File.Exists(currentDatabase))
            {
                lastError = "DB not exist";
                return stockList;
            }

            SQLiteDataReader reader = SelectSQL(currentDatabase, "SELECT * FROM MyStock");
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
