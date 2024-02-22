using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;


// kapselt scope der klasse ab
namespace DataBank
{

    // vgl kotlin "interface" für SQL CRUD operationen
    public class SqliteHelper
    {
        // for debugging, to know where error happened
        private const string Tag = "Riz: SqliteHelper:\t";

        

        public string db_connection_string;
        public IDbConnection db_connection;

        // constructor opens connection to our database
        public SqliteHelper()
        {
            db_connection_string = "URI=file:C:/Users/fabia/EcoHarvest/EcoDataBase.sqlite";

            
            db_connection = new SqliteConnection(db_connection_string);

            db_connection.Open();
            //Debug.Log("opened connection: " + db_connection_string);
        }

        // ~ means destructor (schließt am ende verbindung)
        ~SqliteHelper()
        {
            db_connection.Close();
            //Debug.Log("CLOSE connection via SQLHelper");
        }

        // kleine hilfsfunktionen
        public IDbCommand getDbCommand()
        {
            return db_connection.CreateCommand();
        }
        public virtual IDataReader getDataById(string id) { // damit funktion ohne/mit weniger argumenten aufgerufen werden kann?
            
            
            Debug.Log(Tag + "This function is not implemented");
            throw null;
        }
        public virtual void deleteDataById(string id)
        {
            Debug.Log(Tag + "This function is not implemented");
            throw null;
        }
        public virtual IDataReader getAllData()
        {
            Debug.Log(Tag + "This function is not implemented");
            throw null;
        }
        public virtual void deleteAllData()
        {
            Debug.Log(Tag + "This function is not implemnted");
            throw null;
        }


        // FUnktionen zum Ausführen der Queries
        // virtual = open keyword in kotlin
        public IDataReader getDataById(string id, string tableName, string keyId) // benötigt tabellennamen, attributnamen und pk attributwert
        {

            if(db_connection.State != ConnectionState.Open) {
                db_connection.Open();
            }

            //Debug.Log(Tag + "Getting Data: " + id + " from " + keyId);
        	IDbCommand dbcmd = getDbCommand();
        	dbcmd.CommandText =
            	"SELECT * FROM " + tableName + " WHERE " + keyId + " = '" + id + "'";
        	return dbcmd.ExecuteReader();
        }


        public void deleteDataById(string id, string tableName, string keyId)
        {
            //Debug.Log(Tag + "Deleting Data: " + id + " from " + keyId);
        	IDbCommand dbcmd = getDbCommand();
        	dbcmd.CommandText =
            	"DELETE FROM " + tableName + " WHERE " + keyId + " = '" + id + "'";
        	dbcmd.ExecuteNonQuery();
        }


        public IDataReader getAllData(string tableName)
        {
            //Debug.Log(Tag + "Getting All Data: " + tableName);
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + tableName;
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }


        public void deleteAllData(string tableName)
        {
            //Debug.Log(Tag + "Deleting Data from Table:" + tableName);
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText = "DROP TABLE IF EXISTS " + tableName;
            dbcmd.ExecuteNonQuery();
        }
        

	    public void close()
        {
            db_connection.Close();
	    }

        public int readerIntHelper(IDataReader reader, string spaltenname)
        {


            int outputInt;

            while (reader.Read())
            {

                object localObject = reader[spaltenname];

                if (localObject != null && localObject != System.DBNull.Value)
                {
                    if (int.TryParse(localObject.ToString(), out outputInt))
                    {
                        // Successfully parsed to int
                        
                        reader.Close();
                        return outputInt;
                    }
                    else
                    {
                        Debug.LogError("Failed to parse " + spaltenname + " to int.");
                        return -1;
                    }
                }
                else
                {
                    Debug.LogWarning(spaltenname + " is DBNull or null.");
                    return -1;
                }

            }
            reader.Close();
            Debug.Log("Something wrong.");
            return -1;
        }
    }
}
