using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public GameObject popup;
    public TextMeshProUGUI eventTMP; // add 
    public Button acceptButton;

    void Start()
    {
        ClosePopup();
    }

    // delegate (hier UnityAction) in c# = vgl. lambda in kotlin
    public void OpenPopup(string eventText, UnityEngine.Events.UnityAction answerClickHandler)
    {
        
        eventTMP.text = eventText;
        acceptButton.onClick.AddListener(answerClickHandler); // OnAnswerClicked wird übergeben, dieser beinhaltet auch die ClosePopup Funktion (unten)

        popup.SetActive(true);
    }



    public void ClosePopup()
    {
        popup.SetActive(false);
    }
}
