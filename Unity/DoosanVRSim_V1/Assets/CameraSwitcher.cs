using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera cameraMain;
    public Camera cameraSide;
    public Camera cameraTop;
    public Camera cameraArmCloseup;

    void Start()
    {
        EnableOnly(cameraMain); // Start with the main camera
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            EnableOnly(cameraMain);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            EnableOnly(cameraSide);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            EnableOnly(cameraTop);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            EnableOnly(cameraArmCloseup);
    }

    void EnableOnly(Camera activeCamera)
    {
        cameraMain.enabled = false;
        cameraSide.enabled = false;
        cameraTop.enabled = false;
        cameraArmCloseup.enabled = false;

        activeCamera.enabled = true;
    }
}
