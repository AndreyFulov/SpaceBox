using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessManager : MonoBehaviour {

    public float distanceThreshold = 1000;
    List<Transform> physicsObjects;
    FirstPersonController player;
    Camera playerCamera;

    public event System.Action PostFloatingOriginUpdate;

    void Awake () {
        var player = FindObjectOfType<FirstPersonController> ();
        var bodies = FindObjectsOfType<GravityBody> ();

        physicsObjects = new List<Transform> ();
        physicsObjects.Add (player.transform);
        foreach (var c in bodies) {
            physicsObjects.Add (c.transform);
        }

        playerCamera = Camera.main;
    }

    void LateUpdate () {
        UpdateFloatingOrigin ();
        if (PostFloatingOriginUpdate != null) {
            PostFloatingOriginUpdate ();
        }
    }

    void UpdateFloatingOrigin () {
        Vector3 originOffset = playerCamera.transform.position;
        float dstFromOrigin = originOffset.magnitude;

        if (dstFromOrigin > distanceThreshold) {
            foreach (Transform t in physicsObjects) {
                t.position -= originOffset;
            }
        }
    }

}