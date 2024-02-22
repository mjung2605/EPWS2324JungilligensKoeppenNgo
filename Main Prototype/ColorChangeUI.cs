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

     
        IDataReader reader = parameterTable.getDataById(parameterId);
        int newValueInt = parameterTable.readerIntHelper(reader, "parameter_wert");
        
        
        if(newValueInt == -1) { // check if int was parsed successfully
            Debug.Log("Unsuccessful Int Parsing");
        }
        
        
        
        value = (float)newValueInt / 100.0f; // has to be within 0 and 1
        UpdateColor();
        
        
    }

    private void UpdateColor()
    {
        
        if(image == null) {
            Debug.Log("No image assigned?");
        }
        
        image.color = colorGradient.Evaluate(value);
    }
}