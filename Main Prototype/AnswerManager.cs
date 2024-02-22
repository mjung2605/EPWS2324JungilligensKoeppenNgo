using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.UI;
using TMPro;
using System;
using System.Xml.Linq;
using UnityEngine.Events;
using JetBrains.Annotations;
using Unity.VisualScripting;


public class DecisionManager : MonoBehaviour
{
    private string lastDecisionId = "0"; // startet mit entscheidung mit id "0"


    // variables for ui textdisplay
    public TextMeshProUGUI entscheidungText;
    public TextMeshProUGUI antwort1Text;
    public TextMeshProUGUI antwort2Text;

    private List<AntwortOption> antwortList = new();

    // variables for icon color display
    public ColorChange colorChangeScriptBoden;
    public ColorChange colorChangeScriptBiodiv;
    public ColorChange colorChangeScriptWasser;
    public ColorChange colorChangeScriptGeld;

    private List<ColorChange> colorScriptList = new();


    // variables for asset change
    public SzeneWechselSkript sceneChangeScript;

    // variables for phase change
    public PhaseChange phaseChangeScript;

    // variables for year-end event (UI + button)
    public EventManager eventManagerScript;

    // DB Tables

    ParameterTable parameterTable = new();

    AntwortTable antwortTable = new();

    EntscheidungTable entscheidungTable = new();

    EventTable eventTable = new();

    string sceneIndex;



    void Start()
    {

        string normaleMap = SzeneWechselSkript.NormaleMap;
        sceneIndex = normaleMap;

        // Referenzen auf Skripte für das IconColor changen
        colorScriptList.Add(colorChangeScriptBoden);
        colorScriptList.Add(colorChangeScriptBiodiv);
        colorScriptList.Add(colorChangeScriptWasser);
        colorScriptList.Add(colorChangeScriptGeld);


        // zugriff auf buttons für antwortauswahl
        Button button1 = antwort1Text.GetComponent<Button>();
        Button button2 = antwort2Text.GetComponent<Button>();

        AntwortOption antwort1 = new(antwort1Text, button1);
        antwortList.Add(antwort1);
        AntwortOption antwort2 = new(antwort2Text, button2);
        antwortList.Add(antwort2);


       

        // Hinzufügen und Löschen von DB-Daten findet durch das Team über https://sqlitebrowser.org statt

        // Datenbank abrufen und erste Entscheidung anzeigen
        DisplayDecision(lastDecisionId);

        entscheidungTable.close();
        antwortTable.close();
        parameterTable.close();
    }

    void DisplayDecision(string decisionId)
    {
        // lädt entscheidungen aus Db
        
        Debug.Log("Get Descision with ID: " + decisionId);
        Debug.Log("Current Phase: " + phaseChangeScript.phase.text);

        IDataReader reader = entscheidungTable.GetDecisionWithAnswers(decisionId);

        while(reader.Read()) 
        {
            
            entscheidungText.text = reader["entscheidung_text"].ToString();
            
            antwortList[0].antwortText.text = reader["aw1t"].ToString(); // greift auf Textkomponente des TMP objekts des AntwortOption objekts zu. puh
            
            antwortList[1].antwortText.text = reader["aw2t"].ToString();
        }

        reader.Close();

        // Event-Handler für Antwortklicks hinzufügen
        AddAnswerClickHandlers();
    }

    void AddAnswerClickHandlers()
    {
        // on click event handler für antwortmöglichkeiten. Ruft bei Klick OnAnswerClick mit entspr. ParameterDaten auf
        foreach (var antwort in antwortList)
        {
            // schleifeninterne variable
            string paramId = "";
            int paramAenderung_direkt = parameterTable.readerIntHelper(antwortTable.getDataByText(antwort.antwortText.text), "aenderung_direkt");
            int paramAenderung_verzoegert = parameterTable.readerIntHelper(antwortTable.getDataByText(antwort.antwortText.text), "aenderung_verzoegert");
            string eid_bedingung_erfuellt = null;
            string evid_bedingung_erfuellt = null;

            IDataReader reader = antwortTable.getDataByText(antwort.antwortText.text); // liest entsprechenden parameterwert und parameter id aus den jeweiligen antworten aus und übergibt diese an onclick handler (falls antwort geklickt wirt), um berechnung etc durchzuführen
            
            while(reader.Read())
            {
                // gets param_id
                paramId = reader["parameter_id"].ToString();
                // checks if bedingung-boolean on any entscheidung needs to be changed on click
                if (reader["eid_bedingung_erfuellt"] != null && reader["eid_bedingung_erfuellt"] != System.DBNull.Value)
                {
                    eid_bedingung_erfuellt = reader["eid_bedingung_erfuellt"].ToString();
                }
                // checks if bedingung-boolean on any event needs to be changed on click
                if (reader["evid_bedingung_erfuellt"] != null && reader["evid_bedingung_erfuellt"] != System.DBNull.Value)
                {
                    evid_bedingung_erfuellt = reader["evid_bedingung_erfuellt"].ToString();
                }
            }
            reader.Close();

            antwort.button.onClick.AddListener(() => OnAnswerClicked(antwortTable.getDataByText(antwort.antwortText.text).ToString(),paramId, paramAenderung_direkt, paramAenderung_verzoegert, eid_bedingung_erfuellt, evid_bedingung_erfuellt));
        }

    }

    public void OnAnswerClicked(string awid, string paramId, int paramAenderung_direkt, int paramAenderung_verzoegert, string eid_bedingung_erfuellt, string evid_bedingung_erfuellt)
    {
        
        // parameter aktualisieren in db
        parameterTable.updateData(paramId, paramAenderung_direkt, "parameter_wert");


        // verzögerte Parameter aktualisieren in Db
        parameterTable.updateData(paramId, paramAenderung_verzoegert, "verzoegert_stack");

        // change bedingung status
        if (eid_bedingung_erfuellt!=null)
        {
            entscheidungTable.UpdateBedingung(eid_bedingung_erfuellt);
        }
        if (evid_bedingung_erfuellt != null)
        {
            eventTable.UpdateBedingung(evid_bedingung_erfuellt);
        }

        // for now für Tomaten vs. Mais Asset Change: hardcoded check
        if(awid=="1") // wenn tomaten/Gemüse (anstatt mais/Getreide) angepflanzt werden sollen
        {
            sceneChangeScript.WechsleSzene(WhatToChange.PFLANZE);
        }


        // updates color of parameter icons (nachdem neuer wert in Db ist!!)
        UpdateColor(paramId);

        
        CheckParameters(); // FALLS wir in "REFLEKTION" sind wird hier danach zu "PLANUNG" gegangen, so haben wir keine konflikte mit der Entscheidungsauswahl in der nächsten line

        phaseChangeScript.GoToNextPhase(); // displayed im UI den Namen der nächsten Phase

        // Berechnung der nächsten Entscheidung und Anzeige
        lastDecisionId = CalculateNextDecisionId();
        DisplayDecision(lastDecisionId);
    }

    string CalculateNextDecisionId() 
    {
        // Wählt zufällig eine Entscheidung je nach aktueller Phase aus
        
        List<string> nextList = new();


        // aktuelle Phase wird aus public string aus phaseChangeScript gelesen
        Debug.Log("current phase: " + phaseChangeScript.phase.text);
        using IDataReader readerAll = entscheidungTable.GetDataByPhase(phaseChangeScript.phase.text);
        while (readerAll.Read()) {
            // IDs von Entscheidungen, die zur aktuellen Phase gehören, in eine Liste packen
            
            nextList.Add(readerAll["entscheidung_id"].ToString());
        }
        readerAll.Close();
        
        System.Random numberGenerator = new();

        // returnt id der nächsten entscheidung (random aus der Liste von available Entscheidungen ausgewählt)
        // AUSBLICK: evtl in Zukunft Gewichtungsmechanik einbaubar, so that certain descisions (maybe ones based on others) are more likely to be triggered


        string next = nextList[numberGenerator.Next(0, nextList.Count - 1)].ToSafeString();
        Debug.Log("nächste id: " + next);
        return next;

    }

    void UpdateColor(string paramId) {

        // determine which color change script to use
        foreach (ColorChange script in colorScriptList) {

            // wenn das Script für den entsprechenden Parameter gefunden wird
            // accesses color change script to change icon color
            if(script.parameterId == paramId) {
                script.SetValue(parameterTable, paramId);
            }
        }
    }

    void CheckParameters() {


        // Initialisieren von Parameter Objekten, die aktuellen Wert und Namen(id) getten und speichern

        Debug.Log("in checkParameters: Initializing new ParameterData Objects mit current data");
        ParameterData boden = new("BODEN", parameterTable.readerIntHelper(parameterTable.getDataById("BODEN"), "parameter_wert"));
        ParameterData biodiv = new("BIODIV", parameterTable.readerIntHelper(parameterTable.getDataById("BIODIV"), "parameter_wert"));
        ParameterData wasser = new("WASSER", parameterTable.readerIntHelper(parameterTable.getDataById("WASSER"), "parameter_wert"));
        ParameterData geld = new("GELD", parameterTable.readerIntHelper(parameterTable.getDataById("GELD"), "parameter_wert"));

        List<ParameterData> list = new()
        {
            boden, biodiv, wasser, geld // füllt liste mit diesen daten
        };

        // ÄNDERUNGEN IM LAUFE DES JAHRES: SceneChanger wie verwelkende Pflanzen etc
        
        // scene changes je nach Parameter
        if(boden.wert <= 20) {
            sceneChangeScript.WechsleSzene(WhatToChange.VERWEST); // verwesene/vertrocknete Pflanzen
        }
        if(wasser.wert <= 20)
        {
            sceneChangeScript.WechsleSzene(WhatToChange.FLUSS); // ausgetrockneter Fluss
        }


        // JAHRESENDE FUNKTIONALITÄT:

        if(phaseChangeScript.phase.text == "REFLEKTION") {

            // zur Vergleichsmöglichkeit wird das erste der objekte als extremstes gesetzt

            Boolean isLow = true; // übergibt, ob der Wert des Parameters am unteren Rand oder am oberen Rand ist
            ParameterData extremestValue = boden;

            foreach(ParameterData data in list) {

                // jeweilige differenzen werden berechnet und verglichen
                int diffLow = Math.Abs(data.wert - 0);
                int diffHigh = Math.Abs(data.wert - 100);

                if (diffLow < Math.Abs(extremestValue.wert - 0) || diffHigh > Math.Abs(extremestValue.wert - 100)) {
                    extremestValue = data;
                }
            }
            TriggerEvent(extremestValue.name, isLow);
        }
    }

    void TriggerEvent(string paramId, Boolean isLow) {

        // get event from db and extract text, paramname to change, aenderung_dir

        eventTable.GetAllDataByParameter(paramId);
        List<EventData> eventList = new();

        using IDataReader readerAll = entscheidungTable.GetDataByPhase(phaseChangeScript.phase.text);
        while (readerAll.Read())
        {
            // IDs von Entscheidungen, die zur aktuellen Phase gehören, in eine Liste packen

            eventList.Add(new EventData(readerAll["event_id"].ToString(), readerAll["event_text"].ToString(), readerAll["affected_parameter_id"].ToString(), entscheidungTable.readerIntHelper(readerAll, "aenderung_direkt")));
        }
        readerAll.Close();

        System.Random numberGenerator = new();

        // returnt ein Event (random aus der Liste von available Events ausgewählt)

        EventData nextEvent = eventList[numberGenerator.Next(0, eventList.Count - 1)];

        eventManagerScript.OpenPopup(nextEvent.eventText, () => OnEventClicked(nextEvent.parameterId, nextEvent.aenderung_direkt));

        
    }

    void OnEventClicked(string paramId, int paramAenderung_direkt) {

        if(paramAenderung_direkt != 0) {
            parameterTable.updateData(paramId, paramAenderung_direkt, "aenderung_direkt");

        }

        eventManagerScript.ClosePopup();

        phaseChangeScript.GoToNextYear(); // change phase to "PLANUNG", change year nr in UI

    }

}

class AntwortOption
{
    public TextMeshProUGUI antwortText;
    public Button button;

    // conatructor
    public AntwortOption(TextMeshProUGUI _text, Button _button) {
        antwortText = _text;
        button = _button;
    }
}

class ParameterData
{
    
    public string name;
    public int wert;

    public ParameterData(string _name, int _wert) {
        name = _name;
        wert = _wert;
    }
}

class EventData
{
    public string eventId;
    public string eventText;
    public string parameterId;
    public int aenderung_direkt;

    public EventData(string eventId, string eventText, string parameterId, int aenderung_direkt)
    {
        this.eventId = eventId;
        this.eventText = eventText;
        this.parameterId = parameterId;
        this.aenderung_direkt = aenderung_direkt;
    }
}
