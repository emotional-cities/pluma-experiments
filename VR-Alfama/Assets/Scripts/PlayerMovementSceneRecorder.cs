// Some stupid rigidbody based movement by Dani

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerMovementSceneRecorder : MonoBehaviour
{

    //Assingables
    public Transform playerCam;
    public Transform orientation;
    public bool UseMouseY = false;
    public bool UseVrOrientation = true;

    //Other
    private Rigidbody rb;



    //Rotation and look
    private float xRotation;
    public float MouseSensitivity = 200f;
    public float orientationSlerp = 0.1f;
    private float sensMultiplier = 1f;

    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    //Input
    float x, y;
    float mouseXLerp = 0.0f;
    float mouseYLerp = 0.0f;
    bool jumping, sprinting, crouching;
    List<UnityEngine.XR.InputDevice> hands = null;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;
    private Transform head = null;
    private Transform floor = null;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        head = transform.Find("Head");
        floor = transform.Find("Floor");
    }


    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        MyInput();
        if (UseVrOrientation)
            LookVR();
        else
            Look();
    }


    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput()
    {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");


        if (Input.GetMouseButtonDown(1))
            rotating = true;
        else if (Input.GetMouseButtonUp(1))
            rotating = false;

        jumping = Input.GetButton("Jump");
        crouching = Input.GetKey(KeyCode.LeftControl);

        //Crouching
        if (Input.GetKeyDown(KeyCode.LeftControl))
            StartCrouch();
        if (Input.GetKeyUp(KeyCode.LeftControl))
            StopCrouch();
    }

    private void StartCrouch()
    {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f)
        {
            if (grounded)
            {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement()
    {
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded)
        {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    bool rotating = false;

    private float desiredX;
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.fixedDeltaTime * sensMultiplier;
       
        mouseXLerp = floatlerp(mouseXLerp, mouseX, orientationSlerp);
        mouseYLerp = floatlerp(mouseYLerp, mouseY, orientationSlerp);

        //Find current look rotation
        desiredX = playerCam.transform.localRotation.eulerAngles.y;

        if (rotating)
            desiredX += mouseXLerp;//mouseX;
        //Debug.Log(rotating);

        //Rotate, and also make sure we dont over- or under-rotate.

        if (UseMouseY)
        {
            xRotation -= mouseYLerp; // mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        }

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
        //Perform the rotations with Slerp
        //var playerlocalRotationCopy = playerCam.transform.localRotation;
        //playerCam.transform.localRotation = Quaternion.Slerp(playerlocalRotationCopy, Quaternion.Euler(xRotation, desiredX, 0), orientationSlerp);

        //var localRotationCopy = orientation.transform.localRotation;
        //orientation.transform.localRotation = Quaternion.Slerp(orientation.transform.localRotation, Quaternion.Euler(0, desiredX, 0), orientationSlerp);
    }
    private void LookVR()
    {
        float mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * MouseSensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        desiredX = playerCam.GetChild(0).transform.localRotation.eulerAngles.y;

        head.position = new Vector3(head.position.x, floor.position.y, head.position.z);
        //if (rotating)
        //    desiredX += mouseX;
        //Debug.Log(rotating);

        //Rotate, and also make sure we dont over- or under-rotate.
        //xRotation -= mouseY;
        //xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        //playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private float floatlerp(float v0, float v1, float t)
    {
        return (1 - t) * v0 + t * v1;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal))
            {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }

}
