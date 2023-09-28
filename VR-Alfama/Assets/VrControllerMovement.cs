using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VrControllerMovement : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float translationSpeed;
    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float translationThreshold = 0.3f;
    [SerializeField]
    private float rotationThreshold = 0.3f;

    [SerializeField]
    private Transform forwardSource; // Transform used to determine what is forward relative to controller movement

    List<InputDevice> rightControllers = new List<InputDevice>();
    List<InputDevice> leftControllers = new List<InputDevice>();

    Vector2 leftMovementVector = Vector2.zero;
    Vector2 rightMovementVector = Vector2.zero;

    const InputDeviceCharacteristics rightHandCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right;
    const InputDeviceCharacteristics leftHandCharacteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Awake()
    {
        // Set up input device connect / disconnect callbacks
        InputDevices.deviceConnected += OnInputDeviceConnected;
        InputDevices.deviceDisconnected += OnInputDeviceDisconnected;
    }

    // Update is called once per frame
    void Update()
    {
        // Get left controller input
        if (leftControllers.Count > 0)
        {
            leftMovementVector = GetControllerVector(leftControllers[0]);
        }

        // Get right controller input
        if (rightControllers.Count > 0)
        {
            rightMovementVector = GetControllerVector(rightControllers[0]);
        }

        Vector3 worldForward = Vector3.ProjectOnPlane(forwardSource.forward, Vector3.up);

        Vector3 worldRight = Vector3.ProjectOnPlane(forwardSource.right, Vector3.up);

        // Do translation movement
        if (leftMovementVector.magnitude > translationThreshold)
        {
            Vector3 moveBy = (worldForward * leftMovementVector.y) + (worldRight * leftMovementVector.x);
            rb.MovePosition(transform.localPosition + moveBy.normalized * translationSpeed * Time.deltaTime);
        }

        // Do rotation movement
        if (Mathf.Abs(rightMovementVector.x) > rotationThreshold)
        {
            transform.Rotate(transform.up, rightMovementVector.x * rotationSpeed * Time.deltaTime);
        }
    }

    Vector2 GetControllerVector(InputDevice device)
    {
        Vector2 vector;
        device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out vector);
        return vector;
    }

    void OnInputDeviceConnected(InputDevice device)
    {
        if (device.characteristics.HasFlag(rightHandCharacteristics))
        {
            rightControllers.Add(device);
        }

        if (device.characteristics.HasFlag(leftHandCharacteristics))
        {
            leftControllers.Add(device);
        }
    }

    void OnInputDeviceDisconnected(InputDevice device)
    {
        if (device.characteristics.HasFlag(rightHandCharacteristics))
        {
            rightControllers.Remove(device);
        }

        if (device.characteristics.HasFlag(leftHandCharacteristics))
        {
            leftControllers.Remove(device);
        }
    }
}
