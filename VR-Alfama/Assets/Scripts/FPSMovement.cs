using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FPSMovement : MonoBehaviour
{
    private Rigidbody body;
    [SerializeField]
    float speed;
    [SerializeField] 
    Transform cam;
    [SerializeField] 
    float  threshold;
    // Start is called before the first frame update
    UnityEngine.XR.InputDevice inputDevice;
   
    void Start()
    {
        

        body = GetComponent<Rigidbody>();
    }
    bool GetDevice()
    {
        var rightHandedControllers = new List<UnityEngine.XR.InputDevice>();
        var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, rightHandedControllers);
        bool devices = false;
        foreach (var device in rightHandedControllers)
        {
            Debug.Log(string.Format("Device name '{0}' has characteristics '{1}'", device.name, device.characteristics.ToString()));
            inputDevice = device;
            devices = true;
        }
        return devices;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 value;
        if (!inputDevice.isValid)
        {
            GetDevice();
        }
        inputDevice.TryGetFeatureValue(CommonUsages.secondary2DAxis, out value);
        if (value.magnitude > threshold)
        {
            float x = value.x;
            float z = value.y;
            //Debug.Log(string.Format("AxisX '{0}' AxisY '{1}'", x, z));
            Vector3 moveBy = cam.transform.right * x + cam.transform.forward * z;
            body.MovePosition(transform.position + moveBy.normalized * speed * Time.deltaTime);
        } 
        //float x = Input.GetAxisRaw("Horizontal");
        //float z = Input.GetAxisRaw("Vertical");

    }
}
