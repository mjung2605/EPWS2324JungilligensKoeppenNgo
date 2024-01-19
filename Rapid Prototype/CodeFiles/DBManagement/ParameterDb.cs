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

        public static ParameterEntity GetFakeParameter()
        {
            return new ParameterEntity("WATER", 50, 0);
        }

    }

    public class ParameterTable : SqliteHelper {

        private const String Tag = "Riz: ParameterDb:\t";
        
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
        public void updateData(string parameterId, int aenderung_direkt) 
        {

            // updated parameter wert: Sofort-Auswirkungen

            
            if(db_connection.State != ConnectionState.Open) {
                db_connection.Open();
            }

            Debug.Log("About to update parameter value on verzögert stack");


            int currentValueInt = 200;
            int updatedValue;

            IDataReader reader = getDataById(parameterId); 
            while(reader.Read()) {

                object parameterWertObject = reader["parameter_wert"];

                if (parameterWertObject != null && parameterWertObject != System.DBNull.Value)
                {
                    if (int.TryParse(parameterWertObject.ToString(), out currentValueInt))
                    {
                        // Successfully parsed to int
                        Debug.Log("Current value of newValueInt: " + currentValueInt.ToString());
                    }
                    else
                    {
                        Debug.LogError("Failed to parse parameter_wert to int.");
                    }
                }      
                else
                {
                    Debug.LogWarning("parameter_wert is DBNull or null.");
                }
            }
            reader.Close();
            
            
            updatedValue = currentValueInt + aenderung_direkt;
            Debug.Log("addiert ergibt "+ currentValueInt + " und " +  aenderung_direkt + " = " + updatedValue);
            

            Debug.Log("About to update parameter value: Write new value in db");
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "UPDATE " + TABLE_NAME
                + " SET " + KEY_PVALUE + "=" + updatedValue
                + " WHERE " + KEY_PID + "='" + parameterId + "'";
            dbcmd.ExecuteNonQuery();
                
        }

        public void updateVerzoegertStack(string parameterId, int aenderung_verzoegert) {

            // sammelt erst später "auszulösende" Parameteränderungen auf einem "stack" (pro parameter)

            if(db_connection.State != ConnectionState.Open) {
                db_connection.Open();
            }

            Debug.Log("About to update verzögert stack");


            int currentValueInt = 200;
            int updatedValue;

            IDataReader reader = getDataById(parameterId); 
            while(reader.Read()) {

                object parameterWertObject = reader["parameter_wert"];

                if (parameterWertObject != null && parameterWertObject != System.DBNull.Value)
                {
                    if (int.TryParse(parameterWertObject.ToString(), out currentValueInt))
                    {
                        // Successfully parsed to int
                        Debug.Log("Current value of newValueInt: " + currentValueInt.ToString());
                    }
                    else
                    {
                        Debug.LogError("Failed to parse parameter_wert to int.");
                    }
                }      
                else
                {
                    Debug.LogWarning("parameter_wert is DBNull or null.");
                }
            }
            reader.Close();
            
            
            updatedValue = currentValueInt + aenderung_verzoegert;
            Debug.Log("addiert ergibt "+ currentValueInt + " und " +  aenderung_verzoegert + " = " + updatedValue);
            

            Debug.Log("About to update parameter value: Write new value in db");
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "UPDATE " + TABLE_NAME
                + " SET " + KEY_PVALUE + "=" + updatedValue
                + " WHERE " + KEY_PID + "='" + parameterId + "'";
            dbcmd.ExecuteNonQuery();



        }


        public void updateDataAddVerzoegert(string parameterId) {


            // am Ende des JAhres aufgerufen. Verrechnet akkumulierte Werte aus der verzoegert Spalte mit aktuellen Werten des Parameters
            // TODO: verzoegert danach auf null setzen

            if(db_connection.State != ConnectionState.Open) {
                db_connection.Open();
            }

            Debug.Log("About to add verzögert stack to parameter wert");


            
            int currentValueInt = 200;
            int currentValueIntStack = 200;
            int updatedValue;


            
            IDataReader reader = getDataById(parameterId); 
            while(reader.Read()) {
                
                // get current Wert
                object parameterWertObject = reader["parameter_wert"];

                if (parameterWertObject != null && parameterWertObject != System.DBNull.Value)
                {
                    if (int.TryParse(parameterWertObject.ToString(), out currentValueInt))
                    {
                        // Successfully parsed to int
                        Debug.Log("Current value of newValueInt: " + currentValueInt.ToString());
                    }
                    else
                    {
                        Debug.LogError("Failed to parse parameter_wert to int.");
                    }
                }      
                else
                {
                    Debug.LogWarning("parameter_wert is DBNull or null.");
                }

                // get current Stack
                object parameterStackObject = reader["verzoegert_stack"];

                if (parameterStackObject != null && parameterStackObject != System.DBNull.Value)
                {
                    if (int.TryParse(parameterStackObject.ToString(), out currentValueIntStack))
                    {
                        // Successfully parsed to int
                        Debug.Log("Current value of newValueInt: " + currentValueIntStack.ToString());
                    }
                    else
                    {
                        Debug.LogError("Failed to parse parameter_wert to int.");
                    }
                }      
                else
                {
                    Debug.LogWarning("parameter_wert is DBNull or null.");
                }


            }
            reader.Close();
            
            
            updatedValue = currentValueInt + currentValueIntStack;
            Debug.Log("addiert ergibt "+ currentValueInt + " und " +  currentValueIntStack + " = " + updatedValue);
            

            Debug.Log("About to update parameter value: Write new value in db");
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "UPDATE " + TABLE_NAME
                + " SET " + KEY_PVALUE + "=" + updatedValue
                + " WHERE " + KEY_PID + "='" + parameterId + "'";
            dbcmd.ExecuteNonQuery();

        }


    }

}
