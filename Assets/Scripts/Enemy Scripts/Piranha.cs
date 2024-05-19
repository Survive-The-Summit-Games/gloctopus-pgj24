using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piranha : EnemyFollow
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject player = collision.gameObject;

        if (player.CompareTag("Gloctopus"))
        {
            Debug.Log("Enter");
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject player = collision.gameObject;

        if (player.CompareTag("Gloctopus"))
        {
            Debug.Log("bite");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject player = collision.gameObject;

        if (player.CompareTag("Gloctopus"))
        {
            Debug.Log("Exit");
        }
    }
}
