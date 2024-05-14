using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShoot : MonoBehaviour
{
    public Transform pivot_point;
    public float max_recoil_z { get; set; } = 20;
    public float recoil_speed { get; set; } = 10;
    public float kickback { get; set; } = 10f;

    public Rigidbody2D rb;
    private float recoil = 0.0f;

    private void Start()
    {
    }

    void Update()
    {
        recoiling();
    }

    public void Fire() {
        //every time you fire a bullet, add to the recoil.. of course you can probably set a max recoil etc..
        recoil += 0.1f;
        rb.AddForce(GameObject.FindGameObjectWithTag("Gloctopus").transform.up * kickback);
    }


    public void recoiling()
    {
        if (recoil > 0)
        {
            Quaternion maxRecoil = Quaternion.Euler(0, 0, max_recoil_z);
            // Dampen towards the target rotation
            pivot_point.rotation = Quaternion.Slerp(pivot_point.rotation, maxRecoil, Time.deltaTime * recoil_speed);
            this.GetComponent<Transform>().localEulerAngles = pivot_point.localEulerAngles;

            recoil -= Time.deltaTime;
        }
        else
        {
            recoil = 0;
            Quaternion minRecoil = Quaternion.Euler(0, 0, 0);
            // Dampen towards the target rotation
            pivot_point.rotation = Quaternion.Slerp(pivot_point.rotation, minRecoil, Time.deltaTime * recoil_speed / 2);
            this.GetComponent<Transform>().localEulerAngles = pivot_point.localEulerAngles;
        }
    }
}
