using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float sensitivity = 2.0f;

    private bool active = false;

    public float yaw = 0.0f;
    public float pitch = 0.0f;

    void Update()
    {
        if (this.active)
        {
            this.yaw += this.sensitivity * Input.GetAxis("Mouse X");
            this.pitch -= this.sensitivity * Input.GetAxis("Mouse Y");

            this.transform.localEulerAngles = new Vector3(pitch, 0.0f, 0.0f);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                this.active = false;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            this.active = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
