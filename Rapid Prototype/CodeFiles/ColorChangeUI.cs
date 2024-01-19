using UnityEngine;
using UnityEngine.UI;
using DataBank;
using System.Data;

public class ColorChange : MonoBehaviour
{
    public Gradient colorGradient;
    public Image image;

    private float value;
    public string parameterId;

    private void Start()
    {
        ParameterTable parameterTable = new ParameterTable();

        SetValue(parameterTable, parameterId);
        //image = GetComponent<Image>(); ------ wird jetzt manually im editor assigned, sonst error
        UpdateColor();
    }

    public void SetValue(ParameterTable parameterTable, string parameterId)
    {

        
        int newValueInt = 50; // fallback

        
        using IDataReader reader = parameterTable.getDataById(parameterId);
        while(reader.Read()) {
            
            object parameterWertObject = reader["parameter_wert"];

            if (parameterWertObject != null && parameterWertObject != System.DBNull.Value)
            {
                if (int.TryParse(parameterWertObject.ToString(), out newValueInt))
                {
                    // Successfully parsed to int
                    Debug.Log("Current value of newValueInt: " + newValueInt.ToString());
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
        
        /*
        if(newValueInt == -1.0f) { // check if reassign in while-loop was successful
            Debug.Log("Unsuccessful");
            
        }
        */
        
        value = (float)newValueInt / 100.0f; // has to be within 0 and 1
        Debug.Log("Umrechnung von int zu float: " + newValueInt + " " + value);
        UpdateColor();
        
        
    }

    private void UpdateColor()
    {
        Debug.Log("Updating Icon Color..." + value);
        if(image == null) {
            Debug.Log("No image assigned?");
        }
        
        image.color = colorGradient.Evaluate(value);
    }
}