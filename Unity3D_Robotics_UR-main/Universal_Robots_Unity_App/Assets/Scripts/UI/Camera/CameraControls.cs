using UnityEngine;

public class CameraControls : MonoBehaviour
{
    [SerializeField] Camera MyCamera;

    public static int CameraPanelAngle = 0;
    public static float CobotSpeed = 5f;

    //
    //0 = front view
    //1 = left view
    //2 = right view
    //3 = top view
    //


    void Start()
    {
        Update();
    }

    void Update()
    {
        UpdateHandle();
    }

    void UpdateHandle()
    {
        if (CameraPanelAngle==0)
        { CameraForFrontView(); }

        if (CameraPanelAngle == 1)
        { CameraForLeftView(); }

        if (CameraPanelAngle == 2)
        { CameraForRightView(); }

        if (CameraPanelAngle == 3)
        { CameraForTopView(); }
    }


    void CameraForFrontView()
    {
        if (MyCamera.transform.rotation.y < 0.25f)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Debug.Log(MyCamera.transform.rotation.y);
                MyCamera.transform.Rotate(new Vector​3(0.0f, 0.1f, 0.0f));
            }
        }
        if (MyCamera.transform.rotation.y > -0.25f)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                MyCamera.transform.Rotate(new Vector​3(0.0f, -0.1f, 0.0f));
            }
        }
    }

    void CameraForLeftView()
    {
       // Debug.Log(MyCamera.transform.rotation.y);

        if (MyCamera.transform.rotation.y < 0.36f)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Debug.Log(MyCamera.transform.rotation.y);
                MyCamera.transform.Rotate(new Vector​3(0.0f, 0.06f, 0.0f));
            }
        }
        if (MyCamera.transform.rotation.y > 0.14f)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                MyCamera.transform.Rotate(new Vector​3(0.0f, -0.06f, 0.0f));
            }
        }
    }

    void CameraForRightView()
    {
        //Debug.Log(MyCamera.transform.rotation.y);

        if (MyCamera.transform.rotation.y < -0.14f)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                Debug.Log(MyCamera.transform.rotation.y);
                MyCamera.transform.Rotate(new Vector​3(0.0f, 0.06f, 0.0f));
            }
        }
        if (MyCamera.transform.rotation.y > -0.36f)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                MyCamera.transform.Rotate(new Vector​3(0.0f, -0.06f, 0.0f));
            }
        }
    }

    void CameraForTopView()
    {
        // Debug.Log(MyCamera.transform.rotation.x);

        if (MyCamera.transform.rotation.x < 0.715f)
        {

            if (Input.GetKey(KeyCode.DownArrow))
            {
                MyCamera.transform.Rotate(new Vector​3(0.07f, 0.0f, 0.0f));
            }
        }

        if (MyCamera.transform.rotation.x > 0.60f)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                MyCamera.transform.Rotate(new Vector​3(-0.07f, 0.0f, 0.0f));
            }
        }
    }
}
