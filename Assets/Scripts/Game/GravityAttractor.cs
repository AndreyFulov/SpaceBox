using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityAttractor : MonoBehaviour
{
    
    public void Attract(Transform body)
    {
        Vector3 targetDir = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;
        body.rotation = Quaternion.FromToRotation(bodyUp, targetDir) * body.rotation;
        body.GetComponent<Rigidbody>().AddForce(targetDir*CalcGravity(body));
    }
    
    public float CalcGravity(Transform target)
    {
        float gravity = 10 * ((GetComponent<Planet>().planetMass * target.GetComponent<Rigidbody>().mass) /
                        Vector3.Distance(transform.position, target.position));
        return -gravity;
    }
}
