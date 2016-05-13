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
            public double estimate;
            public double sumEstimate;
            public int parentId;
        };

        private SQLiteConnection connection;
  

        public Database()
        {
            string dir = Directory.GetCurrentDirectory();
            connection = new SQLiteConnection("Data Source=" + dir + "\\database.db;Version=3;New=True;");
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
        //Function is used for get company data by id in database
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
                data.estimate = Convert.ToDouble(reader[2]);
                data.parentId = Convert.ToInt16(reader[3]);
            }
            reader.Close();
            reader = null;
            disconnect();
            return data;
        }
        //Function is used for get company id in database by company name
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
        //Function is used for get count of child companies by id of parent company
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
        //Function is used for get list of child companies by id of parent company
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
                data.sumEstimate = Convert.ToDouble(row[2].ToString().Replace('.', ','));
                data.parentId = Convert.ToInt16(row[3]);
                root.Add(data);
            }
            return root;
        }
        //Function is used for get list of all companies in the database
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
                data.sumEstimate = Convert.ToDouble(row[2]);
                data.parentId = Convert.ToInt16(row[3]);
                companies.Add(data);
            }
            return companies;
        }
        //Function is used for delete company from database
        public void deleteData(int id)
        {
            SQLiteCommand deleteSQLCommand = new SQLiteCommand();
            List<Database.companyData> root = getRoot(id);
            deleteSQLCommand.CommandText = "DELETE FROM data WHERE id=:id";
            deleteSQLCommand.Connection = connection;
            connect();
            deleteSQLCommand.Parameters.Add(":id", DbType.Int16);
            deleteSQLCommand.Parameters[":id"].Value = id;
            deleteSQLCommand.ExecuteNonQuery();
            disconnect();
            if (root.Count > 0)
                root.ForEach(delegate(Database.companyData data)
                {                   
                    deleteData(data.id);
                });
        }
        //Function is used for insert company to the database
        public void insertData(companyData newRow)
        {
            
            SQLiteCommand insertData = new SQLiteCommand();
            insertData.Connection = connection;
            insertData.CommandText = "INSERT INTO data (name,estimated,parent_id) VALUES(:name,:estimated,:parent_id)";
            insertData.Parameters.Add(":name", DbType.String);
            insertData.Parameters[":name"].Value = newRow.name;
            insertData.Parameters.Add(":estimated", DbType.Double);
            insertData.Parameters[":estimated"].Value = newRow.estimate;
            insertData.Parameters.Add(":parent_id", DbType.String);
            insertData.Parameters[":parent_id"].Value = newRow.parentId.ToString();
            connect();
            insertData.ExecuteNonQuery();
            disconnect();
            
        }
        //Function is used for modify company data in the database
        public void updateData(companyData newRow)
        {

            SQLiteCommand insertData = new SQLiteCommand();
            insertData.Connection = connection;
            insertData.CommandText = "UPDATE data SET name=:name, estimated=:estimated,parent_id=:parent_id WHERE id=:id";
            insertData.Parameters.Add(":name", DbType.String);
            insertData.Parameters[":name"].Value = newRow.name;
            insertData.Parameters.Add(":estimated", DbType.Double);
            insertData.Parameters[":estimated"].Value = newRow.estimate;
            insertData.Parameters.Add(":parent_id", DbType.Int16);
            insertData.Parameters[":parent_id"].Value = newRow.parentId;
            insertData.Parameters.Add(":id", DbType.Int16);
            insertData.Parameters[":id"].Value = newRow.id;
            connect();
            insertData.ExecuteNonQuery();
            disconnect();

        }
        //Function is used for get company data from the database by company name
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
                data.estimate = Convert.ToDouble(reader[2]);
                data.parentId = Convert.ToInt16(reader[3]);
            }
            reader.Close();
            reader = null;
            disconnect();
            return data;
        }

      
    }
}
