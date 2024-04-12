#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookWithMouse : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;


    // Start is called before the first frame update
    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
#if ENABLE_INPUT_SYSTEM
        float mouseX = 0, mouseY = 0;

        if (Mouse.current != null)
        {
            if(Mouse.current.leftButton.isPressed)
            {
                var delta = Mouse.current.delta.ReadValue() / 15.0f;
                mouseX += delta.x;
                mouseY += delta.y;
            }
        }
        if (Gamepad.current != null)
        {
            var value = Gamepad.current.rightStick.ReadValue() * 2;
            mouseX += value.x;
            mouseY += value.y;
        }

        mouseX *= mouseSensitivity * Time.deltaTime;
        mouseY *= mouseSensitivity * Time.deltaTime;
#else
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
#endif

        playerBody.Rotate(Vector3.up * mouseX);
        playerBody.Rotate(Vector3.right * -mouseY);
        playerBody.transform.eulerAngles = new Vector3(playerBody.transform.eulerAngles.x, playerBody.transform.eulerAngles.y, 0);
    }
}
