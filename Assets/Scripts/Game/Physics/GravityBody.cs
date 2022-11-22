using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    /*
    private GravityAttractor planet;
    private void Awake()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<GravityAttractor>();
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        planet.Attract(transform);
    }
*/

    public float mass;
    public float radius;
    public Vector3 initialVelocity;
    public float surfaceGravity;
    public Vector3 velocity { get; private set; }
    private Rigidbody rb;
    Transform meshHolder;
    public string bodyName = "Unnamed";

    private void Awake()
    {
        rb = GetComponent<Rigidbody> ();
        rb.mass = mass;
        velocity = initialVelocity;
        
    }
    public void UpdateVelocity(GravityBody[] allBodies, float timeStep) {
        foreach (var otherBody in allBodies)
        {
            if (otherBody != this)
            {
                float sqrDst = (otherBody.GetComponent<Rigidbody>().position - GetComponent<Rigidbody>().position).sqrMagnitude;
                Vector3 forceDir = (otherBody.GetComponent<Rigidbody>().position - GetComponent<Rigidbody>().position).normalized;
                Vector3 force = forceDir * .0001f * mass * otherBody.mass / sqrDst;
                Vector3 acceleration = force / mass;
                velocity += acceleration * timeStep;
            }
        }
    }

    private void FixedUpdate()
    {
        GravityBody[] bodies = FindObjectsOfType<GravityBody>().Where(body => body != this).ToArray();
        foreach (GravityBody body in bodies)
        {
            Vector3 gravityForce = Gravitation.ComputeCelestialBodyForce(rb, body.GetComponent<Rigidbody>());
            gravityForce *= Time.deltaTime;
            Debug.Log(gravityForce);
            rb.AddForce(gravityForce);   
        }
        

        
    }

    public void UpdateVelocity (Vector3 acceleration, float timeStep) {
        velocity += acceleration * timeStep;
    }

    public void UpdatePosition(float timestep)
    {
        rb.position += velocity * timestep;
    }
    
    public Vector3 Position {
        get {
            return rb.position;
        }
    }
    void OnValidate () {
        //mass = surfaceGravity * radius * radius / .0001f;
        meshHolder = transform.GetChild (0);
        gameObject.name = bodyName;
    }
}
