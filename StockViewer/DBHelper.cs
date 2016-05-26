using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace StockViewer
{
    class DBHelper
    {
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

        private void RunSQL(string database, string sqlStr)
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
            }

            if (sqlConn.State == ConnectionState.Open)
                sqlConn.Close();
        }

        private SQLiteDataReader SelectSQL(string database, string sqlStr)
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
                return reader;
            }

            return reader;
        }


        public void OpenDB()
        {
 
        }

    }
}
