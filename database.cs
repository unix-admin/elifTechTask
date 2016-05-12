using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace WindowsFormsApplication3
{

    class Database 
    {                 
        
        public void Dispose()
        {
            connection.Close();
        }
             
        public struct companyData
        {
            public int id;
            public string name;
            public decimal estimate;
            public decimal sumEstimate;
            public int parentId;
        };

        private SQLiteConnection connection;
        private SQLiteCommand selectSQLCommand = new SQLiteCommand();
        private SQLiteCommand insertSQLCommand = new SQLiteCommand();
        private SQLiteCommand updateSQLCommand = new SQLiteCommand();
        private SQLiteCommand deleteSQLCommand = new SQLiteCommand();

        public Database()
        {
            string dir = Directory.GetCurrentDirectory();
            connection = new SQLiteConnection("Data Source=" + dir + "\\database.db;Version=3;New=True;");
            insertSQLCommand.Connection = connection;
            selectSQLCommand.Connection = connection;
            updateSQLCommand.Connection = connection;

        }

        private void connect()
        {
            if (connection.State == System.Data.ConnectionState.Closed)
                connection.Open();
        }
        private void disconnect()
        {
            if (connection.State == System.Data.ConnectionState.Open)
                connection.Close();
            
        }

        public companyData getCompany(int id) { 
            companyData data = new companyData();
            SQLiteDataReader reader = null;
            SQLiteCommand selectData = new SQLiteCommand();
            selectData.Connection = connection;
            selectData.CommandText = "SELECT * FROM data WHERE id=:id";
            selectData.Parameters.Add(":id", DbType.Int16);
            selectData.Parameters[":id"].Value = id;
            connect();
            reader = selectData.ExecuteReader();
            if (reader.Read())
            { 
                data.id = Convert.ToInt16(reader[0]);
                data.name = reader[1].ToString();
                data.estimate = Convert.ToDecimal(reader[2]);
                data.parentId = Convert.ToInt16(reader[3]);
            }
            reader.Close();
            reader = null;
            disconnect();
            return data;
        }

        public int getParentId(string companyName)
        {
            int companyId = 0;
            SQLiteDataReader reader = null;
            SQLiteCommand selectData = new SQLiteCommand();
            selectData.Connection = connection;
            selectData.CommandText = "SELECT id FROM data WHERE name=:companyName";
            selectData.Parameters.Add(":companyName", DbType.String);
            selectData.Parameters[":companyName"].Value = companyName;
            connect();
            reader = selectData.ExecuteReader();
            if (reader.Read())
            {
                companyId = Convert.ToInt16(reader[0]);                
            }
            reader.Close();
            reader = null;
            disconnect();
            return companyId;
        }

        public int getCount(int id)
        {
            int count = 0;
            SQLiteDataReader reader = null;
            SQLiteCommand selectData = new SQLiteCommand();
            selectData.Connection = connection;
            selectData.CommandText = "SELECT COUNT() FROM data WHERE parent_id=:id";
            selectData.Parameters.Add(":id", DbType.Int16);
            selectData.Parameters[":id"].Value = id;
            connect();
            reader = selectData.ExecuteReader();
            if (reader.Read())
            {
                count = Convert.ToInt16(reader[0]);
            }
            reader.Close();
            reader = null;
            disconnect();
            return count;
        }

        public List<companyData> getRoot(int id)
        {
            List<companyData> root = new List<companyData>();            
            SQLiteCommand selectData = new SQLiteCommand();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter();
            DataTable tempTable = new DataTable();
            selectData.Connection = connection;
            selectData.CommandText = "SELECT * FROM data WHERE parent_id=:id";
            selectData.Parameters.Add(":id", DbType.Int16);
            selectData.Parameters[":id"].Value = id;
            adapter.SelectCommand = selectData;
            connect();
            adapter.Fill(tempTable);
            disconnect();
            foreach (DataRow row in tempTable.Rows)
            {
                companyData data = new companyData();
                data.id = Convert.ToInt16(row[0]);
                data.name = row[1].ToString();
                data.sumEstimate = Convert.ToDecimal(row[2]);
                data.parentId = Convert.ToInt16(row[3]);
                root.Add(data);
            }
            return root;
        }

        public List<companyData> getAllCompanies()
        {
            List<companyData> companies = new List<companyData>();
            SQLiteCommand selectData = new SQLiteCommand();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter();
            DataTable tempTable = new DataTable();
            selectData.Connection = connection;
            selectData.CommandText = "SELECT * FROM data";            
            adapter.SelectCommand = selectData;
            connect();
            adapter.Fill(tempTable);
            disconnect();
            foreach (DataRow row in tempTable.Rows)
            {
                companyData data = new companyData();
                data.id = Convert.ToInt16(row[0]);
                data.name = row[1].ToString();
                data.sumEstimate = Convert.ToDecimal(row[2]);
                data.parentId = Convert.ToInt16(row[3]);
                companies.Add(data);
            }
            return companies;
        }

        private void deleteData(companyData data)
        {
            deleteSQLCommand.CommandText = "DELETE FROM data WHERE id=:id";            
            connect();
            deleteSQLCommand.Parameters.Add(":id", DbType.String);
            deleteSQLCommand.Parameters[":id"].Value = data.id;
            deleteSQLCommand.ExecuteNonQuery();
            disconnect();
        }
               
        public void insertData(companyData newRow)
        {
            
            SQLiteCommand insertData = new SQLiteCommand();
            insertData.Connection = connection;
            insertData.CommandText = "INSERT INTO data (name,estimated,parent_id) VALUES(:name,:estimated,:parent_id)";
            insertData.Parameters.Add(":name", DbType.String);
            insertData.Parameters[":name"].Value = newRow.name;
            insertData.Parameters.Add(":estimated", DbType.String);
            insertData.Parameters[":estimated"].Value = newRow.estimate.ToString();
            insertData.Parameters.Add(":parent_id", DbType.String);
            insertData.Parameters[":parent_id"].Value = newRow.parentId.ToString();
            connect();
            insertData.ExecuteNonQuery();
            disconnect();
            
        }

        public void updateData(companyData newRow)
        {

            SQLiteCommand insertData = new SQLiteCommand();
            insertData.Connection = connection;
            insertData.CommandText = "UPDATE data SET name=:name, estimated=:estimated,parent_id=:parent_id WHERE id=:id";
            insertData.Parameters.Add(":name", DbType.String);
            insertData.Parameters[":name"].Value = newRow.name;
            insertData.Parameters.Add(":estimated", DbType.Decimal);
            insertData.Parameters[":estimated"].Value = newRow.estimate;
            insertData.Parameters.Add(":parent_id", DbType.Int16);
            insertData.Parameters[":parent_id"].Value = newRow.parentId;
            insertData.Parameters.Add(":id", DbType.Int16);
            insertData.Parameters[":id"].Value = newRow.id;
            connect();
            insertData.ExecuteNonQuery();
            disconnect();

        }

        public companyData getCompanyByName(string name)
        {
            companyData data = new companyData();
            SQLiteDataReader reader = null;
            SQLiteCommand selectData = new SQLiteCommand();
            selectData.Connection = connection;
            selectData.CommandText = "SELECT * FROM data WHERE name=:name";
            selectData.Parameters.Add(":name", DbType.String);
            selectData.Parameters[":name"].Value = name;
            connect();
            reader = selectData.ExecuteReader();
            if (reader.Read())
            {
                data.id = Convert.ToInt16(reader[0]);
                data.name = reader[1].ToString();
                data.estimate = Convert.ToDecimal(reader[2]);
                data.parentId = Convert.ToInt16(reader[3]);
            }
            reader.Close();
            reader = null;
            disconnect();
            return data;
        }

      
    }
}
