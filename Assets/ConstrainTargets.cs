using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstrainTargets : MonoBehaviour
{
    public List<Transform> targets;
    public float maxDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform t in targets)
        {
            if ((t.position - transform.position).magnitude > maxDistance)
            {
                t.position = transform.position + (t.position - transform.position).normalized * maxDistance;
            }
        }
    }
}
