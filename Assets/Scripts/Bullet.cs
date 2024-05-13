using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Collider c;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        this.c = this.GetComponent<Collider>();
    }

    public void Fire(float speed)
    {
        this.rb = this.GetComponent<Rigidbody>();
        this.rb.AddForce(this.transform.right * speed, ForceMode.Impulse);
    }
}
