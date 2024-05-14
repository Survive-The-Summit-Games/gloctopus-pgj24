using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Collider c;
    private Rigidbody rb;
    public float shot_from { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        this.c = this.GetComponent<Collider>();
    }

    public void Fire(float speed)
    {
        this.rb = this.GetComponent<Rigidbody>();
        Vector3 force_applied = this.transform.right;
        force_applied.z = 0;
        this.rb.AddForce(force_applied * speed, ForceMode.Impulse);
    }
}
