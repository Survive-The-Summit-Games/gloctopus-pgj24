using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstrainTargets : MonoBehaviour
{
    public List<Transform> targets;
    public float averageDistance;
    public float stretchDistance;
    public float swimAnimationSpeed;

    private float currentClampDistance;

    // Start is called before the first frame update
    void Start()
    {
        currentClampDistance = averageDistance + stretchDistance;
    }

    // Update is called once per frame
    void Update()
    {
        currentClampDistance = averageDistance + Mathf.Cos(swimAnimationSpeed * Time.deltaTime) * stretchDistance;
        foreach (Transform t in targets)
        {
            if ((t.position - transform.position).magnitude > currentClampDistance)
            {
                t.position = transform.position + (t.position - transform.position).normalized * currentClampDistance;
            }
        }
    }
}
