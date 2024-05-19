using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SuckScript : MonoBehaviour
{
    public float initialSuckForce;
    public float suckForce;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Let the sucking commence");
        if (collision.gameObject.tag == "Gloctopus")
        {
            collision.gameObject.GetComponent<SimpleMovement>().enabled = false;
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.down * initialSuckForce);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Gloctopus")
        {
            Debug.Log("Continue sucking");
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.AddForce(Vector2.down * suckForce);
        }
    }
}
