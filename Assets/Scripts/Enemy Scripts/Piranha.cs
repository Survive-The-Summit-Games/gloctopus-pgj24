using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piranha : EnemyFollow
{
    public float time_between_bites = 1.0f;
    public float bite_damage = 5.0f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject player = collision.gameObject;

        if (player.CompareTag("Gloctopus"))
        {
            //Debug.Log("Enter");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GameObject player = collision.gameObject;

        if (player.CompareTag("Gloctopus"))
        {
            //Debug.Log("bite");
            if (this.time_since_action >= this.time_between_bites)
            {
                this.time_since_action = 0.0f;
                this.playerHealth.LoseHealth(bite_damage);
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject player = collision.gameObject;

        if (player.CompareTag("Gloctopus"))
        {
            //Debug.Log("Exit");
        }
    }



}
