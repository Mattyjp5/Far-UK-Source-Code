using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedTextUpdate : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI Speed;


    private void Update()
    {
        Speed.text = CameraControls.CobotSpeed.ToString();
    }
}
