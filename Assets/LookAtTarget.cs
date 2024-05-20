using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Between 0 and 180 degrees = bad
        // Between 180 and 360 = good
        float angle = transform.parent.rotation.eulerAngles.z;
        angle = angle < 180f ? -angle : angle;
        transform.localRotation = Quaternion.Euler(0f, 90f, angle);
    }
}
