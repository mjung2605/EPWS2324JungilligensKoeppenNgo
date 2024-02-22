using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using UnityEngine;

namespace DataBank{

    public class ParameterEntity {

        public string ParameterId;
        public int ParameterWert;
        public int VerzoegertStack;

        public ParameterEntity (string parameterId, int parameterWert, int verzoegertStack)
        {
            ParameterId = parameterId;
            ParameterWert = parameterWert;
            VerzoegertStack = verzoegertStack;
        }


    }

    public class ParameterTable : SqliteHelper {
        
        private const String TABLE_NAME = "Parameter";

        // Namen der Tabellenspalten
    	private const String KEY_PID = "parameter_id"; 
    	private const String KEY_PVALUE = "parameter_wert";
        private const String KEY_PSTACK = "verzoegert_stack";


        public ParameterTable(): base()
        {
            /* 
            IDbCommand dbcmdDrop = getDbCommand();
            dbcmdDrop.CommandText = "DROP TABLE IF EXISTS parameter";
            dbcmdDrop.ExecuteNonQuery();
            

            IDbCommand dbcmdDrop2 = getDbCommand();
            dbcmdDrop2.CommandText = "DROP TABLE IF EXISTS Parameters";
            dbcmdDrop2.ExecuteNonQuery();
            
            IDbCommand dbcmd = getDbCommand();
        	dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + "(parameter_id TEXT PRIMARY KEY, parameter_wert FLOAT NOT NULL) ";
        	dbcmd.ExecuteNonQuery();

            */
        }

        public void addData(ParameterEntity parameter) // Datensatz hinzufügen
        {
        	IDbCommand dbcmd = getDbCommand();
        	dbcmd.CommandText =
            	"INSERT OR REPLACE INTO " + TABLE_NAME
            	+ " (parameter_id, parameter_wert) VALUES ( '"
            	+ parameter.ParameterId + "', '"
            	+ parameter.ParameterWert + "' )";
        	dbcmd.ExecuteNonQuery();
        }

        public override IDataReader getDataById(string id)
        {
            if(db_connection.State != ConnectionState.Open) {
                db_connection.Open();
            }
        	return base.getDataById(id, TABLE_NAME, KEY_PID);
        }

        public override void deleteDataById(string id)
        {
        	base.deleteDataById(id, TABLE_NAME, KEY_PID);
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
        public void updateData(string parameterId, int aenderung, string spaltenname) 
        {

            // updated parameter wert: ob Sofort-Auswirkung oder Verzögert wird beim Aufruf gemanaged (übergeben in Parameter spaltenname)

            
            if(db_connection.State != ConnectionState.Open) {
                db_connection.Open();
            }


            int currentValueInt;
            int updatedValue;

            IDataReader reader = getDataById(parameterId);
            currentValueInt = readerIntHelper(reader, spaltenname);

            
            updatedValue = currentValueInt + aenderung;
            
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "UPDATE " + TABLE_NAME
                + " SET " + spaltenname + "=" + updatedValue
                + " WHERE " + KEY_PID + "='" + parameterId + "'";
            dbcmd.ExecuteNonQuery(); 
                
        }



        public void updateDataAddVerzoegert(string parameterId) {


            // am Ende des Jahres aufgerufen. Verrechnet akkumulierte Werte aus der verzoegert Spalte mit aktuellen Werten des Parameters
            // TODO: verzoegert danach auf null setzen

            if(db_connection.State != ConnectionState.Open) {
                db_connection.Open();
            }



            // gettet aktuellen Wert des Parameters
            IDataReader readerVal = getDataById(parameterId);
            int currentValueInt = readerIntHelper(readerVal, "parameter_wert");

            // gettet aktuellen Wert des Parameter-delayed stacks
            IDataReader readerDelay = getDataById(parameterId);
            int currentValueIntStack = readerIntHelper(readerDelay, "verzoegert_stack");

            // verrechnet beide Werte
            int updatedValue = currentValueInt + currentValueIntStack;
            Debug.Log("addiert ergibt "+ currentValueInt + " und " +  currentValueIntStack + " = " + updatedValue);
            
            // schreibt neuen Wert in parameter_value


            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "UPDATE " + TABLE_NAME
                + " SET " + KEY_PVALUE + "=" + updatedValue
                + " WHERE " + KEY_PID + "='" + parameterId + "'";
            dbcmd.ExecuteNonQuery();

            Debug.Log("About to update parameter value:reset verzoegert stack");
            IDbCommand dbcmd2 = getDbCommand();
            dbcmd2.CommandText = 
                "UPDATE " + TABLE_NAME
                + " SET " + KEY_PSTACK + "=" + 0
                + " WHERE " + KEY_PID + "='" + parameterId + "'";
            dbcmd.ExecuteNonQuery();

        }

    }

}
