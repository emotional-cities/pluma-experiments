#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class LookWithMouse : MonoBehaviour
{
    public Transform playerBody;

    public float MouseSensitivity = 200f;
    public float OrientationSlerp = 0.1f;
    private float mouseXLerp = 0.0f;
    private float mouseYLerp = 0.0f;
    private float xRotation;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Mouse.current.leftButton.isPressed)
            Look();
    }
    
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.fixedDeltaTime;

        mouseXLerp = Lerp(mouseXLerp, mouseX, OrientationSlerp);
        mouseYLerp = Lerp(mouseYLerp, mouseY, OrientationSlerp);

        float desiredX = playerBody.transform.localRotation.eulerAngles.y + mouseXLerp;

        
        xRotation -= mouseYLerp; 

        // Avoid tilt more than 180 degrees to ensure usability in fps mode
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        
        //Apply rotation
        playerBody.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
    }
    private float Lerp(float v0, float v1, float smooth)
    {
        return (1 - smooth) * v0 + smooth * v1;
    }
}
