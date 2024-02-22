using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SzeneWechselSkript : MonoBehaviour
{

    // Namen der Ziel-Szene
    public const string NormaleMap = "Normale Map"; // mit mais und wasser
    public const string Tomaten = "Tomaten";
    public const string Tomatenverwest = "Tomatenverwest";
    public const string Maisverwest = "Maisverwest";
    public const string NoWaterMais = "NoWaterMais"; 
    public const string NoWaterTomaten = "NoWaterTomaten"; 


    // Funktion zum Szene-Wechsel
    public void WechsleSzene(WhatToChange change)
    {

        string activeSceneName = SceneManager.GetActiveScene().name;

        string sceneName = null;

        if (change==WhatToChange.PFLANZE)
        {

            switch (activeSceneName)
            {
                case NoWaterMais:
                    sceneName = NoWaterTomaten;
                    break;
                case Maisverwest:
                    sceneName = Tomatenverwest;
                    break;
                default:
                    sceneName = Tomaten;
                    break;
            }
            
        }
        else if (change==WhatToChange.VERWEST){

            if (activeSceneName.Contains("Tomaten")) 
            {
                sceneName = Tomatenverwest;
            } 
            else if (activeSceneName.Contains("Mais") || activeSceneName==NormaleMap)
            {
                sceneName= Maisverwest;
            }
        }
        else if (change==WhatToChange.FLUSS){

            if (activeSceneName.Contains("Tomaten")) 
            {
                sceneName = NoWaterTomaten;
            } 
            else if (activeSceneName.Contains("Mais") || activeSceneName==NormaleMap)
            {
                sceneName= NoWaterMais;
            }
        }

        
        SceneManager.LoadScene(sceneName);
        
    }
}
public enum WhatToChange
{
    VERWEST, PFLANZE, FLUSS
};
