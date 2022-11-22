using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float mouseSensX = 10f;
    public float mouseSensY = 10f;
    
    public float yaw;
    public float pitch;
    float smoothYaw;
    float smoothPitch;

    float yawSmoothV;
    float pitchSmoothV;

    public Vector3 targetVelocity;
    Vector3 cameraLocalPos;
    Vector3 smoothVelocity;
    Vector3 smoothVRef;
    public float rotationSmoothTime = 0.1f;
    
    public float mass = 70;
    public float walkSpeed = 8;
    public float runSpeed = 12;
    public float jumpForce = 220;
    public float stickToGroundForce = 8;
    public float vSmoothTime = 0.1f;
    public float airSmoothTime = 0.5f;

    public LayerMask collisionMask;

    private Transform cameraT;
    private float verticalLookRotation;

    private Vector3 moveAmount;
    private Vector3 smoothMoveVel;
    private Rigidbody rb;
    GravityBody referenceBody;
    public Transform feet;

    void Start()
    {
        cameraT = Camera.main.transform;
        InitRigidbody();
    }
    void InitRigidbody () {
        rb = GetComponent<Rigidbody> ();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.mass = mass;
    }

    // Update is called once per frame
    void Update()
    {
        // Look input
        yaw += Input.GetAxisRaw ("Mouse X") * mouseSensX;
        pitch -= Input.GetAxisRaw ("Mouse Y") * mouseSensY;
        pitch = Mathf.Clamp (pitch - Input.GetAxisRaw ("Mouse Y") * mouseSensY, -90, 90);
        smoothPitch = Mathf.SmoothDampAngle (smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
        float smoothYawOld = smoothYaw;
        smoothYaw = Mathf.SmoothDampAngle (smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);
        cameraT.transform.localEulerAngles = Vector3.right * smoothPitch;
        transform.Rotate (Vector3.up * Mathf.DeltaAngle (smoothYawOld, smoothYaw), Space.Self);
        

        bool grounded = IsGrounded ();
        Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
        float currentSpeed = Input.GetKey (KeyCode.LeftShift) ? runSpeed : walkSpeed;
        targetVelocity = transform.TransformDirection (input.normalized) * currentSpeed;
        smoothVelocity = Vector3.SmoothDamp (smoothVelocity, targetVelocity, ref smoothVRef, (grounded) ? vSmoothTime : airSmoothTime);

        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                rb.AddForce (transform.up * jumpForce, ForceMode.VelocityChange);
                grounded = false;
            } else {
                // Apply small downward force to prevent player from bouncing when going down slopes
                rb.AddForce (-transform.up * stickToGroundForce, ForceMode.VelocityChange);
            }
        }
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1+.1f,collisionMask))
        {
            grounded = true;
        }
    }
    bool IsGrounded () {
        // Sphere must not overlay terrain at origin otherwise no collision will be detected
        // so rayRadius should not be larger than controller's capsule collider radius
        const float rayRadius = .3f;
        const float groundedRayDst = .2f;
        bool isgrounded = false;

        if (referenceBody) {
            var relativeVelocity = rb.velocity - referenceBody.velocity;
            // Don't cast ray down if player is jumping up from surface
            if (relativeVelocity.y <= jumpForce * .5f) {
                RaycastHit hit;
                Vector3 offsetToFeet = (feet.position - transform.position);
                Vector3 rayOrigin = rb.position + offsetToFeet + transform.up * rayRadius;
                Vector3 rayDir = -transform.up;

                isgrounded = Physics.SphereCast (rayOrigin, rayRadius, rayDir, out hit, groundedRayDst, collisionMask);
            }
        }

        return isgrounded;
    }

    private void FixedUpdate()
    {
        GravityBody[] bodies = NBodySimulation.Bodies;
        Vector3 gravityOfNearestBody = Vector3.zero;
        float nearestSurfaceDst = float.MaxValue;

        // Gravity
        foreach (GravityBody body in bodies) {
            float sqrDst = (body.Position - rb.position).sqrMagnitude;
            Vector3 forceDir = (body.Position - rb.position).normalized;
            Vector3 acceleration = forceDir * .0001f * body.mass / sqrDst;
            //rb.AddForce (acceleration, ForceMode.Acceleration);
            
            Vector3 gravityForce = Gravitation.ComputeNonCelestialBodyForce(rb, body);
            gravityForce *= Time.deltaTime;
            rb.AddForce(gravityForce);

            // Find body with strongest gravitational pull 
            float dstToSurface = Mathf.Sqrt (sqrDst) - body.radius;
            // Find body with strongest gravitational pull 
            if (dstToSurface < nearestSurfaceDst) {
                nearestSurfaceDst = dstToSurface;
                gravityOfNearestBody = acceleration;
                referenceBody = body;
            }
        }

        // Rotate to align with gravity up
        Vector3 gravityUp = -gravityOfNearestBody.normalized;
        rb.rotation = Quaternion.FromToRotation (transform.up, gravityUp) * rb.rotation;


        // Move
        Vector3 relativeVelocity = rb.velocity - referenceBody.velocity;
        rb.MovePosition (rb.position + smoothVelocity * Time.fixedDeltaTime);
        /*if (IsGrounded())
        {
            rb.velocity = referenceBody.velocity + smoothVelocity * Time.deltaTime;
        }*/
        /*
        if (IsGrounded())
        {
            gameObject.transform.parent = referenceBody.transform;
        }
        else
        {
            gameObject.transform.parent = null;
        }*/
    }
    
    public void SetVelocity (Vector3 velocity) {
        rb.velocity = velocity;
    }
}
