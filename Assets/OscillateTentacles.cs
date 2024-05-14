using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OscillateTentacles : MonoBehaviour
{
    public List<DistanceJoint2D> distanceJoint2Ds;
    public float medianDistance;
    public float deltaDistance;
    public float stretchSpeed;
    private float speedMultiplier;
    public float maxSpeed;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        speedMultiplier = Mathf.Lerp(0, 1, rb.velocity.magnitude / maxSpeed);
        foreach (DistanceJoint2D distanceJoint2D in distanceJoint2Ds)
        {
            distanceJoint2D.distance = medianDistance + Mathf.Cos(Time.time * stretchSpeed) * deltaDistance * speedMultiplier;
        }
    }
}
