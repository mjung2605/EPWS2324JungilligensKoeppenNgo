using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using UnityEngine;

namespace DataBank
{
    // Klasse mit Struktur für Tabellendaten (Entscheidung)
	public class EntscheidungEntity {


        public string EntscheidungId; // pk
        public string EntscheidungPhase; // pk
        public string AntwortId1; // fk
        public string AntwortId2; // fk
        public string EntscheidungText;


        // konstruktor
        public EntscheidungEntity(string entscheidungId, string entscheidungPhase, string antwortId1, string antwortId2, string entscheidungText)
        {
            EntscheidungId = entscheidungId;
            EntscheidungPhase = entscheidungPhase;
            AntwortId1 = antwortId1;
            AntwortId2 = antwortId2;
            EntscheidungText = entscheidungText;
        }

        public static EntscheidungEntity GetFakeEntscheidung()
        {
            return new EntscheidungEntity("1", "Anbau", "1", "2", "hallo hallo");
        }
    
	}

    // Tabellenimplementation: Entscheidung

    public class EntscheidungTable : SqliteHelper {
		private const String Tag = "Riz: EntscheidungDb:\t";
        
        private const String TABLE_NAME = "Entscheidung";

        // Namen der Tabellenspalten
    	private const String KEY_EID = "entscheidung_id"; 
        private const String KEY_EPHASE = "entscheidung_phase";
    	private const String KEY_ANTWID1 = "antwort_id1";        	
        private const String KEY_ANTWID2 = "antwort_id2";
        private const String KEY_ETEXT = "entscheidung_text";
        
        
        public EntscheidungTable() : base() // nimmt Konstruktor von "super" class
        {
            /*
        	IDbCommand dbcmd = getDbCommand();
        	dbcmd.CommandText = "CREATE TABLE "Entscheidung" (
	            "entscheidung_id"	TEXT NOT NULL,
	            "entscheidung_phase"	TEXT NOT NULL,
	            "antwort_id1"	TEXT NOT NULL,
	            "antwort_id2"	TEXT NOT NULL,
	            "entscheidung_text"	TEXT NOT NULL,
	            PRIMARY KEY("entscheidung_id","entscheidung_phase")
            );";
        	dbcmd.ExecuteNonQuery();
            */
        }
        public void addData(EntscheidungEntity entscheidung) // Datensatz hinzufügen
        {
        	IDbCommand dbcmd = getDbCommand();
        	dbcmd.CommandText =
            	"INSERT OR REPLACE INTO " + TABLE_NAME
            	+ " ( "
            	+ KEY_EID + ", "
                + KEY_EPHASE + ", "
            	+ KEY_ANTWID1 + ", "
            	+ KEY_ANTWID2 + ", "
            	+ KEY_ETEXT + " ) "
            	+ "VALUES ( '"
            	+ entscheidung.EntscheidungId + "', '"
                + entscheidung.EntscheidungPhase + "', '"
            	+ entscheidung.AntwortId1 + "', '"
            	+ entscheidung.AntwortId2 + "', '"
            	+ entscheidung.EntscheidungText + "' )";
        	dbcmd.ExecuteNonQuery();
        }

        // table name und pk (name + wert) in argumentliste gesetzt und funktion der höheren klasse genutzt
        public override IDataReader getDataById(string id)
        {
        	return base.getDataById(id, TABLE_NAME, KEY_EID);
        }
        
        public override void deleteDataById(string id)
        {
        	base.deleteDataById(id, TABLE_NAME, KEY_EID);
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

        public IDataReader GetDecisionWithAnswers(string entscheidungId) // joins Entscheidung und Antw1 und Antw2 ; left join = alles aus entscheidungstabelle (links) und nur matching aus antworttabelle (rechts)
        {
            if(db_connection.State != ConnectionState.Open) {
                db_connection.Open();
            }
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText = "SELECT Entscheidung.entscheidung_text, Antwort1.antwort_text AS aw1t, Antwort2.antwort_text AS aw2t " +
                                "FROM Entscheidung " +
                                "LEFT JOIN Antwort AS Antwort1 ON Entscheidung.antwort_id1 = Antwort1.antwort_id " +
                                "LEFT JOIN Antwort AS Antwort2 ON Entscheidung.antwort_id2 = Antwort2.antwort_id " +
                                "WHERE Entscheidung.entscheidung_id = " + entscheidungId;
            Debug.Log("huge join start" + dbcmd.CommandText);
            return dbcmd.ExecuteReader();
        }
        
	}

}

