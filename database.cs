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
       

        public struct lkkData
        {
            
            public DateTime date;
            public string number;
            public string department;            
            public string doctor;
            public string fio;
            public string birth;
            public string age;
            public string region;
            public string town;
            public string address;
            public string addressWork;
            public string position;
            public string mkbCode;
            public string diagnose;
            public string lkk;
            public string msek;
            public string addition;
            public string sex;
            public string status;
            public bool haveInvalidity;
            public DateTime InvalidityDate;
            public string LPZ;
        };
        public struct invalidityData
        {
            public string fio;
            public string sex;
            public DateTime dateBirth;
            public string age;
            public string region;
            public string town;
            public string address;
            public string addressWork;
            public string position;
            public string LPZ;
            public DateTime InvalidityDate;
            public string mkbCode;
            public string diagnose;
            public string addition;
            public string invalidityGroupe;
        };



        public void Dispose()
        {
            connection.Close();
        }
     
        public enum typesData
        {
            department = 0
           ,doctor
           ,region
           ,town
           ,diagnose
           ,lkk
           ,lpz
           ,mkbcode
           ,invalidityGroupe
           ,headOfDepartment
            ,invalidityRedister
        };

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
            reader = null;
            disconnect();
            return data;
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
        

       public DataSet getData()
        {
            DataSet lkkData = new DataSet();
            lkkData.Tables.Add("departments");
            lkkData.Tables.Add("regions");
            lkkData.Tables.Add("doctors");
            SQLiteDataAdapter data = new SQLiteDataAdapter();
            SQLiteCommand selectData = new SQLiteCommand();
            selectData.Connection = connection;
            selectData.CommandText = "SELECT title FROM departments";
            data.SelectCommand = selectData;
            connect();
            data.Fill(lkkData.Tables["departments"]);
            selectData.CommandText = "SELECT title FROM regions";
            data.Fill(lkkData.Tables["regions"]);
            selectData.CommandText = "SELECT fio FROM doctors";
            data.Fill(lkkData.Tables["doctors"]);
            lkkData.Tables["departments"].Rows.Add();
            lkkData.Tables["regions"].Rows.Add();
            lkkData.Tables["doctors"].Rows.Add();
            disconnect();           
            return lkkData;
        }
       public DataTable getTown(string regionName)
       {
           DataTable towns = new DataTable();
           SQLiteDataAdapter data = new SQLiteDataAdapter();
           SQLiteCommand selectData = new SQLiteCommand();
           selectData.Connection = connection;
           selectData.CommandText = "SELECT towns.type || '.' ||towns.town AS town FROM towns WHERE towns.regionID=(SELECT id FROM regions WHERE regions.title=:regionName) AND towns.deleted='0'";
           selectData.Parameters.Add(":regionName", DbType.String);
           selectData.Parameters[":regionName"].Value = regionName;           
           data.SelectCommand = selectData;
           connect();
           data.Fill(towns);
           disconnect();
           return towns;
       }
       public DataTable getDoctors() 
       {
           DataTable doctors = new DataTable();
           SQLiteDataAdapter data = new SQLiteDataAdapter();
           SQLiteCommand selectData = new SQLiteCommand();
           selectData.Connection = connection;
           selectData.CommandText = "SELECT fio FROM doctors WHERE doctors.deleted='0'";
           data.SelectCommand = selectData;
           connect();
           data.Fill(doctors);
           disconnect();
           return doctors;
       }

       public string getHeadOfDepartment(string department)
       {
           SQLiteCommand selectData = new SQLiteCommand();
           selectData.Connection = connection;
           selectData.CommandText = "SELECT fio FROM doctors,departments WHERE departments.title=:department AND departments.headOfDepartment=doctors.id";
           selectData.Parameters.Add(":department", DbType.String);
           selectData.Parameters[":department"].Value = department;
           SQLiteDataReader reader;
           string result = "";
           connect();
           reader = selectData.ExecuteReader();
           if (reader.Read())
           {
               result = reader[0].ToString();
           }
           reader.Close();
           reader = null;
           disconnect();
           return result;
       }


       public DataTable getMembersLkk()
       {
           DataTable members = new DataTable();
           SQLiteDataAdapter data = new SQLiteDataAdapter();
           SQLiteCommand selectData = new SQLiteCommand();           
           selectData.Connection = connection;
           selectData.CommandText = "SELECT id,fio,head, active FROM members ORDER BY members.head DESC";
           connect();          
           data.SelectCommand = selectData;           
           data.Fill(members);
           disconnect();
           return members;
       }

       private string getMembersLkkToInsert()
       {
           string members = null;
           DataTable membersLkk = new DataTable();
           SQLiteDataAdapter data = new SQLiteDataAdapter();
           SQLiteCommand selectData = new SQLiteCommand();
           selectData.Connection = connection;
           selectData.CommandText = "SELECT id,fio,head FROM members WHERE active=1 ORDER BY members.head DESC";
           connect();
           data.SelectCommand = selectData;
           data.Fill(membersLkk);
           disconnect();
           
           for (int i = 0; i < membersLkk.Rows.Count; i++)
           {
               if (i == membersLkk.Rows.Count - 1)
                   members += membersLkk.Rows[i][0];
               else
                   members += membersLkk.Rows[i][0] + ",";
           }
           return members;
       }

       public DataTable getMemberLkkById(string id)
       {
           DataTable members = new DataTable();
           SQLiteDataAdapter data = new SQLiteDataAdapter();
           SQLiteCommand selectData = new SQLiteCommand();           
           selectData.Connection = connection;
           selectData.CommandText = "SELECT fio,head FROM members WHERE members.active=1 AND members.id=:id";
           selectData.Parameters.Add(":id", DbType.String);
           selectData.Parameters[":id"].Value = id;
           connect();
           data.SelectCommand = selectData;
           data.Fill(members);
           disconnect();
           return members;
       }

       public DataTable getDiagnose(bool fillCombobox)
       {
           DataTable diagnose = new DataTable();
           SQLiteDataAdapter data = new SQLiteDataAdapter();
           SQLiteCommand selectData = new SQLiteCommand();
           selectData.Connection = connection;
           if (fillCombobox)
           {
               selectData.CommandText = "SELECT codeMKB || ' ' || title AS diagnose FROM diagnosis WHERE deleted=0 AND codeMKB !=''";
           }
           else
           {
               selectData.CommandText = "SELECT codeMKB , title, isOrphan AS diagnose FROM diagnosis WHERE deleted = 0";
           }
           data.SelectCommand = selectData;
           connect();
           data.Fill(diagnose);
           disconnect();
           return diagnose;
       }

       public DataTable getLKK()
       {
           DataTable LKK = new DataTable();
           SQLiteDataAdapter data = new SQLiteDataAdapter();
           SQLiteCommand selectData = new SQLiteCommand();
           selectData.Connection = connection;
           selectData.CommandText = "SELECT title FROM inferenceLKK WHERE inferenceLKK.deleted='0'";
           data.SelectCommand = selectData;
           connect();
           data.Fill(LKK);
           disconnect();
           return LKK;
       }
       public DataTable getDepartments()
       {
           DataTable departments = new DataTable();
           SQLiteDataAdapter data = new SQLiteDataAdapter();
           SQLiteCommand selectData = new SQLiteCommand();
           selectData.Connection = connection;
           selectData.CommandText = "SELECT title FROM departments WHERE departments.deleted='0'";
           data.SelectCommand = selectData;
           connect();
           data.Fill(departments);
           disconnect();
           return departments;
       }

       public DataTable getLPZ()
       {
           DataTable lpz = new DataTable();
           SQLiteDataAdapter data = new SQLiteDataAdapter();
           SQLiteCommand selectData = new SQLiteCommand();
           selectData.Connection = connection;
           selectData.CommandText = "SELECT title FROM lpz WHERE lpz.deleted='0'";
           data.SelectCommand = selectData;
           connect();
           data.Fill(lpz);
           disconnect();
           return lpz;
       }

       public DataTable getInvalidityGroups()
       {
           DataTable groupe = new DataTable();
           SQLiteDataAdapter data = new SQLiteDataAdapter();
           SQLiteCommand selectData = new SQLiteCommand();
           selectData.Connection = connection;
           selectData.CommandText = "SELECT groupe FROM invalidityGroups";
           data.SelectCommand = selectData;
           connect();
           data.Fill(groupe);
           disconnect();
           return groupe;
       }

       public DataTable getRegions()
       {
           DataTable regions = new DataTable();
           SQLiteDataAdapter data = new SQLiteDataAdapter();
           SQLiteCommand selectData = new SQLiteCommand();
           selectData.Connection = connection;
           selectData.CommandText = "SELECT title FROM regions WHERE regions.deleted='0'";
           data.SelectCommand = selectData;
           connect();
           data.Fill(regions);
           disconnect();
           return regions;
       }
       
       private SQLiteDataReader reader;
       public string getID(string parameter, typesData type)
       {
           string ID;
           ID = "";
           switch (type)
           {
               case typesData.department:
                   selectSQLCommand.CommandText = "SELECT id FROM departments WHERE title=:parameter";                    
                   break;
               case typesData.doctor:
                   selectSQLCommand.CommandText = "SELECT id FROM doctors WHERE fio=:parameter";
                   break;
               case typesData.region:
                   selectSQLCommand.CommandText = "SELECT id FROM regions WHERE title=:parameter";
                   break;
               case typesData.lpz:
                   selectSQLCommand.CommandText = "SELECT id FROM lpz WHERE title=:parameter";
                   break;
               case typesData.diagnose:
                   selectSQLCommand.CommandText = "SELECT codeMKB FROM diagnosis WHERE title=:parameter";
                   break;
               case typesData.mkbcode:
                   selectSQLCommand.CommandText = "SELECT codeMKB FROM diagnosis WHERE codeMKB || ' ' || title =:parameter";
                   break;
               case typesData.headOfDepartment:
                   selectSQLCommand.CommandText = "SELECT headOfDepartment FROM departments WHERE id =:parameter";
                   break;
               case typesData.invalidityGroupe:
                   selectSQLCommand.CommandText = "SELECT id FROM invalidityGroups WHERE groupe =:parameter";
                   break;

           }
           selectSQLCommand.Parameters.Add(":parameter", DbType.String);
           selectSQLCommand.Parameters[":parameter"].Value = parameter;                      
           connect();
           reader = selectSQLCommand.ExecuteReader();
           if (reader.Read())
           {
               ID = reader[0].ToString();
           }
           reader.Close();
           reader = null;
           disconnect();
           return ID;
       }
       private string getID(string regionID, string town)
       {
           string ID;
           ID = "";
           selectSQLCommand.CommandText = "SELECT towns.id FROM towns WHERE towns.regionID=:regionID AND towns.type || '.' ||towns.town =:town";
           selectSQLCommand.Parameters.Add(":regionID", DbType.String);
           selectSQLCommand.Parameters[":regionID"].Value = regionID;
           selectSQLCommand.Parameters.Add(":town", DbType.String);
           selectSQLCommand.Parameters[":town"].Value = town;
           connect();
           reader = selectSQLCommand.ExecuteReader();
           if (reader.Read())
           {
               ID = reader[0].ToString();
           }
           reader.Close();
           reader = null;
           disconnect();
           return ID;
       }

       public string insertData(lkkData data)
       {
           insertSQLCommand.CommandText = "INSERT INTO LKK (data,number,departmentID, headOfDepartment, doctorID,FIO,birth,age,regionID,townID," +
               "address,addressWork,position,mkbCode,diagnose,lkk,msek,addition,sex,status,invalidityDate,LpzID,haveInvalidity,comission) VALUES(:data,:number,:departmentID," +
               ":headOfDepartment, :doctorID,:FIO,:birth,:age,:regionID,:townID,:address,:addressWork,:position,:mkbCode,:diagnose,:lkk,:msek," +
               ":addition,:sex,:status,:invalidityDate,:LpzID,:haveInvalidity,:comission)";
           insertSQLCommand.Parameters.Add(":data", DbType.Date);
           insertSQLCommand.Parameters[":data"].Value = data.date;
           insertSQLCommand.Parameters.Add(":number", DbType.String);
           insertSQLCommand.Parameters[":number"].Value = data.number;
           insertSQLCommand.Parameters.Add(":departmentID", DbType.String);
           insertSQLCommand.Parameters[":departmentID"].Value = getID(data.department, typesData.department);
           insertSQLCommand.Parameters.Add(":headOfDepartment", DbType.String);
           insertSQLCommand.Parameters[":headOfDepartment"].Value = getID(insertSQLCommand.Parameters[":departmentID"].Value.ToString(), typesData.headOfDepartment);
           insertSQLCommand.Parameters.Add(":doctorID", DbType.String);
           insertSQLCommand.Parameters[":doctorID"].Value = getID(data.doctor, typesData.doctor);
           insertSQLCommand.Parameters.Add(":FIO", DbType.String);
           insertSQLCommand.Parameters[":FIO"].Value = data.fio;
           insertSQLCommand.Parameters.Add(":birth", DbType.String);
           insertSQLCommand.Parameters[":birth"].Value = data.birth;
           insertSQLCommand.Parameters.Add(":age", DbType.String);
           insertSQLCommand.Parameters[":age"].Value = data.age;
           insertSQLCommand.Parameters.Add(":regionID", DbType.String);
           insertSQLCommand.Parameters[":regionID"].Value = getID(data.region, typesData.region);
           insertSQLCommand.Parameters.Add(":townID", DbType.String);
           insertSQLCommand.Parameters[":townID"].Value = getID(insertSQLCommand.Parameters[":regionID"].Value.ToString(), data.town);
           insertSQLCommand.Parameters.Add(":address", DbType.String);
           insertSQLCommand.Parameters[":address"].Value = data.address;
           insertSQLCommand.Parameters.Add(":addressWork", DbType.String);
           insertSQLCommand.Parameters[":addressWork"].Value = data.addressWork;
           insertSQLCommand.Parameters.Add(":position", DbType.String);
           insertSQLCommand.Parameters[":position"].Value = data.position;
           insertSQLCommand.Parameters.Add(":mkbCode", DbType.String);
           insertSQLCommand.Parameters[":mkbCode"].Value = data.mkbCode;
           insertSQLCommand.Parameters.Add(":diagnose", DbType.String);
           insertSQLCommand.Parameters[":diagnose"].Value = data.diagnose;
           insertSQLCommand.Parameters.Add(":lkk", DbType.String);
           insertSQLCommand.Parameters[":lkk"].Value = data.lkk;
           insertSQLCommand.Parameters.Add(":msek", DbType.String);
           insertSQLCommand.Parameters[":msek"].Value = data.msek;
           insertSQLCommand.Parameters.Add(":addition", DbType.String);
           insertSQLCommand.Parameters[":addition"].Value = data.addition;
           insertSQLCommand.Parameters.Add(":sex", DbType.String);
           insertSQLCommand.Parameters[":sex"].Value = data.sex;
           insertSQLCommand.Parameters.Add(":status", DbType.String);
           insertSQLCommand.Parameters[":status"].Value = data.status;
           insertSQLCommand.Parameters.Add(":haveInvalidity", DbType.Boolean);
           insertSQLCommand.Parameters[":haveInvalidity"].Value = data.haveInvalidity;
           insertSQLCommand.Parameters.Add(":LpzID", DbType.String);
           insertSQLCommand.Parameters[":LpzID"].Value = getID(data.LPZ, typesData.lpz);               
           if (data.haveInvalidity)
           {
               insertSQLCommand.Parameters.Add(":invalidityDate", DbType.Date);
               insertSQLCommand.Parameters[":invalidityDate"].Value = data.InvalidityDate;
               
           }
           else 
           {
               insertSQLCommand.Parameters.Add(":invalidityDate", DbType.Date);
               insertSQLCommand.Parameters[":invalidityDate"].Value = Convert.ToDateTime("01.01.1900");                              
           }
           insertSQLCommand.Parameters.Add(":comission", DbType.String);
           insertSQLCommand.Parameters[":comission"].Value = getMembersLkkToInsert();
           
           connect();
           insertSQLCommand.ExecuteNonQuery();
       
           disconnect();
           
           string id = null;
           SQLiteCommand selectData = new SQLiteCommand();
           selectData.Connection = connection;
           connect();
           selectData.CommandText = "SELECT id FROM lkk WHERE data=:date AND number=:number AND fio=:fio";
           selectData.Parameters.Add(":date", DbType.Date);
           selectData.Parameters[":date"].Value = data.date;
           selectData.Parameters.Add(":number", DbType.String);
           selectData.Parameters[":number"].Value = data.number;
           selectData.Parameters.Add(":fio", DbType.String);
           selectData.Parameters[":fio"].Value = data.fio;
           SQLiteDataReader getlkkID;
           getlkkID = selectData.ExecuteReader();
           if (getlkkID.Read())
               id = getlkkID[0].ToString();
           disconnect();
           return id;
       }
       public void addData(typesData typeData, string value)
       {
           switch (typeData)
           {
               case typesData.department:
                   insertSQLCommand.CommandText = "INSERT INTO departments (title) VALUES(:value)";           
                   break;
               case typesData.diagnose:
                   insertSQLCommand.CommandText = "INSERT INTO diagnosis (title) VALUES(:value)";           
                   break;
               case typesData.doctor:
                   insertSQLCommand.CommandText = "INSERT INTO doctors (fio) VALUES(:value)";           
                   break;
               case typesData.lkk:
                   insertSQLCommand.CommandText = "INSERT INTO infedenceLKK (title) VALUES(:value)";           
                   break;
               case typesData.region:
                   insertSQLCommand.CommandText = "INSERT INTO regions (title) VALUES(:value)";           
                   break;
               case typesData.lpz:
                   insertSQLCommand.CommandText = "INSERT INTO lpz (title) VALUES(:value)";
                   break;
           }
           insertSQLCommand.Parameters.Add(":value", DbType.String);
           insertSQLCommand.Parameters[":value"].Value = value;
           connect();
           insertSQLCommand.ExecuteNonQuery();
           disconnect();
       }
        //For diagnoses only

       public void addData(string mkbCode, string title, bool isOrphan)
       {

           insertSQLCommand.CommandText = "INSERT INTO diagnosis (codeMKB, title, isOrphan) VALUES(:codeMKB, :title, :isOrphan)";
           insertSQLCommand.Parameters.Add(":codeMKB", DbType.String);
           insertSQLCommand.Parameters.Add(":title", DbType.String);
           insertSQLCommand.Parameters.Add(":isOrphan", DbType.Boolean);
           insertSQLCommand.Parameters[":codeMKB"].Value = mkbCode;
           insertSQLCommand.Parameters[":title"].Value = title;
           insertSQLCommand.Parameters[":isOrphan"].Value = isOrphan;
           connect();
           insertSQLCommand.ExecuteNonQuery();
           disconnect();
       }

        public void addData(string department, string headOfDepartment)
       {

           insertSQLCommand.CommandText = "INSERT INTO departments (title, headOfDepartment) VALUES(:title,(SELECT id FROM doctors WHERE fio=:fio))";           
           insertSQLCommand.Parameters.Add(":title", DbType.String);
           insertSQLCommand.Parameters.Add(":fio", DbType.String);
           insertSQLCommand.Parameters[":title"].Value = department;
           insertSQLCommand.Parameters[":fio"].Value = headOfDepartment;
           connect();
           insertSQLCommand.ExecuteNonQuery();
           disconnect();
       }

       public void addTown(string type, string regionID, string value)
       {
           insertSQLCommand.CommandText = "INSERT INTO towns (type, regionID, town) VALUES(:type, :regionID, :title)";
           insertSQLCommand.Parameters.Add(":type", DbType.String);
           insertSQLCommand.Parameters[":type"].Value = type;
           insertSQLCommand.Parameters.Add(":regionID", DbType.String);
           insertSQLCommand.Parameters[":regionID"].Value = regionID;
           insertSQLCommand.Parameters.Add(":title", DbType.String);
           insertSQLCommand.Parameters[":title"].Value = value;
           connect();
           insertSQLCommand.ExecuteNonQuery();
           disconnect();
       }
       public void updateData(typesData typeData, string oldValue, string newValue)
       {
           switch (typeData)
           {
               case typesData.department:
                   updateSQLCommand.CommandText = "UPDATE departments SET title=:newValue WHERE title=:oldValue";
                   break;
               case typesData.diagnose:
                   updateSQLCommand.CommandText = "UPDATE diagnosis SET title=:newValue WHERE title=:oldValue";
                   break;
               case typesData.doctor:
                   updateSQLCommand.CommandText = "UPDATE doctors SET fio=:newValue WHERE fio=:oldValue";
                   break;
               case typesData.lkk:
                   updateSQLCommand.CommandText = "UPDATE infedenceLKK SET title=:newValue WHERE title=:oldValue";
                   break;
               case typesData.region:
                   updateSQLCommand.CommandText = "UPDATE regions SET title=:newValue WHERE title=:oldValue";
                   break;
           }

           updateSQLCommand.Parameters.Add(":oldValue", DbType.String);
           updateSQLCommand.Parameters[":oldValue"].Value = oldValue;
           updateSQLCommand.Parameters.Add(":newValue", DbType.String);
           updateSQLCommand.Parameters[":newValue"].Value = newValue;
           connect();
           updateSQLCommand.ExecuteNonQuery();
           disconnect();
       }


       public void updateData(string oldTitle, string newTitle, string newHeadOfDepartment)
       {
           updateSQLCommand.CommandText = "UPDATE departments SET title=:newValue, headOfDepartment=(SELECT id FROM doctors WHERE fio=:fio)  WHERE title=:oldValue";
           updateSQLCommand.Parameters.Add(":oldValue", DbType.String);
           updateSQLCommand.Parameters[":oldValue"].Value = oldTitle;
           updateSQLCommand.Parameters.Add(":newValue", DbType.String);
           updateSQLCommand.Parameters[":newValue"].Value = newTitle;
           updateSQLCommand.Parameters.Add(":fio", DbType.String);
           updateSQLCommand.Parameters[":fio"].Value = newHeadOfDepartment;
           connect();
           updateSQLCommand.ExecuteNonQuery();
           disconnect();
       }


       public void updateData(string regionID, string oldType, string newType, string oldValue, string newValue)
       {
           updateSQLCommand.CommandText = "UPDATE towns SET title=:newValue, type=:newType WHERE type=:oldType "+
                                           " AND title=:oldValue AND regionID=:regionID ";
           updateSQLCommand.Parameters.Add(":oldValue", DbType.String);
           updateSQLCommand.Parameters[":oldValue"].Value = newValue;
           updateSQLCommand.Parameters.Add(":newValue", DbType.String);
           updateSQLCommand.Parameters[":newValue"].Value = newValue;
           updateSQLCommand.Parameters.Add(":newType", DbType.String);
           updateSQLCommand.Parameters[":newType"].Value = newType;
           updateSQLCommand.Parameters.Add(":oldType", DbType.String);
           updateSQLCommand.Parameters[":oldType"].Value = newType;
           updateSQLCommand.Parameters.Add(":regionID", DbType.String);
           updateSQLCommand.Parameters[":regionID"].Value = regionID;
           connect();
           updateSQLCommand.ExecuteNonQuery();
           disconnect();

       }
       public void deleteData(typesData typeData, string id) 
       {
           switch (typeData)
           {
               case typesData.department:
                   updateSQLCommand.CommandText = "SELECT count() FROM departments WHERE id=:id";
                   break;
               case typesData.diagnose:
                   updateSQLCommand.CommandText = "SELECT count() FROM diagnosis WHERE id=:id";
                   break;
               case typesData.doctor:
                   updateSQLCommand.CommandText = "SELECT count() FROM doctors WHERE id=:id";
                   break;
               case typesData.lkk:
                   updateSQLCommand.CommandText = "UPDATE lkk SET deleted=0 WHERE id=:id";
                   break;
               case typesData.region:
                   updateSQLCommand.CommandText = "SELECT count() FROM regions WHERE id=:id";
                   break;
               case typesData.town:
                   updateSQLCommand.CommandText = "SELECT count() FROM towns WHERE id=:id";
                   break;
           }
           connect();
           updateSQLCommand.Parameters.Add(":id", DbType.String);
           updateSQLCommand.Parameters[":id"].Value = id;
           reader = updateSQLCommand.ExecuteReader();
           int count=1;
           if (reader.Read())
           {
               count = Convert.ToInt32(reader[0].ToString());
           }
           reader.Close();
           reader = null;
           disconnect();
           if (count > 0)
               setDeleted(typeData, id);
           else
               realyDelete(typeData, id);

       }

        private void realyDelete(typesData typeData, string id)
        {
            switch (typeData)
            {
                case typesData.department:
                   deleteSQLCommand.CommandText = "DELETE FROM departments WHERE id=:id";
                    break;
                case typesData.diagnose:
                    deleteSQLCommand.CommandText = "DELETE FROM diagnosis WHERE id=:id";
                    break;
                case typesData.doctor:
                    deleteSQLCommand.CommandText = "DELETE FROM doctors WHERE id=:id";
                    break;
                case typesData.lkk:
                    deleteSQLCommand.CommandText = "DELETE FROM lkk WHERE id=:id";
                    break;
                case typesData.region:
                    deleteSQLCommand.CommandText = "DELETE FROM regions WHERE id=:id";
                    break;
                case typesData.town:
                    deleteSQLCommand.CommandText = "DELETE FROM towns WHERE id=:id";
                    break;
            }
            connect();
            deleteSQLCommand.Parameters.Add(":id", DbType.String);
            deleteSQLCommand.Parameters[":id"].Value = id;
            deleteSQLCommand.ExecuteNonQuery();
            disconnect();
        }

        public void setDeleted(typesData typeData, string id)
        {
            SQLiteCommand deleteData = new SQLiteCommand();
            deleteData.Connection = connection;
            switch (typeData)
            {
                case typesData.department:
                    deleteData.CommandText = "UPDATE departments SET deleted='1' WHERE id=:id";
                    break;
                case typesData.diagnose:
                    deleteData.CommandText = "UPDATE diagnosis SET deleted='1' WHERE id=:id";
                    break;
                case typesData.doctor:
                    deleteData.CommandText = "UPDATE doctors SET deleted='1' WHERE id=:id";
                    break;
                case typesData.lkk:
                    deleteData.CommandText = "UPDATE lkk SET deleted='1' WHERE id=:id";
                    break;
                case typesData.region:
                    deleteData.CommandText = "UPDATE regions SET deleted='true' WHERE id=:id";
                    break;
                case typesData.town:
                    deleteData.CommandText = "UPDATE towns SET deleted='1' WHERE id=:id";
                    break;
                case typesData.invalidityRedister:
                    deleteData.CommandText = "UPDATE invalidity SET deleted='1' WHERE id=:id";
                    break;
            }
            connect();
            deleteData.Parameters.Add(":id", DbType.String);
            deleteData.Parameters[":id"].Value = id;
            deleteData.ExecuteNonQuery();
            disconnect();
        }
        public DataSet getInfedenceLKK(string id)
        {
            DataSet infedence = new DataSet();
            SQLiteDataAdapter data = new SQLiteDataAdapter();
            
            infedence.Tables.Add("infedence");
            infedence.Tables.Add("head");
            infedence.Tables.Add("members");
            SQLiteCommand selectData = new SQLiteCommand();                        
            selectData.Connection = connection;
            selectData.CommandText = "SELECT * FROM infedenceView WHERE id=:id";           
            selectData.Parameters.Add(":id", DbType.String);
            selectData.Parameters[":id"].Value = id;
            data.SelectCommand = selectData;
            connect();
            data.Fill(infedence.Tables["infedence"]);
            data.SelectCommand = selectData;
            string[] comission = infedence.Tables["infedence"].Rows[0]["comission"].ToString().Split(',');
            string headLkk = comission[0];
            string membersLKK = null;
            for (int i = 1; i < comission.Length; i++)
            {
                if (i == comission.Length - 1)
                {
                    membersLKK += comission[i];
                }
                else 
                {
                    membersLKK += comission[i] + ",";
                }
            }            
            selectData.CommandText = "SELECT fio FROM members WHERE id IN (" + headLkk + ")";           
            data.Fill(infedence.Tables["head"]);
            selectData.CommandText = "SELECT fio FROM members WHERE id IN (" + membersLKK + ")";
            data.Fill(infedence.Tables["members"]);
            disconnect();
            return infedence;
        }

        

        public lkkData selectLKK(string id)
        {
            lkkData result = new lkkData();
            SQLiteCommand selectData = new SQLiteCommand();
            SQLiteDataReader reader = null;
            selectData.Connection = connection;
            selectData.CommandText = "SELECT * FROM infedenceView WHERE id=:id";
            selectData.Parameters.Add(":id", DbType.String);
            selectData.Parameters[":id"].Value = id;
            connect();
            reader = selectData.ExecuteReader();
            if (reader.Read())
            {
                result.date = Convert.ToDateTime(reader["data"]);
                result.number = reader["number"].ToString();
                result.department = reader["department"].ToString();
                result.doctor = reader["doctor"].ToString();
                result.fio = reader["fio"].ToString();
                result.birth = reader["birth"].ToString();
                result.age = reader["age"].ToString();
                result.region = reader["region"].ToString();
                result.town = reader["town"].ToString();
                result.address = reader["address"].ToString();
                result.addressWork = reader["addressWork"].ToString();
                result.position = reader["position"].ToString();
                result.mkbCode = reader["mkbCode"].ToString();
                result.diagnose = reader["diagnose"].ToString();
                result.lkk = reader["lkk"].ToString();
                result.msek = reader["msek"].ToString();
                result.addition = reader["addition"].ToString();
                result.sex = reader["sex"].ToString();
                result.haveInvalidity = Convert.ToBoolean(reader["haveInvalidity"].ToString());
                result.InvalidityDate = Convert.ToDateTime(reader["invalidityDate"]);
                result.LPZ = reader["LPZ"].ToString();

            }
            reader.Close();
            reader = null;
            return result;
        }

        public void updateData(lkkData data, string id)
        {
            updateSQLCommand.CommandText = "UPDATE lkk SET data=:data, number=:number, departmentID=:departmentID, headOfDepartment=:headOfDepartment, doctorID=:doctorID,FIO=:FIO, " +
                "birth=:birth,age=:age,regionID=:regionID,townID=:townID, address=:address,addressWork=:addressWork,position=:position,mkbCode=:mkbCode, "+
                "diagnose=:diagnose,lkk=:lkk,msek=:msek,addition=:addition,sex=:sex,status=:status,invalidityDate=:invalidityDate,LpzID=:LpzID, "+
                "haveInvalidity=:haveInvalidity,comission=:comission WHERE id=:id";
            updateSQLCommand.Parameters.Add(":id", DbType.String);
            updateSQLCommand.Parameters[":id"].Value = id;
            updateSQLCommand.Parameters.Add(":data", DbType.Date);
            updateSQLCommand.Parameters[":data"].Value = data.date;
            updateSQLCommand.Parameters.Add(":number", DbType.String);
            updateSQLCommand.Parameters[":number"].Value = data.number;
            updateSQLCommand.Parameters.Add(":departmentID", DbType.String);
            updateSQLCommand.Parameters[":departmentID"].Value = getID(data.department, typesData.department);

            updateSQLCommand.Parameters.Add(":headOfDepartment", DbType.String);
            updateSQLCommand.Parameters[":headOfDepartment"].Value = getID(updateSQLCommand.Parameters[":departmentID"].Value.ToString(), typesData.headOfDepartment);

            updateSQLCommand.Parameters.Add(":doctorID", DbType.String);
            updateSQLCommand.Parameters[":doctorID"].Value = getID(data.doctor, typesData.doctor);
            updateSQLCommand.Parameters.Add(":FIO", DbType.String);
            updateSQLCommand.Parameters[":FIO"].Value = data.fio;
            updateSQLCommand.Parameters.Add(":birth", DbType.String);
            updateSQLCommand.Parameters[":birth"].Value = data.birth;
            updateSQLCommand.Parameters.Add(":age", DbType.String);
            updateSQLCommand.Parameters[":age"].Value = data.age;
            updateSQLCommand.Parameters.Add(":regionID", DbType.String);
            updateSQLCommand.Parameters[":regionID"].Value = getID(data.region, typesData.region);
            updateSQLCommand.Parameters.Add(":townID", DbType.String);
            updateSQLCommand.Parameters[":townID"].Value = getID(updateSQLCommand.Parameters[":regionID"].Value.ToString(), data.town);
            updateSQLCommand.Parameters.Add(":address", DbType.String);
            updateSQLCommand.Parameters[":address"].Value = data.address;
            updateSQLCommand.Parameters.Add(":addressWork", DbType.String);
            updateSQLCommand.Parameters[":addressWork"].Value = data.addressWork;
            updateSQLCommand.Parameters.Add(":position", DbType.String);
            updateSQLCommand.Parameters[":position"].Value = data.position;
            updateSQLCommand.Parameters.Add(":mkbCode", DbType.String);
            updateSQLCommand.Parameters[":mkbCode"].Value = data.mkbCode;
            updateSQLCommand.Parameters.Add(":diagnose", DbType.String);
            updateSQLCommand.Parameters[":diagnose"].Value = data.diagnose;
            updateSQLCommand.Parameters.Add(":lkk", DbType.String);
            updateSQLCommand.Parameters[":lkk"].Value = data.lkk;
            updateSQLCommand.Parameters.Add(":msek", DbType.String);
            updateSQLCommand.Parameters[":msek"].Value = data.msek;
            updateSQLCommand.Parameters.Add(":addition", DbType.String);
            updateSQLCommand.Parameters[":addition"].Value = data.addition;
            updateSQLCommand.Parameters.Add(":sex", DbType.String);
            updateSQLCommand.Parameters[":sex"].Value = data.sex;
            updateSQLCommand.Parameters.Add(":status", DbType.String);
            updateSQLCommand.Parameters[":status"].Value = data.status;
            updateSQLCommand.Parameters.Add(":haveInvalidity", DbType.Boolean);
            updateSQLCommand.Parameters[":haveInvalidity"].Value = data.haveInvalidity;
            updateSQLCommand.Parameters.Add(":LpzID", DbType.String);
            updateSQLCommand.Parameters[":LpzID"].Value = getID(data.LPZ, typesData.lpz);
            if (data.haveInvalidity)
            {
                updateSQLCommand.Parameters.Add(":invalidityDate", DbType.Date);
                updateSQLCommand.Parameters[":invalidityDate"].Value = data.InvalidityDate;
                
            }
            else
            {
                updateSQLCommand.Parameters.Add(":invalidityDate", DbType.Date);
                updateSQLCommand.Parameters[":invalidityDate"].Value = "1900-01-01 00:00:00";                
            }
            updateSQLCommand.Parameters.Add(":comission", DbType.String);
            updateSQLCommand.Parameters[":comission"].Value = getMembersLkkToInsert();

            connect();
            updateSQLCommand.ExecuteNonQuery();

            disconnect();
        }


        public void insertInvalidityData(invalidityData data)
        {
            insertSQLCommand.CommandText = "INSERT INTO invalidity (fio,sex,dateBirth,age,region_id,town_id,address,addressWork,position,lpz_id,invalidityDate,mkbCode,diagnose,addition,groupe)" +
               "VALUES(:fio,:sex,:dateBirth,:age,:region_id,:town_id,:address,:addressWork,:position,:lpz_id,:invalidityDate,:mkbCode,:diagnose,:addition,:groupe);";
            insertSQLCommand.Parameters.Add(":fio", DbType.String);
            insertSQLCommand.Parameters[":fio"].Value = data.fio;
            insertSQLCommand.Parameters.Add(":sex", DbType.String);
            insertSQLCommand.Parameters[":sex"].Value = data.sex;
            insertSQLCommand.Parameters.Add(":dateBirth", DbType.Date);
            insertSQLCommand.Parameters[":dateBirth"].Value = data.dateBirth;
            insertSQLCommand.Parameters.Add(":age", DbType.String);
            insertSQLCommand.Parameters[":age"].Value = data.age;
            insertSQLCommand.Parameters.Add(":region_id", DbType.String);
            insertSQLCommand.Parameters[":region_id"].Value = getID(data.region, typesData.region);
            insertSQLCommand.Parameters.Add(":town_id", DbType.String);
            insertSQLCommand.Parameters[":town_id"].Value = getID(insertSQLCommand.Parameters[":region_id"].Value.ToString(), data.town);
            insertSQLCommand.Parameters.Add(":address", DbType.String);
            insertSQLCommand.Parameters[":address"].Value = data.address;
            insertSQLCommand.Parameters.Add(":addressWork", DbType.String);
            insertSQLCommand.Parameters[":addressWork"].Value = data.addressWork;
            insertSQLCommand.Parameters.Add(":position", DbType.String);
            insertSQLCommand.Parameters[":position"].Value = data.position;
            insertSQLCommand.Parameters.Add(":lpz_id", DbType.String);
            insertSQLCommand.Parameters[":lpz_id"].Value = getID(data.LPZ, typesData.lpz);
            insertSQLCommand.Parameters.Add(":invalidityDate", DbType.Date);
            insertSQLCommand.Parameters[":invalidityDate"].Value = data.InvalidityDate;
            insertSQLCommand.Parameters.Add(":mkbCode", DbType.String);
            insertSQLCommand.Parameters[":mkbCode"].Value = data.mkbCode;
            insertSQLCommand.Parameters.Add(":diagnose", DbType.String);
            insertSQLCommand.Parameters[":diagnose"].Value = data.diagnose;
            insertSQLCommand.Parameters.Add(":addition", DbType.String);
            insertSQLCommand.Parameters[":addition"].Value = data.addition;
            insertSQLCommand.Parameters.Add(":groupe", DbType.String);
            insertSQLCommand.Parameters[":groupe"].Value = getID(data.invalidityGroupe, typesData.invalidityGroupe);
            connect();
            insertSQLCommand.ExecuteNonQuery();
            disconnect();

        }
    }
}
