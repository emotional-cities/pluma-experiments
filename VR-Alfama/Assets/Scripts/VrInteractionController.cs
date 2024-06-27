using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

// TODO - should be moved into com.neurogears.plumavr
public class VrInteractionController : MonoBehaviour
{
    private Rigidbody rb;

    private enum Axis
    {
        primary,
        secondary
    }

    [SerializeField]
    private Axis inputAxis;

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

    [SerializeField]
    private Transform RightController;
    private RayPointer RayPointer;

    List<InputDevice> rightControllers = new List<InputDevice>();
    List<InputDevice> leftControllers = new List<InputDevice>();

    Vector2 leftMovementVector = Vector2.zero;
    Vector2 rightMovementVector = Vector2.zero;
    public bool RightInteractionState { get; private set; } = false;

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
        RayPointer = RightController.GetComponentInChildren<RayPointer>();
    }

    // Update is called once per frame
    void Update()
    {

        leftMovementVector = new Vector2(0, 0);
        rightMovementVector = new Vector2(0, 0);
        // Get left controller input
        if (leftControllers.Count > 0)
        {
            foreach (var controller in leftControllers)
                leftMovementVector += GetControllerVector(controller);
        }

        // Get right controller input
        if (rightControllers.Count > 0)
        {
            foreach (var controller in rightControllers)
            {
                rightMovementVector += GetControllerVector(controller);
                RightInteractionState = GetRightInteractionState(controller);
            }
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

        if (inputAxis == Axis.primary)
        {
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out vector);
        }
        else if (inputAxis == Axis.secondary)
        {
            device.TryGetFeatureValue(CommonUsages.secondary2DAxis, out vector);
        }
        else
        {
            device.TryGetFeatureValue(CommonUsages.primary2DAxis, out vector);
        }

        return vector;
    }

    bool GetRightInteractionState(InputDevice device)
    {
        bool state;
        device.TryGetFeatureValue(CommonUsages.triggerButton, out state);
        return state;
    }

    public Transform GetRightControllerTransform()
    {
        return RightController.transform;
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

    public void SetPointerActive(bool active)
    {
        RayPointer.gameObject.SetActive(active);
    }

    public RaycastHit GetPointedObject(LayerMask layer)
    {
        RaycastHit hit;
        Physics.Raycast(RightController.transform.position, RightController.transform.forward, out hit, Mathf.Infinity, layer);

        return hit;
    }
}
