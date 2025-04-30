using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AcceptButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] TMP_InputField SpeedInput;

    float value;

    void Update()
    {
        string speedInputString = SpeedInput.text;
        
        // Check if string is not empty and try to parse it
        if (!string.IsNullOrWhiteSpace(speedInputString) && float.TryParse(speedInputString, out float parsedValue))
        {
            value = parsedValue;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CameraControls.CobotSpeed = value;
    }
}