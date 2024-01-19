using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.UI;
using TMPro;
using System.Xml.Linq;
using UnityEngine.Events;
using JetBrains.Annotations;
using Unity.VisualScripting;

// TODO: data reader mal auslagern in eigene funktion!!!
public class DecisionManager : MonoBehaviour
{
    private string lastDecisionId = "0"; // startet mit entscheidung mit id "0"


    // variables for ui textdisplay
    public TextMeshProUGUI entscheidungText;
    public TextMeshProUGUI antwort1Text;
    public TextMeshProUGUI antwort2Text;

    private List<AntwortOption> antwortList = new List<AntwortOption>();

    // variables for icon color display
    public ColorChange colorChangeScriptBoden;
    public ColorChange colorChangeScriptBiodiv;
    public ColorChange colorChangeScriptWasser;
    public ColorChange colorChangeScriptGeld;

    private List<ColorChange> colorScriptList = new List<ColorChange>();


    // variables for asset change
    public SpriteChanger spriteChangerScript = new SpriteChanger();


    

    void Start()
    {

        // Referenzen auf Skripte für das IconColor changen
        colorScriptList.Add(colorChangeScriptBoden);
        colorScriptList.Add(colorChangeScriptBiodiv);
        colorScriptList.Add(colorChangeScriptWasser);
        colorScriptList.Add(colorChangeScriptGeld);


        // zugriff auf buttons für antwortauswahl
        Button button1 = antwort1Text.GetComponent<Button>();
        Button button2 = antwort2Text.GetComponent<Button>();

        AntwortOption antwort1 = new AntwortOption(antwort1Text, button1);
        antwortList.Add(antwort1);
        AntwortOption antwort2 = new AntwortOption(antwort2Text, button2);
        antwortList.Add(antwort2);


        // referenzen auf DB-Tabellen
        
        ParameterTable parameterTable = new ParameterTable();
        
        
        AntwortTable antwortTable = new AntwortTable();
        
        
        EntscheidungTable entscheidungTable = new EntscheidungTable();


        // fill db

        Debug.Log("begin fill db");
        
        ParameterEntity param = new ParameterEntity("BODEN", 50, 0);
        ParameterEntity param0 = new ParameterEntity("WASSER", 50, 0);
        ParameterEntity param1 = new ParameterEntity("BIODIV", 50, 0); 
        ParameterEntity param2 = new ParameterEntity("GELD", 50, 0);

        parameterTable.deleteDataById("BODEN"); // deleted alte falsche daten
        parameterTable.addData(param);
        parameterTable.addData(param0);
        parameterTable.addData(param1);
        parameterTable.addData(param2);


        
        AntwortEntity antwortEntity1 = new AntwortEntity("0", "BODEN", "Düngen", 20, 0);
        antwortTable.addData(antwortEntity1);
        AntwortEntity antwortEntity2 = new AntwortEntity("1", "BODEN", "Nicht Düngen", 0, -10);
        antwortTable.addData(antwortEntity2);

        AntwortEntity antwortEntity3 = new AntwortEntity("2", "BODEN", "Mais pflanzen", -20, 0);
        antwortTable.addData(antwortEntity3);
        AntwortEntity antwortEntity4 = new AntwortEntity("3", "BODEN", "Mais nicht pflanzen", 10, 0);
        antwortTable.addData(antwortEntity4);
        

        
        EntscheidungEntity entscheidungEntity1 = new EntscheidungEntity("0", "ANBAU", "0", "1", "Boden düngen?");
        entscheidungTable.addData(entscheidungEntity1);
        EntscheidungEntity entscheidungEntity2 = new EntscheidungEntity("1", "ANBAU", "2", "3", "Mais pflanzen?");
        entscheidungTable.addData(entscheidungEntity2);


        // Datenbank abrufen und erste Entscheidung anzeigen
        DisplayDecision(entscheidungTable, antwortTable, parameterTable, lastDecisionId);

        entscheidungTable.close();
        antwortTable.close();
        parameterTable.close();
    }

    void DisplayDecision(EntscheidungTable etable, AntwortTable antwortTable, ParameterTable parameterTable, string decisionId)
    {
        // lädt entscheidungen aus Db
        
        Debug.Log("getting the big join...");

        IDataReader reader = etable.GetDecisionWithAnswers(decisionId);

        while(reader.Read()) 
        {
            
            entscheidungText.text = reader["entscheidung_text"].ToString();
            
            antwortList[0].antwortText.text = reader["aw1t"].ToString(); // greift auf Textkomponente des TMP objekts des AntwortOption objekts zu. puh
            
            antwortList[1].antwortText.text = reader["aw2t"].ToString();
        }

        reader.Close();

        // Event-Handler für Antwortklicks hinzufügen
        AddAnswerClickHandlers(etable, antwortTable, parameterTable);
    }

    void AddAnswerClickHandlers(EntscheidungTable etable, AntwortTable antwortTable, ParameterTable parameterTable)
    {
        // on click event handler für antwortmöglichkeiten. Ruft bei Klick OnAnswerClick mit entspr. ParameterDaten auf
        foreach (var antwort in antwortList)
        {
            // schleifeninterne variable
            string paramId = "";
            int paramAenderung_direkt = readerIntHelper(antwortTable.getDataByText(antwort.antwortText.text), "aenderung_direkt");
            int paramAenderung_verzoegert = readerIntHelper(antwortTable.getDataByText(antwort.antwortText.text), "aenderung_verzoegert");
            
            Debug.Log("direkte aenderung: " + paramAenderung_direkt + " verz aenderung: " + paramAenderung_verzoegert);

            IDataReader reader = antwortTable.getDataByText(antwort.antwortText.text); // liest entsprechenden parameterwert und parameter id aus den jeweiligen antworten aus und übergibt diese an onclick handler (falls antwort geklickt wirt), um berechnung etc durchzuführen
            
            while(reader.Read())
            {
                paramId = reader["parameter_id"].ToString();    
            }
            reader.Close();

            antwort.button.onClick.AddListener(() => OnAnswerClicked(etable, antwortTable, parameterTable, paramId, paramAenderung_direkt, paramAenderung_verzoegert));
        }


    }

    void OnAnswerClicked(EntscheidungTable etable, AntwortTable antwortTable, ParameterTable parameterTable, string paramId, int paramAenderung_direkt, int paramAenderung_verzoegert)
    {
        
        // parameter aktualisieren in db
        parameterTable.updateData(paramId, paramAenderung_direkt);


        // verzögerte Parameter aktualisieren in Db
        parameterTable.updateVerzoegertStack(paramId, paramAenderung_verzoegert);


        // updates color of parameter icons (nachdem neuer wert in Db ist!!)
        UpdateColor(parameterTable, paramId);

        CheckParameters(parameterTable);


        // Berechnung der nächsten Entscheidung und Anzeige
        lastDecisionId = CalculateNextDecisionId();
        DisplayDecision(etable, antwortTable, parameterTable, lastDecisionId);
    }

    string CalculateNextDecisionId()
    {
        // Code zur Berechnung der nächsten Entscheidung 
        

        // returnt ID der nächsten Entscheidung
        return "1";
    }

    void UpdateColor(ParameterTable parameterTable, string paramId) {

        // determine which color change script to use
        foreach (ColorChange script in colorScriptList) {

            // wenn das Script für den entsprechenden Parameter gefunden wird
            // accesses color change script to change icon color
            if(script.parameterId == paramId) {
                script.SetValue(parameterTable, paramId);
            }
        }
    }

    void CheckParameters(ParameterTable parameterTable) {
        
        int bodenWert = readerIntHelper(parameterTable.getDataById("BODEN"), "parameter_wert");
        int biodivWert = readerIntHelper(parameterTable.getDataById("BIODIV"), "parameter_wert");
        int wasserWert = readerIntHelper(parameterTable.getDataById("WASSER"), "parameter_wert");
        int geldWert = readerIntHelper(parameterTable.getDataById("GELD"), "parameter_wert");



        // Für Prototyp wird jetzt nur eine Möglichkeit geprüft. 
        if(bodenWert <= 20) {
            spriteChangerScript.BeginTilechange();

            // nicht Teil des Prototyps;
            // hier findet aber in Zukunft eine Berechnung statt, die bestimmten Events eine Wahrscheinlichkeit gibt, einzutreffen;
            // je "extremer" ein Wert ausfällt, d.h. je weiter er von der "neutralen Mitte" (50) abweicht, desto wahrscheinlicher 
            // ist es für ein entsprechendes Event, einzutreffen.
            // Beispiel: 
            TriggerEvent();
        }

    }

    void TriggerEvent() {
        


    }


    int readerIntHelper(IDataReader reader, string spaltenname) {


        int outputInt = 0;

        while(reader.Read())
        {

            object localObject = reader[spaltenname];

            if (localObject != null && localObject != System.DBNull.Value)
            {
                if (int.TryParse(localObject.ToString(), out outputInt))
                {
                    // Successfully parsed to int
                    Debug.Log("Current value of " + spaltenname + ": " + outputInt.ToString());
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
                Debug.LogWarning( spaltenname + " is DBNull or null.");
                return -1;
            }
            
        }
        reader.Close();
        return -1;
    }
}



//[System.Serializable]
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
