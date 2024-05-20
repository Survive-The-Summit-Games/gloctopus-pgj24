using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Collider c;
    private Rigidbody2D rb;
    public float shot_from { get; set; }
    [SerializeField]
    private int damage;


    // Start is called before the first frame update
    void Start()
    {
        this.c = this.GetComponent<Collider>();
    }

    public void Fire(float speed)
    {
        this.rb = this.GetComponent<Rigidbody2D>();
        Vector2 force_applied = this.transform.right;
        this.rb.AddForce(force_applied * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthManager healthManager = collision.GetComponent<HealthManager>();
        if (healthManager != null)
        {
            healthManager.ChangeHealth(-damage);
            Debug.Log("dealing damage");
        }
        Destroy(this);
    }
}
