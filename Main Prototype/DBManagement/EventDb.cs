using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace DataBank
{
    public class EventEntity
    {
        public string EventId;
        public string EventText;
        public string ParameterId;
        public int AenderungDirekt;

        public EventEntity(string eventId, string eventText, string parameterId, int aenderung_direkt)
        {
            EventId = eventId;
            EventText = eventText;
            ParameterId = parameterId;
            AenderungDirekt = aenderung_direkt;
        }


    }

    public class EventTable: SqliteHelper
    {
        private const string TABLE_NAME = "Event";

        // Namen der Tabellenspalten
        private const string KEY_EVID = "event_id";
        private const string KEY_EVTEXT = "event_text";
        private const string KEY_AFF_PID = "affected_parameter_id";
        private const string KEY_PCHANGENOW = "aenderung_direkt";
        private const string KEY_ERFUELLT = "bedingung_erfuellt";
        private const string KEY_REL_PID = "related_parameter_id";



        public EventTable() : base() { }


        public IDataReader GetAllDataByParameter(string parameterId)
        {
            if (db_connection.State != ConnectionState.Open)
            {
                db_connection.Open();
            }

            Debug.Log("getting Event Data by Parameter");
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText = "SELECT * FROM Event WHERE " + KEY_REL_PID + " ='" + parameterId + "' AND " + KEY_ERFUELLT +"= 1";
            return dbcmd.ExecuteReader();

        }

        public void UpdateBedingung(string eventId)
        {
            Debug.Log("About to update Bedingung event with id:" + eventId);
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "UPDATE " + TABLE_NAME
                + " SET " + KEY_ERFUELLT + "=" + 1
                + " WHERE " + KEY_EVID + "='" + eventId + "'";
            dbcmd.ExecuteNonQuery();
        }
 
    }











}
