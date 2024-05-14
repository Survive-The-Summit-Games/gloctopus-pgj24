using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    [Range(0f, 1f)]
    public float dampenAmount;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector3.Slerp(transform.position, target.position + Vector3.back * 10, dampenAmount); 
    }
}
