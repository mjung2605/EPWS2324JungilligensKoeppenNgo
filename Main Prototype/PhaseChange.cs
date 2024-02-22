using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhaseChange : MonoBehaviour
{ 

    public TextMeshProUGUI phase;

    public TextMeshProUGUI year;

    List<string> phaseList = new()
    {
        "PLANUNG", "ANBAU", "ERNTE", "REFLEKTION"
    };


    // Start is called before the first frame update
    void Start()
    {

        // default start phase
        phase.text = "PLANUNG"; // text, der im UI displayed wird und 
                                //außerdem in Answermanager genutzt wird um in DB Phase abzugleichen und die richtigen Entscheidungen zu getten

        year.text = "1";
        
    }


    public void GoToNextPhase() { // geht nach ablauf der mögl. Anzahl Entsch automatisch zur nächsten Phase über (außer in Phase Reflektion)
        
        if(phase.text=="REFLEKTION")
        {
            GoToNextYear();
            return;
        }

        Debug.Log(" du bist hier: PhaseChange: GoToNextPhase");
        for (int i = 0; i < 3; i++) // iteriert durch phaseList und gleicht ab, welche die aktuelle Phase ist (0 = planung, 1 = anbau, 2 = ernte)
        {                           // in AnswerManager kann auf die public variable der Phase zugegriffen werden. bei Phase Reflektion muss noch 
                                    // nach Lesen des Events (und drücken auf OK oder so) die Phase manuell auf Neues Jahr etc gestellt werden (Funktion unten)
                                    // damit vor dem automatischen Phasenchange noch das Event in ruhe angezeigt werden kann (AnswerManager) etc dies das
            if(phase.text==phaseList[i]) {
                phase.text=phaseList[i+1]; // PhasenText auf nächste Phase gesetzt
                break;
            }
        }
        
    }

    public void GoToNextYear() {


        Debug.Log(" du bist hier: PhaseChange: GoToNextYear");

        if(phase.text!="REFLEKTION") { // error handling/catch
            Debug.Log("Die Funktion zum Jahreswechsel sollte hier nicht aufgerufen werden - Die aktuelle Phase ist nicht Reflektion, sondern " + phase.text);
        
        } else { // geht zur ersten Phase des nächsten Jahres und inkrementiert Jahr-Nr um 1

            Debug.Log("Jahreswechsel...");
            phase.text = "PLANUNG";
            int yearAsInt = int.Parse(year.text);
            yearAsInt++;
            year.text = yearAsInt.ToString();

        }

    }

}
