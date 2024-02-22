using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using UnityEngine;

namespace DataBank
{
    
	public class AntwortEntity {

        public string AntwortId; // pk
        public string ParameterId; // fk
        public string AntwortText;
        public int AenderungDirekt;
        public int AenderungVerzoegert;

        // konstruktor
        public AntwortEntity(string antwortId, string parameterId, string antwortText, int aenderungDirekt, int aenderungVerzoegert)
        {
            AntwortId = antwortId;
            ParameterId = parameterId;
            AntwortText = antwortText;
            AenderungDirekt = aenderungDirekt;
            AenderungVerzoegert = aenderungVerzoegert;
        }

	}


    public class AntwortTable : SqliteHelper {
        
        private const String TABLE_NAME = "Antwort";

        // Namen der Tabellenspalten
    	private const String KEY_AID = "antwort_id"; 
    	private const String KEY_PID = "parameter_id";
        private const String KEY_ATEXT = "antwort_text";
        private const String KEY_PCHANGENOW = "aenderung_direkt";
        private const String KEY_PCHANGELATER = "aenderung_verzoegert";
        
        
        public AntwortTable() : base() // nimmt Konstruktor von "super" class
        {

            /*
            IDbCommand dbcmdDrop = getDbCommand();
            dbcmdDrop.CommandText = "DROP TABLE IF EXISTS antwort";
            dbcmdDrop.ExecuteNonQuery();

            IDbCommand dbcmdDrop2 = getDbCommand();
            dbcmdDrop2.CommandText = "DROP TABLE IF EXISTS Antworten";
            dbcmdDrop2.ExecuteNonQuery();
            */
            /*
        	IDbCommand dbcmd = getDbCommand();
        	dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " (antwort_id TEXT NOT NULL, parameter_id TEXT NOT NULL, " +
                "antwort_text TEXT NOT NULL, parameteraenderung FLOAT NOT NULL, PRIMARY KEY(antwort_id, parameter_id))";
        
            Debug.Log("about to execute antwort create command" + dbcmd.CommandText);
        	dbcmd.ExecuteNonQuery();
            */
        }
        public void addData(AntwortEntity antwort) // Datensatz hinzufügen
        {
        	IDbCommand dbcmd = getDbCommand();
        	dbcmd.CommandText =
            	"INSERT OR REPLACE INTO " + TABLE_NAME
            	+ " ( "
            	+ KEY_AID + ", "
            	+ KEY_PID + ", "
            	+ KEY_ATEXT + ", " 
                + KEY_PCHANGENOW + ", " 
                + KEY_PCHANGELATER + " ) "
            	+ "VALUES ( '"
            	+ antwort.AntwortId + "', '"
            	+ antwort.ParameterId + "', '"
            	+ antwort.AntwortText + "', '"
                + antwort.AenderungDirekt + "', '" 
                + antwort.AenderungVerzoegert + "' ); ";
        	dbcmd.ExecuteNonQuery();
        }

        // table name und pk (name + wert) in argumentliste gesetzt und funktion der höheren klasse genutzt
        public override IDataReader getDataById(string id)
        {
        	return base.getDataById(id, TABLE_NAME, KEY_AID);
        }
        
        public override void deleteDataById(string id)
        {
        	base.deleteDataById(id, TABLE_NAME, KEY_AID);
        }

        public override IDataReader getAllData()
        {
        	return base.getAllData(TABLE_NAME);
        }

        public override void deleteAllData()
        {
        	base.deleteAllData(TABLE_NAME);
        } 


        // speziellere funktionen (anwendungsspezifisch)
       public IDataReader getDataByText(String antwort_text) {

            if(db_connection.State != ConnectionState.Open) {
                db_connection.Open();
            }

            IDbCommand dbCmd = getDbCommand();
            dbCmd.CommandText = "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ATEXT + "='" + antwort_text + "';";
            return dbCmd.ExecuteReader();
       }
        
	}

}

