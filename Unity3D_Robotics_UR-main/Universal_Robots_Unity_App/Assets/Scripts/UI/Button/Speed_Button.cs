using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

public class Speed_Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // -------------------- String -------------------- //
    public float addition;
    
    // -------------------- Int -------------------- //
    public int index;
    // -------------------- UTF8Encoding -------------------- //
    private UTF8Encoding utf8 = new UTF8Encoding();

    // -------------------- Button -> Pressed -------------------- //
    public void OnPointerDown(PointerEventData eventData)
    {
        CameraControls.CobotSpeed = CameraControls.CobotSpeed + addition;
    }

    // -------------------- Button -> Un-Pressed -------------------- //
    public void OnPointerUp(PointerEventData eventData)
    {
        // confirmation variable -> is un-pressed
        ur_data_processing.UR_Control_Data.button_pressed[index] = false;
    }
}
